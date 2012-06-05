using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameOfLife.BoilerPlate.FSM;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;

namespace GameOfLife.GameLogic.GameStates.Actual {
    public class VisuallyMovePlayer : IGameState {
        private const int MoveSpeed = 3;

        public IGameState[] PerformLogic(GameTime gameTime, GameInfo gameInfo) {
            var currentPlayer = gameInfo.CurrentPlayer;

            bool reversing = currentPlayer.CurrentNode == currentPlayer.NextNode;
            var currentLocation = currentPlayer.ActualLocation;
            var targetLocation =
                reversing
                    ? currentPlayer.NextNode.GetTravelPosition(gameInfo.CurrentPlayerIndex, gameInfo.TotalPlayers)
                    : currentPlayer.NextNode.Center;

            float newRotation;
            Vector2 normalisedVector;

            // If we've reached our destination
            if (TravelTo(currentLocation, targetLocation, currentPlayer.Rotation, reversing, out normalisedVector, out newRotation)) {
                // We have now reached the 'next' node, and thus it is our current node.
                currentPlayer.CurrentNode = currentPlayer.NextNode;

                return (!reversing && currentPlayer.RollAmount == 0) ? new IGameState[] { this } : null;
            }

            currentPlayer.ActualLocation += normalisedVector * MoveSpeed;
            currentPlayer.Rotation = newRotation;
 
            return new IGameState[] { this };
        }

        private static bool TravelTo(Vector2 currentLocation, Vector2 endLocation, float currentRotation, bool reversing, out Vector2 normalisedVector, out float newRotation) {
            Vector2 vectorDiffNormalized;
            var vectorDiff = vectorDiffNormalized = endLocation - currentLocation;
            vectorDiffNormalized.Normalize();

            var currentRotationVector = new Vector2((float)Math.Cos(currentRotation), (float)Math.Sin(currentRotation));
            if (!reversing) {
                normalisedVector = Vector2.Lerp(currentRotationVector, vectorDiffNormalized, MathHelper.ToRadians(7));
            } else {
                normalisedVector = vectorDiffNormalized;
            }

            newRotation = (float)Math.Atan2(normalisedVector.Y, normalisedVector.X);

            return vectorDiff.Length() <= 10;
        }
    }
}
