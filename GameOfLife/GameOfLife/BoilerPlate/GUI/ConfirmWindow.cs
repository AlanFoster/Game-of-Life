using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace GameOfLife.BoilerPlate.GUI {
    public class ConfirmWindow : Alert {
        public Button AffirmButton { get; protected set; }

        public ConfirmWindow(Manager manager, string alertText, string title = "Confirm Window", bool autoClose = false, Control control = null, String icon = null)
            : base(manager, alertText, title, autoClose, control, icon) {
        }

        protected override void CreateButtons() {
            base.CreateButtons();
            DenyButton.Text = "No";
            AffirmButton = new Button(Manager) {
                Text = "Yes",
                Top = yPos,
                Parent = this,
            };

            AffirmButton.Init();
            Add(AffirmButton);

            if (AutoClose) {
                AffirmButton.Click += (sender, args) => Close();
            }

            Manager.Add(this);

            // Align the buttons beside each other
            const int buttonGap = 20;
            var totalButtonWidth = DenyButton.Width + AffirmButton.Width + buttonGap;
            var buttonPosLeft = (Width / 2) - (totalButtonWidth / 2);

            AffirmButton.Left = buttonPosLeft;
            buttonPosLeft += AffirmButton.Width + buttonGap;
            DenyButton.Left = buttonPosLeft;
        }
    }

}
