using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.Misc;
using GameOfLife.GameLogic.Assets;
using GameOfLife.GameLogic.PureLogics;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Actual {
    public class ChangeWorldLogic : IGameState {
        private readonly List<Node> _changeableWorldNodes;
        private readonly List<Node> _houseNodes; 

        public ChangeWorldLogic(List<Node> changeableWorldNodes, List<Node> houseNodes) {
            _changeableWorldNodes = changeableWorldNodes;
            _houseNodes = houseNodes;
        }

        public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            // Get changable nodes, but don't change houses
            var changeableNodes = _changeableWorldNodes.Where(i => !(i.BindedLogic.PureLogic is BuyHouse));

            foreach (var node in changeableNodes) {
                var newLogic = gameInfo.BindedLogicPool[RandomHelper.Next(gameInfo.BindedLogicPool.Count)];
                node.SetBindedLogic(newLogic);
            }

            var houseLogics = _houseNodes.Select(i => i.BindedLogic.PureLogic).OfType<BuyHouse>();

            foreach (var houseLogic in houseLogics) {
                houseLogic.House.ChangePrice(RandomHelper.NextFloat(0.7f, 1.3f));
            }

            Loan.ChangeInterestRates();

            return null;
        }
    }
}
