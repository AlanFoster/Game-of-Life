using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.ScreenManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.Screens {
    class Background : GameLayer {
        private readonly Vector2 _cloudSpeed = new Vector2(-1f, 0);
        private Texture2D _background, _cloud;
        private Vector2 _cloudsLocation1, _cloudsLocation2;

        public Background(string layerName)
            : base(layerName) {
        }

        public override void LoadContent() {
            base.LoadContent();
            _background = Content.Load<Texture2D>("Images/background/background");
            _cloud = Content.Load<Texture2D>("Images/background/clouds");
            _cloudsLocation1 = Vector2.Zero;
            _cloudsLocation2 = new Vector2(_cloud.Width, 0);
        }
        public override void Update(GameTime gameTime) {
            _cloudsLocation1 += _cloudSpeed;
            _cloudsLocation2 += _cloudSpeed;
            if (_cloudsLocation1.X + _cloud.Width <= 0) _cloudsLocation1.X = _cloudsLocation2.X + _cloud.Width;
            if (_cloudsLocation2.X + _cloud.Width <= 0) _cloudsLocation2.X = _cloudsLocation1.X + _cloud.Width;
        }

        public override void Draw(GameTime gameTime) {
            SpriteBatch.Begin();
            SpriteBatch.Draw(_background, Vector2.Zero, Color.White);
            SpriteBatch.Draw(_cloud, _cloudsLocation1, Color.White);
            SpriteBatch.Draw(_cloud, _cloudsLocation2, Color.White);

            SpriteBatch.End();
        }
    }
}
