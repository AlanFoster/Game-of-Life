using System;
using System.Linq;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.Data;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.WorldEditing.Misc;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;
using Console = System.Console;

namespace GameOfLife.WorldEditing.Tools {
    [DataContract]
    public class RemoveNode : NodeTool {
        private NodeList _nodeList;

        public RemoveNode(NodeList nodeList) {
            _nodeList = nodeList;
        }

        public override void Apply(Node node) {
            node.ListenMouseDown += RemoveNodeLogic;
        }

        public override void Apply(WorldObject worldObject) {
            base.Apply(worldObject);
            if (worldObject is Spinner) {
                ((Spinner)worldObject).ListenMouseDown += RemoveSpinnerLogic;
            }
        }

        public override void Remove(WorldObject worldObject) {
            base.Remove(worldObject);
            if (worldObject is Spinner) {
                ((Spinner)worldObject).ListenMouseDown -= RemoveSpinnerLogic;
            }
        }

        private void RemoveNodeLogic(Node node, Vector2 location) {
            if (!IsValidNode(node) || node.Parents.Any(parent => !IsValidNode(parent))) return;

            if (node == CurrentlySelectedNode) {
                CurrentlySelectedNode = null;
            }

            _nodeList.Remove(node);
            // Update all nodes that point to this deleted node to no longer point to it
            foreach (var parentNode in node.Parents.ToList()) {
                parentNode.RemoveNode(node);
            }
            node.Parents.Clear();
        }

        private void RemoveSpinnerLogic(Spinner spinner, Vector2 location) {
            CreateAlert("You can not delete the spinner!", icon: "Images/AlertIcons/fail");

        }

        public override void Remove(Node node) {
            node.ListenMouseDown -= RemoveNodeLogic;
        }

        public override void Update(GameTime gameTime) { }
    }
}
