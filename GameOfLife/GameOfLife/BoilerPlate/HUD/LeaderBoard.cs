using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.BoilerPlate.HUD;
using GameOfLife.BoilerPlate.Misc;
using GameOfLife.Core.HUD;
using GameOfLife.Data;
using GameOfLife.GameLogic;
using GameOfLife.GameLogic.GameStates;
using GameOfLife.GameLogic.GameStates.Actual;
using GameOfLife.GameLogic.GameStates.Factories;
using GameOfLife.GameLogic.PureLogics;
using GameOfLife.LevelMenu;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TomShane.Neoforce.Controls;
using StateFactory = GameOfLife.BoilerPlate.FSM.StateFactory;

namespace GameOfLife.BoilerPlate.HUD {
    public class Leaderboarder : GraphicalWidget {
        private Texture2D drawThingy;
        private Vector2 _size;
        private Dictionary<Color, Texture2D> _textureBars;

        public Leaderboarder(GameInfo gameInfo, HUDAlignment hudAlignment) 
            : base(gameInfo, hudAlignment, i => String.Empty, null) {
        }

        public override Vector2 Size {
            get {
                return _size;
            }
        }

        public override void Initialize() {
            base.Initialize();
            _textureBars = new Dictionary<Color, Texture2D>  {
                {Color.Blue, Content.Load<Texture2D>("Images/HUD/Highscores/Blue")},
                {Color.Green, Content.Load<Texture2D>("Images/HUD/Highscores/Green")},
                {Color.Orange, Content.Load<Texture2D>("Images/HUD/Highscores/Orange")},
                {Color.DarkRed, Content.Load<Texture2D>("Images/HUD/Highscores/Red")},
                {Color.Violet, Content.Load<Texture2D>("Images/HUD/Highscores/Violet")},
                {Color.Yellow, Content.Load<Texture2D>("Images/HUD/Highscores/Yellow")},
            };

            _size = new Vector2(370 + 40, 100 * GameInfo.TotalPlayers);
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 drawLocation) {
            var i = 0;
            int previousAmount = 0;
            drawLocation.Y += 100;
            foreach (var player in GameInfo.PlayerOrder) {
                if (previousAmount != player.TotalValue) {
                    i++;
                }
                spriteBatch.Draw(_textureBars[player.PlayerColor], drawLocation - new Vector2(0, 189), Color.White);

                spriteBatch.DrawString(Font,
                    String.Format("{1}{0}. {2}",
                        i, previousAmount == player.TotalValue ? "=" : String.Empty,
                        player.Name),
                    drawLocation + new Vector2(45, 20), Color.White);
                drawLocation.Y += 90;
                previousAmount = player.TotalValue;
            }
        }
    }
}
