using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.GameLogic.Assets;
using GameOfLife.GameLogic.GameStates;
using GameOfLife.GameLogic.GameStates.Actual;
using GameOfLife.GameLogic.GameStates.Factories;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.WorldEditing;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using TomShane.Neoforce.Controls;
using Console = System.Console;

namespace GameOfLife.GameLogic.PureLogics {
    /// <summary>
    /// 
    /// 
    /// This passable tile logic offers the ability for the player to travel to other place on the
    /// +world.
    /// 
    /// The logic is ::
    /// User can ignore this space, move the remaining spin and do whatever the landing space says as per BAU.
    ///     OR
    /// Jump from the current Node to any other same-type TransportType, collect a passport stamp, and end 
    /// the turn.
    /// </summary>
    [DataContract]
    [Editable("Transport")]
    public class Travel : PureLogic {
        [Editable("Given Passport Stamp")]
        [DataMember]
        public IslandType IslandType { get; private set; }

        [Editable("Transport Type")]
        [DataMember]
        private TransportType TransportType { get; set; }

        private Alert _confirmWindow;

        public Travel(IslandType islandType, TransportType transportType) {
            IslandType = islandType;
            TransportType = transportType;
        }

        public override string GetGraphicLocation() {
            return String.Concat("Images/Node/Icons/Transport/", IslandType);
        }

        public override string Description {
            get { return String.Empty; }
        }

        public override IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            var waitState = StateFactory.GetState(GameStates.GameStates.Wait);
            if (IslandType != IslandType.StartIsland) {
                OfferTravel(gameInfo.Manager, gameInfo, waitState);
            } else {
                ForceTravel(gameInfo.Manager, gameInfo, waitState);
            }
            gameInfo.Manager.Add(_confirmWindow);
            return new[] { waitState };
        }

        private void OfferTravel(Manager manager, GameInfo gameInfo, IGameState waitState) {
            // Create a window asking if the player wants to travel
            _confirmWindow = new ConfirmWindow(manager,
                String.Format("Do you want to travel to a different island?\n" +
                             "Travelling will earn you a new passport stamp\n" +
                             "Deciding to travel will end your current go\n"),
                "Travel to another island?", icon: "Images/AlertIcons/Travel");
            ((ConfirmWindow)_confirmWindow).AffirmButton.Click += (sender, args) => BeginTravel(gameInfo, waitState);
            _confirmWindow.DenyButton.Click
                += (sender, args) => { _confirmWindow.Close(); gameInfo.Fsm.Remove(waitState); };

        }

        private void ForceTravel(Manager manager, GameInfo gameInfo, IGameState waitState) {
            // Tell the user they're about to travel!
            _confirmWindow = new Alert(manager,
                "You must now travel to a different island.\n"
                + "Click on the flashing travel icons to begin!", icon: "Images/AlertIcons/Travel");
            _confirmWindow.DenyButton.Click
                += (sender, args) => BeginTravel(gameInfo, waitState);
        }


        private void BeginTravel(GameInfo gameInfo, IGameState waitState) {
            _confirmWindow.Close();
            gameInfo.CurrentPlayer.RollAmount = 0;

            var validTransportNodes = (from travelNode in gameInfo.WorldNodes.Where(i => i.BindedLogic.PureLogic is Travel)
                                       let travelLogic = (Travel)travelNode.BindedLogic.PureLogic
                                       where travelLogic.IslandType != IslandType && travelLogic.IslandType != IslandType.StartIsland && travelLogic.TransportType == TransportType
                                       select travelNode).ToList();

            ChoosePathHelper.TravelToNodes(validTransportNodes, gameInfo, (node, location) => NodeChosen(node, gameInfo, waitState));
        }

        private void NodeChosen(Node node, GameInfo gameInfo, IGameState waitState)
        {
            gameInfo.Fsm.Remove(waitState);
            gameInfo.ClearTargets();
            gameInfo.PanCameraToObject(gameInfo.CurrentPlayer);
            var currentPlayer = gameInfo.CurrentPlayer;
            currentPlayer.NextNode = node;

            // Give the player a passport for the island they are going to arrive at 
            var assetResponse = currentPlayer.Accept(new PassportStamp(((Travel)node.BindedLogic.PureLogic).IslandType));
            // Give the player a car modification if required

            currentPlayer.SetTransport(TransportType);

            if (assetResponse == AssetResponse.CollectedAllPassportStamps) {
                if(gameInfo.GameRuleType == GameRuleType.Passport) {
                    gameInfo.Fsm.LazyPush(StateFactory.GetState(GameStates.GameStates.EndGame));
                }
            }

            gameInfo.CreateMessage(String.Format("You have been given a {0} passport token!", IslandType));

            gameInfo.Fsm.Push(StateFactory.GetState(GameStates.GameStates.VisuallyMovePlayer));
        }

        public override string ToString() {
            return "Island type :: " + IslandType + " Transport type :: " + TransportType;
        }
    }
}
