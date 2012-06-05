using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.Data;
using GameOfLife.WorldEditing.EditSystems;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;
using Console = System.Console;
using EventArgs = TomShane.Neoforce.Controls.EventArgs;

namespace GameOfLife.WorldEditing {
    public delegate void EditorClosed();

    public sealed class ObjectEditor<T> where T : class {
        private const BindingFlags DefaultBindingFlags =
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField;

        private readonly EditorClosed _saveCallback;
        private readonly Manager _manager;
        private readonly Control _control;

        private readonly BindingFlags _bindingFlags;

        private int Spacing;
        public int SizeY { get; private set; }

        private Dictionary<Type, IEditSystem> _controlFuncs;

        /// <summary>
        /// The current type of the object represented by this editor
        /// </summary>
        private Type _currentType;

        /// <summary>
        /// An instance of T which was either passed in. When generating an interface of type, it will use this objects
        /// </summary>
        private T _instance;

        public ObjectEditor(Manager manager, Control control, EditorClosed saveCallback = null, BindingFlags bindingFlags = DefaultBindingFlags, int spacing = 16) {
            _control = control;
            _manager = manager;
            _bindingFlags = bindingFlags;
            _saveCallback = saveCallback;
            Spacing = spacing;

            RegisterDefaultEditSystems();
        }

        private void RegisterDefaultEditSystems() {
            _controlFuncs = new Dictionary<Type, IEditSystem> {
                {typeof(int), new IntEditSystem()},
                {typeof(String), new StringEditSystem()},
                {typeof(Enum), new EnumEditSystem()},
                {typeof(Boolean), new BooleanEditSystem()},
                {typeof(object), new MultilineStringEditSystem()}
            };
        }

        public void SetInstance(T newInstance) {
            _instance = newInstance;
        }

        public bool CreateInterface(Type type = null) {
            _currentType = type = type ?? typeof(T);
            SizeY = Spacing;
            var created = false;
            foreach (var prop in
                    type.GetProperties(_bindingFlags).Where(i => Attribute.IsDefined(i, typeof(EditableAttribute)))) {

                Console.WriteLine(prop.Name);

                var fieldType = prop.PropertyType;

                // Test if there is a preferred edit system for this type
                var editableAttribute = EditableAttribute.GetEditableAttribute(prop);

                // Test if this control needs to have admin turned on
                if (editableAttribute.AdminRequired && !Constants.Editing.IsAdminMode) {
                    Console.WriteLine("Admin required for prop {0}", prop.Name);
                    continue;
                }


                IEditSystem editSystem;
                var preferredEditor = editableAttribute.PreferredEditor;
                if (preferredEditor != null) {
                    editSystem = _controlFuncs.Select(i => i.Value).FirstOrDefault(i => i.GetType() == preferredEditor);
                } else {
                    // If it's null, test if it's an enum.)
                    if (!_controlFuncs.TryGetValue(fieldType, out editSystem) && fieldType.BaseType != null)
                        _controlFuncs.TryGetValue(fieldType.BaseType, out editSystem);
                }

                if (editSystem != null) {
                    var labelText = editableAttribute.PrettyName;


                    object existingFieldValue = null;
                    if (_instance != null)
                        existingFieldValue =
                            _instance.GetType().GetProperty(prop.Name, _bindingFlags).GetValue(_instance, null);

                    var createdControl = editSystem.CreateControl(_manager, _control, prop, existingFieldValue);

                    createdControl.Parent = _control;
                    var label = new Label(_manager) {
                        Text = labelText,
                        Parent = _control,
                        Width = 200,
                        Left = 16,
                        Top = SizeY
                    };
                    label.Init();

                    createdControl.Top = SizeY;
                    createdControl.Left = label.Width;

                    label.Name = String.Format("{0}Label", prop.Name);
                    SizeY += createdControl.Height + Spacing;

                    createdControl.Width = Math.Max(createdControl.Width, 200);
                    created = true;
                } else {
                    Console.WriteLine("Wrong type for {0}", fieldType);
                }
            }

            _control.Height = SizeY + Spacing;
            return created;
        }

        public void Add(Type type, IEditSystem editSystem) {
            _controlFuncs.Add(type, editSystem);
        }

        public T CreateInstance() {

            var newObject = (T)FormatterServices.GetUninitializedObject(_currentType);

            foreach (var prop in _currentType.GetProperties(_bindingFlags).Where(i => Attribute.IsDefined(i, typeof(EditableAttribute)))) {
                var control = _control.GetControl(prop.Name);

                if (control == null) {
                    Console.WriteLine("Couldn't find a matching control for type {0} (Prop Name : {1}) maybe it was incorrectly registered", prop, prop.Name);
                    continue;
                }

                var propType = prop.PropertyType;

                IEditSystem editSystem;
                // If it's null, test if it's an enum.
                if (!_controlFuncs.TryGetValue(propType, out editSystem) && propType.BaseType != null)
                    _controlFuncs.TryGetValue(propType.BaseType, out editSystem);

                if (editSystem != null) {
                    _currentType.GetProperty(prop.Name, _bindingFlags).SetValue(newObject, editSystem.GetValue(control, prop), null);
                } else {
                    Console.WriteLine("Couldn't find a renameMe for type {0} (Prop Name : {1})", propType, prop.Name);
                }
            }

            return newObject;
        }

        public Control GetControl(String name) {
            return _control.GetControl(name);
        }
    }
}
