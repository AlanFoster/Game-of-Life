using System;
using System.Linq;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.BoilerPlate.Misc;
using GameOfLife.Data;
using GameOfLife.GameLogic.Assets;
using GameOfLife.GameLogic.GameStates;
using GameOfLife.GameLogic.GameStates.Factories;
using GameOfLife.WorldEditing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;

namespace GameOfLife.GameLogic.PureLogics {
    [DataContract]
    [Editable("Buy House")]
    class BuyHouse : PureLogic {
        private static int _houseCount;


        [Editable("House Name")]
        [DataMember]
        private String HouseName { get; set; }

        [Editable("Average House Price")]
        [DataMember]
        private int HouseAveragePrice { get; set; }

        // The house stored in this node.
        private House _house;

        public House House
        {
            get { return _house;  }
            private set { _house = value; }
        }

        public override string GetGraphicLocation() {
            return "Images/Node/Icons/Home";
        }

        public override string Description {
            get {
                return String.Format("${0:N0}\n({1})", House.Value, !House.HasOwner ? "Vacant" : "Owned");
            }
        }

        public override void Initialize() {
            House = new House(HouseAveragePrice, HouseName, "Images/AlertIcons/House");
        }

        public override IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            var waitState = StateFactory.GetState(GameStates.GameStates.Wait);

            if (House.HasOwner)
            {
                HouseHasOwner(gameInfo, waitState);
            }
            else if (gameInfo.CurrentPlayer.House != null)
            {
                OfferToSellHouse(gameInfo, waitState);
            }
            else
            {
                OfferPlayerHouse(gameInfo, waitState);
            }
            return new[] { waitState };
        }

        private void HouseHasOwner(GameInfo gameInfo, IGameState waitState) {
            var alert = new Alert(gameInfo.Manager,
                String.Format("This house '{0}' is already owned by {1}",
                House.Name,
                (House == gameInfo.CurrentPlayer.House ? "you" : House.OwnerName)),
                "This house is not for sale"
            );
            alert.Closed += (sender, args) => CloseWindow(alert, gameInfo, waitState);
            gameInfo.Manager.Add(alert);
        }

        private void OfferToSellHouse(GameInfo gameInfo, IGameState waitState)
        {
            var currentHouse = gameInfo.CurrentPlayer.House;
            var houseGraphic = gameInfo.Content.Load<Texture2D>(currentHouse.HouseGraphic);
            var manager = gameInfo.Manager;

            var container = new Control(manager) { Left = 16, Top = 16, Color = Color.Transparent };
            var firstLine = new Label(manager) { Text = "You have been offered to buy a new house!  \nYou must sell your current house to be able to buy this one. \n\n     Current House:", Width = 400, Height = 60};
            container.Add(firstLine);
            // Put the house graphic into an image box
            var imageBox = new ImageBox(manager)
            {
                Left = 16,
                Top = 60,
                Image = houseGraphic,
                Color = Color.White,
                Width = houseGraphic.Width,
                Height = houseGraphic.Height
            };
            imageBox.Init();
            container.Add(imageBox);

            var descriptionText = new Label(manager)
            {
                Text =
                    String.Format(
                        "Name : {0}\n\n" +
                        "Average Value: ${1:N0}" +
                        "\n\nCurrent Value: ${2:N0}\n\n",
                        currentHouse.Name, currentHouse.InitialValue, currentHouse.Value),
                Top = imageBox.Height / 2,
                Left = imageBox.Width + 60,
                Height = 100,
                Width = 200
            };
            container.Add(descriptionText);

            var newHouse = new Label(gameInfo.Manager)
                               {Text = "     New offered house", Width = 150, Parent = container, Top = 200, Left = 16};
            newHouse.Init();

            var imageBox2 = new ImageBox(manager)
            {
                Left = 16,
                Top = 230,
                Image = gameInfo.Content.Load<Texture2D>(House.HouseGraphic),
                Color = Color.White,
                Width = houseGraphic.Width,
                Height = houseGraphic.Height
            };
            imageBox.Init();
            container.Add(imageBox2);

            var descriptionText2 = new Label(manager)
            {
                Text =
                    String.Format(
                        "Name : {0}\n\n" +
                        "Average Value: ${1:N0}" +
                        "\n\nCurrent Value: ${2:N0}\n\n",
                        House.Name, House.InitialValue, House.Value),
                Top = imageBox.Height*4 / 2,
                Left = imageBox2.Width + 60,
                Height = 100,
                Width = 200
            };
            container.Add(descriptionText2);


            var offerHouseLine = new Label(manager) { Text = "Would you like to sell your current house?", Width = 400, Top = imageBox.Height*2 + 140 };
            container.Add(offerHouseLine);

            container.Width = descriptionText.Left + descriptionText.Width + 16;
            container.Height = offerHouseLine.Top + offerHouseLine.Height;

            //"Would you like to buy this house? \n" + House.Name, "\nDo you want to buy a house""
            var confirmWindow = new ConfirmWindow(manager, String.Empty, title: "Do you want to sell your house?", control: container);
            confirmWindow.AffirmButton.Click += (sender, args) => SellCurrentHouse(confirmWindow, gameInfo, waitState);
            confirmWindow.DenyButton.Click += (sender, args) => CloseWindow(confirmWindow, gameInfo, waitState);  
         
        }

        private void OfferPlayerHouse(GameInfo gameInfo, IGameState waitState) {
            var houseGraphic = gameInfo.Content.Load<Texture2D>(House.HouseGraphic);
            var manager = gameInfo.Manager;

            var container = new Control(manager) { Left = 16, Top = 16, Color = Color.Transparent };
            var firstLine = new Label(manager) { Text = "You have been offered the ability to buy a new house!", Width = 400 };
            container.Add(firstLine);
            // Put the house graphic into an image box
            var imageBox = new ImageBox(manager) {
                Left = 16,
                Top = 32,
                Image = houseGraphic,
                Color = Color.White,
                Width = houseGraphic.Width,
                Height = houseGraphic.Height
            };
            imageBox.Init();
            container.Add(imageBox);

            var descriptionText = new Label(manager) {
                Text =
                    String.Format(
                        "Name : {0}\n\n" +
                        "Average Value: ${1:N0}" +
                        "\n\nCurrent Value: ${2:N0}\n\n",
                        House.Name, House.InitialValue, House.Value),
                Top = imageBox.Height / 2 - 30 / 2,
                Left = imageBox.Width + 60,
                Height = 100,
                Width = 200
            };
            container.Add(descriptionText);

            var offerHouseLine = new Label(manager) { Text = "Would You like to buy this house?", Width = 400, Top = imageBox.Height + 40 };
            container.Add(offerHouseLine);

            container.Width = descriptionText.Left + descriptionText.Width + 16;
            container.Height = offerHouseLine.Top + offerHouseLine.Height;

            //"Would you like to buy this house? \n" + House.Name, "\nDo you want to buy a house""
            var confirmWindow = new ConfirmWindow(manager, String.Empty, title: "Do you want to buy a house", control: container);
            confirmWindow.AffirmButton.Click += (sender, args) => BuyNewHouse(confirmWindow, gameInfo, waitState);
            confirmWindow.DenyButton.Click += (sender, args) => CloseWindow(confirmWindow, gameInfo, waitState);    
        }

        private void BuyNewHouse(ConfirmWindow window, GameInfo gameInfo, IGameState waitState) {
            gameInfo.CurrentPlayer.Accept(House);

            CloseWindow(window, gameInfo, waitState);
            gameInfo.CreateMessage(String.Format("{0} just bought a house!", gameInfo.CurrentPlayer.Name));
        }

        private void SellCurrentHouse(ConfirmWindow window, GameInfo gameInfo, IGameState waitState)
        {
            gameInfo.CurrentPlayer.Remove(gameInfo.CurrentPlayer.House);
            gameInfo.CreateMessage(String.Format("{0} just sold a house!", gameInfo.CurrentPlayer.Name));
            window.Close();
            OfferPlayerHouse(gameInfo, waitState);
        }

        private static void CloseWindow(Alert window, GameInfo gameInfo, IGameState waitState) {
            if (gameInfo.Fsm.Remove(waitState)) {
                window.Close();
            }
        }
    }
}
