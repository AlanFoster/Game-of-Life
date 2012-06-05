using System.Linq;
using GameOfLife.Core.HUD;
using GameOfLife.GameLogic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.BoilerPlate.HUD {
    public class TextWidget : HUDWiget {
        protected readonly GameInfo GameInfo;

        public delegate string GetText(GameInfo gameInfo);

        private readonly GetText _getTextFunc;

        public TextWidget(GameInfo gameInfo, HUDAlignment hudAlignment, GetText func)
            : base(hudAlignment) {
            GameInfo = gameInfo;
            _getTextFunc = func;
        }

        public override Vector2 Size {
            get { return Font.MeasureString(_getTextFunc(GameInfo).Replace('\t', ' ')); }
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 drawLocation) {
            var drawText = _getTextFunc(GameInfo).Replace('\t', ' ');
            spriteBatch.DrawString(Font, drawText, drawLocation, Color.White);
        }
    }
}
