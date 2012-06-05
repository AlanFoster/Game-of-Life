using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.Data;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Actual {
    class IncreaseAgeCounter : IGameState {
        public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            // Increase the age
            bool endWorld = gameInfo.IncreaseAgeCounter();

            var waitState = StateFactory.GetState(GameStates.Wait);
            var alert = new Alert(gameInfo.Manager, "You just spun a 10!\nThe world age increases!\n" + (endWorld ? "Retirement Age Reached!" : String.Empty),
                "World age increased",
                icon : Constants.ImageIcons.SpinToWin);
            gameInfo.Manager.Add(alert);
            alert.Closed += (sender, args) => gameInfo.Fsm.Remove(waitState);

            gameInfo.CreateMessage(endWorld ? "Retirement Age Reached!" : "The world age counter increases by 10!");
            return endWorld ? new[] { waitState, StateFactory.GetState(GameStates.EndGame) } : new[] { waitState };
        }
    }
}
