using System;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.Data;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.Screens;
using GameOfLife.WorldEditing.MouseListeners;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace GameOfLife.WorldEditing.Tools {
    [DataContract]
    public class LinkNode : NodeTool {
        private bool _resetRequired;

        public override bool Initialize() {
            base.Initialize();
            CurrentlySelectedNode = null;
            _resetRequired = true;
            return true;
        }

        public override void Apply(Node node) {
            node.ListenMouseDown += NodeClicked;
        }

        public override void Remove(Node node) {
            node.ListenMouseDown -= NodeClicked;
        }

        private void NodeClicked(Node node, Vector2 location) {
            if (!IsValidNode(node)) return;

            if (CurrentlySelectedNode == null || _resetRequired) {
                CurrentlySelectedNode = node;
                _resetRequired = false;
            } else if (CurrentlySelectedNode != null && CurrentlySelectedNode != node) {
                if (CurrentlySelectedNode.ContainsNode(node)) {
                    CurrentlySelectedNode.RemoveNode(node);
                    node.RememberColorState(ColorState.None);
                    node.RemoveParent(node);
                } else {
                    CurrentlySelectedNode.AddLinkedNode(node);
                    node.Parents.Add(CurrentlySelectedNode);
                    node.RememberColorState(ColorState.Glow);
                }

                CurrentlySelectedNode = CurrentlySelectedNode;
                _resetRequired = true;
            }
        }

        public override void Update(GameTime gameTime) {

        }
    }
}
