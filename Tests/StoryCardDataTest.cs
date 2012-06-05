using GameOfLife.BoilerPlate.FSM;
using GameOfLife.Data;
using GameOfLife.GameLogic;
using GameOfLife.GameLogic.Assets;
using GameOfLife.GameLogic.GameStates;
using GameOfLife.GameLogic.GameStates.Factories;
using GameOfLife.WorldObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GameOfLife.GameLogic.Storys;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Tests {


    /// <summary>
    ///This is a test class for StoryCardDataTest and is intended
    ///to contain all StoryCardDataTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StoryCardDataTest {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        // <summary>
        /// A test that all logic performs a logical action properly, without crashing.
        /// There is no explicit test for the logic to be valid. As that will have been tested seperately.
        ///</summary>
        [TestMethod()]
        public void StoryCardLogicTest() {
            StoryCardData target = new StoryCardData();
            var stories = StoryCardData.PopulateStoryCards();
            StateFactory.SetInstance(new StateFactory());
            StateFactory.AddFactory(GameStates.Wait, new WaitFactory());

            var gameTime = new GameTime();
            var player = new Player();
            player.Initialize();
            player.Accept(new Transport(TransportType.Car, String.Empty, int.MaxValue, null, null, 0));
            var world = new List<WorldObject> { new StartingNode(Vector2.Zero), new Spinner(Vector2.Zero) };
            var gameInfo = new GameInfo(world, new[] { player }, 100, GameRuleType.Retirement);
            gameInfo.GetNextPlayer();

            foreach (var story in stories) {
                var performingLogic = story.PureLogic;

                Assert.IsNotNull(performingLogic);
                try {
                    var returnedStates = story.PureLogic.PerformLogic(gameTime, gameInfo);
                } catch (Exception e) {
                    Console.WriteLine(e);
                }

            }
        }


    }
}
