using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Actual {
    class EndOfSpinnerSpunLogic : IGameState {
        public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            var spinnedNumber = gameInfo.Spinner.SpinnedNumber;
            gameInfo.ClearTargets();
            gameInfo.CurrentPlayer.RollAmount = spinnedNumber;
            gameInfo.PanCameraToObject(gameInfo.CurrentPlayer);
            switch (spinnedNumber) {
                case 1:
                    return new[] {StateFactory.GetState(GameStates.ChangeWorld)};
                case 10: return gameInfo.GameRuleType == GameRuleType.Retirement ? new[] { StateFactory.GetState(GameStates.IncreaseAge) } : null;
            }
            return null;
        }
    }
}
