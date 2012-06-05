using System;
using System.Collections.Generic;
using GameOfLife.BoilerPlate.Misc;
using GameOfLife.WorldObjects;

namespace GameOfLife.GameLogic.Assets {
    public class Loan : Asset {
        /// <summary>
        /// Defined within the constructor as being the initial copy of the starting value.
        /// </summary>
        public readonly int StartingValue;

        /// <summary>
        /// The interest rate for this loan which will be applied every time PerformInterestRates is called.
        /// </summary>
        public static double CurrentInterestRate { get; private set; }

        /// <summary>
        /// Creates a new loan asset which has a negitive value attached to it to mimic the fact it is owed.
        /// </summary>
        /// <param name="value">The initial loan value, this should be negitive numbers as it's a loan</param>
        public Loan(int value)
            : base(AssetType.Loan, (int) (value * CurrentInterestRate) - 1, "Loan") {
            if (value > 0) {
                throw new ArgumentException("Input value " + value + " must be negitive as they must be paid back.");
            }
            StartingValue = value;
        }

        public static void ChangeInterestRates() {
            CurrentInterestRate = 1.025d;
            CurrentInterestRate += RandomHelper.Next(0, 5) * 0.025d;
        }

        public override string AssetPath
        {
            get { return "Images/Assets/Loan"; }
        }

        public override AssetResponse Remove(Player player, List<IAsset> assetList) {
            player.Cash += Value;
            assetList.Remove(this);

            return AssetResponse.RemovedSuccessfully;
        }

    }
}
