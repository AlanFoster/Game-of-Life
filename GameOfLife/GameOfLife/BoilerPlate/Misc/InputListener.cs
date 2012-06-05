using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace GameOfLife.BoilerPlate.Misc {
    /// <summary>
    /// Originally written for the konami code input, which is why it's partially obfusicated...
    /// (It was a use-case requirement to do so)
    /// 
    /// However, at the time of writing, it's also used for setting Admin Mode - and possibly more cheats.
    /// 
    /// There is a grace period of 2 seconds before the code is lost. This grace period is reset everytime the key is pressed.
    /// </summary>
    public class InputListener
    {
        public Keys[] Keys { get; set; }
        public Action Callback { get; set; }
        public int i { get; set; }
        KeyboardState p;
        int r;
        static Action<InputListener> s = n => n.r = n.i = 0;
        public readonly Action<KeyboardState, int, InputListener> Update = (kb, i, n) => {
            n.i = (kb != n.p & kb.IsKeyDown(n.Keys[i]) && (n.r = 0) == 0 ? ++i : i);
            if (n.i == n.Keys.Length) { n.Callback(); s(n); }
            if (n.r++ > 0xFF / 2) s(n);
            n.p = kb;
          
        };
    }
}
