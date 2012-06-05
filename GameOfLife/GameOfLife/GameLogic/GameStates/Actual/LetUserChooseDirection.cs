using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.GameLogic.GameStates.Factories;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Actual {
    public sealed class LetUserChooseDirection : IGameState {
        public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            var availablePaths = gameInfo.CurrentPlayer.CurrentNode.LinksTo;
            var waitState = StateFactory.GetState(GameStates.Wait);

            ChoosePathHelper.TravelToNodes(availablePaths, gameInfo, (node, location) => PathChosen(node, gameInfo, waitState));
            return new[] { waitState };
        }

        private void PathChosen(Node selectedNode, GameInfo gameInfo, IGameState waitState) {
            gameInfo.ClearTargets();
            gameInfo.PanCameraToObject(gameInfo.CurrentPlayer);
            var currentPlayer = gameInfo.CurrentPlayer;            
            currentPlayer.NextNode = selectedNode;
            gameInfo.Fsm.Remove(waitState);
        }
    }
}
