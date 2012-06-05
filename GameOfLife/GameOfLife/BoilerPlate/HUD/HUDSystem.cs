using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLife.BoilerPlate.ScreenManager;
using GameOfLife.Core.HUD;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;

namespace GameOfLife.BoilerPlate.HUD {


    // For now the HUDSystem doesn't use the "widget" system
    // as in HUDComponents registering with the HUD
    class HUDSystem : GameLayer
    {
        private readonly ContentManager _content;
        private readonly SpriteFont _font;
        private readonly List<HUDWiget> _widgets;

        public HUDSystem(SpriteFont font, ContentManager content, string name = "hudSystem")
            : base(name) {
            _widgets = new List<HUDWiget>();
            _font = font;
            _content = content;
       }

        public override void Initialize() { }
        public override void LoadContent() { }
        public override void UnloadContent() { }

        public override void Update(GameTime gameTime) {
            foreach (var widget in _widgets.Where(i => i.Active).ToList()) {
                widget.Update(gameTime);
                if (widget.NeedsRemoved) {
                    _widgets.Remove(widget);
                }
            }
        }

        public void Add(HUDWiget widget) {
            widget.DepdencyInjection(_font, _content);
            widget.Initialize();
            _widgets.Add(widget);
        }

        public void Remove(HUDWiget widget) {
            _widgets.Remove(widget);
        }

        public override void Draw(GameTime gameTime) {
            SpriteBatch.Begin();

            foreach (var widget in _widgets.Where(i => i.Active).ToArray()) {
                Vector2 drawLocation;
                var widgetWidth = (int)widget.Size.X;
                var widgetHeight = (int)widget.Size.Y;
                switch (widget.HUDAlignment) {
                    case HUDAlignment.TopLeft:
                        drawLocation = new Vector2(0, 0);
                        break;
                    case HUDAlignment.TopCenter:
                        drawLocation = new Vector2(ScreenWidth / 2 - widgetWidth / 2, 0);
                        break;
                    case HUDAlignment.TopRight:
                        drawLocation = new Vector2(ScreenWidth - widgetWidth, 0);
                        break;
                    case HUDAlignment.BottomLeft:
                        drawLocation = new Vector2(0, ScreenHeight - widgetHeight);
                        break;
                    case HUDAlignment.BottomCenter:
                        drawLocation = new Vector2(ScreenWidth/2 - (widgetWidth/2), ScreenHeight - widgetHeight);
                        break;
                    case HUDAlignment.BottomRight:
                        drawLocation = new Vector2(ScreenWidth - widgetWidth, ScreenHeight - widgetHeight);
                        break;
                    case HUDAlignment.Center:
                        drawLocation = new Vector2(ScreenWidth / 2 - widgetWidth / 2, ScreenHeight / 2 - widgetHeight / 2);
                        break;
                    case HUDAlignment.CenterRight:
                        drawLocation = new Vector2(ScreenWidth - widgetWidth, ScreenHeight / 2 - widgetHeight / 2);
                        break;
                    default:
                        throw new NotImplementedException();
                }
                widget.Draw(SpriteBatch, drawLocation);
            }
            SpriteBatch.End();
        }
    }
}
