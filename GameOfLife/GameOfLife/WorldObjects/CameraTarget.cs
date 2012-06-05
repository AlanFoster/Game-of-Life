using System;
using GameOfLife.BoilerPlate.ScreenManager;
using GameOfLife.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameOfLife.WorldObjects {
    public class CameraTarget : GenericWorldObject<CameraTarget> {
        public WorldObject Target { get; protected set; }
        private readonly Texture2D _arrow;
        private GameMouseState currentMouseState;
        private GameMouseState previousMouseState;
        private Vector2 _Position;
        private float _Rotation;
        private readonly Vector2 Size = new Vector2(50, 50);
        private int DistanceX;
        private int Height;
        private int Width;
        private Vector2 CameraPos;

        public CameraTarget(WorldObject wObject, Texture2D arrow = null, int width = 0, int height = 0)
            : base(Vector2.Zero, new Vector2(50, 50)) {
            Target = wObject;
            _arrow = arrow;
            DistanceX = width / 5;
            Width = width;
            Height = height;
            OverlayColor = Color.White;
            SetColorState(ColorState.Glow);
        }

        public void Update(Vector2 cameraPos, GameMouseState getState, Vector2 mouseInWorld) {
            CameraPos = cameraPos;
            _Position = cameraPos;

            Location = _Position + new Vector2((float)-(DistanceX * Math.Cos(_Rotation)), (float)-(DistanceX * Math.Sin(_Rotation))) +
                new Vector2(Size.X * (float)Math.Cos(_Rotation) - Size.X / 2, Size.Y * (float)Math.Sin(_Rotation) - Size.X / 2);

            currentMouseState = getState;

            if (!IsMouseDown &&
                            currentMouseState.LeftButton == ButtonState.Pressed &&
                            previousMouseState.LeftButton == ButtonState.Released && Rectangle.Contains((int)(mouseInWorld.X), (int)(mouseInWorld.Y))) {
                IsMouseDown = false;
                OnMouseDown(mouseInWorld);
            }

            if (IsMouseDown && currentMouseState.LeftButton == ButtonState.Released &&
                    previousMouseState.LeftButton == ButtonState.Pressed && Rectangle.Contains((int)(mouseInWorld.X), (int)(mouseInWorld.Y))) {
                IsMouseDown = false;
                OnMouseUp(mouseInWorld);
                
            }

            previousMouseState = currentMouseState;
        }

        public void Draw(SpriteBatch spriteBatch) {
            base.Draw(null, spriteBatch);

            var distance = _Position - Target.Center;
            _Rotation = (float)Math.Atan2(distance.Y, distance.X);

            var rectangle = new Rectangle((int)CameraPos.X - Width / 2, (int)CameraPos.Y - Height / 2, Width, Height);

            if (!rectangle.Contains((int)Target.Center.X, (int)Target.Center.Y)) {
                spriteBatch.Draw(_arrow, _Position, null, OverlayColor, _Rotation, new Vector2(DistanceX, _arrow.Height / 2), 1,
                                 SpriteEffects.None, 1f);
            }
        }

        public override void Initialize() {
        }

        public override void LoadContent(ContentManager content) {
        }
    }
}
