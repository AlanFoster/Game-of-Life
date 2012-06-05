using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.GameLogic.Assets {
    public class TransportFactory {
        private static TransportFactory _instance;
        private readonly ContentManager _content;

        public TransportFactory(ContentManager content) {
            _content = content;
        }

        public static void SetInstance(TransportFactory transportFactory) {
            _instance = transportFactory;
        }

        public static TransportFactory GetInstance() {
            return _instance;
        }

        public Transport GetTransport(TransportType type) {
            // A BoatPlane is worth 500k, otherwise it is worth 100k
            var transportValue = type == TransportType.BoatPlane ? 500000 : 100000;
            const int carSize = 6;

            var transportString = type.ToString();
            var normalGraphic = _content.Load<Texture2D>(String.Concat("Images/Transport/", transportString));
            var glowGraphic = _content.Load<Texture2D>(String.Concat("Images/Transport/", transportString, "_glow"));
            var newTransport = new Transport(type, transportString, carSize, normalGraphic, glowGraphic, transportValue);

            return newTransport;
        }
    }
}
