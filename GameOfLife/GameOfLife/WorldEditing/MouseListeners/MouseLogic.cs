using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;

namespace GameOfLife.WorldEditing.MouseListeners {
    public abstract class MouseLogic {
        public void Add(Node node) {
            if (this is Highlight) {
                Console.WriteLine("highlight logic fired!");
            }

            Remove(node);

            node.ListenMouseDown += OnMouseDown;
            node.ListenMouseOut += OnMouseOut;
            node.ListenMouseOver += OnMouseOver;
            node.ListenMouseUp += OnMouseUp;
        }

        public void Remove(Node node) {
            node.ListenMouseDown -= OnMouseDown;
            node.ListenMouseOut -= OnMouseOut;
            node.ListenMouseOver -= OnMouseOver;
            node.ListenMouseUp -= OnMouseUp;
        }


        protected virtual void OnMouseOver(Node node, Vector2 location) {
        }

        protected virtual void OnMouseOut(Node node, Vector2 location) {
        }

        protected virtual void OnMouseDown(Node node, Vector2 location) {
        }

        protected virtual void OnMouseUp(Node node, Vector2 location) {
        }
    }
}
