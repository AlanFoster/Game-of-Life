using System;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.Data;
using GameOfLife.WorldEditing;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.PureLogics {
    [Editable("Give Player Cash")]
    [DataContract]
    public class ModifyCashStory : PureLogic {
        public override string GetGraphicLocation() {
            return Constants.ImageIcons.Money;
        }

        public override string Description {
            get {
                return String.Format(AddCash >= 0 ? "Earn\n${0:N0}" : "Lose\n-${0:N0}", Math.Abs(AddCash));
            }
        }

        [Editable("Cash Given when landed on")]
        [DataMember]
        private int AddCash { get; set; }

        public ModifyCashStory(int addCash) {
            AddCash = addCash;
        }

        public override IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            gameInfo.CurrentPlayer.Cash += AddCash;

            var waitState = StateFactory.GetState(GameStates.GameStates.Wait);

            var alert = new Alert(gameInfo.Manager,
                String.Format(AddCash < 0 ? CashTileData.GetRandomBad() : CashTileData.GetRandomGood(), AddCash),
                "Cash Changed!",
                icon: "Images/AlertIcons/money");
            gameInfo.Manager.Add(alert);
            alert.Closed += (sender, obj) => gameInfo.Fsm.Remove(waitState);
            return new[] { waitState };
        }
    }
}
