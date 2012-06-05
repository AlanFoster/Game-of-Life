using System;
using System.Runtime.Serialization;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.Data;
using GameOfLife.GameLogic.GameStates;
using GameOfLife.GameLogic.GameStates.Factories;
using GameOfLife.WorldEditing;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GameOfLife.GameLogic.PureLogics {
    [DataContract]
    [Editable("Take Exam", true)]
    class TakeExam : PureLogic {
        private const int ExamCost = 50000;

        public override string GetGraphicLocation() {
            return Constants.ImageIcons.Exam;
        }

        public override string Description {
            get { return "Take\nExams"; }
        }

        public override IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            var currentPlayer = gameInfo.CurrentPlayer;

            currentPlayer.CareerType = CareerType.CollegeCareer;

            // Test if they're already taken the exam before
            if (currentPlayer.PassedExam) {
                return null;
            }

            var waitState = StateFactory.GetState(GameStates.GameStates.Wait);

            if (currentPlayer.CurrentlyTakingExam) {
                return TestPassedExamLogic(gameInfo, waitState);
            }


            if (currentPlayer.CareerType != CareerType.CollegeCareer) {
                return CreateAlert(gameInfo, waitState, "You can only take an exam if you took college path", icon: "Images/AlertIcons/Fail");
            }

            // Strip their roll amount;
            currentPlayer.RollAmount = 0;

            // Set their taking exam boolean to be true
            currentPlayer.CurrentlyTakingExam = true;

            return TakeExamLogic(gameInfo);
        }

        private IGameState[] TakeExamLogic(GameInfo gameInfo) {
            gameInfo.CreateMessage("Time for your exam! Spin the spinner!");
            gameInfo.PanCameraToObject(gameInfo.Spinner);
            return new[] { this, StateFactory.GetState(GameStates.GameStates.Spin) };
        }

        private IGameState[] TestPassedExamLogic(GameInfo gameInfo, IGameState waitState) {
            var spinnedNumber = gameInfo.Spinner.SpinnedNumber;

            if (spinnedNumber >= 1 && spinnedNumber <= 4) {
                CreateFailedExamWindow(gameInfo, waitState);
                return new[] { waitState };
            }
            PassExam(gameInfo, waitState);
            return null;
        }

        private void CreateFailedExamWindow(GameInfo gameInfo, IGameState waitState) {
            gameInfo.CurrentPlayer.CurrentlyTakingExam = false;
            var confirmWindow = new ConfirmWindow(gameInfo.Manager, String.Format("You failed. Pay ${0:N0} to pass?", ExamCost), title: "You failed the exam!", autoClose: true, icon: "Images/AlertIcons/Fail");
            gameInfo.Content.Load<SoundEffect>("Sounds/sadtrombone").Play();
            confirmWindow.AffirmButton.Click += (sender, args) => PayForExam(gameInfo, waitState);
            confirmWindow.DenyButton.Click += (sender, args) => gameInfo.Fsm.Remove(waitState);
            gameInfo.Manager.Add(confirmWindow);
        }

        private void PayForExam(GameInfo gameInfo, IGameState waitState) {
            gameInfo.CurrentPlayer.Cash -= ExamCost;
            gameInfo.Fsm.Remove(waitState);
            PassExam(gameInfo, waitState);
        }

        private void PassExam(GameInfo gameInfo, IGameState waitState) {
            gameInfo.CurrentPlayer.PassedExam = true;
            gameInfo.Content.Load<SoundEffect>("Sounds/applause").Play();
            gameInfo.CurrentPlayer.CurrentlyTakingExam = false;
            var alert = new Alert(gameInfo.Manager, "You passed your exam! Start your career", title: "Congratulations! You passed your exam!", icon: "Images/AlertIcons/Graduated" + gameInfo.CurrentPlayer.Gender);
            alert.BringToFront();
            alert.Closed += (sender, args) => gameInfo.Fsm.Remove(waitState);
            gameInfo.Manager.Add(alert);
            gameInfo.Fsm.Push(new StartCareer());
            gameInfo.Fsm.Push(waitState);
        }

        private IGameState[] CreateAlert(GameInfo gameInfo, IGameState waitState, String message, IGameState[] addStates = null, String icon = null) {
            var alert = new Alert(gameInfo.Manager, message, icon: icon);
            gameInfo.Manager.Add(alert);
            alert.Closing += (sender, args) => gameInfo.Fsm.Remove(waitState);

            return addStates ?? new[] { waitState };
        }
    }
}
