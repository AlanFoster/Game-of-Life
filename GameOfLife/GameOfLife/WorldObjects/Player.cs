using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLife.GameLogic;
using GameOfLife.GameLogic.Assets;
using GameOfLife.Screens;
using GameOfLife.WorldEditing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.WorldObjects {
    public enum Gender {
        Male, Female
    }

    public delegate void CashListener(Player player, int previousCash);
    public sealed class Player : GenericWorldObject<Player> {
        public event CashListener CashListeners;

        public Vector2 ActualLocation { get; set; }

        public override Vector2 Location {
            get {
                return ActualLocation - Size / 2;
            }
            set {
                ActualLocation = value;
                base.Location = value;
            }
        }

        [Editable("Player Name")]
        public string Name { get; set; }

        public Gender Gender { get; set; }

        [Editable("Player's Game Colour")]
        public Color PlayerColor { get; set; }

        public bool PassedExam { get; set; }
        public bool CurrentlyTakingExam { get; set; }

        private int _cash;

        public int Cash {
            get { return _cash; }
            set {
                var previousCash = _cash;
                _cash = value;
                CashChanged(previousCash);
            }
        }

        [Editable("Player Avatar")]
        public Texture2D Avatar { get; set; }

        public House House { get { return Assets[AssetType.House].FirstOrDefault() as House; } }

        /// <summary>
        /// The TYPE of career that this player can possibly get
        /// </summary>
        public CareerType CareerType { get; set; }

        private Career _career;
        public String PlayerPosition { get; set; }


        /// <summary>
        /// The actual career that has been given to this player
        /// </summary>
        public Career CurrentCareer {
            get { return _career; }
            set {
                PromotionLevel = 0;
                _career = value;
            }
        }

        public Dictionary<AssetType, List<IAsset>> Assets { get; private set; }

        public int TotalValue { get { return Cash + Assets.Values.SelectMany(i => i).Select(i => i.Value).Sum(); } }

        public int RollAmount { get; set; }
        public float Rotation { get; set; }

        /// <summary>
        /// The current promotion level the player is at. IE 0 is base pay.
        /// </summary>
        public int PromotionLevel { get; protected set; }

        public Node CurrentNode { get; set; }
        public Node NextNode { get; set; }

        /// <summary>
        /// The Car object that this player travels in
        /// </summary>
        private Transport Car { get; set; }
        public int CarSize { get { return Car.CarSize; } }

        public AssetResponse Accept(IAsset asset) {
            return asset.Visit(this, Assets[asset.AssetType]);
        }

        public AssetResponse Remove(IAsset asset) {
            return asset.Remove(this, Assets[asset.AssetType]);
        }

        public AssetResponse Accept(Transport car) {
            var carAssets = Assets[AssetType.Car];
            carAssets.Clear();
            carAssets.Add(car);
            // Set the new car
            Car = car;
            Size = car.TransportType == TransportType.Car ? car.TextureSize / 2f : car.TextureSize;
            return AssetResponse.AddedSuccessfully;
        }

        public void SetTransport(TransportType transportType) {
            if (Car != null) {
                var newTransportType = Car.TransportType | transportType;
                if (Car.TransportType != transportType) {
                    Accept(TransportFactory.GetInstance().GetTransport(newTransportType));
                }
            } else {
                Accept(TransportFactory.GetInstance().GetTransport(transportType));
            }
        }

        public IEnumerable<IAsset> GetAllAssets() {
            return Assets.SelectMany(i => i.Value);
        }

        public bool ReceivePay() {
            if (CurrentCareer == null) return false;
            Cash += CurrentCareer.Salary[PromotionLevel];
            return true;
        }

        public bool IncreaseSalary() {
            if (CurrentCareer == null) return false;
            PromotionLevel = Math.Min(CurrentCareer.Salary.Length, PromotionLevel + 1);
            return true;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            base.Draw(gameTime, spriteBatch);
            Car.Draw(gameTime, spriteBatch, this);
        }

        private void CashChanged(int previousCash) {
            if (CashListeners != null) CashListeners(this, previousCash);
        }

        public override void Initialize() {
            base.Initialize();

            CashListeners = CashListeners ?? delegate { };

            // Create our dictionary of assets for each limited amount of AssetTypes
            // We could potentially lazily initialise this, but it's not important.
            if (Assets != null) return;
            Assets = new Dictionary<AssetType, List<IAsset>>();
            foreach (var assetType in Enum.GetValues(typeof(AssetType)).Cast<AssetType>()) {
                Assets.Add(assetType, new List<IAsset>());
            }
            OverlayColor = PlayerColor;
        }

        public override void LoadContent(ContentManager content) { }

        public bool HasStamp(IslandType islandType) {
            return Assets[AssetType.PassportStamp].Cast<PassportStamp>().Any(i => i.IslandType == islandType);
        }
    }
}