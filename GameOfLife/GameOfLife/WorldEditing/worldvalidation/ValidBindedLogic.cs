using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.GameLogic.PureLogics;
using GameOfLife.WorldObjects;

namespace GameOfLife.WorldEditing.worldvalidation
{
    public class ValidBindedLogic : IWorldValidator
    {
        public WorldValidationResult ValidateWorld(List<WorldObject> worldObjects)
        {
            var incorrectNode =
                worldObjects.OfType<Node>().FirstOrDefault(node => node.BindedLogic.PureLogic is Nothing);
            if (incorrectNode != null)
            {
                return new WorldValidationResult(true, "All nodes must be assigned a logic type.");
            }
            return new WorldValidationResult(true);
        }
    }
}
