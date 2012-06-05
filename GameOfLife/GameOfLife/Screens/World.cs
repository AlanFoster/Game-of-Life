using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate;
using GameOfLife.BoilerPlate.Misc;
using GameOfLife.BoilerPlate.ScreenManager;
using GameOfLife.Data;
using GameOfLife.GameLogic.GameStates.Factories;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameOfLife;
using TomShane.Neoforce.Controls;
using GameOfLife.GameLogic.GameStates;
using Console = System.Console;

namespace GameOfLife.Screens {
    public enum ColorState {
        None, Glow, Dark, DarkDark, Light
    }

    public delegate void MouseOver<T>(T node, Vector2 location);
    public delegate void MouseOut<T>(T node, Vector2 location);
    public delegate void MouseDown<T>(T node, Vector2 location);
    public delegate void MouseUp<T>(T node, Vector2 location);

    public abstract class World : GameLayer {
        public static Vector2 MouseInWorld;
        public static Vector2 CameraPosition;
        protected GameMouseState CurrentMouseState;
        protected GameMouseState PreviousMouseState;
        protected List<WorldObject> WorldObjects;
        private KeyboardState _currentKeyboardState;
        private const int CameraMovementSpeed = 10;
        protected bool Enabled { get; set; }

        private Camera2D _camera;
        public WorldObject CameraTarget;
        public List<CameraTarget> LowPriorityTargets;
        public bool AutoCamera;

        private Texture2D _cameraTargetArrow;

        public Vector2 CameraPos {
            get { return _camera.Pos; }
            set { _camera.Pos = value; }
        }

        protected World(List<WorldObject> worldObjects, string layerName)
            : base(layerName) {
            WorldObjects = worldObjects;
            Enabled = true;
            LowPriorityTargets = new List<CameraTarget>();
        }

        public override void Added() {
            base.Added();
            ScreenManager.HideScreen(Constants.ScreenNames.Logo);
            ControlManager.Manager.PlayingWorld = true;
        }

        public override void Removed() {
            base.Added();
            ScreenManager.ShowScreen(Constants.ScreenNames.Logo);
            ControlManager.Manager.PlayingWorld = false;
        }

        public override void Initialize() {
            base.Initialize();
            _camera = new Camera2D(GraphicsDevice.Viewport, 4000, 4000, 1f) { Pos = new Vector2(0, 0) };
        }

        public override void LoadContent() {
            base.LoadContent();
            _cameraTargetArrow = Content.Load<Texture2D>("Images/pointsTo");
        }


        public override void Update(GameTime gameTime) {
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = GameMouse.GetState();

            if (!Enabled) return;
            UpdateCamera();
            FireMouseEvents();
        }

        private void UpdateCamera() {
            foreach (var cameraTarget in LowPriorityTargets.ToArray())
                cameraTarget.Update(_camera.Pos, CurrentMouseState, MouseInWorld);

            CameraPosition = _camera.Pos;
            _currentKeyboardState = Keyboard.GetState();

            if (_currentKeyboardState.IsKeyDown(Keys.Right) || _currentKeyboardState.IsKeyDown(Keys.D)) {
                _camera.Pos += new Vector2(CameraMovementSpeed, 0);
                AutoCamera = false;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Left) || _currentKeyboardState.IsKeyDown(Keys.A)) {
                _camera.Pos += new Vector2(-CameraMovementSpeed, 0);
                AutoCamera = false;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Up) || _currentKeyboardState.IsKeyDown(Keys.W)) {
                _camera.Pos += new Vector2(0, -CameraMovementSpeed);
                AutoCamera = false;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.Down) || _currentKeyboardState.IsKeyDown(Keys.S)) {
                _camera.Pos += new Vector2(0, CameraMovementSpeed);
                AutoCamera = false;
            }

            if (AutoCamera) {
                MoveCamera();
            }

            _camera.AccelerateZoom(0.05f * Math.Sign(CurrentMouseState.ScrollWheelValue - PreviousMouseState.ScrollWheelValue));
            _camera.UpdateZoom();
        }

        private void FireMouseEvents() {
            MouseInWorld = Vector2.Transform(new Vector2(CurrentMouseState.X, CurrentMouseState.Y), Matrix.Invert(_camera.GetTransformation()));

            foreach (var worldObject in WorldObjects.ToArray()) {

                if (worldObject.Rectangle.Contains((int)MouseInWorld.X, (int)MouseInWorld.Y)) {
                    if (!worldObject.IsMouseOver) {
                        worldObject.IsMouseOver = true;
                        worldObject.OnMouseOver(MouseInWorld);
                    }

                    if (!worldObject.IsMouseDown &&
                            CurrentMouseState.LeftButton == ButtonState.Pressed &&
                            PreviousMouseState.LeftButton == ButtonState.Released) {
                        worldObject.IsMouseDown = true;
                        worldObject.OnMouseDown(MouseInWorld);
                    }

                } else {
                    if (worldObject.IsMouseOver) {
                        worldObject.IsMouseOver = false;
                        worldObject.OnMouseOut(MouseInWorld);
                    }
                }
                if (worldObject.IsMouseDown && CurrentMouseState.LeftButton == ButtonState.Released &&
                    PreviousMouseState.LeftButton == ButtonState.Pressed) {
                    worldObject.IsMouseDown = false;
                    worldObject.OnMouseUp(MouseInWorld);
                }
            }
        }

        public override void Draw(GameTime gameTime) {
            SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _camera.GetTransformation());
            // Tell objects to draw themselves, whatever they may be. 
            foreach (var worldObject in WorldObjects) {
                worldObject.Draw(gameTime, SpriteBatch);
            }

            foreach (var node in WorldObjects.OfType<Node>()) {
                node.DrawLinks(SpriteBatch);
            }
            foreach (var cameraTarget in LowPriorityTargets)
                cameraTarget.Draw(SpriteBatch);
            SpriteBatch.End();
        }

        protected void PanCameraToObject(WorldObject obj) {
            CameraTarget = obj;
            AutoCamera = true;
        }

        protected void AddLowPriorityTarget(WorldObject obj) {
            LowPriorityTargets.Add(GetCameraTarget(obj));
        }

        protected void ClearTargets() {
            LowPriorityTargets.Clear();
        }

        private CameraTarget GetCameraTarget(WorldObject worldObject) {
            var cameraTarget = new CameraTarget(worldObject, _cameraTargetArrow, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            cameraTarget.SetColorState(ColorState.Glow);
            cameraTarget.ListenMouseDown += (target, location) => PanCameraToObject(worldObject);
            return cameraTarget;
        }

        private void MoveCamera() {
            double cameraSpeed = Math.Sqrt((CameraTarget.Center - _camera.Pos).Length()) - 3;

            var target = CameraTarget.Center - _camera.Pos;

            var angle = (float)Math.Atan2(target.Y, target.X);

            var move = new Vector2((float)(Math.Cos(angle) * cameraSpeed), (float)(Math.Sin(angle) * cameraSpeed));

            if (!(Math.Abs((CameraTarget.Center - _camera.Pos).Length()) < 20))
                _camera.Pos += move;
        }
    }
}
