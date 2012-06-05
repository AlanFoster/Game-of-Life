using System.Collections.Generic;
using System.Runtime.Serialization;
using GameOfLife.WorldObjects;

namespace GameOfLife.GameLogic.Assets {
    public interface IAsset {
        AssetType AssetType { get; }
        string AssetPath { get; }
        int Value { get; }
        string Name { get; }
        void UpdateValue();
        AssetResponse Visit(Player player, List<IAsset> assetList);
        AssetResponse Remove(Player player, List<IAsset> assets);
    }
}