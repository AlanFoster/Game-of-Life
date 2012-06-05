using System.Collections.Generic;
using System.Runtime.Serialization;
using GameOfLife.WorldObjects;

namespace GameOfLife.GameLogic.Assets {
    [DataContract]
    public abstract class Asset : IAsset {
        [DataMember]
        public string Name { get; protected set; }

        [DataMember]
        public AssetType AssetType { get; set; }

        public abstract string AssetPath {
          get;
        }

        [DataMember]
        public int Value { get; set; }


        protected Asset(AssetType assetType, int value, string name) {
            AssetType = assetType;
            Value = value;
            Name = name;
        }

        public virtual AssetResponse Visit(Player player, List<IAsset> assetList) {
            assetList.Add(this);
            return AssetResponse.AddedSuccessfully;
        }


        public virtual AssetResponse Remove(Player player, List<IAsset> assetList) {
            assetList.Remove(this);
            player.Cash += Value;
            return AssetResponse.RemovedSuccessfully;
        }

        public virtual void UpdateValue() { }
    }
}
