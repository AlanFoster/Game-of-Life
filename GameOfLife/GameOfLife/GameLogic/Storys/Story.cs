using System;
using GameOfLife.GameLogic.PureLogics;

namespace GameOfLife.GameLogic.Storys {
    [Serializable]
    public class Story {
        public StoryType StoryType;
        public String DisplayedMessage;
        public String StoryGraphic { get; private set; }
        public PureLogic PureLogic;
        public bool Positive { get; private set; }

        public Story(StoryType storyType, String displayedMessage, PureLogic pureLogic, bool positive, String storyGraphic)
        {
            DisplayedMessage = displayedMessage;
            StoryType = storyType;
            PureLogic = pureLogic;
            Positive = positive;
            StoryGraphic = storyGraphic;
        }
    }
}
