using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.Screens;
using GameOfLife.WorldEditing.MouseListeners;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace GameOfLife.WorldEditing.Tools {
    public class SelectObject : NodeTool {
        public override void Apply(WorldObject worldObject) {
            base.Apply(worldObject);

            if (worldObject is Spinner) {
                ((Spinner)worldObject).ListenMouseDown += SelectWorldObject;
            }
        }

        public override void Remove(WorldObject worldObject) {
            base.Remove(worldObject);

            if (worldObject is Spinner) {
                ((Spinner)worldObject).ListenMouseDown -= SelectWorldObject;
            }
        }

        public override void Apply(Node node) {
            node.ListenMouseDown += SelectWorldObject;
        }

        public override void Remove(Node node) {
            node.ListenMouseDown -= SelectWorldObject;
            node.ListenMouseDown -= SelectWorldObject;
            node.ListenMouseDown -= SelectWorldObject;
        }

        private void SelectWorldObject(WorldObject worldObject, Vector2 location)
        {
            if (!IsValidNode(worldObject as Node)) return;

            SetWorldObject(worldObject);
        }

        public override void Update(GameTime gameTime) {
        }
    }

}
