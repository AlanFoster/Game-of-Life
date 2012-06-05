using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GameOfLife.BoilerPlate.GUI;
using TomShane.Neoforce.Controls;

namespace GameOfLife.WorldEditing.EditSystems {
    public class StringEditSystem : IEditSystem {
        public object GetValue(Control control, PropertyInfo field) {
            return control.Text;
        }

        public virtual Control CreateControl(Manager manager, Control parent, PropertyInfo field, object existingData) {
            var textToShow = existingData == null ? String.Empty : existingData.ToString();

            var textBox = new ValidationTextField(manager) { Name = field.Name, Parent = parent };
            textBox.Init();
            textBox.Text = textToShow;

            return textBox;
        }
    }

}
