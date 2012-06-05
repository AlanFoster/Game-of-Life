using System;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.Misc;
using GameOfLife.Data;
using GameOfLife.GameLogic;
using GameOfLife.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace GameOfLife.WorldObjects {
    [DataContract]
    public class Spinner : GenericWorldObject<Spinner>, IGameState {
        /// <summary>
        /// Stores the currently rolled number by the spinner. This is from 1 to the number of slots the spinner has (presumably 10).
        /// </summary>
        [DataMember]
        public int SpinnedNumber { get; protected set; }

        /// <summary>
        /// </summary>
        [DataMember]
        public bool IsBlack { get; private set; }

        /// <summary>
        ///  The number of 'slots' a spinner has, by default 10
        /// </summary>
        public int NumberOfSlots { get; protected set; }

        public float CurrentRotation { get; private set; }

        public Color[] HighlightedSections { get; set; }
        public bool DrawHighlights { get; set; }

        private int CurrentlyPointedNumber { get; set; }

        /// <summary>
        /// Minimum spin time that the spinner can spin for, in milliseconds
        /// </summary>
        private int MinSpinTime;

        /// <summary>
        /// Maximum spin time that the spinner can spin for, in milliseconds
        /// </summary>
        private int MaxSpinTime;


        /// <summary>
        /// Set to a random time in milliseconds between MinSpinTime nad MaxSpinTime. Start spin time is kept track of to work out the percentage of speed
        /// </summary>
        private float _startingSpinningTime;

        /// <summary>
        /// when logically updating it is reduced until 0. When it reaches less than 0 we calculate what we landed on.
        /// </summary>
        private float _remainingSpinningTime;

        /// <summary>
        /// The initial rotation speed that we spin at. As we reach our remaniningSpinningTime goal of 0 we slow down.
        /// The rate of rotation depends on the percentage of _remainingSpinningTime / _startingSpinningTime
        /// </summary>
        private float MaxRotationSpeed;

        private int _previousNumber;
        private Texture2D _pointingArrow, _spinningGraphic, _glowingGraphic, _spinToWin;
        private Vector2 _spinningGraphicCenter;
        private SoundEffect sound;
        private SoundEffectInstance spinSound;

        /// <summary>
        /// Set to true when we are waiting for the player to click the spinner to start it spinning
        /// </summary>
        private bool _waitingPlayerClick;

        public bool IsSpinning { get; private set; }

        public Spinner(Vector2 location, int numberOfSlots = 10)
            : base(location, null) {

            NumberOfSlots = numberOfSlots;
        }

        public override void Initialize() {
            base.Initialize();
            NumberOfSlots = 10;
            CurrentRotation = MathHelper.PiOver2;
            _previousNumber = CalculcateCurrentNumber();
            MinSpinTime = 2000;
            MaxSpinTime = 7000;
            MaxRotationSpeed = 0.18f;
            HighlightedSections = new Color[10];
        }

        public override void LoadContent(ContentManager content) {
            _pointingArrow = content.Load<Texture2D>("Images/spinner/pointer");
            _spinningGraphic = content.Load<Texture2D>("Images/spinner/spinningDial");
            _glowingGraphic = content.Load<Texture2D>("Images/spinner/spinnerGlow");
            _spinToWin = content.Load<Texture2D>("Images/spinner/SpinToWin");
            sound = content.Load<SoundEffect>("Sounds/SpinnerClick");
            spinSound = sound.CreateInstance();
            spinSound.Volume = Options.EffectsVolume;
            Size = new Vector2(_spinningGraphic.Width, _spinningGraphic.Height);
            _spinningGraphicCenter = Size / 2;
        }

        public void ClickedSpinner(Spinner foo, Vector2 location) {
            _waitingPlayerClick = false;
            SetColorState(ColorState.None);
            ListenMouseDown -= ClickedSpinner;

            if (Constants.Editing.IsAdminMode) {
                SetNumber(CalculateMouseNumber(location));
            }
        }

        public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            if (_waitingPlayerClick) return new IGameState[] { this };

            if (!IsSpinning) {
                IsSpinning = true;
                _startingSpinningTime = _remainingSpinningTime = RandomHelper.Next(MinSpinTime, MaxSpinTime);
                _waitingPlayerClick = true;
                RememberColorState(ColorState.Glow);
                SetColorState(ColorState.Glow);
                ListenMouseDown += ClickedSpinner;
            } else if ((_remainingSpinningTime -= gameTime.ElapsedGameTime.Milliseconds) <= 0) {
                SpinnedNumber = CurrentlyPointedNumber;

                IsBlack = (SpinnedNumber & 1) == 0;
                IsSpinning = false;
                return null;
            } else {
                // Spin ourselves, with our MaxRotationSpeed multiplied by our percentage of ending our spin,
                // so that as time progresses we spin slower
                IncreaseRotation();
                CurrentlyPointedNumber = (int)(((((float)Math.PI / 2 - CurrentRotation + Math.PI * 4) % (Math.PI * 2)) / (Math.PI * 2)) * 10) + 1;
            }

            return new IGameState[] { this };
        }

        private void IncreaseRotation() {
            var amount = MaxRotationSpeed * (_remainingSpinningTime / _startingSpinningTime);
            CurrentRotation = MathHelper.WrapAngle(CurrentRotation + amount);

            // Calculate the current number to see if it has changed, IE if we need to play a sound
            var currentSpunNumber = CalculcateCurrentNumber();
            if (currentSpunNumber == _previousNumber) return;
            _previousNumber = currentSpunNumber;

            spinSound.Play();
            spinSound.Volume = Options.EffectsVolume;
        }


        private int CalculcateCurrentNumber() {
            return (int)(((((float)Math.PI / 2 - CurrentRotation + Math.PI * 4) % (Math.PI * 2)) / (Math.PI * 2)) * 10) + 1;
        }

        public void SetNumber(int num) {
            while (CalculcateCurrentNumber() != num) {
                IncreaseRotation();
            }
            CurrentlyPointedNumber = num;
            _remainingSpinningTime = 0;
        }

        /// <summary>
        /// Calculate the number that the mouse position is on
        /// </summary>
        /// <param name="mouseLocation">The current mouse within the world</param>
        /// <returns>The mouse's number</returns>
        public int CalculateMouseNumber(Vector2 mouseLocation) {
            Vector2 difference = Center - mouseLocation;
            difference.Normalize();
            var clickedRotation = (float)Math.Atan2(difference.Y, difference.X);
            var mouseNumber = (int)(Math.Floor((((clickedRotation - CurrentRotation + Math.PI * 4) % (Math.PI * 2)) / (Math.PI * 2)) * 10) + 1);

            return mouseNumber;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            base.Draw(gameTime, spriteBatch);

            // Draw the background glow
            spriteBatch.Draw(_glowingGraphic, Center, null, OverlayColor * Opacity, (float)(Math.PI * 2 - CurrentRotation),
                  _spinningGraphicCenter, 1f, SpriteEffects.None, 1f);

            // Draw the rotating part
            spriteBatch.Draw(_spinningGraphic, Center, null, Color.White * Opacity, (float)(CurrentRotation),
                                 _spinningGraphicCenter, 1f, SpriteEffects.None, 1f);

            if (DrawHighlights) {
                DrawHighlight(spriteBatch);
            }

            // draw the pointer arrow
            spriteBatch.Draw(_pointingArrow, Center, null, Color.White * Opacity, 0,
                 _spinningGraphicCenter, 1f, SpriteEffects.None, 1f);
        }

        private void DrawHighlight(SpriteBatch spriteBatch) {
            for (var i = 0; i < NumberOfSlots; i++) {
                var tabColor = HighlightedSections[i];

                var tabRotation = MathHelper.WrapAngle((float)((CurrentRotation - MathHelper.PiOver2) + ((Math.PI * 2) / NumberOfSlots) * i));

                spriteBatch.Draw(_spinToWin, Center, null, tabColor, (float)(tabRotation),
                                    _spinningGraphicCenter, 1f, SpriteEffects.None, 1f);
            }
        }
    }
}
