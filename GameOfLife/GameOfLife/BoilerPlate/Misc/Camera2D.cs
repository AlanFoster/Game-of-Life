using System;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.BoilerPlate.Misc {
    class Camera2D {
        private Matrix _cameraMovement;
        private Vector2 _position;
        private readonly int _viewportWidth;
        private readonly int _viewportHeight;
        private readonly int _worldWidth;
        private readonly int _worldHeight;


        private float _zoom;
        private const float MaxZoomAcceleration = 0.2f;
        private const float ZoomDeceleration = 0.8f;
        private const float ZoomUpperLimit = 2f;
        private const float ZoomLowerLimit = .3f;
        private float _zoomAcceleration;

        private float Zoom {
            get { return _zoom; }
            set { _zoom = MathHelper.Clamp(value, ZoomLowerLimit, ZoomUpperLimit); }
        }

        public Vector2 Pos {
            get { return _position; }
            set {
                var leftBound = ((float)_viewportWidth -4000) * .5f / _zoom;
                var rightBound = _worldWidth - _viewportWidth * .5f / _zoom;
                var topBound = _worldHeight - _viewportHeight * .5f / _zoom;
                var bottomBound = ((float)_viewportHeight - 4000) * .5f / _zoom;

                _position.X = MathHelper.Clamp(value.X, leftBound, rightBound);
                _position.Y = MathHelper.Clamp(value.Y, bottomBound, topBound);
            }
        }

        public void AccelerateZoom(float amount) {
            _zoomAcceleration += amount;
            _zoomAcceleration = MathHelper.Clamp(_zoomAcceleration, -MaxZoomAcceleration, MaxZoomAcceleration);
        }

        public void UpdateZoom() {
            Zoom += _zoomAcceleration;
            _zoomAcceleration *= ZoomDeceleration;
        }

        /// <summary>
        /// Constructor.
        /// Constructs a new Camera2D object.
        /// </summary>
        /// <param name="viewport">current viewport</param>
        /// <param name="worldWidth">width of current level</param>
        /// <param name="worldHeight">height of current level</param>
        /// <param name="initialZoom">initial zoom to start with</param>
        public Camera2D(Viewport viewport, int worldWidth, int worldHeight, float initialZoom) {
            _zoom = initialZoom;
            _viewportWidth = viewport.Width;
            _viewportHeight = viewport.Height;
            _worldWidth = worldWidth;
            _worldHeight = worldHeight;
        }

        /// <summary>
        /// This method creates the matrix transformation for us.
        /// </summary>
        /// <returns></returns>
        public Matrix GetTransformation() {
            _cameraMovement = Matrix.CreateTranslation(new Vector3(-_position.X, -_position.Y, 0)) *
                Matrix.CreateRotationZ(0) *
                Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                Matrix.CreateTranslation(new Vector3(_viewportWidth * 0.5f, _viewportHeight * 0.5f, 0));
            return _cameraMovement;
        }
    }
}
