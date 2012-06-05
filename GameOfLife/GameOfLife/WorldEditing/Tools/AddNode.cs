using System;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate.ScreenManager;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.GameLogic.PureLogics;
using GameOfLife.WorldEditing.Misc;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using GameOfLife.Screens;

namespace GameOfLife.WorldEditing.Tools {
    [DataContract]
    public class AddNode : NodeTool {
        private NodeList _nodeList;
        private GameMouseState _previousMouseState;
        private ContentManager _content;
        private BindedLogic _bindedLogic;

        public AddNode(ContentManager content, NodeList nodeList) {
            _nodeList = nodeList;
            _content = content;
            _bindedLogic = new BindedLogic(new Nothing());
        }

        public override bool Initialize() {
            CurrentlySelectedNode = null;
            return base.Initialize();
        }

        public override void Apply(Node node) {
        }

        public override void Remove(Node node) {
        }

        public override void Update(GameTime gameTime) {
            var mouseState = GameMouse.GetState();

            if (mouseState.LeftButton == ButtonState.Released && _previousMouseState.LeftButton == ButtonState.Pressed) {

                if (!(CurrentlySelectedNode != null && CurrentlySelectedNode.IsMouseDown)) {
                    // Create a new node, and also set it to be the newest created node
                    if (CurrentlySelectedNode != null) CurrentlySelectedNode.SetColorState(ColorState.None);
                    CurrentlySelectedNode = CreateNode((int)World.MouseInWorld.X, (int)World.MouseInWorld.Y);
                    CurrentlySelectedNode.SetColorState(ColorState.DarkDark);
                }
            }
            _previousMouseState = mouseState;
        }

        private Node CreateNode(int x, int y) {
            var newNode = new Node(new Vector2(x, y), false);
            newNode.SetBindedLogic(_bindedLogic);
            newNode.InitializeContent(_content);

            newNode.Location -= newNode.Size / 2;
            _nodeList.Add(newNode);

            return newNode;
        }
    }
}
