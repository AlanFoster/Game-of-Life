using System;
using System.Text.RegularExpressions;
using GameOfLife.WorldObjects;
using Microsoft.Xna.Framework;

namespace GameOfLife.Data {
    public static class Constants {
        public static class GameRules {
            public static readonly int TransportableValue = 100000;
            public static readonly int StartingCash = 10000;
            public static readonly int MinPlayers = 2;
            // Changing this above 6 players requires that DefaultPlayerInfo (below) has more information added to it.
            // Obviously it can be decreased though
            public static readonly int MaxPlayers = 6;
            public static readonly int MinYears = 10;
            public static readonly int MaxYears = 100;
            public static readonly int UniversityCost = 20000;
        }

        public static class Locations {
            public static readonly String DefaultWorldPath = "Data/DefaultWorld.xml";
            public static readonly String DefaultWorldName = "Basic world";
            public static readonly String TestWorldName = "Test World (Admin)";
            public static readonly String TestWorldPath = "Data/TestWorld.xml";
            public static readonly String ContainerName = "CustomMaps";
        }

        public static class DefaultPlayerInfo {
            public static readonly String[] AvatarImages = new[] { "Images/Avatars/dude1", "Images/Avatars/dude2", "Images/Avatars/dude3",
                                                                    "Images/Avatars/girl1", "Images/Avatars/girl2", "Images/Avatars/girl3"};
            public static readonly Tuple<String, Color, Gender>[] NamesAndColors = new[] {
                new Tuple<string, Color, Gender>("Alan", Color.Blue, Gender.Male),
                new Tuple<string, Color, Gender>("Michael", Color.DarkRed, Gender.Male),
                new Tuple<string, Color, Gender>("Daniel", Color.Green, Gender.Male), 
                new Tuple<string, Color, Gender>("Sarah", Color.Yellow, Gender.Female), 
                new Tuple<string, Color, Gender>("Christina", Color.Orange, Gender.Female), 
                new Tuple<string, Color, Gender>("Olivia", Color.Violet, Gender.Female), 
            };
        }

        public static class ScreenNames {
            public static readonly String Level = "Level";
            public static readonly String SetupLevel = "Begin Game";
            public static readonly String Editors = "Editors";
            public static readonly String ObjectEditor = "Object Editor";
            public static readonly String WorldEditor = "World Editor";
            public static readonly String Options = "Options";
            public static readonly String Exit = "Exit";
            public static readonly String MainMenu = "Main Menu";
            public static readonly String Background = "Background";
            public static readonly String WorldEditorSetup = "World Editor";
            public static readonly String Logo = "Logo";
        }

        public static class RegexValidations {
            public static readonly Regex Number = new Regex(@"^-?\d+$");
            public static readonly Regex NoSpaces = new Regex(@"\S+");
            public static readonly Func<int, Regex> ValidSize = i => new Regex(String.Format("^.{{{0}}}$", i));
        }

        public static class NodeColors {
            public static readonly Color Passing = new Color(0x73, 0x9F, 0x14);
            public static readonly Color Halting = new Color(0xF0, 0x00, 0x00);
            public static readonly Color Normal = new Color(0xEd, 0xA4, 0x00);
        }

        public static class ImageIcons {
            public static readonly String StopSign = "Images/Node/Icons/StopSign";
            public static readonly String Money = "Images/Node/Icons/Money";
            public static readonly String Heart = "Images/Node/Icons/Heart";
            public static readonly String Exam = "Images/Node/Icons/Exam";
            public static readonly String Book = "Images/Node/Icons/Book";
            public static readonly String SpinToWin = "Images/Node/Icons/SpinToWin";
            public static readonly String Child = "Images/Node/Icons/Child";
        }

        public static class Editing {
            /// <summary>
            /// When in admin mode we can use logic types that wouldn't be available to the typical end user.
            /// For instance the admin will require to use the Enum for starting island within IslandType,
            /// However the user shouldn't be able to use that selection.
            /// 
            /// Toggled/Settable by typing 'ADMIN PLEASE'
            /// </summary>
            public static bool IsAdminMode = false;

            /// <summary>
            /// The alert given when the user tries to perform an admin-only action.
            /// </summary>
            public static readonly string ErrorMessage = "You cannot modify this Node.";
            public static readonly string ErrorIcon = "Images/AlertIcons/Fail";
        }

        public static class Graphical {
            public static readonly float FadeValue = 0.02f;
        }
    }
}
