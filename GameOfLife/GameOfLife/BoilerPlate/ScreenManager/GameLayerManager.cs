using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace GameOfLife.BoilerPlate.ScreenManager {
    public class GameLayerManager {
        public CustomManager Manager { get; private set; }
        public int ScreenWidth { get { return Manager.ScreenWidth; } }
        public int ScreenHeight { get { return Manager.ScreenHeight; } }
        private Panel _Panel;
        public Vector2 Position { set { _Panel.SetPosition((int)value.X, (int)value.Y); } }
        public Vector2 Size { get { return new Vector2(_Panel.Width, _Panel.Height); } }

        public GameLayerManager(CustomManager manager) {
            Manager = manager;
            _Panel = new Panel(manager) { Color = Color.Transparent, Width = 1224, Height = 550, Parent = manager.MainWindow, Passive = true };
            _Panel.Init();
            manager.Add(_Panel);
        }

        public void Add(Control control) {
            control.Parent = _Panel;
            _Panel.Width = Math.Max(_Panel.Width, control.Left + control.Width);
            _Panel.Height = Math.Max(_Panel.Height, control.Top + control.Height);
            RecalculatePosition();
        }

        public void RecalculatePosition() {
            _Panel.SetPosition(ScreenWidth / 2 - _Panel.Width / 2, ScreenHeight / 2 - _Panel.Height / 2);
        }

        public void Remove(Control control) {
            _Panel.Remove(control);
        }

        public void SetChildren(Boolean active) {
            if (active) {
                RecalculatePosition();
                _Panel.Show();
                _Panel.BringToFront();
            } else {
                _Panel.Hide();
            }
        }

        public void Dispose() {
            Manager.Remove(_Panel);
        }
    }
}
