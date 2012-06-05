using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate;
using GameOfLife.Data;
using GameOfLife.WorldObjects;

namespace GameOfLife.GameLogic.Assets {
    [DataContract]
    public abstract class Transportable : Asset {
        public abstract String Icon { get; }
        protected Transportable(string name)
            : base(AssetType.Transportable, Constants.GameRules.TransportableValue, name) {
        }

        public override AssetResponse Visit(Player player, List<IAsset> assetList) {
            return assetList.Count >= player.CarSize ? AssetResponse.CarFull : base.Visit(player, assetList);
        }
    }
}
