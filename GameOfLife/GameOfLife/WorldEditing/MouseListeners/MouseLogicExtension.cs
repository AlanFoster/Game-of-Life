using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.WorldObjects;

namespace GameOfLife.WorldEditing.MouseListeners {
    // Used to add convenience methods to the Node so that we can use node.AddMouseLogic(foo); instead of foo.Add(node);
    public static class MouseLogicExtension {
        public static void AddMouseLogic(this Node node, MouseLogic mouseLogic) {
            mouseLogic.Add(node);
        }

        public static void RemoveMouseLogic(this Node node, MouseLogic mouseLogic) {
            mouseLogic.Remove(node);
        }
    }
}
