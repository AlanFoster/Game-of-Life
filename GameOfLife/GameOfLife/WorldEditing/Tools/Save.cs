using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.BoilerPlate.Misc;
using GameOfLife.Data;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.GameLogic.PureLogics;
using GameOfLife.Screens;
using GameOfLife.WorldEditing.worldvalidation;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;

namespace GameOfLife.WorldEditing.Tools {
    public class Save : Tool {
        private List<WorldObject> _worldObjects;
        private List<IWorldValidator> _validationRules;
        private String _worldName;
        private Action<WorldObject> FocusOnObject;

        public Save(Action<WorldObject> focusOnObject, String worldName, List<WorldObject> worldObjects) {
            FocusOnObject = focusOnObject;
            _worldObjects = worldObjects;
            _worldName = worldName;

            CreateWorldValidations();
        }

        private void CreateWorldValidations() {
           _validationRules = new List<IWorldValidator>();
           AddValidation(new AllLinked());
           AddValidation(new ValidBindedLogic());
        }

        public void AddValidation(IWorldValidator validator) {
            _validationRules.Add(validator);
        }

        public override bool Initialize() {
            base.Initialize();
            if(IsValidWorld()) {
                Persister.SaveToDevice(_worldObjects, _worldName, WorldSaved);
            }

            return false;
        }

        private bool IsValidWorld() {
            // If we're not in admin mode we must validate the world
            if(!Constants.Editing.IsAdminMode) {
                foreach (var validationResult in _validationRules.Select(i => i.ValidateWorld(_worldObjects)).Where(r => !r.Valid)) {
                    CreateAlert(validationResult.AlertReason, "Images/AlertIcons/Fail");
                    if(validationResult.InvalidNode != null) {
                        FocusOnObject(validationResult.InvalidNode);
                    }
                    return false;
                }
            }
            return true;
        }

        private void WorldSaved() {
            CreateAlert("World saved!", "Images/AlertIcons/Pass");
        }

        public override void Apply(WorldObject worldObject) {
        }

        public override void Remove(WorldObject worldObject) {
        }

        public override void Update(GameTime gameTime) {
        }
    }
}
