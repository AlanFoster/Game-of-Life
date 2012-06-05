using System;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Factories {
    public class ChooseDirectionFactory : IStateFactory {
        private Lazy<ChooseDirectionLogic> singleton = new Lazy<ChooseDirectionLogic>();

        public IGameState GetState(GameStates gameStates) {
            return singleton.Value;
        }

        private sealed class ChooseDirectionLogic : IGameState {
            public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
                var availablePaths = gameInfo.CurrentPlayer.CurrentNode.LinksTo;
                var waitState = StateFactory.GetState(GameStates.Wait);

                ChoosePathHelper.TravelToNodes(availablePaths, gameInfo, (node, location) => PathChosen(node, gameInfo, waitState));
                gameInfo.ShowDirections();
                return new[] { waitState };
            }

            private void PathChosen(Node selectedNode, GameInfo gameInfo, IGameState waitState) {
                var currentPlayer = gameInfo.CurrentPlayer;
                currentPlayer.NextNode = selectedNode;
                gameInfo.Fsm.Remove(waitState);
            }
        }
    }
}
