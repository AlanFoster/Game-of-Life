using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TomShane.Neoforce.Controls;

namespace GameOfLife.WorldEditing.EditSystems {
    /// <summary>
    /// Provided as a public interface for users to add new functionality as they require
    /// ObjectEditor provides a new adding method for editing systems
    /// </summary>
    public interface IEditSystem {
        object GetValue(Control control, PropertyInfo field);
        Control CreateControl(Manager manager, Control parent, PropertyInfo field, object existingData);
    }
}
