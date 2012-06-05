using System;
using System.Runtime.Serialization;

namespace GameOfLife.GameLogic.Assets {
    [DataContract]
    public class Pet : Transportable {
        public Pet()
            : base("Pet") {
        }

        public override string Icon
        {
            get { return "Images/AlertIcons/Pet"; }
        }

        public override string AssetPath
        {
            get { return Icon; }
        }
    }
}
