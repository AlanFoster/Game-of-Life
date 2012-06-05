using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using GameOfLife.Screens;
using Microsoft.Xna.Framework;

namespace GameOfLife.WorldObjects {
    [DataContract]
    public abstract class GenericWorldObject<T> : WorldObject where T : class {
        public event MouseOver<T> ListenMouseOver = delegate { };
        public event MouseOut<T> ListenMouseOut = delegate { };
        public  MouseDown<T> ListenMouseDown = delegate { };
        public event MouseUp<T> ListenMouseUp = delegate { };

        protected GenericWorldObject()
            : base(null, null) {

        }

        protected GenericWorldObject(Vector2? location, Vector2? size)
            : base(location, size) {
        }

        public override void OnMouseOver(Vector2 location) {
            ListenMouseOver(this as T, location);
        }

        public override void OnMouseOut(Vector2 location) {
            ListenMouseOut(this as T, location);
        }

        public override void OnMouseDown(Vector2 location) {
            ListenMouseDown(this as T, location);
        }

        public override void OnMouseUp(Vector2 location) {
            ListenMouseUp(this as T, location);
        }

        public override void Initialize() {
            ListenMouseOver = delegate { };
            ListenMouseOut = delegate { };
            ListenMouseDown = delegate { };
            ListenMouseUp = delegate { };
            Opacity = 1f;
        }
    }
}
