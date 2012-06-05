using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLife.BoilerPlate;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.BoilerPlate.Misc;
using GameOfLife.BoilerPlate.ScreenManager;
using GameOfLife.Data;
using GameOfLife.GameLogic;
using GameOfLife.GameLogic.Assets;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.GameLogic.PureLogics;
using GameOfLife.Screens;
using GameOfLife.WorldEditing;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using TomShane.Neoforce.Controls;
using Console = System.Console;
using EventArgs = TomShane.Neoforce.Controls.EventArgs;

namespace GameOfLife {
    /// <summary>
    /// The GUI application class your application will be built upon.
    /// </summary>
    public sealed class TheGameOfLife : Application {
        private readonly ScreenManager _screenManager;
        private StorageDevice _storageDevice;
        private SoundEffectInstance _music;
        private SoundEffect _backgroundMusic;
        private InputListener _AdminListener;

        /// <summary>
        /// Creates an application using the Default skin file. 
        /// </summary>
        public TheGameOfLife()
            : base(true) {
            _screenManager = new ScreenManager(this);
            Manager = new CustomManager(this, _screenManager);

            Content.RootDirectory = "Content";
            Manager.SkinDirectory = "Content/Skins";
            Manager.LayoutDirectory = "Content/Layouts";


            ClearBackground = true;
            BackgroundColor = Color.White;

            TargetElapsedTime = TimeSpan.FromMilliseconds(1000 / 60f);

            IsFixedTimeStep = true;
            SystemBorder = true;
            ExitConfirmation = false;


            Manager.Visible = false;
            _screenManager = new ScreenManager(this);
        }

        /// <summary>
        /// Initializes the application.
        /// </summary>
        protected override void Initialize() {
            base.Initialize();
            bool FSEM = true;

            Graphics.SynchronizeWithVerticalRetrace = true;
            if (FSEM) {
                //Getting the maximum supported resolution.
                var maxResolution = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

                //Setting the game to start in full screen mode.
                Graphics.PreferredBackBufferWidth = maxResolution.Width;
                Graphics.PreferredBackBufferHeight = maxResolution.Height;
                Graphics.ToggleFullScreen();
            } else {
                Graphics.PreferredBackBufferWidth = 1224;
                Graphics.PreferredBackBufferHeight = 550;
            }
            Graphics.ApplyChanges();

            ((CustomManager)Manager).MainWindow = MainWindow;
            MainWindow.Alpha = 0;
            MainWindow.StayOnBack = true;
            MainWindow.CloseButtonVisible = false;

            StorageDevice.BeginShowSelector(result => { _storageDevice = StorageDevice.EndShowSelector(result); }, null);
            Persister.DepdencyInjection(_storageDevice);

            WorldLoader.SetInstance(new WorldLoader(this, Content));
            TransportFactory.SetInstance(new TransportFactory(Content));

            InitScreenManager();

            _music = _backgroundMusic.CreateInstance();
            _music.IsLooped = true;
            _music.Play();


            // Toggles admin mode when 'Admin Please' is typed
            _AdminListener = new InputListener {
                // "Admin please"
                Keys = new[] {
                            Keys.A,Keys.D,Keys.M,Keys.I,Keys.N,
                            Keys.Space,
                            Keys.P,Keys.L,Keys.E,Keys.A,Keys.S,Keys.E
                        },
                Callback = ChangeAdminMode
            };
        }

        private void ChangeAdminMode() {
            var adminMode = (Constants.Editing.IsAdminMode = !Constants.Editing.IsAdminMode);
            Manager.Add(adminMode
                            ? new Alert(Manager, "Admin mode is now Enabled", icon: "Images/AlertIcons/Pass")
                            : new Alert(Manager, "Admin mode is now Disabled", icon: "Images/AlertIcons/Fail"));
        }

        protected override void LoadContent() {
            base.LoadContent();
            _backgroundMusic = Content.Load<SoundEffect>("Sounds/BlackLounge");
        }

        private void InitScreenManager() {
            _screenManager.AddGameLayer(new Background(Constants.ScreenNames.Background), true);
            _screenManager.AddGameLayer(new Logo(Constants.ScreenNames.Logo), true);

            _screenManager.AddGameLayer(new SetupLevel());
            _screenManager.AddGameLayer(new WorldEditorSetup());
            _screenManager.AddGameLayer(new Options(Constants.ScreenNames.Options));

            _screenManager.AddGameLayer(new Menu(Constants.ScreenNames.MainMenu,
                true,
                Constants.ScreenNames.SetupLevel,
                Constants.ScreenNames.WorldEditorSetup,
                Constants.ScreenNames.Options));

            // Show the default screen that runs when the game is played
            // _screenManager.ShowScreen(Constants.ScreenNames.WorldEditorSetup);
          //   _screenManager.AddGameLayer(new Level(CreateTestWorld(null)), true);
          //  WorldLoader.instance.LoadCustomWorld("New Xml", objs => _screenManager.AddGameLayer(new WorldEditor("New Xml", objs), true));

           // _screenManager.AddGameLayer(new WorldEditor("foo", CreateBasicEmptyWorld(Content)), true);


             _screenManager.ShowScreen(Constants.ScreenNames.MainMenu);

            _screenManager.Initialize();
        }

        public GameInfo CreateTestWorld(List<WorldObject> worldObjects) {
            bool newWorld = false;

            // Create players
            List<Player> playerList = new List<Player>();
            const int playerCount = 2;

            for (int i = 0; i < playerCount; i++) {
                var newPlayer = new Player { Name = Constants.DefaultPlayerInfo.NamesAndColors[i].Item1 };
                newPlayer.InitializeContent(Content);
                newPlayer.Cash = 5000;
                newPlayer.PlayerColor = Constants.DefaultPlayerInfo.NamesAndColors[i].Item2;

                newPlayer.Avatar = Content.Load<Texture2D>(Constants.DefaultPlayerInfo.AvatarImages[i]);

                newPlayer.CareerType = CareerType.CollegeCareer;
                newPlayer.Accept(TransportFactory.GetInstance().GetTransport(TransportType.PlaneCar));
                playerList.Add(newPlayer);

                // Give them all their assets for testing
                if (i != 1) continue;
                {
                    newPlayer.Accept(new Partner());
                    newPlayer.Accept(new Pet());
                    newPlayer.Accept(new PassportStamp(IslandType.Jungle));
                    newPlayer.Accept(new Loan(-4000));
                    newPlayer.Accept(new PassportStamp(IslandType.City));
                    newPlayer.Accept(new PassportStamp(IslandType.Beach));
                    newPlayer.Accept(new PassportStamp(IslandType.Jungle));
                    newPlayer.Accept(new PassportStamp(IslandType.Snow));
                    newPlayer.Accept(new PassportStamp(IslandType.City));
                    newPlayer.Accept(new PassportStamp(IslandType.City));
                    newPlayer.Accept(new Child());
                    newPlayer.Accept(new Child());
                    newPlayer.Accept(new Child());

                    newPlayer.Accept(new Pet());
                    newPlayer.Accept(new Child());
                    newPlayer.Accept(new Loan(-4000));
                    newPlayer.Accept(new Loan(-43450));

                    newPlayer.Accept(new House(12123, "Awesome House", "Images/AlertIcons/House"));
                }
               
            }

            var gameInfo = new GameInfo(worldObjects == null || newWorld ? CreateBasicEmptyWorld(Content) : worldObjects, playerList.ToArray(), 100, GameRuleType.Passport) { Manager = Manager as CustomManager };
            return gameInfo;
        }

        private static List<WorldObject> CreateBasicEmptyWorld(ContentManager content) {
            var worldObjects = new List<WorldObject>();
            var startingNode = new StartingNode(new Vector2(100, 100));

            startingNode.InitializeContent(content);

            //  var universityLogic = new BindedLogic(new StartUniversity(), true, "Start\nCollege\n-$20k");
            //  var universityLogic = new BindedLogic(new BlankStory(), true, false, "Story");
            // var getMarried = new BindedLogic(new GetMarried(10000), true, false, "Get\nMarried");
            //  var travelOne = new BindedLogic(new Travel(IslandType.Snow, WorldTransportType.Boat), true, false, "Travel A");
           // var minusCash = new BindedLogic(new ModifyCashStory(10000), hasPassLogic: true);

          //  var universityNode = new BindedLogic(new StartUniversity(), hasPassLogic: true);
            var universityPath = new Node(new Vector2(100, 220), true);
            universityPath.SetBindedLogic(new BindedLogic(new StartUniversity(), hasPassLogic: true));
            universityPath.InitializeContent(content);
            startingNode.AddLinkedNode(universityPath);

            // var careerLogic = new BindedLogic(new StartCareer(), true, "Start\nCareer");
            // var spinToWin = new BindedLogic(new SpinToWin(50000), true, false, "Spin To\nWin\n$50k");
            // var travelTwo = new BindedLogic(new Travel(IslandType.City, WorldTransportType.Boat), true, false, "Travel B");
            // var giveChild = new BindedLogic(new GiveChild(), true, false, "Acquire\nChild");
            //var givePet = new BindedLogic(new GivePet(), true, false, "Get\nPet");
            //var buyHouse = new BindedLogic(new BuyHouse(null), true, false, "Buy house");
            // var takeExam = new BindedLogic(new TakeExam(), true, true, "Take\nExam");
            //var addCash = new BindedLogic(new Nothing(), hasPassLogic: true);
            var careerPath = new Node(new Vector2(245, 100), true);
            //careerPath.SetBindedLogic(addCash);
            careerPath.SetBindedLogic(new BindedLogic(new StartCareer(), hasPassLogic: true));
            careerPath.InitializeContent(content);

            careerPath.AddLinkedNode(universityPath);
            startingNode.AddLinkedNode(careerPath);
            universityPath.AddLinkedNode(careerPath);

            var spinner = new Spinner(new Vector2(400, 100));
            spinner.InitializeContent(content);

            worldObjects.Add(startingNode);
            worldObjects.Add(universityPath);
            worldObjects.Add(careerPath);
            worldObjects.Add(spinner);

            return worldObjects;
        }


        /// <summary>
        /// Unloads content.
        /// </summary>
        protected override void UnloadContent() {
            base.UnloadContent();
            _screenManager.UnloadContent();
            Content.Unload();
        }

        /// <summary>
        /// Allows the application to run logic.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime) {
            base.Update(gameTime);

            _music.Volume = Options.Volume;
            _screenManager.Update(gameTime);

            // Test if the user wants to quit the game
            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Escape)) {
                MainWindow.Close();
            }

            // Update our InputListener for changing admin mode
            _AdminListener.Update(keyboardState, _AdminListener.i, _AdminListener);
        }

        protected override void DrawScene(GameTime gameTime) {
            _screenManager.Draw(gameTime);
        }
    }
}
