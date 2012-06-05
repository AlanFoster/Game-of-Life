using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Actual
{
    public sealed class EndOfExecutionLogic : IGameState
    {
        public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo)
        {
            var currentPlayer = gameInfo.CurrentPlayer;
            return new[] { currentPlayer.RollAmount <= 0 ? StateFactory.GetState(GameStates.Roll) : StateFactory.GetState(GameStates.ChooseNextNode) };
        }
    }
}
