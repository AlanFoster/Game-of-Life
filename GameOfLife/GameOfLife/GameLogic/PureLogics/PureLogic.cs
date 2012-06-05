using System;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate.FSM;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.PureLogics {
    [DataContract]
    public abstract class PureLogic : IGameState {
        public virtual String GetGraphicLocation() {
            return null;
        }

        public abstract String Description { get; }

        public abstract IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo);

        public virtual void Initialize() { }
    }
}
