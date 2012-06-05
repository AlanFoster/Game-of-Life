using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.GameLogic;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.BoilerPlate.HUD {
    class AgeCounterWidget : GraphicalWidget {
        public AgeCounterWidget(GameInfo gameInfo, HUDAlignment hudAlignment)
            : base(gameInfo, hudAlignment, i => String.Format("Current Years {0}/{1} ", i.AgeCounterCurrent, i.AgeCounterTarget), null) {
        }

        public override void Initialize() {
            base.Initialize();
            BaseTexture = Content.Load<Texture2D>("Images/HUD/msgBar");
        }
    }
}
