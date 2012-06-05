using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.GameLogic.Assets;
using GameOfLife.GameLogic.GameStates;
using GameOfLife.GameLogic.GameStates.Factories;
using GameOfLife.WorldEditing;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.PureLogics {
    [DataContract]
    public abstract class AddTransportable : PureLogic {
        protected Transportable Transportable { get; set; }
        protected abstract String SuccessText { get; }

        protected AddTransportable() {
            Initialize();
        }

        public override IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            return HandleAssetResponse(gameInfo, gameInfo.CurrentPlayer.Accept(Transportable));
        }

        protected virtual IGameState[] HandleAssetResponse(GameInfo gameInfo, AssetResponse assetResponse) {
            switch (assetResponse) {
                case AssetResponse.AddedSuccessfully:
                    return CreateSuccessWindow(gameInfo);
                case AssetResponse.CarFull:
                    return CreateCarFullWindow(gameInfo);
            }

            throw new NotImplementedException("AssetResponse is not handled correctly.");
        }

        private IGameState[] CreateCarFullWindow(GameInfo gameInfo) {
            var waitState = StateFactory.GetState(GameStates.GameStates.Wait);
            var alert = new Alert(gameInfo.Manager, String.Format("Your car is too full for another {0}", Transportable.Name), "Car Full!", icon: "Images/AlertIcons/Fail");
            alert.Closed += (sender, args) => WindowClosed(gameInfo, waitState);
            gameInfo.Manager.Add(alert);
            return new[] { waitState };
        }

        private IGameState[] CreateSuccessWindow(GameInfo gameInfo) {
            var waitState = StateFactory.GetState(GameStates.GameStates.Wait);
            var alert = new Alert(gameInfo.Manager, SuccessText, "You now have a new " + Transportable.Name, icon: Transportable.Icon);
            alert.Closed += (sender, args) => WindowClosed(gameInfo, waitState);
            gameInfo.Manager.Add(alert);
            return new[] { waitState };
        }

        protected void WindowClosed(GameInfo gameInfo, IGameState waitState) {
            gameInfo.Fsm.Remove(waitState);
        }

        // Both abstract and virtual/overriden
        public abstract override void Initialize();
    }
}
