using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.Screens;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;

namespace GameOfLife.WorldEditing.Tools {
    public class ObjectDragger {
        private Vector2 _worldObjectStartingPosition;
        private Vector2 _clickingStartPosition;
        public WorldObject CurrentlySelectedObject { private get; set; }
        public bool Dragging { get; private set; }

        public void Update(GameTime gameTime) {
            if (CurrentlySelectedObject == null) return;

            if (CurrentlySelectedObject.IsMouseDown && !Dragging) {
                Dragging = true;
                _worldObjectStartingPosition = CurrentlySelectedObject.Location;
                _clickingStartPosition = World.MouseInWorld;
            }

            if (CurrentlySelectedObject.IsMouseDown) {
                var currentMousePosition = World.MouseInWorld;

                var newLocationVector =
                    new Vector2((int)((currentMousePosition.X - _clickingStartPosition.X) + _worldObjectStartingPosition.X),
                              (int)((currentMousePosition.Y - _clickingStartPosition.Y) + _worldObjectStartingPosition.Y));

                CurrentlySelectedObject.Location = newLocationVector;
            }

            Dragging = CurrentlySelectedObject.IsMouseDown;
        }
    }
}
