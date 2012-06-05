using System;
using GameOfLife.GameLogic.Assets;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;

namespace GameOfLife.LevelMenu {
    public delegate void SellHouseCallback(Player player, House house);

    public class SellHouse : PlayerTab {
        private SellHouseCallback _houseCallback;

        public SellHouse(SellHouseCallback callback) {
            _houseCallback = callback;
        }

        public override int AddInformation(Manager manager, Player player, Control control, int yPos) {
            var currentHouse = player.House;
            const int spacing = 20;
            int xPos = spacing;
            if (currentHouse == null) {
                var noHouseLabel = new Label(manager) { Text = "You have no house to sell!", Width = 400, Left = xPos, Top = yPos, Name = IgnoreString };
                noHouseLabel.Init();
                control.Add(noHouseLabel);
            } else {
                var houseGraphic = manager.Game.Content.Load<Texture2D>(currentHouse.HouseGraphic);
                yPos += 20;
                // Put the house graphic into an image box
                var imageBox = new ImageBox(manager) {
                    Left = 16,
                    Top = yPos,
                    Image = houseGraphic,
                    Color = Color.White,
                    Width = houseGraphic.Width,
                    Height = houseGraphic.Height,
                    Name = IgnoreString,
                    Parent = control
                };
                imageBox.Init();

                var descriptionText = new Label(manager) {
                    Text =
                        String.Format(
                            "Name : {0}\n\n" +
                            "Average Value: ${1:N0}\n\n" +
                            "Current Value: ${2:N0}\n\n" +
                            "Bought for : ${3:N0}\n\n" +
                            "Total profit :: ${4:N0}",
                            currentHouse.Name, currentHouse.InitialValue, currentHouse.Value, currentHouse.PlayerBuyingValue, currentHouse.PlayerBuyingValue - currentHouse.Value),
                    Top = yPos,
                    Left = imageBox.Width + 60,
                    Height = 130,
                    Width = 200,
                    Parent = control,
                    Name = IgnoreString
                };
                yPos += imageBox.Height + 20;
                var sellHouseButton = new Button(manager) {
                    Text =
                        "Sell House",
                    Width = 400, Left = xPos, Top = yPos,
                    Name = IgnoreString
                };
                sellHouseButton.Click += (sender, args) => {
                    _houseCallback(player, currentHouse);
                    PopulateTab(manager, player, control);
                };
                sellHouseButton.Init();
                control.Add(sellHouseButton);
            }
            return yPos;
        }
    }
}