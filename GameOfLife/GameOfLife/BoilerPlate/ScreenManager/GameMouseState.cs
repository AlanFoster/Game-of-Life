using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GameOfLife.BoilerPlate.ScreenManager {
    public struct GameMouseState {
        public readonly int ScrollWheelValue;
        public readonly int X;
        public readonly int Y;
        public readonly ButtonState LeftButton;
        public readonly ButtonState RightButton;

        public GameMouseState(int scrollWheelValue, int x, int y, ButtonState leftButton, ButtonState rightButton) {
            ScrollWheelValue = scrollWheelValue;
            Y = y;
            X = x;
            LeftButton = leftButton;
            RightButton = rightButton;
        }
    }
}
