using GameOfLife.BoilerPlate.FSM;
using GameOfLife.NoIdea;
using GameOfLife.WorldObjects;

namespace GameOfLife.GameLogic.GameStates.Factories {
    public class SpinFactory : IStateFactory {
        private Spinner Spinner { get; set; }

        public SpinFactory(Spinner spinner) {
            Spinner = spinner;
        }

        public IGameState GetState(GameStates gameStates) {
            return Spinner;
        }
    }
}
