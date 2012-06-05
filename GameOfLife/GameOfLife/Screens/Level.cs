using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.BoilerPlate.HUD;
using GameOfLife.BoilerPlate.Misc;
using GameOfLife.Core.HUD;
using GameOfLife.Data;
using GameOfLife.GameLogic;
using GameOfLife.GameLogic.Assets;
using GameOfLife.GameLogic.GameStates;
using GameOfLife.GameLogic.GameStates.Actual;
using GameOfLife.GameLogic.GameStates.Factories;
using GameOfLife.GameLogic.PureLogics;
using GameOfLife.LevelMenu;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;
using StateFactory = GameOfLife.BoilerPlate.FSM.StateFactory;

namespace GameOfLife.Screens {
    public sealed class Level : World {
        private GameInfo _gameInfo;
        private FSM<IGameState, GameInfo> _fsm;
        private HUDSystem HUDSystem { get; set; }

        private GraphicalWidget _previousHudMessage;
        private TextWidget _debugWidget;
        private InputListener _konami;
        private InputListener _teleport;

        private MenuManager _menuManager;
        private bool _previousAdminMode;
        private bool _gameStarted;

        public Level(GameInfo gameInfo)
            : base(gameInfo.WorldObjects, Constants.ScreenNames.Level) {

            _gameInfo = gameInfo;
            _gameInfo.PanCameraToObject = PanCameraToObject;
            _gameInfo.AddLowPriorityTarget = AddLowPriorityTarget;
            _gameInfo.ClearTargets = ClearTargets;
            gameInfo.Fsm = _fsm = new FSM<IGameState, GameInfo>();

            foreach (var player in gameInfo.PlayerList) {
                player.CashListeners += LoanListener;
            }

            _gameInfo.CreateMessage = CreateHUDMessage;
        }

        private void ReturnToMenu() {
            ScreenManager.SwapScreens(this, Constants.ScreenNames.SetupLevel);
            ScreenManager.Remove(HUDSystem);
            ScreenManager.Remove(this);
        }

        public override void Dispose() {
            base.Dispose();
            _fsm.Clear();
            _gameInfo = null;
            _menuManager.Dispose();
        }

        private void CreateBasicStateFactories() {
            StateFactory.SetInstance(new StateFactory());
            StateFactory.AddFactory(GameStates.Roll, new RollFactory(_gameInfo));
            StateFactory.AddFactory(GameStates.ChooseNextNode, new Factory(new ChooseNextNode()));
            StateFactory.AddFactory(GameStates.EndSpin, new Factory(new EndOfSpinnerSpunLogic()));
            StateFactory.AddFactory(GameStates.Spin, new Factory(_gameInfo.Spinner));
            StateFactory.AddFactory(GameStates.VisuallyMovePlayer, new Factory(new VisuallyMovePlayer()));
            StateFactory.AddFactory(GameStates.LetUserChooseDirection, new Factory(new LetUserChooseDirection()));
            StateFactory.AddFactory(GameStates.Wait, new WaitFactory());
            StateFactory.AddFactory(GameStates.Execute, new Factory(new ExecuteLogic()));
            StateFactory.AddFactory(GameStates.EndExecute, new Factory(new EndOfExecutionLogic()));
            StateFactory.AddFactory(GameStates.ChangeWorld,
                new Factory(new ChangeWorld(_gameInfo.WorldNodes.Where(i => i.IsChangeable).ToList(),
                _gameInfo.WorldNodes.Where(i => i.BindedLogic.PureLogic is BuyHouse).ToList())));
            StateFactory.AddFactory(GameStates.IncreaseAge, new Factory(new IncreaseAgeCounter()));
            StateFactory.AddFactory(GameStates.EndGame, new Factory(new EndGame(EndGame)));
        }

        private void EndGame() {
            _menuManager.EndGame();
        }

        private void CreateHUDMessage(string displayText) {
            if (_previousHudMessage != null)
                HUDSystem.Remove(_previousHudMessage);
            if (displayText == String.Empty) return;
            var newShowingMessage = new GraphicalWidget(_gameInfo, HUDAlignment.BottomCenter, i => displayText, Content.Load<Texture2D>("Images/HUD/msgBar"));
            HUDSystem.Add(newShowingMessage);
            _previousHudMessage = newShowingMessage;
        }

        private void LoanListener(Player player, int oldCash) {
            if (player.Cash < 0) {
                _fsm.LazyPush(new TakeLoan(player, player.Cash));
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        public override void Initialize() {
            base.Initialize();
            CameraPos = _gameInfo.Spinner.Center;
            _gameInfo.Manager = ControlManager.Manager;

            // Set up our konami code callback to give the current player 0xBEEF
            _konami = new InputListener {
                Keys = new List<int> { 38, 38, 40, 40, 37, 39, 37, 39, 66, 65 }.Cast<Keys>().ToArray(),
                Callback = () => {
                    _fsm.Push(new ModifyCash(0xBEEF));
                    ControlManager.Add(new Alert(ControlManager.Manager, "Konami code detected! Acquire 0xBEEF", "Konami Code Activated"));
                }
            };

            _teleport = new InputListener {
                Keys = new[] { Keys.T, Keys.E, Keys.L, Keys.E, Keys.P, Keys.O, Keys.R, Keys.T },
                Callback = () => {
                    _gameInfo.Fsm.Push(new Teleport());
                    ControlManager.Add(new Alert(ControlManager.Manager, "Click the place you want to teleport to!", "Teleport Activated"));
                }
            };

            CreateWelcomeMessage();
        }

        private void BeginGame() {
            CreateHudSystem();
            CreateMenu();
            CreateBasicStateFactories();
            _fsm.Push(StateFactory.GetState(GameStates.Roll));
            _gameInfo.PanCameraToObject(_gameInfo.Spinner);
            _gameStarted = true;

            var houseLogics = _gameInfo.WorldNodes.Select(i => i.BindedLogic.PureLogic).OfType<BuyHouse>();

            foreach (var houseLogic in houseLogics) {
                houseLogic.House.ChangePrice(RandomHelper.NextFloat(0.7f, 1.3f));
            }

            Loan.ChangeInterestRates();
        }

        private void CreateMenu() {
            _menuManager = new MenuManager(_gameInfo, ControlManager, ReturnToMenu);
            var menuButton = _menuManager.CreateMenuButton();
            menuButton.SetPosition(ScreenWidth - menuButton.Width - 20, ScreenHeight - menuButton.Height - 20);
            ControlManager.Add(menuButton);
        }

        private void CreateHudSystem() {
            HUDSystem = new HUDSystem(Content.Load<SpriteFont>("Fonts/HUDFont"), Content);

            // Create the debugging widget (Only shows in admin mode)
            _debugWidget = new TextWidget(_gameInfo, HUDAlignment.BottomLeft, i => i.Fsm.ToString());

            // Create an avatar widget
            var avatarWidget = new AvatarWidget(_gameInfo, HUDAlignment.TopLeft);
            HUDSystem.Add(avatarWidget);

            // Create a widget for the game type that the user will be playing
            HUDWiget gameTypeWidget = null;
            switch (_gameInfo.GameRuleType) {
                case GameRuleType.Retirement: gameTypeWidget = new AgeCounterWidget(_gameInfo, HUDAlignment.TopCenter); break;
                case GameRuleType.Passport: gameTypeWidget = new PassportWidget(_gameInfo, HUDAlignment.TopCenter); break;
            }
            HUDSystem.Add(gameTypeWidget);

            // Create the current highscore list widget
            var highscoreList = new Leaderboarder(_gameInfo, HUDAlignment.TopRight);
            HUDSystem.Add(highscoreList);

            ScreenManager.AddGameLayer(HUDSystem, true);
        }

        private void CreateWelcomeMessage() {
            var padding = 15;
            var window = new Window(_gameInfo.Manager) { Width = 400, Height = 300, Text = "Welcome to Game of Life!", AutoScroll = false, Resizable = false };
            var label = new Label(_gameInfo.Manager) {
                Width = window.Width, Parent = window, Height = 180, Top = padding, Left = padding,
                Text = "You are about to begin the game.  \nThe camera can be controlled using the following keys: \n\n -W A S D \n -Up Down Left Right \n\nAny active game objects visible on screen will glow. \n\nObjects outside of the visible area will have \nclickable arrows pointing to them. \n\nClicking the arrow will adjust the camera automatically. \n\nClose this window to begin."
            };
            label.Init();

            var close = new Button(_gameInfo.Manager) { Text = "Close", Parent = window, Top = window.Height - 75, Left = window.Width / 2 - 50 };
            close.Click += (sender, args) => window.Close();
            window.Closed += (sender, args) => BeginGame();
            close.Init();

            window.Init();
            _gameInfo.Manager.Add(window);
        }

        public override void Update(GameTime gameTime) {
            _fsm.PerformLogic(gameTime, _gameInfo);
            _konami.Update(Keyboard.GetState(), _konami.i, _konami);
            _teleport.Update(Keyboard.GetState(), _teleport.i, _teleport);
            base.Update(gameTime);


            if (_previousAdminMode == Constants.Editing.IsAdminMode || !_gameStarted) return;
            _previousAdminMode = Constants.Editing.IsAdminMode;
            if (_previousAdminMode) {
                HUDSystem.Add(_debugWidget);
            } else {
                HUDSystem.Remove(_debugWidget);
            }
        }
    }
}
