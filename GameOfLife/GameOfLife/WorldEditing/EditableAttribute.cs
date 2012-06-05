using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace GameOfLife.WorldEditing {
    /// <summary>
    /// Marks a class or attribute as being editor 'editable'.
    /// 
    /// Note, this MUST BE added to any class that wants to be used by the ObjectEditor class
    /// Any class/field that has this attribute will be implicitly registered for any sort of editing.
    /// And does not require any additional interaction or registering with the system.
    /// </summary>
    /// <see cref="ObjectEditor{T}"/>
    [DataContract]
    class EditableAttribute : Attribute {
        /// <summary>
        /// The "Pretty Name" which will be used in any created user interface instead of the
        /// attribute's actual programming name
        /// </summary>
        [DataMember]
        public readonly String PrettyName;

        [DataMember]
        public readonly Type PreferredEditor;

        [DataMember]
        public readonly bool AdminRequired;

        public EditableAttribute(String prettyName, bool adminRequired = false) {
            PrettyName = prettyName;
            AdminRequired = adminRequired;
        }

        public EditableAttribute(String prettyName, Type type, bool adminRequired = false)
            : this(prettyName, adminRequired) {
            PreferredEditor = type;
        }

        /// <summary>
        /// Returns the Pretty Name of a field marked with the Editable Attribute
        /// </summary>
        /// <param name="type">The field that we extract the pretty name from</param>
        /// <returns>
        /// The pretty name of the field. If there was an error getting the pretty name,
        /// the field's original name is returned instead.
        /// </returns>
        public static string GetPrettyName(PropertyInfo type) {
            var editableAttribute = GetCustomAttribute(type, typeof(EditableAttribute)) as EditableAttribute;
            return editableAttribute != null ? editableAttribute.PrettyName : type.Name;
        }

        public static string GetPrettyName(Type type) {
            var editableAttribute = GetCustomAttribute(type, typeof(EditableAttribute)) as EditableAttribute;
            return editableAttribute != null ? editableAttribute.PrettyName : type.Name;
        }

        public static Type GetPreferredType(PropertyInfo type) {
            var editableAttribute = GetCustomAttribute(type, typeof(EditableAttribute)) as EditableAttribute;
            return editableAttribute != null ? editableAttribute.PreferredEditor : null;
        }

        public static EditableAttribute GetEditableAttribute(PropertyInfo prop) {
            var editableAttribute = GetCustomAttribute(prop, typeof(EditableAttribute)) as EditableAttribute;
            return editableAttribute;
        }

        public static EditableAttribute GetEditableAttribute(Type type) {
            var editableAttribute = GetCustomAttribute(type, typeof(EditableAttribute)) as EditableAttribute;
            return editableAttribute;
        }
    }
}
