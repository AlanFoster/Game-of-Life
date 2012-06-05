using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.Screens;
using GameOfLife.WorldEditing;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.PureLogics {
    [Editable("Sue Other Player")]
    [DataContract]
    class ChoosePlayerCash : PureLogic {
        [DataMember]
        [Editable("Amount to sue for")]
        private int CashValue { get; set; }

        private MouseDown<Player> _clickPlayer;

        public ChoosePlayerCash(int cashValue) {
            CashValue = cashValue;
        }

        public override string Description {
            get { return String.Format("Sue Player\nfor\n${0:N0}", CashValue); }
        }

        public override IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            var waitState = StateFactory.GetState(GameStates.GameStates.Wait);

            var alert = new Alert(gameInfo.Manager, "Lawsuit!\n\nChoose a player to sue by clicking on their vehicle!", "Sue Player", icon: "Images/AlertIcons/money");
            alert.Closed += (sender, args) => CreateDelegates(gameInfo, waitState);
            gameInfo.Manager.Add(alert);

            return new[] { waitState };
        }

        private void CreateDelegates(GameInfo gameInfo, IGameState waitState) {
            _clickPlayer = (p, location) => PlayerClicked(p, location, gameInfo, waitState);
            foreach (var player in gameInfo.PlayerList.Where(i => i != gameInfo.CurrentPlayer)) {
                player.ListenMouseDown += _clickPlayer;
                player.ListenMouseOver += PlayerMouseIn;
                player.ListenMouseOut += PlayerMouseOut;
                gameInfo.AddLowPriorityTarget(player);
            }
        }

        private void PlayerMouseIn(Player player, Vector2 location) {
            player.SetColorState(ColorState.DarkDark);
        }

        private void PlayerMouseOut(Player player, Vector2 location) {
            player.ResetColorState();
        }

        private void PlayerClicked(Player clickedPlayer, Vector2 location, GameInfo gameInfo, IGameState waitState) {
            //Cash will be entered negative, so "minus-minus" for you,
            //"plus-minus for the player you sued" 
            clickedPlayer.Cash += CashValue;
            gameInfo.CurrentPlayer.Cash -= CashValue;

            foreach (var player in gameInfo.PlayerList) {
                player.ListenMouseDown -= _clickPlayer;
                player.ListenMouseOver -= PlayerMouseIn;
                player.ListenMouseOut -= PlayerMouseOut;
                player.ResetColorState();
            }
            gameInfo.ClearTargets();

            //Alert will be appear to confirm you sued them
            var alert = new Alert(gameInfo.Manager, String.Format("You successfully sue {0}!", clickedPlayer.Name), String.Format("Sue {0}!", clickedPlayer.Name), icon : "Images/AlertIcons/Loan");
            gameInfo.Manager.Add(alert);
            alert.Closed += (sender, args) => gameInfo.Fsm.Remove(waitState);
        }
    }
}
