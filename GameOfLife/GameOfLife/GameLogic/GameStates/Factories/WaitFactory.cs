using System;
using GameOfLife.BoilerPlate.FSM;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Factories {
    public class WaitFactory : IStateFactory {
        // Always returns a new instance
        public IGameState GetState() {
            return new WaitLogic();
        }

        /// <summary>
        /// Always returns itself.
        /// This means it will have to explicitly be popped off the FSM by an external system.
        /// </summary>
        private sealed class WaitLogic : IGameState {
            public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
                return new IGameState[] { this };
            }
        }
    }

}
