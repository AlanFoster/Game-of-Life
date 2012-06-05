using System;
using GameOfLife.Screens;
using GameOfLife.WorldEditing;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.GameLogic.Assets {
    [Flags]
    public enum TransportType {
        [Editable("Car", adminRequired: true)]
        Car = 0,
        [Editable("Boat Car")]
        BoatCar = 1,
        [Editable("Plane Car")]
        PlaneCar = 2,
        [Editable("Boat Plane", adminRequired: true)]
        BoatPlane = BoatCar | PlaneCar
    }

    public class Transport : Asset {
        public TransportType TransportType { get; private set; }
        private readonly Texture2D _currentGraphic;
        private readonly Texture2D _glowingGraphic;
        public readonly int CarSize;
        public Vector2 TextureSize { get { return _currentGraphic == null ? new Vector2(300, 300) : new Vector2(_currentGraphic.Width, _currentGraphic.Height); } }

        public Transport(TransportType transportType, String assetName, int carSize, Texture2D graphic, Texture2D glowingGraphic, int value)
            : base(AssetType.Car, value, assetName) {
            TransportType = transportType;
            _currentGraphic = graphic;
            _glowingGraphic = glowingGraphic;
            CarSize = carSize;
        }


        public override string ToString() {
            return "Car size :: " + CarSize;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Player player) {
            spriteBatch.Draw(_glowingGraphic, player.ActualLocation, null, (player.OverlayColor * 0.8f) * player.Opacity, player.Rotation,
                   new Vector2(_glowingGraphic.Width, _glowingGraphic.Height) / 2,
                   1f, SpriteEffects.None, 1f);
            spriteBatch.Draw(_currentGraphic,
                player.ActualLocation,
                null,
                (player.CurrentColorState == ColorState.None ? Color.White : player.PlayerColor) * player.Opacity,
                player.Rotation,
                new Vector2(_currentGraphic.Width, _currentGraphic.Height) / 2,
                1f, SpriteEffects.None, 1f);
        }

        public override string AssetPath
        {
            get { return String.Format("Images/assets/{0}",TransportType); }
        }
    }
}
