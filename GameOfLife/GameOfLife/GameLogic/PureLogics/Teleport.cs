using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.GameLogic.GameStates;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.PureLogics {
    public class Teleport : IGameState {
        public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            var availablePaths = gameInfo.WorldNodes;
            var waitState = StateFactory.GetState(GameStates.GameStates.Wait);

            ChoosePathHelper.TravelToNodes(availablePaths, gameInfo, (node, location) => PathChosen(node, gameInfo, waitState), false);
            return new[] { waitState };
        }

        private void PathChosen(Node selectedNode, GameInfo gameInfo, IGameState waitState) {
            gameInfo.PanCameraToObject(gameInfo.CurrentPlayer);
            var currentPlayer = gameInfo.CurrentPlayer;
            // Teleport them
            currentPlayer.CurrentNode = selectedNode;
            currentPlayer.Location = selectedNode.GetTravelPosition(gameInfo.CurrentPlayerIndex,
                                                                    gameInfo.TotalPlayers);
            gameInfo.Fsm.Remove(waitState);
        }
    }
}
