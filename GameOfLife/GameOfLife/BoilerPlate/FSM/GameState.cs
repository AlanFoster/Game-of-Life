using GameOfLife.GameLogic;

namespace GameOfLife.BoilerPlate.FSM {
    public interface IGameState : IState<IGameState, GameInfo> {
    }
}
