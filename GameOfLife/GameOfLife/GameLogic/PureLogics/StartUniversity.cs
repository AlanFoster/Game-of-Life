using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.BoilerPlate.GUI;
using GameOfLife.Data;
using GameOfLife.WorldEditing;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TomShane.Neoforce.Controls;

namespace GameOfLife.GameLogic.PureLogics {
    [DataContract]
    [Editable("Start University", adminRequired: true)]
    class StartUniversity : PureLogic {
        public override string Description {
            get { return String.Format("Start\nUniversity\n-${0:N0}", Constants.GameRules.UniversityCost); }
        }

        public override IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            var waitState = StateFactory.GetState(GameStates.GameStates.Wait);
            var alert = new Alert(gameInfo.Manager,
                                  String.Format("You have just started University and \nmust pay ${0:N0} in fees.\nYou will need to pass your exam to start career", +Constants.GameRules.UniversityCost),
                                  "Start University");
            var university = new ImageBox(gameInfo.Manager) { Image = gameInfo.Content.Load<Texture2D>("Images/AlertIcons/Graduated" + gameInfo.CurrentPlayer.Gender), Top = 15, Left = 10 };
            university.Init();
            university.Parent = alert;
            gameInfo.Manager.Add(alert);
            alert.Closed += (sender, args) => gameInfo.Fsm.Remove(waitState);
            gameInfo.CurrentPlayer.CareerType = CareerType.CollegeCareer;
            gameInfo.CurrentPlayer.Cash -= Constants.GameRules.UniversityCost;
            return new[] { waitState };
        }
    }
}
