using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using GameOfLife.BoilerPlate;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.BoilerPlate.ScreenManager;
using GameOfLife.Data;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.GameLogic.PureLogics;
using GameOfLife.Screens;
using GameOfLife.WorldEditing.Misc;
using GameOfLife.WorldEditing.MouseListeners;
using GameOfLife.WorldEditing.Tools;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;
using Console = System.Console;

namespace GameOfLife.WorldEditing {
    public class WorldEditor : World {
        private Dictionary<ToolType, Tool> toolList;
        private Tool _currentTool;
        private NodeList WorldNodes;
        private SideBar _sideBar;
        private SideBarPanel _reflectionArea;
        private Highlight highlight;
        private WorldObject _currentlySelectedWorldObject;

        private const int Padding = 16;
        protected String WorldName { get; set; }

        public WorldEditor(String worldName, List<WorldObject> worldObjects)
            : base(worldObjects, Constants.ScreenNames.WorldEditor) {

            WorldName = worldName;
            toolList = new Dictionary<ToolType, Tool>();
        }

        public override void Initialize() {
            base.Initialize();

            WorldNodes = new NodeList(WorldObjects);
            CreateTools();
            WorldNodes.AddCallback(WorldNodesCallback);
            SetTool(ToolType.Select);

            CreateCommonInterfaces();

            CameraPos = WorldObjects.OfType<StartingNode>().First().Center;

            //  SetSelectedNode(WorldObjects.OfType<Node>().First());
            // EditNode(WorldObjects.OfType<Node>().First());

            _objectDragger = new ObjectDragger();
        }

        private ObjectDragger _objectDragger;

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            _objectDragger.Update(gameTime);
            // If this were more advanced, I might consider a chain of command design pattern?
            if (!_objectDragger.Dragging) {
                _currentTool.Update(gameTime);
            }
        }

        private void CreateTools() {
            highlight = new Highlight();
            AddTool(ToolType.Add, new AddNode(Content, WorldNodes));
            AddTool(ToolType.Delete, new RemoveNode(WorldNodes));
            AddTool(ToolType.LinkNodes, new LinkNode());
            AddTool(ToolType.Save, new Save(PanCameraToObject, WorldName, WorldObjects));
            AddTool(ToolType.Select, new SelectObject());
        }

        public void AddTool(ToolType toolType, Tool tool) {
            tool.DependencyInjection(CreateAlert, GetCurrentlySelectedNode, SetSelectedWorldObject);
            toolList.Add(toolType, tool);
        }

        public void SetTool(ToolType toolType) {
            Tool nextTool;

            if (!toolList.TryGetValue(toolType, out nextTool)) {
                Console.WriteLine("No matching tool found for :: {0}", toolType);
                return;
            }

            if (!nextTool.Initialize()) return;

            if (_currentTool != null) {
                foreach (var worldObject in WorldObjects) {
                    _currentTool.Remove(worldObject);
                }
            }

            foreach (var worldObject in WorldObjects) {
                nextTool.Apply(worldObject);
            }

            _currentTool = nextTool;
        }

        private void WorldNodesCallback(Node node) {
            node.AddMouseLogic(highlight);
            if (_currentTool != null) {
                _currentTool.Apply(node);
            }
        }

        private Node GetCurrentlySelectedNode() {
            return _currentlySelectedWorldObject as Node;
        }

        private void SetSelectedWorldObject(WorldObject worldObject) {
            if (_currentlySelectedWorldObject != null) {
                _currentlySelectedWorldObject.ResetColorState();
            }

            SetSelectedNode(worldObject as Node);
            _currentlySelectedWorldObject = worldObject;
            _objectDragger.CurrentlySelectedObject = worldObject;
        }

        private void SetSelectedNode(Node node) {
            var currentlySelectedNode = GetCurrentlySelectedNode();
            if (currentlySelectedNode != null) {
                currentlySelectedNode.AddMouseLogic(highlight);
                foreach (var child in currentlySelectedNode.LinksTo) {
                    child.RememberColorState(ColorState.None);
                }
            }

            if (node == null && _reflectionArea != null) {
                _reflectionArea.Visible = false;
            } else if (node != null) {
                currentlySelectedNode = node;
                currentlySelectedNode.SetColorState(ColorState.DarkDark);
                currentlySelectedNode.RemoveMouseLogic(highlight);
                foreach (var child in node.LinksTo) {
                    child.RememberColorState(ColorState.Glow);
                    child.SetColorState(ColorState.Glow);
                }

                if (_reflectionArea == null) {
                    CreateReflectionOptions();
                }
                _reflectionArea.Visible = true;
            }
        }

        private void CreateAlert(String message, String icon) {
            var alert = new Alert(ControlManager.Manager, message, icon: icon);
            EnableWorld(false);
            alert.Closed += (sender, args) => EnableWorld(true);
            ControlManager.Add(alert);
        }

        private void CreateCommonInterfaces() {
            CreateSideBar();
            CreateToolOptions();
        }

        private void CreateSideBar() {
            _sideBar = new SideBar(ControlManager.Manager) { StayOnBack = true, Passive = true, Width = 250, Height = ScreenHeight };
            _sideBar.Left = ScreenWidth - _sideBar.Width;
            _sideBar.Init();
            ControlManager.Add(_sideBar);
        }

        private int ToolOptionsY = Padding;
        private void CreateToolOptions() {
            foreach (var enumOption in Enum.GetValues(typeof(ToolType)).Cast<ToolType>()) {
                var button = new Button(ControlManager.Manager) { Text = enumOption.ToString(), Left = Padding, Top = ToolOptionsY, Parent = _sideBar, Width = _sideBar.Width - Padding * 2 };
                var enumType = enumOption;
                button.Click += (sender, args) => SetTool(enumType);
                button.Init();
                _sideBar.Add(button);

                ToolOptionsY += button.Height + Padding;
            }

            var back = new Button(ControlManager.Manager) { Text = "Return to Menu", Top = ScreenHeight - Padding * 2, Left = Padding, Parent = _sideBar, Width = _sideBar.Width - Padding * 2 };
            back.Init();
            back.Text = "Back to previous screen";
            back.Click += (sender, args) => ScreenManager.SwapScreens(this, Constants.ScreenNames.WorldEditorSetup);
            _sideBar.Add(back);
        }

        private void CreateReflectionOptions() {
            _reflectionArea = new SideBarPanel(ControlManager.Manager) {
                Passive = true,
                Parent = _sideBar,
                Left = 16,
                Top = ToolOptionsY,
                Height = 56,
                CanFocus = false
            };

            _reflectionArea.Width = _sideBar.Width - _reflectionArea.Left;
            _reflectionArea.Init();
            _reflectionArea.Parent = _sideBar;

            var editButton = new Button(ControlManager.Manager) { Text = "Edit Node Logic", Left = 16, Top = 16, Width = _reflectionArea.Width - 32 };
            editButton.Init();
            editButton.Parent = _reflectionArea;
            editButton.Click += (sender, args) => EditNode(GetCurrentlySelectedNode());
        }

        public void EditNode(Node node) {
            // Create the node editing instance if needed
            if (_logicWindow == null) {
                _logicWindow = new NodeEditingWindow(ControlManager.Manager) { Resizable = true };
                _logicWindow.Init();
                _logicWindow.Closing += (sender, args) => LogicWindowClosed(args);
                ControlManager.Add(_logicWindow);
            }

            _logicWindow.CreateInterface(node);

            _logicWindow.Show();
            EnableWorld(false);
            _reflectionArea.Show();
        }

        public void EditObject(BindedLogic selectedLogic) {
            ((Node)_currentlySelectedWorldObject).SetBindedLogic(selectedLogic);
        }

        private NodeEditingWindow _logicWindow;

        private void LogicWindowClosed(WindowClosingEventArgs args) {
            if (args != null) args.Cancel = true;
            _logicWindow.Hide();

            // Update the logic within the currently selected node
            _logicWindow.GetInstance(GetCurrentlySelectedNode());

            EnableWorld(true);
        }

        private void EnableWorld(bool enabled) {
            IterateAllControls(_sideBar, i => i.Enabled = enabled);
            Enabled = enabled;
        }

        /// <summary>
        /// Use recursion to iterate all controls and any subcontrols to perform an action on them
        /// </summary>
        /// <param name="parentControl">The control to start recursion from</param>
        /// <param name="action">The action to be applied to each control</param>
        private static void IterateAllControls(Control parentControl, Action<Control> action) {
            foreach (var childControl in parentControl.Controls) {
                IterateAllControls(childControl, action);
                action(childControl);
            }
        }
    }
}
