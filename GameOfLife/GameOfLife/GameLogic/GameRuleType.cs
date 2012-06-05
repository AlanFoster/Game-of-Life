using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.WorldEditing;

namespace GameOfLife.GameLogic {
    public enum GameRuleType {
        [Editable("Retirement age Reached")]
        Retirement,
        [Editable("A stamp from every island")]
        Passport
    }
}
