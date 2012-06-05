using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using GameOfLife.BoilerPlate.Misc;
using GameOfLife.Data;
using GameOfLife.GameLogic.Assets;
using TomShane.Neoforce.Controls;
using Console = System.Console;

namespace GameOfLife.WorldEditing.EditSystems {
    public class EnumEditSystem : IEditSystem {
        public object GetValue(Control control, PropertyInfo field)
        {
            var comboBox = (ComboBox) control;
            return Enum.Parse(field.PropertyType, comboBox.GetValue());
        }

        public Control CreateControl(Manager manager, Control parent, PropertyInfo field, object existingData) {
            var enumValues = Enum.GetNames(field.PropertyType);
            var selectedItemIndex = -1;

            // Get a list of the pretty names
            var prettyNames = new List<ItemValue>(enumValues.Length);
            for (var i = 0; i < enumValues.Length; i++) {
                var enumMember = field.PropertyType.GetMember(enumValues[i])[0];
                var editableAttributeDetails = enumMember.GetCustomAttributes(typeof(EditableAttribute), false).Cast<EditableAttribute>().FirstOrDefault();
                if (editableAttributeDetails != null) {
                    // Logical implication
                    // requiresAdmin => isAdmin
                    if (!editableAttributeDetails.AdminRequired || Constants.Editing.IsAdminMode) {
                        prettyNames.Add(new ItemValue(editableAttributeDetails.PrettyName, enumValues[i]));
                    }
                } else {
                    Console.WriteLine("Didn't find an editable attribute for {0}", enumValues[i]);
                }
            }

            if (existingData != null)
            {
                var found = false;
                foreach(var prettyName in prettyNames) {
                    selectedItemIndex++;
                    if(prettyName.Value == existingData.ToString())
                    {
                        found = true;
                        break;
                    }
                }
                if(!found) {
                    selectedItemIndex = 0;
                }
            } else  {
                selectedItemIndex = 0;
            }

            var dropDownMenu = new ComboBox(manager) { Parent = parent, Name = field.Name };
            dropDownMenu.Items.AddRange(prettyNames);
            dropDownMenu.Init();
            dropDownMenu.ItemIndex = selectedItemIndex;
            //dropDownMenu.Text = prettyNames[selectedItemIndex].Text; 

            return dropDownMenu;
        }
    }
}
