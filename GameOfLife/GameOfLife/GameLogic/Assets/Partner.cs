using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLife.WorldObjects;

namespace GameOfLife.GameLogic.Assets {
    public class Partner : Transportable {
        public Partner()
            : base("Partner") {
        }

        public override string Icon {
            get { return "Images/AlertIcons/Married"; }
        }

        public override string AssetPath
        {
            get { return "Images/Assets/Partner"; }
        }

        public override AssetResponse Visit(Player player, List<IAsset> assetList) {
            var hasWifeAlready = assetList.Cast<Transportable>().Any(i => i is Partner);
            return hasWifeAlready ? AssetResponse.HasPartnerAlready : base.Visit(player, assetList);
        }
    }
}