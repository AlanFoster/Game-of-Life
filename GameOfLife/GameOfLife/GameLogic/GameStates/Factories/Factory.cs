using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.FSM;

namespace GameOfLife.GameLogic.GameStates.Factories {
    public class Factory : IStateFactory {
        private IGameState _gameState;
        public Factory(IGameState gameState) {
            _gameState = gameState;
        }

        public IGameState GetState() {
            return _gameState;
        }
    }
}
