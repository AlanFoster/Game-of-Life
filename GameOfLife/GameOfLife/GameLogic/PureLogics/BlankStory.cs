using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.Data;
using GameOfLife.GameLogic.GameStates;
using GameOfLife.GameLogic.GameStates.Factories;
using GameOfLife.GameLogic.Storys;
using GameOfLife.WorldEditing;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;

namespace GameOfLife.GameLogic.PureLogics {
    [DataContract]
    [Editable("Story Card")]
    class BlankStory : PureLogic {
        private Story redStory;
        private Story blackStory;
        private IGameState waitState;

        public override string GetGraphicLocation() {
            return Constants.ImageIcons.Book;
        }

        public override string Description {
            get { return "Life\nStory"; }
        }

        public override IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            if (redStory != null && blackStory != null) {
                var newLogicStates = CardSelected(gameInfo.Manager, gameInfo, gameInfo.Spinner.IsBlack ? blackStory : redStory);
                blackStory = redStory = null;
                return newLogicStates;
            }


            var possibleStories = Data.StoryCardData.PopulateStoryCards();
            var shuffledStories = possibleStories.OrderBy(i => Guid.NewGuid());

            redStory = shuffledStories.First(i => i.StoryType == StoryType.Red);
            blackStory = shuffledStories.First(i => i.StoryType == StoryType.Black);

            gameInfo.PanCameraToObject(gameInfo.Spinner);
            CreateFirstDisplayWindow(gameInfo.Manager, gameInfo, redStory, blackStory);
            waitState = StateFactory.GetState(GameStates.GameStates.Wait);
            return new[] { this, gameInfo.Spinner, waitState };
        }

        private IGameState[] CardSelected(Manager manager, GameInfo gameInfo, Story story) {
            var storyGraphic = gameInfo.Content.Load<Texture2D>(story.StoryGraphic);
            const int spacing = 20;
            const int width = 400;
            var yPos = spacing;

            var window = new Window(manager) {
                Text =
                    String.Format("Your fate has been decided! You spun a {0} story", story.StoryType), AutoScroll = false
            };
    
            window.Init();

            var storybox = new GroupBox(manager) { Width = 400, Height = 100, Left = 30, Top = yPos, Parent = window, Color = story.StoryType == StoryType.Red ? Color.Red : Color.Black, Text = ""+story.StoryType, TextColor = Color.White };
            storybox.Init();

            var storylabel = new Label(manager)
            {
                Width = storybox.Width,
                Height = storybox.Height,
                Parent = storybox,
                Text = story.DisplayedMessage,
                Left = spacing,
                StayOnTop = true
            };
            storylabel.Init();

            var imageBox = new ImageBox(manager)
            {
                Left = window.Width/2-100,
                Top = 140,
                Image = storyGraphic,
                Color = Color.White,
                Width = storyGraphic.Width,
                Height = storyGraphic.Height
            };
            imageBox.Init();
            window.Add(imageBox);
            var close = new Button(manager) { Text = "OK", Top = window.Height-50-spacing, Left = window.Width / 2 - 50, Parent = window };
            close.Init();
            close.Click += (sender, args) => window.Close();

            window.Closed += (sender, args) => WindowClosed(gameInfo);

            manager.Add(window);

            if (story.Positive)
                gameInfo.Content.Load<SoundEffect>("Sounds/applause").Play();
            else
                gameInfo.Content.Load<SoundEffect>("Sounds/sadtrombone").Play();

            return new[] { story.PureLogic, waitState };
        }

        private void CreateFirstDisplayWindow(Manager manager, GameInfo gameInfo, Story red, Story black) {
            const int spacing = 20;
            const int width = 500;
            int yPos = spacing;
            var window = new Window(manager) { Text = "Story card", Width = 600 };
            window.Init();

            var descriptionlabel = new Label(manager) { Text = red.DisplayedMessage, Top = yPos, Width = width, Height = 70, Left = 30};
            descriptionlabel.Text =
                "When this window closes you will be required to spin the spinner.\n" +
                "If the spinner lands on a black spot you will undergo what the black card says.\n" +
                "If the spinner lands on a red spot you will undergo what the red card says.\n\n" +
                "These are stories you are spinning for!";
            yPos += descriptionlabel.Height + spacing / 2;

            var redstorybox = new GroupBox(manager) { Width = 500, Height = 100, Left = 30, Top = 100, Parent = window, Color = Color.Red, Text = "Red Story", TextColor = Color.White};
            redstorybox.Init();
            yPos += redstorybox.Height;
            var redstorylabel = new Label(manager)
                                    {
                                        Width = redstorybox.Width,
                                        Height = redstorybox.Height,
                                        Parent = redstorybox,
                                        Text = red.DisplayedMessage,
                                        Left = spacing,
                                        StayOnTop = true
                                    };
            redstorylabel.Init();

            var blackstorybox = new GroupBox(manager) { Width = 500, Height = 100, Left = 30, Top = 200, Parent = window, Color = Color.Black, Text = "Black Story", TextColor = Color.White };
            blackstorybox.Init();
            yPos += blackstorybox.Height+spacing/2;

            var blackstorylabel = new Label(manager)
            {
                Width = blackstorybox.Width,
                Height = blackstorybox.Height,
                Parent = blackstorybox,
                Text = black.DisplayedMessage,
                Left = spacing,
                StayOnTop = true
            };
            blackstorylabel.Init();

            var close = new Button(manager) { Text = "OK", Top = yPos, Left = window.Width / 2 - 50, Parent = window };
            close.Init();
            close.Click += (sender, args) => window.Close();
            yPos += close.Height + spacing;

            window.Add(descriptionlabel);
            window.Height = blackstorybox.Height + redstorybox.Height + yPos/2;
            manager.Add(window);

            window.Closed += (sender, args) => WindowClosed(gameInfo);

            gameInfo.CreateMessage("Click the spinner to see your story!");
        }

        private void WindowClosed(GameInfo gameInfo) {
            gameInfo.Fsm.Remove(waitState);
        }
    }
}
