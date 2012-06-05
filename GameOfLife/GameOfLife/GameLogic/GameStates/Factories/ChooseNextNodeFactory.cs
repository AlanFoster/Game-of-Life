using System;
using GameOfLife.BoilerPlate.FSM;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Factories {
    public class ChooseNextNodeFactory : IStateFactory {
        private readonly Lazy<ChooseNextNodeLogic> _singleton = new Lazy<ChooseNextNodeLogic>();

        public IGameState GetState() {
            return _singleton.Value;
        }

        private sealed class ChooseNextNodeLogic : IGameState {
            public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
                var currentPlayer = gameInfo.CurrentPlayer;
                if (currentPlayer.RollAmount > 0) {
                    // Reduce the player's roll value
                    currentPlayer.RollAmount--;
                    var linksToCount = currentPlayer.CurrentNode.LinksTo.Count;
                    if (linksToCount == 1) {
                        currentPlayer.NextNode = currentPlayer.CurrentNode.LinksTo[0];
                    } else {
                        return new[] { StateFactory.GetState(GameStates.Execute), StateFactory.GetState(GameStates.VisuallyMovePlayer), StateFactory.GetState(GameStates.LetUserChooseDirection) };
                    }
                    return new[] { StateFactory.GetState(GameStates.Execute), StateFactory.GetState(GameStates.VisuallyMovePlayer) };
                }
               
                return new[] { StateFactory.GetState(GameStates.VisuallyMovePlayer) };
            }
        }
    }
}
