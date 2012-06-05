using GameOfLife.BoilerPlate.FSM;

namespace GameOfLife.GameLogic.GameStates.Factories {
    public interface IStateFactory {
        IGameState GetState();

    }
}
