using System;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.Data;
using GameOfLife.WorldEditing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;

namespace GameOfLife.GameLogic.PureLogics {
    [DataContract]
    [Editable("Get a Promotion")]
    class Promotion : PureLogic {
        public override string GetGraphicLocation() {
            return Constants.ImageIcons.Money;
        }

        public override string Description {
            get { return "Promotion\n+Payday"; }
        }

        public override IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            var waitState = StateFactory.GetState(GameStates.GameStates.Wait);

            if (gameInfo.CurrentPlayer.PromotionLevel < 3) {
                CreateMessage(gameInfo.Manager, gameInfo, waitState);
            } else {
                var alert = new Alert(gameInfo.Manager, "You are already at your highest salary range!",
                                 "Cannot get promotion", true);
                alert.Init();
                gameInfo.Manager.Add(alert);
                alert.Closed += (sender, args) => alertClosed(gameInfo, waitState);
            }

            return new[] { new PayDay(), waitState };
        }

        private void alertClosed(GameInfo gameInfo, IGameState waitState) {
            gameInfo.Fsm.Remove(waitState);
        }
        private void CreateMessage(Manager manager, GameInfo gameInfo, IGameState waitState) {
            var currentPlayer = gameInfo.CurrentPlayer;
            var previousPromotionLevel = currentPlayer.PromotionLevel;
            currentPlayer.IncreaseSalary();
            var newPromotionLevel = currentPlayer.PromotionLevel;

            gameInfo.Content.Load<SoundEffect>("Sounds/applause").Play();
            var messageWindow = new Window(manager) { Text = "Promotion!", AutoScroll = false, CloseButtonVisible = false};
            messageWindow.SetSize(400, 230);

            if (previousPromotionLevel != newPromotionLevel) {
                var message = new Label(manager) { Text = "Well Done! You have received a  promotion!!\nYou will now receive a higher salary.", Width = 300, Height = 40, Left = 5, Top = 10 };
                message.Init();
                message.Parent = messageWindow;
                var career = new ImageBox(manager) {
                    Image =
                        gameInfo.Content.Load<Texture2D>("Images/career_icons/" +
                                                         gameInfo.CurrentPlayer.CurrentCareer),
                    Top = 50,
                    Left = 10,
                    StayOnBack = true
                };
                career.Init();
                career.Parent = messageWindow;

                var job = new Label(manager) { Text = "Current Career:\n" + currentPlayer.CurrentCareer.Title, Top = 110, Left = 10, Width = 150, Height = 50 };
                job.Init();
                job.Parent = messageWindow;

                var salary = new Label(manager) {
                    Text = String.Format("New Promotion Level : {0}\n\n" +
                                         "Previous Salary : ${1:N0}\n\n" +
                                         "New Salary: ${2:N0}",
                   newPromotionLevel + 1, currentPlayer.CurrentCareer.Salary[previousPromotionLevel], currentPlayer.CurrentCareer.Salary[newPromotionLevel]),
                    Top = 65, Left = 140, Width = 200, Height = 300,
                    Alignment = Alignment.TopLeft
                };
                salary.Init();
                salary.Parent = messageWindow;


            } else {
                var message = new Label(manager) {
                    Text = String.Format("You have reached the maximum promotion level of {0} for being a {1} ",
                    currentPlayer.PromotionLevel, currentPlayer.CurrentCareer),
                    Width = 300,
                    Height = 40,
                    Left = 5,
                    Top = 10
                };
                message.Init();
                message.Parent = messageWindow;
            }

            var close = new Button(manager) { Text = "OK", Top = 150, Left = messageWindow.Width / 2 - 50 };
            close.Init();
            close.Parent = messageWindow;
            close.Click += (sender, args) => { messageWindow.Close(); gameInfo.Fsm.Remove(waitState); };
            gameInfo.Manager.Add(messageWindow);
            messageWindow.Init();
            gameInfo.Content.Load<SoundEffect>("Sounds/coins").Play();
        }
    }
}
