
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.Data;
using GameOfLife.GameLogic.Assets;
using GameOfLife.GameLogic.GameStates.Factories;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace GameOfLife.GameLogic.GameStates {
    public class TakeLoan : IGameState {
        private Player Player { get; set; }
        private Loan Loan { get; set; }
        private int LoanRequired { get; set; }

        public TakeLoan(Player player, int newCash) {
            Player = player;
            LoanRequired = 0 - newCash;
            Loan = new Loan(-LoanRequired);
            player.Accept(Loan);
            player.Cash += LoanRequired;
        }

        public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            var waitState = StateFactory.GetState(GameStates.Wait);
            var alert = new Alert(gameInfo.Manager,
                string.Format("{3} you are now in debt!\n" +
                    "Your original loan was for ${0:N0}\n" +
                    "But with current interest rates at {1}%" +
                    "\nYou must pay back ${2:n0}", LoanRequired, Loan.CurrentInterestRate, Loan.Value * -1, Player.Name),
                    "You required a loan", icon:
                    "Images/AlertIcons/Wallet");
            alert.Init();
            alert.Closing += (sender, args) => gameInfo.Fsm.Remove(waitState);
            gameInfo.Manager.Add(alert);
            alert.SendToBack();
            return new[] { waitState };
        }
    }
}
