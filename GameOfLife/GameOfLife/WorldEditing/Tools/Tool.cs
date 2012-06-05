using System;
using System.Runtime.Serialization;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;

namespace GameOfLife.WorldEditing.Tools {
    [DataContract]
    public abstract class Tool {
        private Action<String, String> _createAlert;
        protected Func<WorldObject> GetWorldObject;
        protected Action<WorldObject> SetWorldObject;

        public virtual bool Initialize() {
            return true;
        }

        public void DependencyInjection(Action<String, String> createAlert, Func<WorldObject> getWorldObject, Action<WorldObject> setWorldObject) {
            _createAlert = createAlert;
            GetWorldObject = getWorldObject;
            SetWorldObject = setWorldObject;
        }

        protected void CreateAlert(String message, String icon = null) {
            _createAlert(message, icon);
        }

        public abstract void Apply(WorldObject worldObject);
        public abstract void Remove(WorldObject worldObject);
        public abstract void Update(GameTime gameTime);
    }
}
