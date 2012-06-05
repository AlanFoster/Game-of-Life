using System;

namespace GameOfLife.GameLogic.Assets {
    [Serializable]
    public enum AssetType {
        // A member of a car IE child, pet, etc
        Transportable,
        House,
        Loan,
        PassportStamp,
        Car
    }
}
