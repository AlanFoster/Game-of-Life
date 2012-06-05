using System;
using System.Collections.Generic;
using GameOfLife.BoilerPlate;
using GameOfLife.BoilerPlate.Misc;
using GameOfLife.Data;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GameOfLife.WorldEditing {
    public interface ILoader {
        WorldObject LoadObject(ContentManager content, WorldObject worldObject);
    }
    public class WorldLoader {
        public static WorldLoader instance { get; private set; }
        private readonly Game Game;
        private readonly ContentManager Content;
        private readonly Dictionary<Type, ILoader> loaders;

        public WorldLoader(Game game, ContentManager contentManager) {
            Game = game;
            Content = contentManager;
            loaders = new Dictionary<Type, ILoader>();
            RegisterDefaultLoaders();
        }

        public static void SetInstance(WorldLoader worldLoader) {
            instance = worldLoader;
        }

        public void RegisterLoader(Type type, ILoader loader) {
            loaders.Add(type, loader);
        }

        protected virtual void RegisterDefaultLoaders() { }

        public void LoadCustomWorld(String name, Action<List<WorldObject>> callback) {
            Persister.LoadFromDevice<List<WorldObject>>(name, (objectsLoaded => ObjectsLoaded(objectsLoaded, callback)));
        }

        public void LoadWorld(String name, Action<List<WorldObject>> callback) {
            Persister.LoadFromLoose<List<WorldObject>>(name, (objectsLoaded => ObjectsLoaded(objectsLoaded, callback)));
        }

        public void GetCustomWorldNames(Action<String[]> callback) {
            Persister.LoadAllFileNames(@"*", callback);
        }

        public void GetAllWorldNames(Action<String[]> callback) {
            GetCustomWorldNames(names => {
                var allNames = new List<String>();
                allNames.Add(Constants.Locations.DefaultWorldName);
                if (Constants.Editing.IsAdminMode) {
                    allNames.Add(Constants.Locations.TestWorldName);
                }

                allNames.AddRange(names);
                callback(allNames.ToArray());
            });
        }


        private void ObjectsLoaded(List<WorldObject> rawObjects, Action<List<WorldObject>> callback) {
            if (rawObjects == null) {
                callback(null);
                return;
            }

            foreach (var rawObject in rawObjects) {
                ILoader loader;

                if (loaders.TryGetValue(rawObject.GetType(), out loader)) {
                    loader.LoadObject(Content, rawObject);
                } else {
                    rawObject.InitializeContent(Content);
                }
            }

            callback(rawObjects);
        }
    }
}
