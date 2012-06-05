using System;

namespace GameOfLife.BoilerPlate.Misc {
    // Decorator pattern, but with static access to the methods, and storing a single Random object.
    public static class RandomHelper {
        private static readonly Random Random = new Random();

        public static int Next(int minValue, int maxValue) {
            return Random.Next(minValue, maxValue);
        }

        public static int Next(int maxValue) {
            return Random.Next(0, maxValue);
        }

        public static bool NextBool() {
            return Random.NextDouble() > 0.5d;
        }

        public static float NextFloat(float min, float max) {
            return (float)NextDouble(min, max);
        }

        public static double NextDouble(float min, float max) {
            return min + ((max - min) * Random.NextDouble());
        }
    }
}
