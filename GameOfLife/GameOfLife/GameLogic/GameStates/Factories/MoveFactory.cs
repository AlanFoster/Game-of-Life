using System;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Factories {
    public class MoveFactory : IStateFactory {
        private readonly Lazy<MovePlayerLogic> _singleton = new Lazy<MovePlayerLogic>();
        public IGameState GetState(GameStates gameStates) {
            return _singleton.Value;
        }

        private sealed class MovePlayerLogic : IGameState {
            private const int MoveSpeed = 3;

            public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
                var currentPlayer = gameInfo.CurrentPlayer;

                bool reversing = currentPlayer.CurrentNode == currentPlayer.NextNode;
                var currentLocation = currentPlayer.Location;
                var targetLocation =
                    reversing
                        ? currentPlayer.NextNode.GetTravelPosition(gameInfo.CurrentPlayerIndex, gameInfo.TotalPlayers)
                        : currentPlayer.NextNode.Center;

                float newRotation;
                Vector2 normalisedVector;

                // If we've reached our destination
                if (TravelTo(currentLocation, targetLocation, out normalisedVector, out newRotation)) {
                    // We have now reached the 'next' node, and thus it is our current node.
                    currentPlayer.CurrentNode = currentPlayer.NextNode;

                    return  (!reversing && currentPlayer.RollAmount == 0) ? new[] {this} : null;
                    return null;
                }

                currentPlayer.Location += normalisedVector * MoveSpeed;
                currentPlayer.Rotation = newRotation;
                gameInfo.PanCameraToObject(new CameraTarget(gameInfo.CurrentPlayer));

                return new IGameState[] { this };
            }

            private static bool TravelTo(Vector2 currentLocation, Vector2 endLocation, out Vector2 normalisedVector, out float newRotation) {
                Vector2 vectorDiffNormalized;
                var vectorDiff = vectorDiffNormalized = endLocation - currentLocation;
                vectorDiffNormalized.Normalize();

                newRotation = (float)Math.Atan2(vectorDiffNormalized.Y, vectorDiffNormalized.X);
                normalisedVector = vectorDiffNormalized;

                return vectorDiff.Length() < 5;
            }
        }
    }
}
