using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.WorldEditing.Tools;
using GameOfLife.WorldObjects;

namespace GameOfLife.WorldEditing.worldvalidation {
    public interface IWorldValidator {
        WorldValidationResult ValidateWorld(List<WorldObject> worldObjects);
    }
}
