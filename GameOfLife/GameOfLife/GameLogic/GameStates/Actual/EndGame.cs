﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Actual {
    public class EndGame : IGameState {
        private readonly Action _endGameCallback;
        public EndGame(Action endGameCallback) {
            _endGameCallback = endGameCallback;
        }

        public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            _endGameCallback();
            return new[] { StateFactory.GetState(GameStates.Wait) };
        }
    }
}
