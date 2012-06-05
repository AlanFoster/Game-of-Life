using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.GameLogic;

namespace GameOfLife.Screens {
    class TestLevel : Level {
        public TestLevel(GameInfo gameInfo)
            : base(gameInfo) {
        }

        protected override void CreateHUDMessage(string displayText)
        {
        }

        protected override void CreateHudSystem() {
        }
    }
}
