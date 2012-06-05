using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLife.Core;
using GameOfLife.GameLogic;
using GameOfLife.GameLogic.Assets;
using GameOfLife.GameLogic.Nodes;
using GameOfLife.NoIdea;
using GameOfLife.Screens;
using GameOfLife.WorldEdi;
using GameOfLife.WorldEditing;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

// Ecq2ww267rCjy7fZ
namespace GameOfLife {
    public enum Gender {
        Male, Female
    }

    public delegate bool CashListener(Player player);
    public sealed class Player : GenericWorldObject<Player> {
        public event CashListener CashListeners;

        /// <summary>
        /// The player's inputted username
        /// </summary>
        [Editable("Player Name")]
        public string Name { get; set; }

        [Editable("Gender")]
        public Gender Gender { get; set; }

        public bool PassedExam { get; set; }

        private int _cash;

        [Editable("Starting Cash")]
        public int Cash {
            get { return _cash; }
            set {
                _cash = value;
                CashChanged();
            }
        }


        public House House { get { return Assets[AssetType.House].FirstOrDefault() as House; } }

        /// <summary>
        /// The TYPE of career that this player can possibly get
        /// </summary>
        public CareerType CareerType { get; set; }


        private Career _career;
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
        private int PromotionLevel { get; set; }

        public Node CurrentNode { get; set; }
        public Node NextNode { get; set; }

        /// <summary>
        /// The Car object that this player travels in
        /// </summary>
        // TODO Maybe "transport" instead. To allow for flying/boats etc.
        public Car Car { get; private set; }
        public int CarSize { get { return Car.CarSize; } }
        public Texture2D CurrentGraphic { get { return Car.CurrentGraphic; } }


        /// <param name="asset">The asset to add to the player. </param>
        /// <returns>A specific asset response based on any logic given within. If succesful AddedSuccessfully returned</returns>
        public AssetResponse Accept(IAsset asset) {
            return asset.Visit(this, Assets[asset.AssetType]);
        }

        /// <param name="asset">The asset to remove from the player. </param>
        /// // TODO
        /// <returns>A specific asset response based on any logic given within. If successful AddedSuccessfully returned</returns>
        public AssetResponse Remove(IAsset asset) {
            return asset.Remove(this, Assets[asset.AssetType]);
        }

        // TODO
        // Car is private set, so we have to specifically provide an overload for it.
        // Is there a different way to do that?
        public AssetResponse Accept(Car car) {
            var carAssets = Assets[AssetType.Car];
            carAssets.Clear();
            carAssets.Add(car);
            // Set the new car
            Car = car;
            return AssetResponse.AddedSuccessfully;
        }


        public bool ReceivePay() {
            if (CurrentCareer == null) return false;
            Cash += CurrentCareer.Salary[PromotionLevel];
            return true;
        }

        public bool IncreaseSalary() {
            if (CurrentCareer == null) return false;
            PromotionLevel = Math.Max(CurrentCareer.Salary.Length, PromotionLevel + 1);
            return true;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch) {
            spriteBatch.Draw(CurrentGraphic, Location, null, Color.White, Rotation,
                   new Vector2(CurrentGraphic.Width, CurrentGraphic.Height) / 2,
                   1f, SpriteEffects.None, 1f);
        }

        private void CashChanged() {
            if(CashListeners != null) CashListeners(this);
        }

        public override void Initialize() {
            base.Initialize();

            CashListeners = CashListeners ?? delegate { return true; };

            // Create our dictionary of assets for each limited amount of AssetTypes
            // We could potentially lazily initialise this, but it's not important.
            if (Assets != null) return;
            Assets = new Dictionary<AssetType, List<IAsset>>();
            foreach (var assetType in Enum.GetValues(typeof(AssetType)).Cast<AssetType>()) {
                Assets.Add(assetType, new List<IAsset>());
            }
        }


        public override void LoadContent(ContentManager content) {
            // TODO this shouldn't be here!
            Texture2D carGraphic = content.Load<Texture2D>("Images/playerCar");
            Accept(new Car("test car", 2, carGraphic, 50000));
        }

        public override string ToString() {
            return "Player name :: " + Name + "\n"
                   + "Total worth :: " + TotalValue + "\n"
                   + "Rolls :: " + RollAmount + "\n"
                   + "Career type :: " + CareerType + "\n"
                   + "Current Job :: " + CurrentCareer + "\n"
                   + "Current Transportables :: " + Assets[AssetType.Transportable].Count + "\n"
                   + "CarSize :: " + Car;
        }
    }
}