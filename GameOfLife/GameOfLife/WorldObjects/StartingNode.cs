using System.Runtime.Serialization;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.GameLogic.PureLogics;
using Microsoft.Xna.Framework;

namespace GameOfLife.WorldObjects {
    [DataContract]
    class StartingNode : Node {
        public override string DescriptionText {
            get { return "Start Node"; }
        }

        public StartingNode(Vector2 location)
            : base(location, false) {

            BindedLogic = new BindedLogic(new Nothing());
        }

        public override void SetBindedLogic(BindedLogic bindedLogic) { }
    }
}
