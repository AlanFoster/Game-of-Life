using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GameOfLife.WorldEditing;
using GameOfLife.WorldObjects;

namespace GameOfLife.GameLogic.Assets {
    /// <summary>
    /// List of possible passport stamps that can be given to a player
    /// </summary>
    [Serializable]
    public enum IslandType {
        [Editable("Beach Passport Stamp")]
        Beach,
        [Editable("City Passport Stamp")]
        City,
        [Editable("Jungle Passport Stamp")]
        Jungle,
        [Editable("Snow Passport Stamp")]
        Snow,
        [Editable("StartIsland Passport Stamp", adminRequired: true)]
        StartIsland
    }

    /// <summary>
    /// A player stamp is an asset which is given to a player when they travel to a new island
    /// </summary>
    [DataContract]
    public class PassportStamp : Asset {
        [DataMember]
        public IslandType IslandType;
        public PassportStamp(IslandType islandType)
            : base(AssetType.PassportStamp, 100000, String.Format("{0} passport stamp", islandType)) {
            IslandType = islandType;
        }

        public override string AssetPath
        {
            get { return String.Format("Images/assets/{0} passport stamp", IslandType); }
        }

        public override AssetResponse Visit(Player player, List<IAsset> assetList) {
            base.Visit(player, assetList);

            var collectedAllStamps = Enum.GetValues(typeof(IslandType)).Cast<IslandType>()
                .Except(assetList.Cast<PassportStamp>().Select(i => i.IslandType)).All(j => j == IslandType.StartIsland);

            //player.Assets[AssetType.PassportStamp];
            if (collectedAllStamps) {
                // Add the asset directly and return our response
                assetList.Add(this);
                return AssetResponse.CollectedAllPassportStamps;
            }

            return AssetResponse.AddedSuccessfully;
        }
    }
}
