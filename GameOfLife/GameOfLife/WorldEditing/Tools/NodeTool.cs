using System;
using System.Runtime.Serialization;
using GameOfLife.Data;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.WorldObjects;

namespace GameOfLife.WorldEditing.Tools {
    [DataContract]
    public abstract class NodeTool : Tool {
        
        protected Node CurrentlySelectedNode {
            get { return GetWorldObject() as Node; }
            set { SetWorldObject(value); }
        }

        public override bool Initialize() {
            base.Initialize();

            return true;
        }

        public override void Apply(WorldObject worldObject) {
            if (worldObject is Node)
                Apply(worldObject as Node);
        }

        public override void Remove(WorldObject worldObject) {
            if (worldObject is Node)
                Remove(worldObject as Node);
        }

        protected bool IsValidNode(Node node) {
            if (node != null && node.IsEditable && !Constants.Editing.IsAdminMode) {
                CreateAlert(Constants.Editing.ErrorMessage, Constants.Editing.ErrorIcon);
                node.IsMouseDown = false;
                return false;
            }
            return true;
        }

        public abstract void Apply(Node node);
        public abstract void Remove(Node node);
    }
}
