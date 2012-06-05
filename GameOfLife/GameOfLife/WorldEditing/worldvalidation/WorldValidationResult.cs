using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.WorldObjects;

namespace GameOfLife.WorldEditing.worldvalidation
{
    public struct WorldValidationResult
    {
        public String AlertReason { get; private set; }
        public bool Valid { get; private set; }
        public WorldObject InvalidNode { get; private set; }

        public WorldValidationResult(bool valid, String alertReason = null, WorldObject invalidNode = null)
            : this()
        {
            AlertReason = alertReason;
            Valid = valid;
            InvalidNode = invalidNode;
        }
    }
}
