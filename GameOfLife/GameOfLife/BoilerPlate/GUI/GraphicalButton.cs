using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;

namespace GameOfLife.BoilerPlate.GUI {
    public sealed class GraphicalButton : ImageBox {
        private readonly Texture2D _buttonTextureNormal;
        private readonly Texture2D _buttonTextureHover;
        private readonly Texture2D _buttonTexturePushed;

        private new Texture2D Image {
            get { return base.Image; }
            set {
                Width = value.Width;
                Height = value.Height;
                base.Image = value;
            }
        }

        public GraphicalButton(Manager manager, Texture2D buttonTextureNormal, Texture2D buttonTextureHover, Texture2D buttonTexturePushed)
            : base(manager) {
            _buttonTextureNormal = buttonTextureNormal;
            _buttonTextureHover = buttonTextureHover;
            _buttonTexturePushed = buttonTexturePushed;

            Image = _buttonTextureNormal;
        }


        protected override void OnMouseUp(MouseEventArgs e) {
            Image = _buttonTextureNormal;
            base.OnMouseUp(e);
        }

        protected override void OnMouseOver(MouseEventArgs e) {
            Image = _buttonTextureHover;
            base.OnMouseOver(e);
        }

        protected override void OnMouseOut(MouseEventArgs e) {
            Image = _buttonTextureNormal;
            base.OnMouseOut(e);
        }

        protected override void OnMouseDown(MouseEventArgs e) {
            Image = _buttonTexturePushed;
            base.OnMouseDown(e);
        }
    }
}
