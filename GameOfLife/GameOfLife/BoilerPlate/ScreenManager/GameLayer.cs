using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;

namespace GameOfLife.BoilerPlate.ScreenManager {
    public abstract class GameLayer {
        public readonly String LayerName;
        public bool IsActive { get; private set; }
        public bool IsInitialized { get; set; }

        protected Application Application { get; private set; }
        protected ContentManager Content { get; private set; }
        protected SpriteBatch SpriteBatch { get { return ScreenManager.SpriteBatch; } }
        protected GraphicsDevice GraphicsDevice { get { return Application.GraphicsDevice; } }
        protected ScreenManager ScreenManager { get; private set; }

        protected GameLayerManager ControlManager { get; private set; }
        protected int ScreenWidth { get { return ControlManager.ScreenWidth; } }
        protected int ScreenHeight { get { return ControlManager.ScreenHeight; } }

        protected GameLayer(String layerName) {
            LayerName = layerName;
        }

        public void DependencyInjection(ScreenManager screenManager) {
            ScreenManager = screenManager;

            Application = screenManager.Application;
            Content = Application.Content;
            ControlManager = new GameLayerManager(Application.Manager as CustomManager);
        }

        public void SetActive(Boolean newActive) {
            IsActive = newActive;
            ControlManager.SetChildren(newActive);
        }

        public virtual void Initialize() { LoadContent(); }
        public virtual void LoadContent() { }
        public virtual void UnloadContent() { }
        public virtual void Added() { }
        public virtual void Removed() { }

        public virtual void Dispose() {
            ControlManager.Dispose();
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);

        public override string ToString() {
            return "Layer name :: " + LayerName;
        }

        public void RecalculatePosition() {
            ControlManager.RecalculatePosition();
        }
    }
}
