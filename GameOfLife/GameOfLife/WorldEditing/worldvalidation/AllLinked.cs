using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.GameLogic.Assets;
using GameOfLife.GameLogic.PureLogics;
using GameOfLife.WorldEditing.Tools;
using GameOfLife.WorldObjects;

namespace GameOfLife.WorldEditing.worldvalidation
{
    public class AllLinked : IWorldValidator
    {
        public WorldValidationResult ValidateWorld(List<WorldObject> worldObjects)
        {
            // All modes must be connected to eachother
            // The only exception is if it is the starting island
            var incorrectNodes =
               worldObjects.OfType<Node>().Where(node => node.LinksTo.Count == 0 && !(node.BindedLogic.PureLogic is Travel && ((Travel) node.BindedLogic.PureLogic).IslandType == IslandType.StartIsland));
            if (incorrectNodes.Any())
            {
                return new WorldValidationResult(false, "All nodes must be Linked to eachother.\nPlease use the LinkNodes tool.", incorrectNodes.First());
            }
            return new WorldValidationResult(true);
        }
    }
}
