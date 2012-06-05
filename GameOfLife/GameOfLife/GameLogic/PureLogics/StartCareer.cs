using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.Misc;
using GameOfLife.Data;
using GameOfLife.GameLogic.GameStates;
using GameOfLife.GameLogic.GameStates.Factories;
using GameOfLife.WorldEditing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;

namespace GameOfLife.GameLogic.PureLogics {
    [DataContract]
    [Editable("Start Career", adminRequired: true)]
    class StartCareer : PureLogic {
        public override string GetGraphicLocation() {
            return Constants.ImageIcons.Money;
        }

        public override string Description {
            get { return "Start\nCareer"; }
        }

        public override IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            // Get three random jobs
            var currentPlayer = gameInfo.CurrentPlayer;
            
            var randomJobs = CareerData.PopulateCareers().Shuffle().Where(i => i.CareerType == currentPlayer.CareerType).Take(3).ToArray();

            var waitState = StateFactory.GetState(GameStates.GameStates.Wait);
            CreateCareerWindow(gameInfo.Manager, gameInfo, randomJobs, waitState);

            return new[] { new PayDay(), waitState };
        }

        private void CreateCareerWindow(Manager manager, GameInfo gameInfo, Career[] randomJobs, IGameState waitState) {
            const int spacing = 20;
            const int width = 500;

            int yPos = spacing;
            int xPos = spacing * 4;
            var window = new Window(manager) {
                Text = "Choose a career!",
                Width = width,
                CloseButtonVisible = false,
                Resizable = false
            };

            window.Init();

            var description = new Label(manager) { Text = "Choose a random unknown career!\nWhen you select the career it will show you your fate!", Width = 400, Left = 16, Top = 16, Height = 40 };
            description.Init();
            yPos += description.Height + spacing;
            window.Add(description);

            var chosenJob = new Label(manager) { Text = String.Empty, Width = 400, Left = 16, Top = 16, Height = 70 };
            chosenJob.Init();
            window.Add(chosenJob);

            var possibleJobImages = new List<Button>();
            var chooseCareerTexture = gameInfo.Content.Load<Texture2D>("Images/career_icons/BlankJob");
            foreach (var career in randomJobs) {
                var chosenCareer = career;

                var careerButton = new Button(manager) {
                    Top = yPos,
                    Left = xPos,
                    StayOnBack = true,
                    Width = 100,
                    Height = 100,
                    Glyph = new Glyph(chooseCareerTexture)
                };
                possibleJobImages.Add(careerButton);
                careerButton.Init();
                window.Add(careerButton);

                careerButton.Click += (sender, args) => {
                    gameInfo.CurrentPlayer.CurrentCareer = chosenCareer;
                    window.CloseButtonVisible = true;

                    chosenJob.Text = "Congratulations! You have become a " +
                                       chosenCareer.Title 
                                       + "\n\nClose the window to receive your first Pay Day!";

                    int i = 0;
                    foreach (var possibleJob in possibleJobImages) {
                        possibleJob.Glyph = new Glyph(gameInfo.Content.Load<Texture2D>("images/career_icons/" + randomJobs[i++].Title));
                        possibleJob.Enabled = false;
                        possibleJob.SetSize(100, 100);
                    }

                    careerButton.Color = Color.Red;

                    yPos += chosenJob.Height + 16;

                    var close = new Button(manager) { Text = "OK", Parent = window, Top = yPos, Left = window.Width / 2 - 50 };
                    close.Click += (s, a) => {window.Close(); WindowClosed(gameInfo, waitState);};
                    close.Init();
                };

                xPos += careerButton.Width + 20;
            }

            yPos += possibleJobImages[0].Height + 16;

            chosenJob.Top = yPos;
            manager.Add(window);
            window.Closed += (sender, args) => WindowClosed(gameInfo, waitState);
        }

        private void WindowClosed(GameInfo gameInfo, IGameState wait) {
            gameInfo.Fsm.Remove(wait);
        }
    }
}
