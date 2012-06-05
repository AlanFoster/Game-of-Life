using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLife.Data;
using GameOfLife.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;
using Console = System.Console;
using EventArgs = TomShane.Neoforce.Controls.EventArgs;

namespace GameOfLife.BoilerPlate.ScreenManager {
    public sealed class ScreenManager {
        public readonly Application Application;
        public SpriteBatch SpriteBatch { get; private set; }
        public readonly List<GameLayer> _gameLayers;
        private bool _initialized;

        public ScreenManager(Application application) {
            Application = application;
            _gameLayers = new List<GameLayer>();
        }

        private bool ModifyScreen(string requiredLayerName, bool newActive) {
            GameLayer screen = _gameLayers.FirstOrDefault(i => i.LayerName == requiredLayerName);
            if (screen == null) {
                Console.WriteLine("Name {0} not  found", requiredLayerName);
                return false;
            }
            return ModifyScreen(screen, newActive);
        }

        private bool ModifyScreen(GameLayer screen, bool newActive) {
            if (newActive) {
                screen.Added();
            } else {
                screen.Removed();
            }
            screen.SetActive(newActive);
            InitializeWaitingLayers();
            return true;
        }

        public bool HideScreen(String name) {
            return ModifyScreen(name, false);
        }

        public bool ShowScreen(String name) {
            return ModifyScreen(name, true);
        }

        public bool HideScreen(GameLayer gameLayer) {
            return ModifyScreen(gameLayer, false);
        }

        public bool ShowScreen(GameLayer gameLayer) {
            return ModifyScreen(gameLayer, true);
        }

        public void SwapScreens(GameLayer one, GameLayer two) {
            HideScreen(one);
            ShowScreen(two);
        }

        public void SwapScreens(GameLayer one, String twoName) {
            SwapScreens(one, _gameLayers.FirstOrDefault(i => i.LayerName == twoName));
        }

        public void AddGameLayer(GameLayer gameLayer, Boolean active = false) {
            gameLayer.DependencyInjection(this);
            _gameLayers.Add(gameLayer);
            if (active) {
                ShowScreen(gameLayer);
            }
        }

        public void Remove(GameLayer gameLayer) {
            _gameLayers.Remove(gameLayer);
            gameLayer.Dispose();
        }

        public void Initialize() {
            _initialized = true;
            SpriteBatch = Application.Manager.Renderer.SpriteBatch;
            InitializeWaitingLayers();
        }

        private void InitializeWaitingLayers() {
            if (!_initialized) return;
            foreach (var gameLayer in _gameLayers.Where(i => i.IsActive && !i.IsInitialized).ToList()) {
                gameLayer.IsInitialized = true;
                gameLayer.Initialize();
                gameLayer.LoadContent();
            }
        }

        public void LoadContent() {
            //foreach (var gameLayer in _gameLayers.Where(i => i.IsActive && !i.IsInitialized)) {
            //    gameLayer.LoadContent();
            //}
        }

        public void UnloadContent() {
            foreach (var gameLayer in _gameLayers.Where(i => i.IsActive)) {
                gameLayer.UnloadContent();
            }
        }

        public void Update(GameTime gameTime) {
            foreach (var gameLayer in _gameLayers.Where(i => i.IsActive).ToList()) {
                gameLayer.Update(gameTime);
            }
        }

        public void RecalculatePosition() {
            foreach (var gameLayer in _gameLayers.Where(i => i.IsActive)) {
                gameLayer.RecalculatePosition();
            }
        }
        public void Draw(GameTime gameTime) {
            foreach (var gameLayer in _gameLayers.Where(i => i.IsActive)) {
                gameLayer.Draw(gameTime);
            }
        }
    } 
}
