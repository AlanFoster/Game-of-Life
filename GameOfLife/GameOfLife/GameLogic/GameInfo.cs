using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.ScreenManager;
using GameOfLife.GameLogic.GameStates;
using GameOfLife.GameLogic.GameStates.Factories;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.Screens;
using GameOfLife.WorldEditing;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TomShane.Neoforce.Controls;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.GameLogic {
    /// <summary>
    /// Class of all common game details
    /// It is a class and not a struct so we can implicitly pass by reference.
    /// Singleton pattern was not choosen for testing purposes 
    /// </summary>
    public class GameInfo {
        public List<WorldObject> WorldObjects { get; set; }
        public Player[] PlayerList { get; private set; }
        public int TotalPlayers { get; private set; }
        public Node StartingNode { get; private set; }
        public Player CurrentPlayer { get; private set; }
        public int CurrentPlayerIndex { get; private set; }
        public Spinner Spinner { get; private set; }
        public List<BindedLogic> BindedLogicPool { get; private set; }
        public GameRuleType GameRuleType { get; private set; }
        public IOrderedEnumerable<Player> PlayerOrder { get { return PlayerList.OrderByDescending(i => i.TotalValue);  } }

        public int AgeCounterCurrent { get; private set; }
        public int AgeCounterTarget { get; private set; }


        public GameInfo(List<WorldObject> worldObjects, Player[] playerList, int ageCounterTarget, GameRuleType gameRuleType) {
            AgeCounterTarget = ageCounterTarget;
            GameRuleType = gameRuleType;

            WorldObjects = worldObjects;
            WorldNodes = worldObjects.OfType<Node>().ToList();

            StartingNode = worldObjects.OfType<StartingNode>().First();
            Spinner = worldObjects.OfType<Spinner>().First();
            // Mix in our common world logic with the existing changable logic
            BindedLogicPool = Data.CommonNodeLogic.GetCommonNodeLogic()
                                .Concat(worldObjects.OfType<Node>().Where(i => i.IsChangeable).Select(i => i.BindedLogic)).ToList();

            SetPlayers(playerList);
            CurrentPlayerIndex = -1;
        }

        public void GetNextPlayer() {
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % TotalPlayers;
            CurrentPlayer = PlayerList[CurrentPlayerIndex];

            if(PanCameraToObject != null) PanCameraToObject(CurrentPlayer);
        }

        private void SetPlayers(Player[] players) {
            TotalPlayers = players.Length;
            PlayerList = players;
            var i = 0;
            foreach (var player in players) {
                player.CurrentNode = StartingNode;
                player.ActualLocation = StartingNode.GetTravelPosition(i, TotalPlayers);
                WorldObjects.Add(player);
                i++;
            }
        }

        public bool IncreaseAgeCounter() {
            AgeCounterCurrent += 10;
            // Return true if the world needs to end now, otherwise false
            return AgeCounterCurrent >= AgeCounterTarget && GameRuleType == GameRuleType.Retirement;
        }

        #region .
        public Action<WorldObject> PanCameraToObject;
        public Action<WorldObject> AddLowPriorityTarget;
        public Action ClearTargets;
        public Action<String> CreateMessage;
        public CustomManager Manager { get; set; }
        public FSM<IGameState, GameInfo> Fsm;
        public ContentManager Content { get { return Manager.Game.Content; } }
        public List<Node> WorldNodes;
        #endregion
    }
}