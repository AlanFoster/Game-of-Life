using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;
using GameOfLife.GameLogic;

namespace GameOfLife.BoilerPlate.HUD {
    public class GraphicalWidget : TextWidget {
        protected Texture2D BaseTexture;

        public GraphicalWidget(GameInfo gameInfo, HUDAlignment hudAlignment, GetText func, Texture2D baseTexture)
            : base(gameInfo, hudAlignment, func) {
                BaseTexture = baseTexture;
        }

        public override Vector2 Size {
            get { return new Vector2(BaseTexture.Width, BaseTexture.Height); }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 drawLocation) {
            spriteBatch.Draw(BaseTexture, drawLocation, Color.White);
            base.Draw(spriteBatch, drawLocation + Size / 2 - base.Size / 2);
        }
    }
}
