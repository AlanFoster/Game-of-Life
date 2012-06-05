using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.Data;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.GameLogic.PureLogics;
using GameOfLife.WorldObjects;
using TomShane.Neoforce.Controls;
using Console = System.Console;

namespace GameOfLife.WorldEditing {
    class NodeEditingWindow : Window {
        private ObjectEditor<Node> _nodeEditor;
        private ObjectEditor<BindedLogic> _bindedLogicEditor;
        private ObjectEditor<PureLogic> _pureLogicEditor;
        private readonly Type[] _pureLogicTypes;

        private GroupPanel pureLogicEditing;
        private Panel pureLogicContainer;

        private Node _node;

        public NodeEditingWindow(Manager manager)
            : base(manager) {
            Text = "Edit Node Logic";
            _pureLogicTypes = GetKnownTypes();
            Width = 570;

            AutoScroll = true;
            Resizable = false;
        }


        public void CreateInterface(Node node) {
            foreach (var child in Controls.First().Controls.ToList()) Controls.First().Remove(child);

            _node = node;

            int yPos = 16;

            // Create the node editing section
            var nodeEditing = new GroupPanel(Manager) { Text = "Node Details" };
            nodeEditing.Init();
            // nodeEditing.Anchor = Anchors.Left | Anchors.Right | Anchors.Top;
            nodeEditing.Left = 16;
            nodeEditing.Top = yPos;
            nodeEditing.Width = Width - 48;
            nodeEditing.Parent = this;
            nodeEditing.Visible = true;

            _nodeEditor = new ObjectEditor<Node>(Manager, nodeEditing);
            _nodeEditor.SetInstance(node);
            _nodeEditor.CreateInterface();
            yPos += nodeEditing.Height + 16;


            // Create the binded logic section
            var bindedLogicEditing = new GroupPanel(Manager) { Text = "Node Details" };
            bindedLogicEditing.Init();
            //bindedLogicEditing.Anchor = Anchors.Left | Anchors.Right | Anchors.Top;
            bindedLogicEditing.Left = 16;
            bindedLogicEditing.Top = yPos;
            bindedLogicEditing.Width = Width - 48;
            bindedLogicEditing.Parent = this;
            bindedLogicEditing.Visible = true;

            _bindedLogicEditor = new ObjectEditor<BindedLogic>(Manager, bindedLogicEditing);
            _bindedLogicEditor.SetInstance(node.BindedLogic);
            _bindedLogicEditor.CreateInterface();

            yPos += bindedLogicEditing.Height + 16;

            // Create the pure logic section
            pureLogicEditing = new GroupPanel(Manager) {
                Text = "Node Details",
                AutoScroll = true,
                Left = 16,
                Top = yPos,
                Width = Width - 48,
                Parent = this,
                Visible = true,
                Height = 80
            };

            pureLogicEditing.Init();

            CreatePureLogicEditor(pureLogicEditing);

            Invalidate();
            Refresh();

            pureLogicEditing.Invalidate();
            pureLogicEditing.Refresh();
        }

        private void CreatePureLogicEditor(Control parent) {
            var prettyNames = new string[_pureLogicTypes.Count()];
            var index = 0;
            foreach (var type in _pureLogicTypes) {
                prettyNames[index++] = EditableAttribute.GetPrettyName(type);
            }

            var logicLabel = new Label(Manager) {
                Text = "Node Logic :: ",
                Width = 150,
                Left = 16,
                Top = 16,
                Parent = parent
            };

            var logicChoices = new ComboBox(Manager) { Parent = parent, Width = 100, Left = logicLabel.Left + logicLabel.Width + 16, Top = 16, Height = 20 };
            logicChoices.Items.AddRange(prettyNames);
            logicChoices.Init();
            logicChoices.Width = 200;
            logicChoices.Parent = parent;

            pureLogicContainer = new Panel(Manager) {
                Width = parent.Width - 48,
                Top = logicChoices.Height + 32,
                Left = 16,
                AutoScroll = true,
                Parent = parent
            };
            pureLogicContainer.Init();
            pureLogicContainer.Visible = true;

            logicChoices.ItemIndexChanged += (sender, args) => PureLogicChanged(_pureLogicTypes[((ComboBox)sender).ItemIndex], pureLogicContainer);
            var currentPos = Array.IndexOf(_pureLogicTypes, _node.BindedLogic.PureLogic.GetType());
            if (currentPos == -1) {
                Console.WriteLine("Missing logic type within drop down menu");
                logicChoices.ItemIndex = 0;
            } else {
                logicChoices.ItemIndex = currentPos;
            }
        }

        private void PureLogicChanged(Type newLogic, Control pureLogicContainer) {
            foreach (var child in pureLogicContainer.Controls.First().Controls.ToList()) pureLogicContainer.Controls.First().Remove(child);
            _pureLogicEditor = new ObjectEditor<PureLogic>(Manager, pureLogicContainer);

            if (newLogic == _node.BindedLogic.PureLogic.GetType()) {
                _pureLogicEditor.SetInstance(_node.BindedLogic.PureLogic);
            }
            var interfaceCreated = _pureLogicEditor.CreateInterface(newLogic);

            if (interfaceCreated) {
                pureLogicContainer.Visible = true;
                pureLogicEditing.Height = pureLogicContainer.Height + 16 * 5;
            } else {
                pureLogicContainer.Visible = false;
                pureLogicEditing.Height = 80;
            }

            Show();
        }

        public void GetInstance(Node node) {
            var bindedLogicInstance = _bindedLogicEditor.CreateInstance();
            var pureLogicInstance = _pureLogicEditor.CreateInstance();
            bindedLogicInstance.PureLogic = pureLogicInstance;
            bindedLogicInstance.Initialize();

            var nodeInstance = _nodeEditor.CreateInstance();
            node.IsChangeable = nodeInstance.IsChangeable;
            if (Constants.Editing.IsAdminMode) {
                node.IsEditable = nodeInstance.IsEditable;
            }
            node.SetBindedLogic(bindedLogicInstance);
        }

        private static Type[] GetKnownTypes() {
            var type = typeof(WorldEditor);
            var pureLogicType = typeof(PureLogic);
            var editableList = type.Assembly.GetTypes().Where(i => i.IsSubclassOf(pureLogicType)
                && Attribute.IsDefined(i, typeof(EditableAttribute))
                // Logical implication ~pVq
                // If a control needs admin mode, then we check the value of IsAdminMode, otherwise true.
                && (!EditableAttribute.GetEditableAttribute(i).AdminRequired || Constants.Editing.IsAdminMode));
            return editableList.ToArray();
        }


        public override void Show() {
            base.Show();
            Height = pureLogicEditing.Top + pureLogicEditing.Height + 40;
            pureLogicEditing.Invalidate();
            pureLogicEditing.Refresh();
            Invalidate();
            Refresh();
        }
    }
}
