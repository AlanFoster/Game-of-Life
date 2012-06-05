using System;
using GameOfLife.BoilerPlate.ScreenManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.Screens {
    class Logo : GameLayer {
        private float alpha;
        private float alphaSpeed;
        private Texture2D logo;

        public Logo(string layerName)
            : base(layerName) {
            alphaSpeed = 0.02f;
        }

        public override void LoadContent() {
            base.LoadContent();
            logo = Content.Load<Texture2D>("Images/background/logo");
        }

        public override void Update(GameTime gameTime) {
            alpha += alphaSpeed;
            if (alpha <= 0f || alpha >= 1f) {
                alphaSpeed *= -1;
            }
        }

        public override void Draw(GameTime gameTime) {
            SpriteBatch.Begin();

            var previousBlendState = GraphicsDevice.BlendState;
            GraphicsDevice.BlendState = new BlendState { ColorSourceBlend = Blend.DestinationColor, ColorDestinationBlend = Blend.SourceColor };

            SpriteBatch.Draw(logo, new Vector2(ScreenWidth/2 -logo.Width/2 + 110, ScreenHeight / 2 - logo.Height / 2), Color.LightBlue * alpha);

            GraphicsDevice.BlendState = previousBlendState;

            SpriteBatch.Draw(logo, new Vector2(ScreenWidth / 2 - logo.Width / 2 + 110, ScreenHeight / 2 - logo.Height / 2), Color.White);
            SpriteBatch.End();
        }
    }
}
