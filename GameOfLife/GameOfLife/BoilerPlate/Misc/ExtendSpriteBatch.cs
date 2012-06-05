using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.BoilerPlate.Misc {
    // Uses extension methods to add new additional drawing functionality to the SpriteBatch object.
    // This will make life a lot easier for debugging..
    public static class ExtendSpriteBatch {
        public static void DrawDebugString(this SpriteBatch spriteBatch, SpriteFont font, String text, Vector2 location) {
            //   Vector2 fontOrigin = nodeFont.MeasureString(text) / 2;
            spriteBatch.DrawString(font, text,
                location, Color.Black, 0,
                Vector2.Zero, 1f, SpriteEffects.None, 0.2f);
        }

        public static void DrawDebugString(this SpriteBatch spriteBatch, SpriteFont font, String text) {
            DrawDebugString(spriteBatch, font, text, Vector2.One);
        }
    }
}