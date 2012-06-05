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
    [Editable("Receieve Payday")]
    class PayDay : PureLogic {
        public override string GetGraphicLocation() {
            return Constants.ImageIcons.Money;
        }

        public override string Description {
            get { return "PayDay"; }
        }

        public override IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            if (gameInfo.CurrentPlayer.CurrentCareer != null) {
                var waitState = StateFactory.GetState(GameStates.GameStates.Wait);
                CreateMessage(gameInfo.Manager, gameInfo, waitState);
                return new[] { waitState };
            }

            return null;
        }

        private void CreateMessage(Manager manager, GameInfo gameInfo, IGameState waitState) {
            var currentPlayer = gameInfo.CurrentPlayer;

            var messageWindow = new Window(manager) { Text = "Pay Day!", AutoScroll = false, CloseButtonVisible = false};
            messageWindow.Init();
            messageWindow.SetSize(400, 230);
            var message = new Label(manager) { Text = "Well Done! you have received your Pay Day! \nBelow are the details.", Width = 400, Height = 40, Left = 5, Top = 10 };
            message.Init();
            message.Parent = messageWindow;
            var careerImage = gameInfo.Content.Load<Texture2D>("Images/career_icons/" +
                                                               gameInfo.CurrentPlayer.CurrentCareer);
            var career = new ImageBox(manager) {
                Image = careerImage,
                Width = careerImage.Width,
                Height = careerImage.Height,
                Top = 50,
                Left = 16,
                StayOnBack = true
            };
            career.Init();
            career.Parent = messageWindow;

            var icon = new ImageBox(manager) {
                Image =
                    gameInfo.Content.Load<Texture2D>("Images/payday"),
                Top = 60,
                Left = career.Left + career.Width + 70
            };
            icon.Init();
            icon.Parent = messageWindow;

            var job = new Label(manager) { Text = "Current Career:\n" + currentPlayer.CurrentCareer.Title, Top = 110, Left = 10, Width = 150, Height = 50 };
            job.Init();
            job.Parent = messageWindow;

            var salary = new Label(manager) {
                Text = String.Format("Level {1} Promotion\nReceived: ${0:N0}",
                currentPlayer.CurrentCareer.Salary[currentPlayer.PromotionLevel],
                currentPlayer.PromotionLevel + 1),
                Top = 110, Left = 140, Width = 140, Height = 50
            };
            salary.Init();
            salary.Parent = messageWindow;


            var close = new Button(manager) { Text = "OK", Top = 160, Left = messageWindow.Width / 2 - 50 };
            close.Init();
            close.Parent = messageWindow;
            close.Click += (sender, args) => { messageWindow.Close(); WindowClosed(gameInfo, waitState); };
            messageWindow.Closed += (sender, args) => WindowClosed(gameInfo, waitState);
            gameInfo.Manager.Add(messageWindow);
            messageWindow.Init();
            gameInfo.Content.Load<SoundEffect>("Sounds/coins").Play();
        }

        private void WindowClosed(GameInfo gameInfo, IGameState waitState)
        {
            gameInfo.CurrentPlayer.ReceivePay();
            gameInfo.Fsm.Remove(waitState);
        }
    }
}
