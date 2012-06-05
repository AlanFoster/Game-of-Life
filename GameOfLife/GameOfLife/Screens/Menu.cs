using System;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.BoilerPlate.ScreenManager;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace GameOfLife.Screens {
    public class Menu : GameLayer {
        private String[] ScreensNames { get; set; }
        private readonly bool ExitButton;

        public Menu(String menuName, params String[] screenNames)
            : base(menuName) {
            ScreensNames = screenNames;
        }

        public Menu(String menuName, bool exitButton, params String[] screenNames)
            : this(menuName, screenNames) {
            ExitButton = exitButton;
        }

        public override void Initialize() {
            base.Initialize();
            var top = 50;
            foreach (var screenName in ScreensNames) {
                var button = new Button(ControlManager.Manager) { Text = screenName, Left = 50, Top = top, Width = 200, Height = 50 };
                button.Init();
                var label1 = screenName;
                button.Click += (sender, args) => OpenScreen(label1);
                ControlManager.Add(button);
                top += 100;
            }

            if (ExitButton) {
                var button = new Button(ControlManager.Manager) { Text = "Exit Game", Left = 50, Top = top, Width = 200, Height = 50 };
                button.Init();
                button.Click += (sender, args) => Application.MainWindow.Close();
                ControlManager.Add(button);
            }
        }

        private void OpenScreen(String label) {
            if (ScreenManager.ShowScreen(label)) {
                ScreenManager.HideScreen(LayerName);
            } else {
                ControlManager.Add(new Alert(ControlManager.Manager, "Not implemented"));
            }
        }

        public override void LoadContent() {
        }

        public override void UnloadContent() {

        }

        public override void Update(GameTime gameTime) {

        }

        public override void Draw(GameTime gameTime) {
        }
    }
}
