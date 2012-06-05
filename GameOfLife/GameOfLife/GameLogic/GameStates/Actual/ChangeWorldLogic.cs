using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.Data;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Actual
{
    public sealed class ChangeWorld : IGameState
    {
        private readonly List<Node> _changeableNode;
        private readonly List<Node> _houseNodes;

        public ChangeWorld(List<Node> changeableNodes, List<Node> houseNodes)
        {
            _changeableNode = changeableNodes;
            _houseNodes = houseNodes;
        }

        public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo)
        {
            var waitState = StateFactory.GetState(GameStates.Wait);
            var alert = new Alert(gameInfo.Manager,
                "You have just spun a 1!\n" +
                "The world will now begin to change!\n\n" +
                "This will affect both house prices\n" +
                "And the logic in the world tiles!\n",
                "The world is going to change!", icon: Constants.ImageIcons.SpinToWin);
            gameInfo.Manager.Add(alert);
            alert.Closed += (sender, args) => gameInfo.Fsm.Remove(waitState);
            gameInfo.CreateMessage("The world is changing!");

            return new[] {
                    new FadeInWorld(_changeableNode.Concat(_houseNodes).Cast<WorldObject>().ToList()),
                    new ChangeWorldLogic(_changeableNode, _houseNodes),
                    new FadeOutWorld(_changeableNode.Concat(_houseNodes).Cast<WorldObject>().ToList()),
                    waitState
                };
        }
    }
}
