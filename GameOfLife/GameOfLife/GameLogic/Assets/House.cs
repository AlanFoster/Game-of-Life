using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.GameLogic.Assets {
    [DataContract]
    public class House : Asset {
        [DataMember]
        public String HouseGraphic { get; private set; }

        [DataMember]
        private Player _owner;

        [DataMember]
        public int InitialValue { get; private set; }

        public int PlayerBuyingValue { get; private set; }
        public String OwnerName { get { return _owner.Name ?? "No one"; } }
        public bool HasOwner { get { return _owner != null; } }

        public House(int value, string name, string houseGraphic)
            : base(AssetType.House, value, name) {
            InitialValue = value;
            HouseGraphic = houseGraphic;
        }

        public void ChangePrice(float percent) {
            Value = (int)(InitialValue * percent);
        }

        public override string AssetPath
        {
            get { return "Images/assets/House"; }
        }

        public override AssetResponse Visit(Player player, List<IAsset> assetList) {
            if (HasOwner) {
                return AssetResponse.HouseAlreadyOwned;
            }

            var currentPlayerHouse = player.House;
            PlayerBuyingValue = Value;
            if (currentPlayerHouse != null) {
                return AssetResponse.HouseAlreadyOwned;
            }

            // Reduce the player's cash, give them our asset and call our base logic
            player.Cash -= Value;
            _owner = player;

            return base.Visit(player, assetList);
        }

        public override AssetResponse Remove(Player player, List<IAsset> assetList) {
            _owner = null;
            PlayerBuyingValue = -1;
            return base.Remove(player, assetList);
        }
    }
}
