using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.WorldEditing;
using GameOfLife.WorldEditing.EditSystems;
using Microsoft.Xna.Framework.Content;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GameOfLife.Screens {
    class ImageSelector {
        protected Panel HightLightPanel { get; set; }
        public GraphicalButton CurrentlySelected { get; protected set; }
        private readonly List<GraphicalButton> _graphicalButtons;
        public Panel Panel { get; private set; }
        private const int Padding = 20;

        public ImageSelector(Manager manager, params String[] imagePaths) {
            Panel = new Panel(manager) { AutoScroll = true };
            Panel.Init();

            _graphicalButtons = new List<GraphicalButton>();

            var loadedImages = LoadImages(manager.Game.Content, imagePaths);
            CreateButtons(manager, loadedImages);

            Panel.Init();
            Panel.MinimumWidth = 600;
            Panel.MinimumHeight = CurrentlySelected.Height + 58;
        }

        private Texture2D[] LoadImages(ContentManager content, String[] imagePaths) {
            var textures = new Texture2D[imagePaths.Length];
            var counter = 0;
            foreach (var path in imagePaths) {
                textures[counter] = content.Load<Texture2D>(path);
                counter++;
            }
            return textures;
        }

        private void CreateButtons(Manager manager, Texture2D[] textures) {
            HightLightPanel = new Panel(manager) { Width = textures[0].Width + Padding, Height = textures[0].Height + Padding, Color = Color.DarkCyan };
            HightLightPanel.Init();
            HightLightPanel.Parent = Panel;
            HightLightPanel.SendToBack(); 

            var left = Padding;
            const int top = Padding;

            foreach (var texture in textures) {
                var graphicalButton = new GraphicalButton(manager, texture, texture, texture) { Left = left, Top = top, Width = texture.Width, Height = texture.Height };
                graphicalButton.Init();
                graphicalButton.Click += (sender, args) => SelectAvatar(graphicalButton);
                graphicalButton.MouseOver += (sender, args) => MouseOverAvatar(graphicalButton);
                graphicalButton.MouseOut += (sender, args) => MouseOutAvatar(graphicalButton);

                graphicalButton.Parent = Panel;
                _graphicalButtons.Add(graphicalButton);
                left += (Padding + graphicalButton.Width);
            }

            SetSelected(0);
        }


        private void SelectAvatar(GraphicalButton button) {
            HightLightPanel.SetPosition(button.Left - Padding / 2, button.Top - Padding / 2);
            CurrentlySelected = button;
        }

        private void MouseOverAvatar(Control button) {
            button.Color = Color.Yellow;
        }

        private void MouseOutAvatar(Control button) {
            button.Color = Color.White;
        }

        public void SetSelected(int defaultIndex) {
            SelectAvatar(_graphicalButtons[defaultIndex]);
            Panel.ScrollTo((int)MathHelper.Clamp(CurrentlySelected.Left - Panel.Width / 2 - CurrentlySelected.Width / 2, 0, Panel.Width + CurrentlySelected.Width), 0);
        }
    }

    public class ImageSelectorEditSystem : IEditSystem {
        private readonly String[] _imagePaths;
        private ImageSelector _imageSelector;
        private readonly int _defaultIndex;

        public ImageSelectorEditSystem(string[] imagePaths, int defaultIndex = 0) {
            _imagePaths = imagePaths;
            _defaultIndex = defaultIndex;
        }

        public object GetValue(Control control, PropertyInfo field) {
            return _imageSelector.CurrentlySelected.Image;
        }

        public Control CreateControl(Manager manager, Control parent, PropertyInfo field, object existingData) {
            _imageSelector = new ImageSelector(manager, _imagePaths);
            _imageSelector.SetSelected(_defaultIndex);
            var rawControl = _imageSelector.Panel;
            rawControl.Name = field.Name;
            rawControl.Parent = parent;

            return rawControl;
        }
    }
}
