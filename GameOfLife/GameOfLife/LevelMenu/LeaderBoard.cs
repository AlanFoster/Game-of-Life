using System;
using System.Linq;
using GameOfLife.BoilerPlate.Misc;
using GameOfLife.GameLogic;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;
using System.Collections.Generic;
using Console = System.Console;

namespace GameOfLife.LevelMenu {
    public class EndGameResults : PlayerTab
    {
        public override TabControl GetTabbedControl(TabControl tabControl, Manager manager, Player[] playerList) {
            var firstPage = tabControl.AddPage("Leader Board");
            CreateFirstPage(manager, playerList, firstPage);
            return base.GetTabbedControl(tabControl, manager, playerList);
        }

        private void CreateFirstPage(Manager manager, Player[] playerList, TabPage control) {
            var yPos = 16;
            var padding = 10;
            // Get the player that won
            var winningPlayer = playerList.MaxBy(i => i.TotalValue);

            var winningPlayerLabel = new Label(manager) {
                Text = "The current leader is " + winningPlayer.Name,
                Width = 400,
                Alignment = Alignment.MiddleCenter,
                Parent = control,
                Top = yPos,
                Name = IgnoreString
            };
            yPos += winningPlayerLabel.Height + 16;

            var playerAvatar = winningPlayer.Avatar;

            var imageIcon = new Button(manager) {
                Glyph = new Glyph(playerAvatar),
                Height = (int)(playerAvatar.Height / 1.5),
                Width = (int)(playerAvatar.Width / 1.5),
                Left = 16,
                Name = IgnoreString,
                Top = yPos,
                Parent = control,
                Color = winningPlayer.PlayerColor,
            };
            imageIcon.Init();
            imageIcon.FocusGained += (sender, args) => imageIcon.Enabled = false;

            var descriptionText = new Label(manager) {
                Left = imageIcon.Left + imageIcon.Width + 16, Width = 200,
                Height = imageIcon.Height,
                Top = yPos + 9,
                Text = String.Format(
                    "Player Name : {0}\n\n" +
                    "Player Cash : ${1:N0}\n\n" +
                    "Player Total Worth : ${2:N0}\n\n" +
                    "Player Position : {3:N0}\n\n",
                    winningPlayer.Name, winningPlayer.Cash, winningPlayer.TotalValue, 1),
                Alignment = Alignment.TopLeft,
                Parent = control,
                Name = IgnoreString,
            };
            descriptionText.Init();

            yPos = imageIcon.Top + imageIcon.Height + 16;

            int playerCount = 0;


            var leaderBoard = new ImageBox(manager) {
                Image = manager.Game.Content.Load<Texture2D>("images/leaderboard"),
                Top = yPos - padding,
                Left = 200 - 100, Parent = control, Width = 250,
                                Name = IgnoreString
            };
            leaderBoard.Init();
            yPos += leaderBoard.Height / 3;
            foreach (var player in playerList.OrderByDescending(i => i.TotalValue))
            {
                playerCount++;
                var label = new Label(manager)
                                {
                                    Text = String.Format("{0}. {1}\n\n          " +
                                                         "Total Value : ${3:N0}\n          " +
                                                         "Current Cash : ${2:N0}",
                                                         playerCount, player.Name, player.Cash, player.TotalValue),
                                    Height = 50,
                                    Width = 400,
                                    Parent = control,
                                    Left = 80,
                                    Top = yPos,
                                    Name = IgnoreString
                                };
                yPos += label.Height + 10;
            }

            createdPages[control] = yPos + 30;
        }


        public override int AddInformation(Manager manager, Player player, Control control, int yPos) {
            const int spacing = 16;

            var title = new ImageBox(manager)
                            {
                                Image = manager.Game.Content.Load<Texture2D>("images/assetstitle"),
                                Top = yPos,
                                Left = 150,
                                Parent = control,
                                Width = 150,
                                Name = IgnoreString
                            };
            title.Init();
            yPos += title.Height/2;

            foreach (var assetList in player.Assets)
            {
                foreach (var asset in assetList.Value)
                {
                    var assetName = new Label(manager)
                    {
                        Text = String.Format("Type: {0}   Value: ${1:N0}",
                                             asset.Name, asset.Value),
                        Top = yPos+spacing,
                        Width = 400,
                        Left = 120,
                        Parent = control,
                       Name = IgnoreString
                    };

                    var texture = manager.Game.Content.Load<Texture2D>(asset.AssetPath);
                    var imageBox = new ImageBox(manager)
                    {
                        Image = texture,
                        Parent = control,
                        Left = 40,
                        Top = yPos,
                       Name = IgnoreString,
                       Color = Color.White
                    };

                    assetName.Init();
                    yPos += imageBox.Height + spacing;
                }
            }

            return yPos + 30;
        }
    }
}
