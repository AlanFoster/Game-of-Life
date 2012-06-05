using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using GameOfLife.BoilerPlate;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.BoilerPlate.ScreenManager;
using GameOfLife.GameLogic;
using GameOfLife.WorldEdi;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;
using Console = System.Console;
using EventArgs = TomShane.Neoforce.Controls.EventArgs;



namespace GameOfLife.Screens {
    class Setup : GameLayer {
        const int Margin = 30;

        private const int minPlayers = 2;
        private const int maxPlayers = 6;
        private TabControl tabs;
        private TabPage gameInfoTab;
        const int padding = 50;
        private int playerCount;

        private ObjectEditor<Player>[] playerObjectEditors;

        public Setup()
            : base(Constants.ScreenNames.SetupGame) {
        }

        public override void Initialize() {
            base.Initialize();

            int top = padding;
            const int buttonWidth = 200;
            const int buttonHeight = 50;

            playerObjectEditors = new ObjectEditor<Player>[maxPlayers];

            tabs = new TabControl(ControlManager.Manager);
            tabs.Init();

            tabs.SetPosition(300, 50);

            int width = (int)(ScreenWidth * 0.75f);
            int height = (int)(ScreenHeight * 0.75f);

            tabs.MinimumHeight = height;
            tabs.MinimumWidth = width;

            var playGame = new Button(ControlManager.Manager) { Text = "Play Game", Left = padding, Top = padding, Width = buttonWidth, Height = buttonHeight};
            playGame.Init();
            playGame.Click += (sender, args) => StartGame();

            top += playGame.Height + padding;

            var addPlayer = new Button(ControlManager.Manager) { Text = "Add Player", Left = padding, Top = top, Width = buttonWidth, Height = buttonHeight };
            addPlayer.Init();
            addPlayer.Click += (sender, args) => AddPlayer();

            top += addPlayer.Height+padding;

            var removePlayer = new Button(ControlManager.Manager) { Text = "Remove Player", Left = padding, Top = top, Width = buttonWidth, Height = buttonHeight };
            removePlayer.Init();
            removePlayer.Click += (sender, args) => RemovePlayer();

            top += addPlayer.Height + padding;

            var backButton = new Button(ControlManager.Manager) { Text = "Main Menu", Left = padding, Top = top, Width = buttonWidth, Height = buttonHeight };
            backButton.Init();
            backButton.Click += (sender, args) => ScreenManager.SwapScreens(this, Constants.ScreenNames.MainMenu);

            ControlManager.Add(tabs);
            ControlManager.Add(addPlayer);
            ControlManager.Add(removePlayer);
            ControlManager.Add(playGame);
            ControlManager.Add(backButton);

            gameInfoTab = tabs.AddPage();
            PopulateGameInfo(gameInfoTab);

            for (var i = 0; i < minPlayers; i++) {
                AddPlayer();
            }
            SwitchToTab(0);
        }

        private void StartGame() {
            var playerList = new Player[playerCount];
            for (var i = 0; i < playerCount; i++) {
                var player = playerList[i] = playerObjectEditors[i].CreateInstance();
                player.InitializeContent(Content);
               Console.WriteLine(playerList[i].ToString());
            }

            var gameInfo = new GameInfo(TheGameOfLife.CreateBasicEmptyWorld(Content), playerList);
            var level = new Level(gameInfo);
            ScreenManager.AddGameLayer(level);
            ScreenManager.SwapScreens(this, level);
        }

        private void RemovePlayer() {
            if (playerCount > minPlayers && !tabs.SelectedPage.Text.Equals("Game Info")) {
                tabs.RemovePage(tabs.SelectedPage);
                playerCount--;
                SwitchToTab(playerCount);
            }
        }

        private void AddPlayer() {
            if (playerCount == maxPlayers) return;
            playerCount++;
            tabs.AddPage();
            PopulatePlayerPage(tabs.TabPages[playerCount]);
            SwitchToTab(playerCount);
        }

        private void SwitchToTab(int num) {
            tabs.SelectedIndex = 0;
            tabs.SelectedIndex = num;
        }

        public override void Update(GameTime gameTime) {
        }

        public override void Draw(GameTime gameTime) {
        }

        private void PopulatePlayerPage(TabPage tabPage) {
            const string playerName = "Unknown Player";
            tabPage.Text = playerName;
            var objectEditor = playerObjectEditors[playerCount - 1] = new ObjectEditor<Player>(ControlManager.Manager, tabPage, saveButton: false, spacing: Margin);
            objectEditor.CreateInterface(typeof(Player));

            var nameControl = objectEditor.GetControl("Name");
            nameControl.TextChanged += (sender, args) => tabPage.Text = (((TextBox)sender).Text);
            nameControl.Text = playerName;

            var cashControl = objectEditor.GetControl("Cash");
            cashControl.Enabled = false;
            cashControl.Text = Constants.GameRules.StartingCash.ToString(CultureInfo.InvariantCulture);

            // Add any additional controls!
            var endLocation = objectEditor.SizeY;
            var buttonTest = new Button(ControlManager.Manager) { Text = "Moar controls!", Top = endLocation, Left = Margin, Width = 200 };
            buttonTest.Init();
            tabPage.Add(buttonTest);
            endLocation += buttonTest.Height + Margin;

            tabPage.MinimumHeight = endLocation;
        }


        public void PopulateGameInfo(TabPage tabPage) {
            /*tabPage.Text = "Game Info";

            var gameType = GuiHelpers.AddComboBox(ControlManager.Manager, tabPage, "Select Game Type", "gameType", new string[] { "Passport Stamps", "Age Counter" }, 50, 200, 30, 130);

            var ageTextBox = GuiHelpers.AddInputRow(ControlManager.Manager, tabPage, "Age Limit", "AgeLimit", 100, 200, 30, -1);

            gameType.ItemIndexChanged += (sender, args) => ChangeGameType(ageTextBox, gameType.ItemIndex);

            string[] temp = { "Default", "Custom" };
            var worldSelection = GuiHelpers.AddComboBox(ControlManager.Manager, tabPage, "Select World", "SelectWorld", temp, 150, 200, 30, 130);*/
            var Padding = 20;
            var imageSelector = new ImageSelector(ControlManager.Manager, "1", "2", "3", "4", "5", "6", "7", "8", "9", "10") { Width = Convert.ToInt16(tabPage.Width / 1.5f),
                                                                                Height = tabPage.Height / 2 - Padding, Parent = tabPage, Left = 200, Top = 200, AutoScroll = true }; 
             imageSelector.Initialise();
        }

        private void ChangeGameType(TextBox ageLimit, int index) {
            if (index == 1) {
                ageLimit.Text = "";
                ageLimit.Enabled = true;
            } else {
                ageLimit.Text = "N/A";
                ageLimit.Enabled = false;
            }
        }
    }
}
