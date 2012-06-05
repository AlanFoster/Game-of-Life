using System;
using System.Collections.Generic;
using System.Linq;
using GameOfLife.WorldObjects;
using TomShane.Neoforce.Controls;
using EventArgs = TomShane.Neoforce.Controls.EventArgs;

namespace GameOfLife.LevelMenu {
    public abstract class PlayerTab {
        private Player[] _playerList;
        protected const String IgnoreString = "lawl";
        protected Dictionary<TabPage, int> createdPages;

        protected PlayerTab(){
            createdPages = new Dictionary<TabPage, int>();
        }

        public virtual TabControl GetTabbedControl(TabControl tabControl, Manager manager, Player[] playerList) {
            _playerList = playerList;
            tabControl.Left = 10;
            tabControl.Top = 10;

            foreach (var player in playerList) {
                var newTab = tabControl.AddPage(player.Name);
                var componentHeight = PopulateTab(manager, player, newTab);
                tabControl.Height = Math.Max(tabControl.Height, componentHeight + 50);
                createdPages[newTab] = componentHeight;
            }
            
            tabControl.PageChanged += (sender, args) => RecalculateHeight(tabControl);
            tabControl.PageChanged += (sender, args) => RecalculateHeight(tabControl);
            tabControl.PageChanged += (sender, args) => RecalculateHeight(tabControl);
            tabControl.PageChanged += (sender, args) => RecalculateHeight(tabControl);

            return tabControl;
        }


        private void RecalculateHeight(TabControl tabControl) {
            var size = Math.Max(createdPages[tabControl.TabPages[tabControl.SelectedIndex]] + 50, 400);
            Resize(tabControl, size);

            Control parent = tabControl;
            do {
                parent.Refresh();
                parent.Invalidate();
            } while ((parent = parent.Parent) != null);
        }

        protected int PlayerPos(Player curPlayer) {
            return 1 + _playerList.OrderBy(i => i.TotalValue).TakeWhile(i => i != curPlayer).Count();
        }

        public virtual TabControl GetTabbedControl(Manager manager, Player[] playerList) {
            return GetTabbedControl(new TabControl(manager), manager, playerList);
        }

        public virtual int PopulateTab(Manager manager, Player player, Control mainControl) {
            foreach (var childControl in mainControl.Controls.ToList()) childControl.Dispose();
            var yPos = 16;
            CreatePlayerInfo(manager, player, mainControl, ref yPos);
            yPos = AddInformation(manager, player, mainControl, yPos);
            return yPos;
        }

        public virtual void CreatePlayerInfo(Manager manager, Player player, Control control, ref int yPos) {
            var playerAvatar = player.Avatar;

            var imageIcon = new Button(manager) {
                Glyph = new Glyph(playerAvatar),
                Height = (int)(playerAvatar.Height / 1.5),
                Width = (int)(playerAvatar.Width / 1.5),
                Left = 16,
                Top = 16,
                Parent = control,
                Color = player.PlayerColor,
                Name = IgnoreString
            };
            imageIcon.Init();
            imageIcon.FocusGained += (sender, args) => imageIcon.Enabled = false;

            var descriptionText = new Label(manager) {
                Left = imageIcon.Left + imageIcon.Width + 16, Width = 200,
                Height = imageIcon.Height,
                Top = 25,
                Text = String.Format(
                    "Player Name : {0}\n\n" +
                    "Player Cash : ${1:N0}\n\n" +
                    "Player Total Worth : ${2:N0}\n\n" +
                    "Player Position : {3:N0}\n\n",
                    player.Name, player.Cash, player.TotalValue, PlayerPos(player)),
                Alignment = Alignment.TopLeft,
                Parent = control,
                Name = IgnoreString
            };
            descriptionText.Init();


            yPos = imageIcon.Top + imageIcon.Height + 16;
        }

        private void Resize(Control parent, int size)
        {
            if (parent.Name != IgnoreString)
            {
                parent.Height = size;
                parent.MinimumHeight = size;
                parent.MaximumHeight = size;
                parent.Refresh();

                foreach (var child in parent.Controls)
                {
                    Resize(child, size);
                }
            }
        }


        public abstract int AddInformation(Manager manager, Player player, Control control, int yPos);
    }
}


