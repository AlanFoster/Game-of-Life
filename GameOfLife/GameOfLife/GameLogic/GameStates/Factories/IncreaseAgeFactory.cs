using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Factories {
    class IncreaseAgeFactory : IStateFactory {
        private Lazy<IncreaseAgeCounter> singleton = new Lazy<IncreaseAgeCounter>();

        public IGameState GetState(GameStates gameStates) {
            return singleton.Value;
        }

        private sealed class IncreaseAgeCounter : IGameState {
            public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
                gameInfo.IncreaseAgeCounter();
                return null;
            }
        }
    }
}