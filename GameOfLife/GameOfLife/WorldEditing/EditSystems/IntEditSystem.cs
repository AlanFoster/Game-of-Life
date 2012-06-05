using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.Data;
using TomShane.Neoforce.Controls;

namespace GameOfLife.WorldEditing.EditSystems {
    class IntEditSystem : IEditSystem {
        public object GetValue(Control control, PropertyInfo field) {
            // We don't bother using tryparse, as our created Control should have validated this.
            return int.Parse(control.Text);
        }

        public Control CreateControl(Manager manager, Control parent, PropertyInfo field, object existingData) {
            var textToShow = existingData == null ? "0" : existingData.ToString();
            var textBox = new ValidationTextField(manager, "0", Constants.RegexValidations.Number) { Text = textToShow, Name = field.Name, Parent = parent };
            textBox.Init();
            return textBox;
        }
    }
}
