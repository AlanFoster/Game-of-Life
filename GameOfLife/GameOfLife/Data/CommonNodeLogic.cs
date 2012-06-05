using System.Collections.Generic;
using GameOfLife.GameLogic.Assets;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.GameLogic.PureLogics;

namespace GameOfLife.Data {
    public static class CommonNodeLogic {
        private static readonly List<BindedLogic> CommonLogic = new List<BindedLogic> {
                new BindedLogic(new Nothing()),
                new BindedLogic(new PayDay(), hasPassLogic: true),
                new BindedLogic(new ModifyCashStory(20000)),
                new BindedLogic(new SpinToWin(50000)),
                new BindedLogic(new GivePet()),

                new BindedLogic(new BlankStory()),
                new BindedLogic(new Promotion(), hasPassLogic: false),

                new BindedLogic(new ModifyCashStory(10000)), 
                new BindedLogic(new ModifyCashStory(-10000)),
                new BindedLogic(new ModifyCashStory(20000)),
                new BindedLogic(new ModifyCashStory(-20000)),
                new BindedLogic(new ModifyCashStory(50000)),
                new BindedLogic(new ModifyCashStory(-50000)),
            };

        public static List<BindedLogic> GetCommonNodeLogic() {
            return CommonLogic;
        }
    }
}