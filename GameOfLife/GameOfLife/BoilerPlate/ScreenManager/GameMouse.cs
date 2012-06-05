using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GameOfLife.BoilerPlate.ScreenManager {
    public class GameMouse {
        private static GameMouse _instance;
        public ButtonState LeftButton { get; set; }
        public ButtonState RightButton { get; set; }
        
        public static GameMouse GetInstance() {
            return _instance ?? (_instance = new GameMouse());
        }

        public static GameMouseState GetState() {
            return GetInstance().Moo();
        }

        private GameMouseState Moo() {
            var mouseState = Mouse.GetState();
            return new GameMouseState(mouseState.ScrollWheelValue, mouseState.X, mouseState.Y, LeftButton, RightButton);
        }
    }
}
