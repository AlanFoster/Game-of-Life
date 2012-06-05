using System;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.Screens;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;

namespace GameOfLife.WorldEditing.MouseListeners {
    public class Highlight : MouseLogic {
        protected override void OnMouseOver(Node node, Vector2 location) {
            node.SetColorState(ColorState.Dark);
        }

        protected override void OnMouseOut(Node node, Vector2 location) {
            node.ResetColorState();
        }

        protected override void OnMouseDown(Node node, Vector2 location) {
            node.SetColorState(ColorState.DarkDark);
        }

        protected override void OnMouseUp(Node node, Vector2 location) {
            node.SetColorState(ColorState.Dark);
        }
    }
}
