using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates {
    class TimedWait : IGameState {
        private int _milliseconds;
        public TimedWait(int milliseconds) {
            _milliseconds = milliseconds;
        }

        public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            _milliseconds -= gameTime.ElapsedGameTime.Milliseconds;
            return _milliseconds > 0 ? new IGameState[] { this } : null;
        }
    }
}
