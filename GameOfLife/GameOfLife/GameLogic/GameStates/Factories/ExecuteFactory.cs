using System;
using System.Collections.Generic;
using GameOfLife.BoilerPlate.FSM;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Factories {

    public class ExecuteFactory : IStateFactory {
        private Lazy<ExecuteLogic> singleton = new Lazy<ExecuteLogic>();

        public IGameState GetState(GameStates gameStates) {
            return singleton.Value;
        }

        private sealed class ExecuteLogic : IGameState {
            public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
                var currentPlayer = gameInfo.CurrentPlayer;

                var currentNode = currentPlayer.CurrentNode;
                var returnedGameStates = new List<IGameState> {StateFactory.GetState(GameStates.EndExecute)};



                if(currentNode.HasStopLogic) {
                    // TODO
                    currentPlayer.RollAmount = 0;
                }

                // TODO might not work anymore
                if (currentNode.HasPassingLogic || currentNode.HasStopLogic || currentNode.HasBeginningLogic || currentPlayer.RollAmount <= 1) {
                    returnedGameStates.Add(currentNode.BindedLogic);
                }

                return returnedGameStates.ToArray();
            }
        }

    }

}
