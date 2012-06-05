using System;
using System.Threading;
using GameOfLife;
using GameOfLife.BoilerPlate.FSM;
using Microsoft.Xna.Framework;

namespace GameOfLife {
#if WINDOWS || XBOX
    public static class Program {
        [STAThread]
        public static void Main(string[] args) {
            using (var app = new TheGameOfLife()) {
                app.Run();
            }
        }

    }
#endif
}