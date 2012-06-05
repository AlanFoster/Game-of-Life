using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using GameOfLife.BoilerPlate;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.BoilerPlate.Misc;
using GameOfLife.BoilerPlate.ScreenManager;
using GameOfLife.Data;
using GameOfLife.GameLogic;
using GameOfLife.GameLogic.Assets;
using GameOfLife.WorldEditing;
using GameOfLife.WorldEditing.EditSystems;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;
using Console = System.Console;
using EventArgs = TomShane.Neoforce.Controls.EventArgs;
using EventHandler = TomShane.Neoforce.Controls.EventHandler;

namespace GameOfLife.Screens {
    public class GameInfoLoader {
        [Editable("Game world to load", typeof(SetupLevel.WorldLoaderDropDowner))]
        public String LoadingWorld { get; private set; }

        [Editable("Game Rule Type")]
        public GameRuleType GameRuleType { get; private set; }

        [Editable("Retirement Age")]
        public int AgeCounterTarget { get; private set; }
    }

    public class WorldLoaderDropDown {
        public String Value { get; private set; }
        public WorldLoaderDropDown(String name) {
            Value = name;
        }
    }

    public class SetupLevel : GameLayer {
        const int Margin = 30;

        private static readonly int MinPlayers = Constants.GameRules.MinPlayers;
        private static readonly int MaxPlayers = Constants.GameRules.MaxPlayers;

        // The minimum and maximum amount of years that a world must last for
        // Only applies if we are in Retirement mode
        private static readonly int MinAge = Constants.GameRules.MinYears;
        private static readonly int MaxAge = Constants.GameRules.MaxYears;

        private TabControl _tabs;
        private TabPage _gameInfoTab;
        const int Padding = 50;
        private int _playerCount;

        private ObjectEditor<GameInfoLoader> _gameLoaderEditor;
        private Dictionary<TabPage, ObjectEditor<Player>> _playerObjectEditors;
        private readonly Tuple<String, Color, Gender>[] _defaultPlayerInfo = Constants.DefaultPlayerInfo.NamesAndColors;

        private bool[] _playerPositions;

        public SetupLevel()
            : base(Constants.ScreenNames.SetupLevel) {
        }

        public override void Initialize() {
            base.Initialize();

            CreateMenuButtons();
        }

        public override void Added() {
            base.Added();
            if (_tabs != null) ControlManager.Remove(_tabs);
            WorldLoader.instance.GetAllWorldNames(CreateContentArea);
        }

        private void CreateContentArea(String[] worldNames) {
            CreateTabs();
            _playerCount = 0;
            _gameInfoTab = _tabs.AddPage();
            CreateGameRules(_gameInfoTab, worldNames);
            CreatePlayers();

            SwitchToTab(0);
        }

        private void CreatePlayers() {
            _playerObjectEditors = new Dictionary<TabPage, ObjectEditor<Player>>();
            _playerPositions = new bool[MaxPlayers];

            for (var i = 0; i < MinPlayers; i++) {
                AddPlayer();
            }
        }

        private void CreateTabs() {
            _tabs = new TabControl(ControlManager.Manager);
            _tabs.Init();
            _tabs.SetPosition(300, 50);

            _tabs.MinimumHeight = 467;
            _tabs.MinimumWidth = 856;

            ControlManager.Add(_tabs);
        }

        private void CreateMenuButtons() {
            int top = Padding;
            const int buttonWidth = 200;
            const int buttonHeight = 50;

            var menuInfos = new[] {
                new Tuple<string, EventHandler>("Play Game", (sender, args) => StartGame()),
                    new Tuple<string, EventHandler>("Add Player", (sender, args) => AddPlayer()),
                    new Tuple<string, EventHandler>("Remove Player", (sender, args) => RemovePlayer()),
                    new Tuple<string, EventHandler>("Main Menu", (sender, args) => ScreenManager.SwapScreens(this, Constants.ScreenNames.MainMenu))
            };
            foreach (var menuInfo in menuInfos) {
                var menuButton = new Button(ControlManager.Manager) {
                    Text = menuInfo.Item1,
                    Top = top,
                    Left = Padding,
                    Width = buttonWidth,
                    Height = buttonHeight
                };
                menuButton.Init();
                menuButton.Click += menuInfo.Item2;

                top += menuButton.Height + Padding;
                ControlManager.Add(menuButton);
            }
        }

        private void StartGame() {
            var gameLoaderObject = _gameLoaderEditor.CreateInstance();
            // Test if the user inputted the a valid amount of years
            if (gameLoaderObject.GameRuleType == GameRuleType.Retirement && (gameLoaderObject.AgeCounterTarget < MinAge || gameLoaderObject.AgeCounterTarget > MaxAge)) {
                ControlManager.Add(new Alert(ControlManager.Manager,
                    String.Format("Sorry, the world target age must be between\n{0} and {1}!", MinAge, MaxAge),
                    "Invalid Target World Age", icon: "Images/AlertIcons/Fail"));
                SwitchToTab(0);
            } else {
                if (gameLoaderObject.LoadingWorld == Constants.Locations.DefaultWorldName) {
                    WorldLoader.instance.LoadWorld(Constants.Locations.DefaultWorldPath, worldObjects => WorldReady(worldObjects, gameLoaderObject));
                } else if (gameLoaderObject.LoadingWorld == Constants.Locations.TestWorldName) {
                    WorldLoader.instance.LoadWorld(Constants.Locations.TestWorldPath, worldObjects => WorldReady(worldObjects, gameLoaderObject));
                } else {
                    WorldLoader.instance.LoadCustomWorld(gameLoaderObject.LoadingWorld, worldObjects => WorldReady(worldObjects, gameLoaderObject));
                }
            }
        }

        private void WorldReady(List<WorldObject> worldObjects, GameInfoLoader gameInfoLoader) {
            if (worldObjects == null) {
                var alert = new Alert(ControlManager.Manager, "That world has become corrupt!\nPlease choose or create a different world", "Invalid world!");
                ControlManager.Add(alert);
                return;
            }

            var playerList = new Player[_playerCount];
            var i = 0;
            foreach (var tabPage in _tabs.TabPages) {
                if (tabPage == _gameInfoTab) continue;
                var player = playerList[i] = _playerObjectEditors[tabPage].CreateInstance();
                player.Cash = Constants.GameRules.StartingCash;
                player.Gender =
                    Constants.DefaultPlayerInfo.NamesAndColors.First(tuple => tuple.Item2 == player.PlayerColor).Item3;
                player.InitializeContent(Content);
                // Give them their basic car transport
                player.SetTransport(TransportType.Car);
                i++;
            }

            var gameInfo = new GameInfo(worldObjects, playerList, gameInfoLoader.AgeCounterTarget, gameInfoLoader.GameRuleType);
            var level = new Level(gameInfo);
            ScreenManager.AddGameLayer(level);
            ScreenManager.SwapScreens(this, level);
        }

        private void RemovePlayer() {
            if ((! Constants.Editing.IsAdminMode && _playerCount <= MinPlayers) || _playerCount == 1 || _tabs.SelectedPage.Text.Equals("Game Info")) return;
            var currentTabNum = _tabs.SelectedIndex;
            var selectedTab = _tabs.SelectedPage;
            _playerPositions[int.Parse(selectedTab.Name)] = false;
            _tabs.RemovePage(selectedTab);
            _playerObjectEditors.Remove(selectedTab);
            _playerCount--;
            SwitchToTab((int)MathHelper.Clamp(currentTabNum, 1, _playerCount));
        }

        private void AddPlayer() {
            if (_playerCount == MaxPlayers) return;
            // Get the player position that is free
            var freePosition = -1;
            while (_playerPositions[++freePosition]) { }
            _playerPositions[freePosition] = true;

            _playerCount++;
            var page = _tabs.AddPage();
            PopulatePlayerPage(page, freePosition);
            SwitchToTab(_playerCount);
        }

        private void SwitchToTab(int num) {
            _tabs.SelectedIndex = 0;
            _tabs.SelectedIndex = num;
        }

        public override void Update(GameTime gameTime) {
        }

        public override void Draw(GameTime gameTime) {
        }

        private void PopulatePlayerPage(TabPage tabPage, int playerPosition) {
            var playerInfo = _defaultPlayerInfo[playerPosition];
            tabPage.Name = playerPosition.ToString(CultureInfo.InvariantCulture);
            var playerName = playerInfo.Item1;
            var playerColor = playerInfo.Item2;

            var objectEditor = new ObjectEditor<Player>(ControlManager.Manager, tabPage, spacing: Margin);
            objectEditor.Add(typeof(Color), new ColorEditSystem());
            objectEditor.Add(typeof(Texture2D), new ImageSelectorEditSystem(Constants.DefaultPlayerInfo.AvatarImages, playerPosition));
            objectEditor.CreateInterface();


            var nameControl = objectEditor.GetControl("Name");
            nameControl.TextChanged += (sender, args) => tabPage.Text = (((TextBox)sender).Text);
            nameControl.Text = playerName;

            var colorControl = objectEditor.GetControl("PlayerColor");
            colorControl.Color = playerColor;

            var endLocation = objectEditor.SizeY;
            tabPage.MinimumHeight = endLocation;

            _playerObjectEditors.Add(tabPage, objectEditor);
        }


        public class WorldLoaderDropDowner : IEditSystem {
            private readonly String[] _worldNames;

            public WorldLoaderDropDowner(String[] worldNames) {
                _worldNames = worldNames;
            }

            public object GetValue(Control control, PropertyInfo field) {
                return control.Text;
            }

            public Control CreateControl(Manager manager, Control parent, PropertyInfo field, object existingData) {
                var worldNamesSelection = new ComboBox(manager) { Width = 200, Name = field.Name };
                worldNamesSelection.Items.AddRange(_worldNames);
                worldNamesSelection.Init();
                worldNamesSelection.ItemIndex = 0;

                return worldNamesSelection;
            }
        }

        public class ColorEditSystem : IEditSystem {
            public object GetValue(Control control, PropertyInfo field) {
                return control.Color;
            }

            public Control CreateControl(Manager manager, Control parent, PropertyInfo field, object existingData) {
                var panel = new Panel(manager) { Name = field.Name, Color = Color.Pink, Width = 200, Height = 20 };
                panel.Init();
                return panel;
            }
        }

        public void CreateGameRules(TabPage tabPage, String[] worldNames) {
            tabPage.Text = "Game rules";
            _gameLoaderEditor = new ObjectEditor<GameInfoLoader>(ControlManager.Manager, tabPage, spacing: Margin);
            _gameLoaderEditor.Add(typeof(WorldLoaderDropDown), new WorldLoaderDropDowner(worldNames));
            _gameLoaderEditor.CreateInterface();

            var gameRuleType = (ComboBox)_gameLoaderEditor.GetControl("GameRuleType");
            gameRuleType.ItemIndexChanged += (sender, args) => {
                var visible = gameRuleType.GetValue() == GameRuleType.Retirement.ToString();
                _gameLoaderEditor.GetControl("AgeCounterTarget").Visible = visible;
                _gameLoaderEditor.GetControl("AgeCounterTargetLabel").Visible = visible;
            };
            _gameLoaderEditor.GetControl("AgeCounterTarget").Text = "100";
        }
    }
}
