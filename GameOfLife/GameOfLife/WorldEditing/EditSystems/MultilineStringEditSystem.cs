using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GameOfLife.BoilerPlate.GUI;
using TomShane.Neoforce.Controls;

namespace GameOfLife.WorldEditing.EditSystems {
    class MultilineStringEditSystem : StringEditSystem {
        public override Control CreateControl(Manager manager, Control parent, PropertyInfo field, object existingData) {
            var control = (ValidationTextField)base.CreateControl(manager, parent, field, existingData);
            control.Height = 100;
            control.Width = 400;
            control.Mode = TextBoxMode.Multiline;
            // We have to overwrite the textfield's existing string to solve a neoforce bug with new lines
            control.Text = existingData == null ? String.Empty : existingData.ToString();

            return control;
        }
    }
}
