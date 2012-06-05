using System;
using GameOfLife.BoilerPlate.FSM;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Factories {
    public class EndExecuteFactory : IStateFactory {
        private Lazy<EndExecuteLogic> singleton = new Lazy<EndExecuteLogic>();
        public IGameState GetState(GameStates gameStates) {
            return singleton.Value;
        }

        private sealed class EndExecuteLogic : IGameState {
            public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
                var currentPlayer = gameInfo.CurrentPlayer;

                return new[] { currentPlayer.RollAmount <= 0 ? StateFactory.GetState(GameStates.Roll) : StateFactory.GetState(GameStates.ChooseNextNode) };
            }
        }
    }

}
