using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.WorldEditing;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.PureLogics {
    // This PureLogic does absolutely nothing when executed.
    [DataContract]
    [Editable("None")]
    class Nothing : PureLogic {
        public override string Description {
            get { return "Do\nNothing"; }
        }

        public override IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            return null;
        }
    }
}
