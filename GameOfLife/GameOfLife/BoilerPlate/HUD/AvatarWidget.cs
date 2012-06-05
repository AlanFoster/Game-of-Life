using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.GameLogic;
using GameOfLife.GameLogic.GameStates;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.BoilerPlate.HUD {
    public class AvatarWidget : GraphicalWidget, IGameState {
        private class UpdateCash {
            public int StartingCash;
            public int TargetCash;
            public Color textColor;
            public UpdateCash() {

            }
        }

        private Player _currentPlayer;
        private Texture2D _currentPlayerAvatar;
        private Dictionary<Color, Texture2D> _textureBars;
        // private List<int> _startingPlayerCash;
        private Color _textColor;
        //private UpdateCash currentCashState;
        private int _startingPlayerCash;
        private bool _updatingCash;


        public AvatarWidget(GameInfo gameInfo, HUDAlignment hudAlignment)
            : base(gameInfo, hudAlignment, info => String.Empty, null) {
            _textColor = Color.White;
            //_startingPlayerCash = new List<int>();
        }

        public override Vector2 Size {
            get { return new Vector2(BaseTexture.Width, BaseTexture.Height) + new Vector2(_currentPlayerAvatar.Width, _currentPlayerAvatar.Height); }
        }


        public override void Initialize() {
            base.Initialize();

            _textureBars = new Dictionary<Color, Texture2D>  {
                {Color.Blue, Content.Load<Texture2D>("Images/HUD/AvatarBars/Blue")},
                {Color.Green, Content.Load<Texture2D>("Images/HUD/AvatarBars/Green")},
                {Color.Orange, Content.Load<Texture2D>("Images/HUD/AvatarBars/Orange")},
                {Color.DarkRed, Content.Load<Texture2D>("Images/HUD/AvatarBars/Red")},
                {Color.Violet, Content.Load<Texture2D>("Images/HUD/AvatarBars/Violet")},
                {Color.Yellow, Content.Load<Texture2D>("Images/HUD/AvatarBars/Yellow")},
            };
        }

        public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            return null;
            if (_startingPlayerCash != _currentPlayer.Cash) {
                _startingPlayerCash = _startingPlayerCash < _currentPlayer.Cash
                                          ? Math.Min(_startingPlayerCash + 123, _currentPlayer.Cash)
                                          : Math.Max(_startingPlayerCash - 123, _currentPlayer.Cash);
                return new IGameState[] { this };
            }
            _updatingCash = false;
            _textColor = Color.White;
            return new IGameState[] { new TimedWait(500) };
        }

        public void PlayerCashChanged(Player player, int previousCash, GameInfo gameInfo) {
            return;
            if (player != _currentPlayer) return;
            _startingPlayerCash = previousCash;
            _textColor = _startingPlayerCash > player.Cash ? Color.Red : Color.Green;
            _updatingCash = true;
            gameInfo.Fsm.LazyPush(this);
        }

        public override void Update(GameTime gameTime) {
            base.Update(gameTime);
            // if (currentCashState != null) return;
            _currentPlayer = GameInfo.CurrentPlayer;
            _currentPlayerAvatar = _currentPlayer.Avatar;
            BaseTexture = _textureBars[_currentPlayer.PlayerColor];
            _startingPlayerCash = _currentPlayer.Cash;
        }

        public override void Draw(SpriteBatch spriteBatch, Vector2 drawLocation) {
            drawLocation.X += 20;
            spriteBatch.Draw(BaseTexture, drawLocation, Color.White);

            spriteBatch.Draw(_currentPlayerAvatar, drawLocation + new Vector2(BaseTexture.Width / 2 - _currentPlayerAvatar.Width / 2, 20), Color.White);
            spriteBatch.DrawString(Font, String.Format("${0:N0}", _startingPlayerCash), drawLocation + new Vector2(85, 210), _startingPlayerCash <= 0 ? Color.Red : Color.White);
            spriteBatch.DrawString(Font, String.Format("${0:N0}", _currentPlayer.TotalValue), drawLocation + new Vector2(80, 265), _currentPlayer.TotalValue <= 0 ? Color.Red : Color.White);
            /* spriteBatch.DrawString(Font,
                                    _updatingCash ? String.Format("${0:N0}", _startingPlayerCash) : _currentPlayer.Name,
                                    drawLocation + new Vector2(55, 210), _textColor);*/
        }
    }

}
