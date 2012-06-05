using System;
using System.Collections.Generic; //for the lists
using GameOfLife.BoilerPlate.Misc;
using Microsoft.Xna.Framework;

namespace GameOfLife.Data {
    //Should possibly be split out into different sections to match the rest of the program,
    //All in here for now to ensure it works
    //could easily be modified to check player stats, eg - has a kid, has a plane car, etc and return an appropriate message
    //however, that is not a priority right now
    public static class CashTileData {
        private static readonly List<String> MessagesGood = new List<string> {
            "Win Employee of the Year Award ${0:N0}",
            "Earn a big bonus! ${0:N0}",
            "Start up a company ${0:N0}",
            "Win a Red Bull fight in your Plane Car ${0:N0}",
            "I'm going to Disneyland!! ${0:N0}",
            "Meet Jeffrey at the zoo ${0:N0}",
            "Meet an old friend ${0:N0}",
            "Tax rebate! ${0:N0}",
            "Licked by a puppy ${0:N0}",
            "Win tickets to U2 ${0:N0}",
            "Star in an advertisement ${0:N0}",
            "Letter published in a newspaper ${0:N0}",
            "Find a winning scratchcard ${0:N0}",
         };

        private static readonly List<String> MessagesBad = new List<string> {
            "Buy Car fuel ${0:N0}",
            "Car has a flat tyre ${0:N0}",
            "Blow a gasket in your Car ${0:N0}",
            "Take a corner too fast ${0:N0}",
            "Caught speeding! ${0:N0}",
            "Fall off a ladder ${0:N0}",
            "Computer catches fire ${0:N0}",
            "Crack the glass on your smartphone ${0:N0}",
            "Roadworks ${0:N0}",
            "Lose a bet on the local sports team ${0:N0}",
            "Bitten by an ostrich ${0:N0}",
            "School bake sale ${0:N0}",
            "Overdue tax ${0:N0}",
            "Parking fine ${0:N0}",
         };

        public static String GetRandomGood() {
            return MessagesGood[RandomHelper.Next(0, MessagesGood.Count)];
        }

        public static String GetRandomBad() {
            return MessagesBad[RandomHelper.Next(0, MessagesBad.Count)];
        }
    }
}
