using System;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.GameLogic.PureLogics;
using GameOfLife.WorldEditing;
using GameOfLife.WorldEditing.EditSystems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.GameLogic.Nodes {
    [DataContract]
    public class BindedLogic : IGameState {
        // No longer a datamember, or editable. Set explicitly by the PureLogic that it holds.
        // [DataMember]
       // [Editable("Description", typeof(MultilineStringEditSystem))]
       // public String DescriptionText { get; protected set; }
        public String DescriptionText { get { return PureLogic == null ? "Error" : PureLogic.Description; } }


        [DataMember]
        [Editable("Call logic when player passes")]
        public bool HasPassLogic { get; private set; }

        [DataMember]
        [Editable("End player's turn when passed")]
        public bool IsStopSquare { get; private set; }

        [DataMember]
        public bool HasBeginningLogic { get; set; }


        [DataMember]
        public PureLogic PureLogic { get; set; }

        public String GraphicLocation {
            get { return PureLogic == null ? null : PureLogic.GetGraphicLocation(); }
        }

        public BindedLogic(PureLogic purelogic, bool hasPassLogic = false, bool hasBeginingLogic = false, bool isStopSquare = false) {
            HasPassLogic = hasPassLogic;
            PureLogic = purelogic;
            HasBeginningLogic = hasBeginingLogic;
            IsStopSquare = isStopSquare;
        }

        public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            return PureLogic.PerformLogic(gameTime, gameInfo);
        }

        public bool HasPureLogic() {
            return PureLogic != null;
        }

        public void Initialize() {
            PureLogic.Initialize();
        }

        public override string ToString() {
            return String.Format("HasPassLogic : {0}\n\tisStopSquare : {1}\n\tDescriptionText : \"{2}\"\n\tpure logic : {3}", HasPassLogic, IsStopSquare, DescriptionText, PureLogic);
        }
    }
}
