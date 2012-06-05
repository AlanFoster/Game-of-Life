using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;
using EventHandler = TomShane.Neoforce.Controls.EventHandler;

namespace GameOfLife.BoilerPlate.GUI {
    public class Alert : Window {
        protected const int MinWidth = 450;
        protected int yPos { get; set; }
        public Label TextLabel { get; protected set; }
        public Button DenyButton { get; protected set; }
        public ImageBox Icon { get; protected set; }
        protected bool AutoClose { get; private set; }

        public Alert(Manager manager, string alertText, string title = "Alert", bool autoClose = true, Control control = null, String icon = null)
            : base(manager) {
            yPos = 16;

            AutoClose = autoClose;
            Resizable = true;
            CloseButtonVisible = false;
            Text = title;
            if (control != null) {
                Add(control);
                yPos += control.Height + 16;
            }

            if (icon != null) {
                CreateIcon(icon);
            }

            if (alertText != String.Empty) {
                CreateTextContent(alertText);
            }

            CreateButtons();

            Height = DenyButton.Top + DenyButton.Height + 50;
            Width = Math.Max(MinWidth + 48, (Icon != null ? Icon.Width + 16 : 0) + (TextLabel != null ? TextLabel.Width + 16 : 0) + 20);
        }

        protected virtual void CreateIcon(String iconPath) {
            var loadedIcon = Manager.Game.Content.Load<Texture2D>(iconPath);

            var imageIcon = new ImageBox(Manager) {
                Left = 16,
                Top = yPos,
                Image = loadedIcon,
                Width = loadedIcon.Width,
                Height = loadedIcon.Height,
                Color = Color.White
            };
           
            Add(imageIcon);
            Icon = imageIcon;
        }

        protected virtual void CreateTextContent(String alertText) {
            var lines = alertText.Count(i => i == '\n') + 1;
            
            TextLabel = new Label(Manager) {
                Text = alertText,
                Left = (Icon != null ? Icon.Width + 16 : 0) + 16,
                Top = yPos,
                Width = Icon == null ? MinWidth : 360,
                Height = 20 * lines
            };
            TextLabel.Alignment = Alignment.MiddleCenter;

            yPos += TextLabel.Height + 16;

            TextLabel.Init();
            Add(TextLabel);
        }

        protected virtual void CreateButtons() {
            DenyButton = new Button(Manager) {
                Text = "OK",
                Top = yPos,
                Parent = this,
            };
            if (AutoClose) {
                DenyButton.Click += (sender, args) => Close();
            }
            DenyButton.Init();
            Add(DenyButton);

            Init();

            // Align the confirm button center
            DenyButton.Left = Width / 2 - DenyButton.Width / 2;
        }
    }
}
