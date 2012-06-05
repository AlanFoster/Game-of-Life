using System;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.GameLogic.Assets;
using GameOfLife.WorldEditing;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.PureLogics {
    [DataContract]
    [Editable("Get A Pet")]
    public class GivePet : AddTransportable {
        public GivePet() {
            Initialize();
        }

        public override string Description {
            get { return "Get a\nPet"; }
        }

        protected override string SuccessText {
            get { return "You now own a pet!"; }
        }

        public override void Initialize() {
            Transportable = new Pet();
        }
    }
}
