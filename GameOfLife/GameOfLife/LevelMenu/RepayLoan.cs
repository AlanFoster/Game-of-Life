using System.Globalization;
using GameOfLife.GameLogic.Assets;
using GameOfLife.WorldObjects;
using TomShane.Neoforce.Controls;

namespace GameOfLife.LevelMenu {
    public delegate void LoanPaidCallBack(Player player, Loan loan);
    public class RepayLoan : PlayerTab {
        private readonly LoanPaidCallBack _loanPaidCallBack;

        public RepayLoan(LoanPaidCallBack loanPaidCallBack) {
            _loanPaidCallBack = loanPaidCallBack;
        }

        public override int AddInformation(Manager manager, Player player, Control control, int yPos) {
            const int spacing = 20;
            int initialYPos = yPos;
            int xPos = spacing;
            var currentLoans = player.Assets[AssetType.Loan];

            if (currentLoans.Count == 0) {
                var noHouseLabel = new Label(manager) { Text = "You have no loans to repay", Width = 400, Left = xPos, Top = yPos, Name = IgnoreString };
                noHouseLabel.Init();
                noHouseLabel.Parent = control;
            } else {
                foreach (var loan in currentLoans) {

                    var loanAmountLabel = new Label(manager) {
                        Text = string.Format("-${0:N0}", -loan.Value),
                        Top = yPos,
                        Left = xPos,
                        Name = IgnoreString
                    };
                    loanAmountLabel.Init();

                    var repayButton = new Button(manager) { Text = "Repay loan", Top = yPos, Left = xPos + 250, Name = IgnoreString };
                    var loanVar = loan;

                    repayButton.Click += (sender, args) => {
                        _loanPaidCallBack(player, loanVar as Loan);
                        PopulateTab(manager, player, control);
                    };


                    loanAmountLabel.Parent = control;
                    repayButton.Parent = control;

                    yPos += loanAmountLabel.Height + spacing;
                }
            }
            return yPos;
        }
    }
}