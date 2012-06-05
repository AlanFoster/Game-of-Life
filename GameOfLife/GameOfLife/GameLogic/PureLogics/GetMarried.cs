using System;
using System.Globalization;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.Data;
using GameOfLife.GameLogic.Assets;
using GameOfLife.GameLogic.GameStates;
using GameOfLife.GameLogic.GameStates.Factories;
using GameOfLife.WorldEditing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GameOfLife.GameLogic.PureLogics {
    [Editable("Become Married")]
    [DataContract]
    class GetMarried : AddTransportable {
        [DataMember]
        [Editable("Anniversary Cash Given")]
        private int AnniversaryAmount { get; set; }

        public override string GetGraphicLocation() {
            return Constants.ImageIcons.Heart;
        }

        public override string Description {
            get { return String.Format("Marry\nOr Get\n${0:N0}", AnniversaryAmount); }
        }

        public override void Initialize() {
            Transportable = new Partner();
        }

        public GetMarried(int anniversaryAmount)
            : base() {
            AnniversaryAmount = anniversaryAmount;
        }

        protected override string SuccessText {
            get { return "Congratulations you are now Married!!"; }
        }

        protected override IGameState[] HandleAssetResponse(GameInfo gameInfo, AssetResponse assetResponse) {
            if (assetResponse == AssetResponse.HasPartnerAlready) {
                return CreateAlreadyMarriedWindow(gameInfo);
            }
            gameInfo.Content.Load<SoundEffect>("Sounds/wedding").Play();
            return base.HandleAssetResponse(gameInfo, assetResponse);
        }

        private IGameState[] CreateAlreadyMarriedWindow(GameInfo gameInfo) {
            var waitState = StateFactory.GetState(GameStates.GameStates.Wait);
            gameInfo.CurrentPlayer.Cash += AnniversaryAmount;
            var alert = new Alert(gameInfo.Manager, String.Format("You are already married!\nCelebrate your anniversary and get ${0:N0}", AnniversaryAmount), "Celebrate your Anniversary", icon: "Images/AlertIcons/Married");
            alert.Closed += (sender, args) => WindowClosed(gameInfo, waitState);
            gameInfo.Manager.Add(alert);
            return new[] { waitState };
        }

    }
}
