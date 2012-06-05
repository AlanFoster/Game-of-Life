using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GameOfLife.Data;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.GameLogic.PureLogics;
using GameOfLife.Screens;
using GameOfLife.WorldEditing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.WorldObjects {
    [DataContract(Name = "Node")]
    public class Node : GenericWorldObject<Node> {
        private static readonly Vector2 MinSize = new Vector2(172, 135);
        /// <summary>
        /// Stores all nodes which point to this one, so that when this node is deleted we can easily remove all other nodes that link to this one.
        /// Only the world editor needs this requirement so the base class does not store it.
        /// </summary>
        [DataMember]
        public List<Node> Parents { get; protected set; }

        [DataMember]
        public List<Node> LinksTo { get; protected set; }

        [DataMember]
        public BindedLogic BindedLogic { get; protected set; }

        private Texture2D _tileGraphic;
        private Texture2D _tileIcon;
        private Texture2D _arrow;
        private SpriteFont _nodeFont;

        private int Radius { get { return (int)(Math.Sqrt(2) * Size.X / 2); } }

        private ContentManager _content;

        /// <summary>
        /// The world changes when you roll a 1.
        /// If this field is true, the BindedLogic associated with this Node will also change.
        /// If false, it will not change its BindedLogic.
        /// </summary>
        [DataMember]

        [Editable("Changes logic on world change")]
        public bool IsChangeable { get; set; }

        /// <summary>
        /// States whether or not admin mode is required to manipulate this node.
        /// This means, if we're not in admin mode we can't
        /// Change logic, Move Node, Delete Node, Change Links etc.
        /// If we /are/ in admin mode, we can freely manipulate the world as desired
        /// 
        /// Likewise, the world editor can modify this boolean, but only if in admin mode.
        /// </summary>
        [DataMember]
        [Editable("Admin-Mode Required to Edit", adminRequired: true)]
        public bool IsEditable { get; set; }


        public virtual String DescriptionText { get { return BindedLogic == null ? "Error" : BindedLogic.DescriptionText; } }

        public bool HasPassingLogic {
            get { return (BindedLogic != null) && BindedLogic.HasPassLogic; }
        }

        public bool IsStopSquare {
            get { return (BindedLogic != null) && BindedLogic.IsStopSquare; }
        }

        public Node(Vector2 location, bool isChangeable, bool isEditable = false)
            : base(location, null) {

            LinksTo = new List<Node>();
            Parents = new List<Node>();
            IsChangeable = isChangeable;
            IsEditable = isEditable;
        }

        public virtual void SetBindedLogic(BindedLogic bindedLogic) {
            if (bindedLogic == null) throw new NullReferenceException("Binded logic was null");

            BindedLogic = bindedLogic;
            if (_nodeFont != null) {
                ReCalculateVisuals();
            }
        }

        public static Vector2 MaxSize;

        private void ReCalculateVisuals() {
            Vector2 physicalSize = _nodeFont.MeasureString(DescriptionText) + new Vector2(50, 50);
            Size.X = Math.Max(physicalSize.X, MinSize.X);
            Size.Y = Math.Max(physicalSize.Y, MinSize.Y);

            MaxSize.X = Math.Max(Size.X, MaxSize.X);
            MaxSize.Y = Math.Max(Size.Y, MaxSize.Y);

            // Calculate our new color
            if (BindedLogic.IsStopSquare) {
                CurrentColor = Constants.NodeColors.Halting;
            } else if (BindedLogic.HasPassLogic && !(BindedLogic.PureLogic is Travel)) {
                CurrentColor = Constants.NodeColors.Passing;
            } else {
                CurrentColor = Constants.NodeColors.Normal;
            }
            RefreshColorState();

            // Set our icon
            var iconLocation = BindedLogic.GraphicLocation;
            _tileIcon = iconLocation == null ? null : _content.Load<Texture2D>(iconLocation);
        }

        public override void LoadContent(ContentManager content) {
            if (this is StartingNode) {
                Console.WriteLine("foo");
            }
            _nodeFont = content.Load<SpriteFont>("Fonts/Courier New");
            _tileGraphic = content.Load<Texture2D>("Images/Node/Tile");
            _arrow = content.Load<Texture2D>("Images/pointsTo");


            _content = content;
            ReCalculateVisuals();
            SetColorState(ColorState.None);
            Opacity = 1f;
        }

        public override void Initialize() {
            base.Initialize();
            BindedLogic.Initialize();
        }

        /// <summary>
        /// Adds a node to the current node so that it is linked to it.
        /// </summary>
        /// <param name="node">The node to link this one to</param>
        /// <returns>
        ///          True if successfully added to the list that this node links to.
        ///          False if this node already links to that node.
        /// </returns>
        public bool AddLinkedNode(Node node) {
            if (!LinksTo.Contains(node)) {
                LinksTo.Add(node);
                Parents.Add(node);
                return true;
            }
            return false;
        }

        public void RemoveParent(Node node) {
            Parents.Remove(node);
        }

        public void AddParent(Node node) {
            Parents.Add(node);
        }

        public bool ContainsNode(Node node) {
            return LinksTo.Contains(node);
        }

        public bool RemoveNode(Node node) {
            return LinksTo.Remove(node);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            base.Draw(gameTime, spriteBatch);
            // Draw the node itself
            spriteBatch.Draw(_tileGraphic, Rectangle, OverlayColor * Opacity);

            // Draw the Icon this node has in the center of the tile if it has one
            if (_tileIcon != null) {
                spriteBatch.Draw(_tileIcon, Location + Size / 2 - new Vector2(_tileIcon.Width, _tileIcon.Height) / 2, (Color.White * 0.8f) * Opacity);
            }

            // Draw the text ontop of the Node
            DrawText(spriteBatch);
        }

        private void DrawText(SpriteBatch spriteBatch) {
            // Draw the text within the node itself, and position it in the center within each row
            var rows = DescriptionText.Split('\n');

            var drawLocation = new Vector2(0, Location.Y - _nodeFont.MeasureString(DescriptionText).Y / 2 + Size.Y / 2);
            foreach (var row in rows) {
                drawLocation.X = Location.X - _nodeFont.MeasureString(row).X / 2 + Size.X / 2;
                spriteBatch.DrawString(_nodeFont, row, drawLocation, Color.Black * Opacity);

                drawLocation.Y += _nodeFont.MeasureString(row).Y;
            }
        }

        public void DrawLinks(SpriteBatch spriteBatch) {
            // Draw all nodes that this node points to
            foreach (var linksToNodeLocation in LinksTo.Select(i => i.Center)) {
                var vectorDiff = Center - linksToNodeLocation;
                vectorDiff.Normalize();
                var rotationAngle = (float)Math.Atan2(vectorDiff.Y, vectorDiff.X);

                var boxRadius = Radius - (_arrow.Height / 2f);
                var drawArrowLocation = new Vector2(
                    (float)(Center.X + (boxRadius) * Math.Cos(Math.PI - rotationAngle)),
                    (float)(Center.Y + (boxRadius) * Math.Sin(-rotationAngle)));

                spriteBatch.Draw(_arrow, drawArrowLocation, null, Color.White, rotationAngle,
                                new Vector2(_arrow.Width, _arrow.Height) / 2, 1f, SpriteEffects.None, 1f);
            }
        }

        public override string ToString() {
            return "[Node logic :: " + BindedLogic
                + " HasPassingLogic :: " + HasPassingLogic
                + " Links To :: " + LinksTo + "]";
        }

        public Vector2 GetTravelPosition(int i, int size) {
            var pos = Math.PI * 2 * ((i + 1f) / size);
            var radius = Radius / 2f;

            return new Vector2(
                (float)(Center.X + radius * Math.Cos(pos)),
                (float)(Center.Y + radius * Math.Sin(pos)));
        }
    }
}
