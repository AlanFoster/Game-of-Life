using System;
using System.Linq;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.BoilerPlate.ScreenManager;
using GameOfLife.GameLogic;
using GameOfLife.GameLogic.Assets;
using GameOfLife.WorldObjects;
using TomShane.Neoforce.Controls;
using EventHandler = TomShane.Neoforce.Controls.EventHandler;

namespace GameOfLife.LevelMenu {

    public class MenuManager {
        private Window MenuWindow { get; set; }
        private Button MenuButton { get; set; }
        private GameLayerManager ControlManager { get; set; }
        private readonly GameInfo _gameInfo;
        private readonly Action _returnToMainMenu;

        public MenuManager(GameInfo gameInfo, GameLayerManager controlManager, Action returnToMainMenu) {
            ControlManager = controlManager;
            _gameInfo = gameInfo;
            _returnToMainMenu = returnToMainMenu;
        }

        public Button CreateMenuButton() {
            // Create the button that opens the window
            MenuButton = new Button(ControlManager.Manager) { Text = "Menu" };
            MenuButton.Init();
            MenuButton.Click += (sender, args) => OpenMenu();

            ControlManager.Add(MenuButton);

            // Create the window
            MenuWindow = new Window(ControlManager.Manager) { Text = "Menu window" };
            MenuWindow.Init();
            MenuWindow.Hide();

            const int padding = 16, buttonWidth = 300, buttonHeight = 30;
            var yPos = padding;
            var xPos = MenuWindow.Controls.First().Width / 2 - buttonWidth / 2;


            var descriptionLabel = new Label(ControlManager.Manager) {
                Left = padding / 2,
                Top = yPos,
                Width = 400,
                Text = "Click what option you want, or press close to continue.",
            };
            descriptionLabel.Init();
            MenuWindow.Add(descriptionLabel);
            yPos += descriptionLabel.Height + padding * 2;

            var menuInfos = new[] {
                new Tuple<string, EventHandler>("View Game Status", (sender, args) => { CloseMenuWindow(); CreateGameResult(); }),
                    new Tuple<string, EventHandler>("Sell House", (sender, args) => { CloseMenuWindow(); CreateSellHouseWindow(); }),
                    new Tuple<string, EventHandler>("Repay Loan", (sender, args) => { CloseMenuWindow(); CreateRepayLoanWindow(); }),
                    new Tuple<string, EventHandler>("Return to previous screen", (sender, args) => _returnToMainMenu()),
                    new Tuple<string, EventHandler>("Exit Game", (sender, args) => ControlManager.Manager.MainWindow.Close())
            };

            foreach (var menuInfo in menuInfos) {
                var menuButton = new Button(ControlManager.Manager) {
                    Text = menuInfo.Item1,
                    Top = yPos,
                    Left = xPos,
                    Width = buttonWidth,
                    Height = buttonHeight
                };
                menuButton.Init();
                menuButton.Click += menuInfo.Item2;
                MenuWindow.Add(menuButton);
                yPos += menuButton.Height + padding;
            }

            MenuWindow.Closing += (sender, args) => {
                MenuButton.Enabled = true;
                CloseMenuWindow();
            };

            ControlManager.Add(MenuWindow);

            return MenuButton;
        }

        public void CreateRepayLoanWindow() {
            var repayLoanWindow = new Window(ControlManager.Manager);
            repayLoanWindow.BringToFront();
            repayLoanWindow.Init();
            var tabs = new RepayLoan((player, loan) => RepayLoan(player, loan, repayLoanWindow)).GetTabbedControl(ControlManager.Manager, _gameInfo.PlayerList);
            tabs.Init();
            tabs.MinimumWidth = repayLoanWindow.Width;
            tabs.MinimumHeight = repayLoanWindow.Height;
            tabs.Parent = repayLoanWindow;

            repayLoanWindow.Closed += (sender, args) => { MenuButton.Enabled = true; };
            ControlManager.Add(repayLoanWindow);
            tabs.SelectedIndex = 0;
        }

        public Window CreateGameResult() {
            var resultWindow = new Window(_gameInfo.Manager) {
                Text = "Game Results",
                Height = 400,
                AutoScroll = true,
                Resizable = false   
            };
            resultWindow.Closed += (sender, args) => { MenuButton.Enabled = true; };
            resultWindow.BringToFront();
            resultWindow.Init();

            var tabbedControl = new EndGameResults().GetTabbedControl(_gameInfo.Manager, _gameInfo.PlayerList);
            tabbedControl.Init();
            tabbedControl.MinimumHeight = resultWindow.Height - 40;
            tabbedControl.MinimumWidth = resultWindow.Width - 40;
            tabbedControl.Parent = resultWindow;

            ControlManager.Add(resultWindow);
            tabbedControl.SelectedIndex = 0;

            return resultWindow;
        }

        public void CreateSellHouseWindow() {
            CloseMenuWindow();
            var sellHouseWindow = new Window(ControlManager.Manager);
            sellHouseWindow.BringToFront();
            sellHouseWindow.Init();

            var tabs = new SellHouse((player, house) => SellHouse(player, house, sellHouseWindow)).GetTabbedControl(ControlManager.Manager, _gameInfo.PlayerList);
            tabs.Init();

            tabs.MinimumWidth = sellHouseWindow.Width;
            tabs.MinimumHeight = sellHouseWindow.Height;

            tabs.Parent = sellHouseWindow;

            sellHouseWindow.Closed += (sender, args) => { MenuButton.Enabled = true; };
            ControlManager.Add(sellHouseWindow);
            tabs.SelectedIndex = 0;
        }

        private void SellHouse(Player player, House house, Window window) {
            CloseMenuWindow();
            var alertString = String.Format("You just sold your house {0} for a price of ${1:N0}" +
                                            "\n\nYou made a profit of ${2:N0}",
                                            house.Name, house.Value, house.Value - house.PlayerBuyingValue);
            var alert = new Alert(ControlManager.Manager, alertString, "House Sold");
            ControlManager.Add(alert);
            player.Remove(house);
            alert.BringToFront();
            window.Enabled = false;
            alert.Closed += (sender, args) => window.Enabled = true;
        }

        private void RepayLoan(Player player, Loan loan, Window window) {
            Alert alert;
            if (player.Cash > -loan.Value) {
                player.Remove(loan);
                alert = new Alert(ControlManager.Manager, String.Format("You have successfully repaid your loan of ${0}", loan.Value), "Loan repaid", icon : "Images/AlertIcons/Loan");
            } else {
                alert = new Alert(ControlManager.Manager, "Sorry, you do not have enough funds to pay this loan back", "Loan Not Repaid");
            }
            ControlManager.Add(alert);
            alert.BringToFront();
            window.Enabled = false;
            alert.Closed += (sender, args) => window.Enabled = true;
        }

        public void OpenMenu() {
            MenuWindow.Show();
            MenuWindow.BringToFront();
            MenuButton.Enabled = false;
        }

        public void CloseMenuWindow() {
            MenuWindow.Hide();
        }

        public void EndGame() {
            var window = CreateGameResult();
            window.SendToBack();
            //    window.Resizable = false;
            //   MenuButton.Hide();
            window.Closed += (sender, args) => _returnToMainMenu();
        }

        public void Dispose() {

        }
    }
}
