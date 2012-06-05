using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.Data;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Actual {
    public class FadeOutWorld : IGameState {
        private readonly List<WorldObject> _worldObjects;

        public FadeOutWorld(List<WorldObject> worldObjects) {
            _worldObjects = worldObjects;
        }


        private float _opacity = 1;
        public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            _opacity -= Constants.Graphical.FadeValue;
            foreach (var worldObject in _worldObjects) {
                worldObject.Opacity = _opacity;
            }
            return _opacity >= 0 ? new IGameState[] { this } : null;
        }
    }
}
