using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TomShane.Neoforce.Controls;

namespace GameOfLife.WorldEditing.EditSystems {
    class BooleanEditSystem : IEditSystem {
        public object GetValue(Control control, PropertyInfo field) {
            return ((CheckBox)control).Checked;
        }

        public Control CreateControl(Manager manager, Control parent, PropertyInfo field, object existingData) {
            var radioButton = new CheckBox(manager) { Parent = parent, Name = field.Name, Text = String.Empty };
            radioButton.Init();

            if (existingData != null) {
                radioButton.Checked = (bool)existingData;
            }

            return radioButton;
        }
    }
}
