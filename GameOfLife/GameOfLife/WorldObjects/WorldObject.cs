using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using GameOfLife.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.WorldObjects {
    [DataContract(Name = "WorldObject")]
    public abstract class WorldObject {
        /// <summary>
        /// The step value of opacity when glowing per logical update.
        /// </summary> 
        private const float ConstOverlayColorOpacityStep = 0.04f;

        public bool IsMouseOver { get; set; }
        public bool IsMouseDown { get; set; }

        protected Color CurrentColor { get; set; }
        public Color OverlayColor { get; protected set; }

        [DataMember]
        public ColorState CurrentColorState { get; private set; }
        [DataMember]
        public ColorState RememberedColorState { get; private set; }

        [DataMember]
        public virtual Vector2 Location { get; set; }

        [DataMember]
        public Vector2 Size;

        private float OverlayColorOpacityStep;
        private float OverlayColorOpacity;
        public float Opacity { get; set; }


        public virtual Vector2 Center { get { return Location + Size / 2; } }
        public Rectangle Rectangle { get { return new Rectangle((int)Location.X, (int)Location.Y, (int)Size.X, (int)Size.Y); } }

        protected WorldObject(Vector2? location, Vector2? size) {
            Location = location ?? Vector2.Zero;
            Size = size ?? new Vector2(120, 120);
            RememberedColorState = CurrentColorState = ColorState.None;
        }

        public void SetColorState(ColorState colorState) {
            CurrentColorState = colorState;
            switch (colorState) {
                case ColorState.Dark:
                    OverlayColor = Color.Lerp(CurrentColor, Color.Black, 0.4f);
                    break;
                case ColorState.DarkDark:
                    OverlayColor = Color.Lerp(CurrentColor, Color.Black, 0.6f);
                    break;
                case ColorState.Light:
                    OverlayColor = Color.Lerp(CurrentColor, Color.White, 0.5f);
                    break;
                case ColorState.None:
                    OverlayColor = Color.Lerp(CurrentColor, CurrentColor, 1f);
                    OverlayColorOpacity = 0;
                    OverlayColorOpacityStep = ConstOverlayColorOpacityStep;
                    break;
                case ColorState.Glow:
                    OverlayColorOpacityStep = ConstOverlayColorOpacityStep;
                    break;
            }
        }

        public void RememberColorState(ColorState colorState) {
            RememberedColorState = colorState;
            SetColorState(colorState);
        }

        public void ResetColorState() {
            CurrentColorState = RememberedColorState;
            SetColorState(CurrentColorState);
        }

        public void RefreshColorState() {
            SetColorState(CurrentColorState);
        }

        public abstract void OnMouseOver(Vector2 location);
        public abstract void OnMouseOut(Vector2 location);
        public abstract void OnMouseDown(Vector2 location);
        public abstract void OnMouseUp(Vector2 location);

        public abstract void Initialize();
        public abstract void LoadContent(ContentManager content);

        public void InitializeContent(ContentManager content) {
            RememberColorState(ColorState.None);
            Initialize();
            LoadContent(content);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            if (CurrentColorState != ColorState.Glow) return;
            OverlayColorOpacity += OverlayColorOpacityStep;
            // Reverse our color step if we need to 
            if (OverlayColorOpacity >= 1 || OverlayColorOpacity <= 0) {
                OverlayColorOpacityStep *= -1;
            }
            OverlayColor = Color.Lerp(Color.White, Color.Red, OverlayColorOpacity);
        }
    }
}
