using System;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.Data;
using GameOfLife.GameLogic.Assets;
using GameOfLife.GameLogic.GameStates;
using GameOfLife.GameLogic.GameStates.Factories;
using GameOfLife.WorldEditing;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.PureLogics {
    [DataContract]
    [Editable("Have A Child")]
    class GiveChild : AddTransportable {

        public override string GetGraphicLocation() {
            return Constants.ImageIcons.Child;
        }

        protected override string SuccessText {
            get { return "You and your partner have a child!"; }
        }

        protected override IGameState[] HandleAssetResponse(GameInfo gameInfo, AssetResponse assetResponse) {
            if (assetResponse == AssetResponse.HasNoPartner) {
                return CreateNoPartnerWindow(gameInfo);
            }

            return base.HandleAssetResponse(gameInfo, assetResponse);
        }

        public override string Description {
            get { return "Have\nAn\nChild"; }
        }

        public override void Initialize() {
            Transportable = new Child();
        }

        private IGameState[] CreateNoPartnerWindow(GameInfo gameInfo) {
            var waitState = StateFactory.GetState(GameStates.GameStates.Wait);
            var alert = new Alert(gameInfo.Manager, "You can not have children\nYou must have a partner", "No partner", icon: "Images/AlertIcons/NoPartner");
            alert.Closed += (sender, args) => WindowClosed(gameInfo, waitState);
            gameInfo.Manager.Add(alert);
            return new[] { waitState };
        }
    }
}
