using System;
using GameOfLife;
using GameOfLife.BoilerPlate.Misc;
using GameOfLife.Data;
using GameOfLife.GameLogic.Assets;
using GameOfLife.WorldObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace Tests {
    /// <summary>
    ///This is a test class for PlayerTest and is intended
    ///to contain all PlayerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class PlayerTest {


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

        /// <summary>
        /// A test for testing if it calculates the correct total worth
        ///</summary>
        [TestMethod()]
        public void TotalValueTest() {
            int playerCash = RandomHelper.Next(0, 500000);
            int houseValue = RandomHelper.Next(0, 500000);
            int peopleValue = Constants.GameRules.TransportableValue;
            int carValue = RandomHelper.Next(0, 500000);

            Player player = new Player();
            player.Initialize();
            player.Cash += playerCash;

            Transport car = new Transport(TransportType.Car, String.Empty, 100, null, null, carValue);
            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(car));
            Assert.AreEqual(playerCash + carValue, player.TotalValue);

            Assert.AreEqual(playerCash, player.Cash);


            Asset asset1 = new House(houseValue, "testing house", String.Empty);
            Asset asset2 = new Partner();

            // make sure the player can afford the house
            player.Cash += asset1.Value;
            Assert.AreEqual(playerCash + houseValue, player.Cash);
            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(asset1));
            Assert.AreEqual(playerCash + houseValue + carValue, player.TotalValue);

            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(asset2));

            Assert.AreEqual( playerCash + houseValue + peopleValue + carValue, player.TotalValue);
        }

        /// <summary>
        ///A test for Cash
        ///</summary>
        [TestMethod()]
        public void CashTest() {
            const int cash = 12312;
            Vector2 location = Vector2.Zero;

            Player player = new Player();
            player.Initialize();
            player.Cash += cash;
            Transport worthlessCar = new Transport(TransportType.Car, String.Empty, 100, null, null, 0);
            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(worthlessCar));

            Asset asset1 = new House(15212, "testing house", String.Empty);
            Asset asset2 = new Partner();

            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(asset1));
            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(asset2));

            int expected = cash; 
            int actual;
            player.Cash = expected;
            actual = player.Cash;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        /// Test wife adding logic. One player can only have one wife at a time.
        ///</summary>
        [TestMethod()]
        public void WiveTest() {
            Player player = new Player();
            player.Initialize();
            Transport worthlessCar = new Transport(TransportType.Car, String.Empty, 100, null, null, 0);
            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(worthlessCar));

            Asset wive = new Partner();

            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(wive));
            Assert.AreEqual(AssetResponse.HasPartnerAlready, player.Accept(wive));
        }

        /// <summary>
        /// Testing children
        ///</summary>
        [TestMethod()]
        public void ChildrenTest() {
            Player player = new Player();
            player.Initialize();
            Transport worthlessCar = new Transport(TransportType.Car, String.Empty, 100, null, null, 0);
            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(worthlessCar));

            Asset wive = new Partner();
            Asset child = new Child();

            Assert.AreEqual(AssetResponse.HasNoPartner, player.Accept(child));
            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(wive));
            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(child));
        }


        /// <summary>
        /// Testing that cars are changed only one at a time, and the player's value is accurate.
        ///</summary>
        [TestMethod()]
        public void CarTest() {
            Player player = new Player();
            player.Initialize();
            const int carSize = 5;
            const int carValue = 123111;
            Transport car = new Transport(TransportType.Car, String.Empty, 100, null, null, carValue);
            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(car));

            Assert.AreEqual(carValue, player.TotalValue);
            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(car));
            Assert.AreEqual(carValue, player.TotalValue);

            const int secondCarValue = 1000;
            Transport car2 = new Transport(TransportType.Car, String.Empty, 100, null, null, secondCarValue);

            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(car2));

            Assert.AreEqual(secondCarValue, player.TotalValue);
            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(car2));
            Assert.AreEqual(secondCarValue, player.TotalValue);
        }


        /// <summary>
        /// Testing car size
        ///</summary>
        [TestMethod()]
        public void CarSizeTest() {
            Player player = new Player();
            player.Initialize();
            const int carSize = 5;
            Transport car = new Transport(TransportType.Car, String.Empty, 100, null, null, 10000);
            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(car));
            Asset wife = new Partner();
            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(wife));
            Asset[] carTravellers = {   
                                       new Child(), 
                                       new Pet()
                                   };

            // Start from 1 as we already added wife asset 
            for (var i = 1; i < player.CarSize; i++) {
                Assert.AreEqual(AssetResponse.AddedSuccessfully,
                                player.Accept(carTravellers[RandomHelper.Next(0, carTravellers.Length)]));
            }

            Assert.AreEqual(player.CarSize, player.Assets[AssetType.Transportable].Count);

            for (var i = 0; i < player.CarSize; i++) {
                Assert.AreEqual(AssetResponse.CarFull,
                                player.Accept(carTravellers[RandomHelper.Next(0, carTravellers.Length)]));
            }
        }

        /// <summary>
        /// Testing passport stamps
        ///</summary>
        [TestMethod()]
        public void PassportStampTest() {
            Player player = new Player();
            player.Initialize();

            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(new PassportStamp(IslandType.Beach)));
            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(new PassportStamp(IslandType.Beach)));
            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(new PassportStamp(IslandType.City)));
            Assert.AreEqual(AssetResponse.AddedSuccessfully, player.Accept(new PassportStamp(IslandType.Jungle)));
            Assert.AreEqual(AssetResponse.CollectedAllPassportStamps, player.Accept(new PassportStamp(IslandType.Snow)));
            Assert.AreEqual(AssetResponse.CollectedAllPassportStamps, player.Accept(new PassportStamp(IslandType.City)));
        }
    }
}
