using GameOfLife.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.BoilerPlate.HUD {
    class TimedWidget : TextWidget {
        private int _timer;

        public TimedWidget(GameInfo gameInfo, int timer, GetText func, HUDAlignment hudAlignment = HUDAlignment.Center)
            : base(gameInfo, hudAlignment, func) {
            _timer = timer;
            
        }

        public override void Update(GameTime gameTime) {
            _timer -= gameTime.ElapsedGameTime.Milliseconds;
            if (_timer <= 0) {
                NeedsRemoved = true;
            }
        }
    }
}
