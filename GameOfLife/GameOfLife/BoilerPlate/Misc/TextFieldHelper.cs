using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TomShane.Neoforce.Controls;


namespace GameOfLife.BoilerPlate.Misc {
    /// <summary>
    /// Stores the Text representation of a combo box and the value representation.
    /// Similar to how you'd actually expect a combo box to work, like in html with
    /// its value attribute
    /// </summary>
    public class ItemValue {
            public readonly String Text;
            public readonly String Value;
            public ItemValue(String text, String value)
            {
                Text = text;
                Value = value;
            }

            public override string ToString() {
                return Text;
            }
     }

     public static class ComboBoxHelper {
             public static String GetValue(this ComboBox comboBox) {
                 return comboBox.Items.OfType<ItemValue>().First(i => i.Text == comboBox.Text).Value;
             }
    }
}
