using System;
using System.Collections.Generic;
using GameOfLife.Screens;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife.GameLogic.GameStates {
    public static class ChoosePathHelper {
        private static List<Node> _possibleNodes;
        private static MouseDown<Node> _cleanupCallback;
        private static MouseDown<Node> _callback;

        public static void TravelToNodes(List<Node> possibleNodes, GameInfo gameInfo, MouseDown<Node> callback, bool createCameratTargets = true)
        {
            gameInfo.PanCameraToObject(gameInfo.CurrentPlayer);
            gameInfo.CreateMessage("Choose your destination!");
            _cleanupCallback = (node, location) => CleanUp(node, location, gameInfo);
            _callback = callback;
            _possibleNodes = possibleNodes;
            foreach (var possibleNode in possibleNodes) {
                possibleNode.RememberColorState(ColorState.Glow);
                possibleNode.SetColorState(ColorState.Glow);
                possibleNode.ListenMouseDown += callback;
                possibleNode.ListenMouseDown += _cleanupCallback;
                if (createCameratTargets) {
                    gameInfo.AddLowPriorityTarget(possibleNode);
                }

            }
        }

        private static void CleanUp(Node node, Vector2 location, GameInfo gameInfo) {
            gameInfo.CreateMessage(String.Empty);
            gameInfo.ClearTargets();
            foreach (var possibleNode in _possibleNodes) {
                possibleNode.RememberColorState(ColorState.None);
                possibleNode.SetColorState(ColorState.None);
                possibleNode.ListenMouseDown -= _cleanupCallback;
                possibleNode.ListenMouseDown -= _callback;
            }
        }
    }
}
