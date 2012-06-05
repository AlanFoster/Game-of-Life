using System;
using GameOfLife.BoilerPlate.FSM;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Factories {

    public class EndSpinFactory : IStateFactory {
        private readonly Lazy<SpinnerEnded> _singleton = new Lazy<SpinnerEnded>();

        public IGameState GetState(GameStates gameStates) {
            return _singleton.Value;
        }

        private sealed class SpinnerEnded : IGameState {
            public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
                var spinnedNumber = gameInfo.Spinner.SpinnedNumber;
                gameInfo.CurrentPlayer.RollAmount = spinnedNumber;

                // TODO UNCOMMENT THIS CODE TODO
                return null;
                switch (spinnedNumber) {
                    case 1: return new[] { StateFactory.GetState(GameStates.ChangeWorld) };
                    case 10: return new[] { StateFactory.GetState(GameStates.IncreaseAge) };
                    default: return null;
                }
            }
        }
    }
}
