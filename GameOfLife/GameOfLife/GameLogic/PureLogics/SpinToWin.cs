using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.BoilerPlate.Misc;
using GameOfLife.Data;
using GameOfLife.GameLogic.GameStates;
using GameOfLife.GameLogic.GameStates.Factories;
using GameOfLife.Screens;
using GameOfLife.WorldEditing;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.GameLogic.PureLogics {
    [DataContract]
    [Editable("Spin To Win")]
    class SpinToWin : PureLogic {
        /// <summary>
        /// The amount given to the player for their number being rolled on the spinner
        /// </summary>
        [DataMember]
        [Editable("Cash Amount Won")]
        private int SpinToWinCashValue { get; set; }

        [DataMember]
        private Player[] PlacedBets { get; set; }

        private MouseDown<Spinner> placeBetDelegate;
        private IGameState waitState;

        public SpinToWin(int value) {
            SpinToWinCashValue = value;
        }

        private bool inBet;

        public override string Description {
            get { return String.Format("Spin To\nWin\n${0:N0}", SpinToWinCashValue); }
        }

        public override string GetGraphicLocation() {
            return Constants.ImageIcons.SpinToWin;
        }

        public override IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            if (inBet) {
                return EvaluateSpinToWinWinners(gameInfo);
            }

            var waitStateAlert = StateFactory.GetState(GameStates.GameStates.Wait);

            var alert = new Alert(gameInfo.Manager,
                                  "Spin to Win!\n" +
                                  "Each player will click the spinner to place their bets!\n" +
                                  gameInfo.CurrentPlayer.Name + " will be allowed to place two bets", "Spin to Win",
                                  icon: Constants.ImageIcons.SpinToWin);
            gameInfo.Manager.Add(alert);
            alert.Closed += (sender, args) => gameInfo.Fsm.Remove(waitStateAlert);

            waitState = StateFactory.GetState(GameStates.GameStates.Wait);
            inBet = true;
            var gameSpinner = gameInfo.Spinner;
            gameSpinner.DrawHighlights = true;
            gameSpinner.HighlightedSections = new Color[10];
            PlacedBets = new Player[10];
            playerNum = -1;

            if (placeBetDelegate == null) placeBetDelegate = (spinner, location) => TakePlayerBet(gameInfo, spinner, location);
            gameInfo.PanCameraToObject(gameSpinner);
            gameInfo.Spinner.ListenMouseDown += placeBetDelegate;
            TakeNextPlayerBet(gameInfo);
            return new[] { this, StateFactory.GetState(GameStates.GameStates.Spin), waitState, waitStateAlert };
        }

        private Player bettingPlayer;
        private int playerNum;

        private void TakePlayerBet(GameInfo gameInfo, Spinner spinner, Vector2 clickLocation) {
            var bettedNumber = spinner.CalculateMouseNumber(clickLocation);

            if (PlacedBets[bettedNumber - 1] != null) {
                 gameInfo.CreateMessage("Number already chosen!");
                  return;
            }
            spinner.HighlightedSections[bettedNumber - 1] = bettingPlayer.PlayerColor;
            PlacedBets[bettedNumber - 1] = bettingPlayer;
            playerNum++;

            TakeNextPlayerBet(gameInfo);
        }

        private void TakeNextPlayerBet(GameInfo gameInfo) {
            if (playerNum == -1) {
                bettingPlayer = gameInfo.CurrentPlayer;
                gameInfo.CreateMessage(String.Format("{0} Place your extra bet!", bettingPlayer.Name));
            } else if (playerNum < gameInfo.PlayerList.Count()) {
                bettingPlayer = gameInfo.PlayerList[playerNum];
                gameInfo.CreateMessage(String.Format("{0} Place your bet!", bettingPlayer.Name));
            } else {
                gameInfo.CreateMessage("Click the spinner to see who won!");
                gameInfo.Spinner.ListenMouseDown -= placeBetDelegate;
                gameInfo.Fsm.Remove(waitState);
            }
        }

        private IGameState[] EvaluateSpinToWinWinners(GameInfo gameInfo) {
            int spinnedNumber = gameInfo.Spinner.SpinnedNumber;

            Player winner = PlacedBets[spinnedNumber - 1];
            if (winner != null) {
                inBet = false;
                gameInfo.Spinner.DrawHighlights = false;
                return WinnerFound(gameInfo, winner);
            }

            return NoWinnerFound(gameInfo);
        }

        private IGameState[] WinnerFound(GameInfo gameInfo, Player player) {
            gameInfo.Content.Load<SoundEffect>("Sounds/applause").Play();
            player.Cash += SpinToWinCashValue;
            var alert = new Alert(gameInfo.Manager, String.Format("Congratulations, {0}! You won {1:N0}!", player.Name, SpinToWinCashValue), "Spin To Win :: Winner!", icon: Constants.ImageIcons.SpinToWin);
            alert.Closed += (sender, args) => {
                gameInfo.Fsm.Remove(waitState);
                gameInfo.CreateMessage(String.Empty);
            };
            gameInfo.Manager.Add(alert);
            return new[] { waitState };
        }

        // The game rules dictate that we keep spinning the spinner until a winner
        // is found
        private IGameState[] NoWinnerFound(GameInfo gameInfo) {
            gameInfo.CreateMessage("No winner found! Spin again!");
            return new[] { this, StateFactory.GetState(GameStates.GameStates.Spin) };
        }
    }
}
