using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GameOfLife.WorldObjects;

namespace GameOfLife.WorldEditing.Misc {
    public class NodeList : Collection<Node> {
        private Action<Node> _callback;
        private List<WorldObject> worldObjects;

        public NodeList(List<WorldObject> worldObjects) {
            this.worldObjects = worldObjects;
            foreach (var node in worldObjects.OfType<Node>()) {
                Items.Add(node);
            }
        }

        public void AddCallback(Action<Node> callback) {
            _callback = callback;
            foreach (var node in worldObjects.OfType<Node>()) {
                _callback(node);
            }
        }

        protected override void InsertItem(int index, Node item) {
            if (_callback != null) {
                _callback(item);
            }
            worldObjects.Add(item);
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index) {
            worldObjects.Remove(this[index]);
            base.RemoveItem(index);
        }
    }

}
