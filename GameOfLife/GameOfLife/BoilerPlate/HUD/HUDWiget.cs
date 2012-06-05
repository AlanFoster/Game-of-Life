using GameOfLife.BoilerPlate.HUD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.Core.HUD {
    public abstract class HUDWiget {
        public bool Active { get; protected set; }
        public HUDAlignment HUDAlignment { get; private set; }
        public bool NeedsRemoved { get; protected set; }
        protected SpriteFont Font { get; private set; }
        protected ContentManager Content { get; private set; }

        /// <summary>
        /// An abstract memember which will return the total width and height of this widget
        /// as a vector.
        /// 
        /// This is used for calculating HUD placement corresponding with the HUDAlignment enum
        /// </summary>
        public abstract Vector2 Size { get; }

        /// <summary>
        /// Creates a new HUDWidget which will need to be explicitly added to the HUDSystem.
        /// By default HUDWidgets have an active property of true.
        /// </summary>
        /// <param name="hudAlignment">
        ///     The location for this Widget to be rendered at. This location will be calculated
        ///     using the abstract CarSize member. The HUDSystem will pass in where to draw the 
        ///     widget directly as a Vector2.
        ///     <seealso cref="HUDAlignment" />
        /// </param>
        protected HUDWiget(HUDAlignment hudAlignment = HUDAlignment.Center) {
            Active = true;
            HUDAlignment = hudAlignment;
        }

        public void DepdencyInjection(SpriteFont font, ContentManager content) {
            Font = font;
            Content = content;
        }

        public virtual void Initialize() { }

        public virtual void Update(GameTime gameTime) { }
        public abstract void Draw(SpriteBatch spriteBatch, Vector2 drawLocation);
    }
}
