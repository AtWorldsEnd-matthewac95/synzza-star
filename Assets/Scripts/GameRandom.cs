using System;

namespace AWE.Synzza {
    public readonly struct FloatRange {
        public float Min { get; }
        public float Max { get; }

        public FloatRange(float min, float max) { Min = min; Max = max; }
    }

    public static class GameRandom {
        private readonly static Random _rng;

        static GameRandom() {
            _rng = new();
        }

        public static float NextFloat() => (float)_rng.NextDouble();
        public static int Range(int inclusiveMin, int exclusiveMax) => _rng.Next(inclusiveMin, exclusiveMax);
    }
}
