using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GameOfLife.WorldObjects;

namespace GameOfLife.GameLogic.Assets {
    [DataContract]
    public class Child : Transportable {
        public Child()
            : base("Child") {
        }

        public override string Icon
        {
            get { return "Images/AlertIcons/Child"; }
        }

        public override string AssetPath
        {
            get { return "Images/assets/Child"; }
        }

        public override AssetResponse Visit(Player player, List<IAsset> assetList) {
            var hasWife = assetList.Cast<Transportable>().Any(i => i is Partner);
            return !hasWife ? AssetResponse.HasNoPartner : base.Visit(player, assetList);
        }
    }
}
