using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.GameLogic.GameStates;
using GameOfLife.GameLogic.GameStates.Factories;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.Screens;
using Microsoft.Xna.Framework;

namespace GameOfLife.BoilerPlate.FSM {
    public class StateFactory {
        private static StateFactory _instance;
        private readonly Dictionary<GameStates, IStateFactory> factories;

        public StateFactory() {
            factories = new Dictionary<GameStates, IStateFactory>();
        }

        private static StateFactory GetInstance() {
            return _instance;
        }

        public static void SetInstance(StateFactory instance) {
            _instance = instance;
        }

        public static void AddFactory(GameStates gameStates, IStateFactory stateFactory) {
            GetInstance().factories.Add(gameStates, stateFactory);
        }

        public static IGameState GetState(GameStates gameStates) {
            IStateFactory stateFactory;
            if (!GetInstance().factories.TryGetValue(gameStates, out stateFactory)) {
                Console.WriteLine("Matching factory not found for {0}", gameStates);
                throw new NotImplementedException();
            }

            return stateFactory.GetState();
        }
    }
}
