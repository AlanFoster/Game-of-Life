using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.GameLogic;
using GameOfLife.GameLogic.Assets;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.BoilerPlate.HUD
{
    public class PassportWidget : GraphicalWidget
    {
        private const int GapSize = 16;
        private Player currentPlayer;
        private Dictionary<IslandType, Texture2D> PassportStampImages;
        private Vector2 _size;
        public override Vector2 Size { get { return _size; } }

        public PassportWidget(GameInfo gameInfo, HUDAlignment hudAlignment)
            : base(gameInfo, hudAlignment, i => String.Empty, null)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            PassportStampImages = new Dictionary<IslandType, Texture2D>();
            int maxHeight = 0;
            int totalX = 0;
            var islandTypes = Enum.GetValues(typeof(IslandType)).Cast<IslandType>().ToArray();

            for (var i = 0; i < islandTypes.Count() - 1; i++)
            {
                var passportStamp = Content.Load<Texture2D>("Images/Node/Icons/Transport/" + islandTypes[i]);
                PassportStampImages[islandTypes[i]] = passportStamp;
                totalX += passportStamp.Width + GapSize;
                maxHeight = Math.Max(maxHeight, passportStamp.Height);
            }

            _size = new Vector2(totalX + GapSize, maxHeight + GapSize);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            currentPlayer = GameInfo.CurrentPlayer;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 drawLocation)
        {
            //base.Draw(spriteBatch, drawLocation);
            var islandTypes = Enum.GetValues(typeof(IslandType)).Cast<IslandType>().ToArray();
            drawLocation.Y += GapSize;
            for (var i = 0; i < islandTypes.Count() - 1; i++)
            {
                // Draw the passport
                var passportImage = PassportStampImages[islandTypes[i]];
                var hasToken = currentPlayer.HasStamp(islandTypes[i]);
                var opacity = hasToken ? 1.0f : 0.5f;
                spriteBatch.Draw(passportImage, drawLocation, Color.White * opacity);

                drawLocation.X += passportImage.Width + GapSize;
            }
        }
    }

}
