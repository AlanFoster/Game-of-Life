using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.Data;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Actual {
    public class FadeInWorld : IGameState {
        private readonly List<WorldObject> _worldObjects;

        public FadeInWorld(List<WorldObject> worldObjects) {
            _worldObjects = worldObjects;
        }


        private float _opacity;
        public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            _opacity += Constants.Graphical.FadeValue;
            foreach (var worldObject in _worldObjects) {
                worldObject.Opacity = _opacity;
            }
            return _opacity <= 1f ? new IGameState[] { this } : null;
        }
    }
}
