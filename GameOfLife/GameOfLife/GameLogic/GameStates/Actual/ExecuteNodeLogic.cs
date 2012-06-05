using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Actual
{
     public sealed class ExecuteLogic : IGameState {
            public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
                var currentPlayer = gameInfo.CurrentPlayer;

                var currentNode = currentPlayer.CurrentNode;
                var returnedGameStates = new List<IGameState> { StateFactory.GetState(GameStates.EndExecute) };

                // Force the current player's move to be zero if the current node 'HasStopLogic'
                // IE the red tiles.
                if(currentNode.IsStopSquare) {
                    currentPlayer.RollAmount = 0;
                }

                if (currentNode.HasPassingLogic || currentNode.IsStopSquare || currentPlayer.RollAmount <= 0) {
                    returnedGameStates.Add(currentNode.BindedLogic);
                }

                return returnedGameStates.ToArray();
            }
        }

}
