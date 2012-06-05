using System;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.GameLogic.PureLogics;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Factories {
    public class RollFactory : IStateFactory {
        private readonly Lazy<RollLogic> _singleton = new Lazy<RollLogic>();
        private GameInfo GameInfo { get; set; }

        public RollFactory(GameInfo gameInfo) {
            GameInfo = gameInfo;
        }

        public IGameState GetState() {
            GameInfo.CreateMessage("Click the Spinner!");
            GameInfo.ClearTargets();
            GameInfo.GetNextPlayer();
            GameInfo.PanCameraToObject(GameInfo.Spinner);
            GameInfo.AddLowPriorityTarget(GameInfo.Spinner);
            return _singleton.Value;
        }

        private sealed class RollLogic : IGameState {
            public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
                // If the player has unfinished business on a square execute it instead of spinning.
                var currentPlayerNode = gameInfo.CurrentPlayer.CurrentNode;

                if (currentPlayerNode.BindedLogic.PureLogic is TakeExam && !gameInfo.CurrentPlayer.PassedExam) {
                    return new[] { StateFactory.GetState(GameStates.Execute) };
                }

                return new[] { 
                    StateFactory.GetState(GameStates.ChooseNextNode),
                    StateFactory.GetState(GameStates.EndSpin),
                    StateFactory.GetState(GameStates.Spin)
                };
            }
        }
    }

}
