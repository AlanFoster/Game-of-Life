using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.BoilerPlate.ScreenManager;
using GameOfLife.Data;
using GameOfLife.WorldEditing;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;
using EventHandler = TomShane.Neoforce.Controls.EventHandler;

namespace GameOfLife.Screens {
    public class WorldEditorSetup : GameLayer {
        private Panel NewWorldPanel { get; set; }
        private Panel ExistingWorldPanel { get; set; }
        private String[] WorldNames { get; set; }

        public WorldEditorSetup()
            : base(Constants.ScreenNames.WorldEditorSetup) {
        }

        public override void Initialize() {
            base.Initialize();
            const int Padding = 50;
            int top = Padding;
            const int buttonWidth = 200;
            const int buttonHeight = 50;

            var menuInfos = new[] {
                new Tuple<string, EventHandler>("Load Existing World", (sender, args) =>ShowExistingWorld()),
                    new Tuple<string, EventHandler>("Create New World", (sender, args) => ShowNewWorld()),
                    new Tuple<string, EventHandler>("Return", (sender, args) => ScreenManager.SwapScreens(this, Constants.ScreenNames.MainMenu))
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

        public override void Added() {
            base.Added();
            WorldLoader.instance.GetCustomWorldNames(CreateInterfaces);
        }

        private void ShowNewWorld() {
            ExistingWorldPanel.Hide();
            NewWorldPanel.Show();
            NewWorldPanel.GetControl("worldName").Focused = true;
        }

        private void ShowExistingWorld() {
            ExistingWorldPanel.Show();
            NewWorldPanel.Hide();
        }

        private void CreateInterfaces(String[] worldNames) {
            WorldNames = worldNames;
            NewWorldPanel = CreateNewWorldInterface();
            ExistingWorldPanel = CreateExistingWorldInterface(worldNames);

            ShowExistingWorld();
        }

        private Panel CreateNewWorldInterface() {
            var panel = new Panel(ControlManager.Manager) { Width = 856, Height = 467, AutoScroll = true, Left = 300, Top = 50 };
            panel.Init();
            ControlManager.Add(panel);

            var newNameLabel = new Label(ControlManager.Manager) { Text = "New World Name :: ", Width = 200, Left = 16, Top = 16, Parent = panel };
            var newName = new TextBox(ControlManager.Manager) { Name = "worldName", Parent = panel, Top = 16, Left = newNameLabel.Left + newNameLabel.Width, Width = 200 };
            newName.Init();

            var createWorld = new Button(ControlManager.Manager) { Text = "Create world", Top = 200, Left = 100, Width = 200, Parent = panel };
            createWorld.Init();
            createWorld.Click += (sender, args) => CreateNewWorld(newName.Text);

            return panel;
        }

        private Panel CreateExistingWorldInterface(String[] worldNames) {
            var panel = new Panel(ControlManager.Manager) { Width = 856, Height = 467, AutoScroll = true, Left = 300, Top = 50 };
            panel.Init();
            ControlManager.Add(panel);

            if (!worldNames.Any()) {
                var label = new Label(ControlManager.Manager) {
                    Height = 50,
                    Left = 20,
                    Width = 400,
                    Text =
                        "There are no existing worlds to edit!\nSelect the Create New World tab to create one."
                };

                panel.Add(label);


                return panel;
            }

            var existingWorldLabel = new Label(ControlManager.Manager) { Text = "Existing World :: ", Width = 200, Left = 16, Top = 16, Parent = panel };

            var worldNamesSelection = new ComboBox(ControlManager.Manager) { Width = 200, Parent = panel, Top = 16, Left = existingWorldLabel.Left + existingWorldLabel.Width + 20 };
            worldNamesSelection.Items.AddRange(worldNames);
            worldNamesSelection.Init();
            worldNamesSelection.ItemIndex = 0;
            worldNamesSelection.Left = existingWorldLabel.Left + existingWorldLabel.Width + 16;


            panel.Add(worldNamesSelection);

            var beginWorldEditor = new Button(ControlManager.Manager) { Text = "Load world", Top = 200, Left = 100, Width = 200 };
            beginWorldEditor.Init();
            beginWorldEditor.Click += (sender, args) => LoadExistingWorld(worldNamesSelection.Text);
            panel.Add(beginWorldEditor);

            return panel;
        }

        private void CreateNewWorld(String worldName) {
            // Check the world doesn't already exist and isn't the default world name
            if (WorldNames.Contains(worldName) && worldName != Constants.Locations.DefaultWorldName) {
                var alert = new Alert(ControlManager.Manager, "Sorry a world with that name already exists!", "Can not create this world");
                ControlManager.Add(alert);
                return;
            } else if (worldName == string.Empty) {
                var alert = new Alert(ControlManager.Manager, "The world name can not be empty!", "Can not create this world");
                ControlManager.Add(alert);
                return;
            }

            WorldLoader.instance.LoadWorld(Constants.Locations.DefaultWorldPath, i => CreateWorldEditor(worldName, i));
        }

        private void LoadExistingWorld(String worldName) {
            WorldLoader.instance.LoadCustomWorld(worldName, worldObjects => CreateWorldEditor(worldName, worldObjects));
        }

        private void CreateWorldEditor(String worldName, List<WorldObject> worldObjects) {
            if (worldObjects == null) {
                var alert = new Alert(ControlManager.Manager, "That world has become corrupt!\nPlease choose or create a different world", "Invalid world!");
                ControlManager.Add(alert);
            } else {
                ScreenManager.AddGameLayer(new WorldEditor(worldName, worldObjects), true);
                ScreenManager.HideScreen(this);
            }
        }

        public override void Update(GameTime gameTime) { }
        public override void Draw(GameTime gameTime) { }
    }
}
