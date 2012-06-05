using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.Data;
using GameOfLife.WorldEditing;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.PureLogics {
    [DataContract]
    public class ModifyCash : PureLogic {
        public override string GetGraphicLocation() {
            return Constants.ImageIcons.Money;
        }

        public override string Description {
            get {
                return String.Format(AddCash >= 0 ? "Earn\n${0:N0}" : "Lose\n-${0:N0}", Math.Abs(AddCash));
            }
        }
        [DataMember]
        private int AddCash { get; set; }

        public ModifyCash(int addCash) {
            AddCash = addCash;
        }

        public override IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            gameInfo.CurrentPlayer.Cash += AddCash;
            return null;
        }
    }
}
