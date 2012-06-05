using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace GameOfLife.WorldEditing.Tools {
    class Help : Tool {
        public override bool Initialize() {
            CreateAlert("foo bar", null);
            return false;
        }

        public override void Apply(WorldObject worldObject) { }
        public override void Remove(WorldObject worldObject) { }
        public override void Update(GameTime gameTime) { }
    }
}
