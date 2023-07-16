using JetBrains.Annotations;
using System;
using Unity.VisualScripting;

namespace AWE.Synzza {
    public readonly struct FloatRange {
        public float Min { get; }
        public float Max { get; }

        public FloatRange(float min, float max) { Min = min; Max = max; }

        public bool IsWithin(float value) => value > Min && value < Max;
    }

    public static class GameRandom {
        private readonly static Random _rng;

        static GameRandom() {
            _rng = new();
        }

        public static float NextFloat() => (float)_rng.NextDouble();
        public static int Range(int inclusiveMin, int exclusiveMax) => _rng.Next(inclusiveMin, exclusiveMax);
    }

    public readonly struct float3 {
        public float x { get; }
        public float y { get; }
        public float z { get; }

        public float3(float X, float Y, float Z) { x = X; y = Y; z = Z; }

        public static float3 operator+(in float3 a, in float3 b) => new(a.x + b.x, a.y + b.y, a.z + b.z);
        public static float3 operator-(in float3 a, in float3 b) => new(a.x - b.x, a.y - b.y, a.z - b.z);
        public static float3 operator*(float a, in float3 b) => new(a * b.x, a * b.y, a * b.z);
        public static float3 operator*(in float3 a, float b) => new(a.x * b, a.y * b, a.z * b);

        public float MagnitudeSquared => (x * x) + (y * y) + (z * z);
    }

    public readonly struct Volume {
        private readonly float3 _dimensions;

        public Volume(float width, float height, float depth) => _dimensions = new(width, height, depth);
        public Volume(in float3 dimensions) => _dimensions = dimensions;

        public float Width => _dimensions.x;
        public float Height => _dimensions.y;
        public float Depth => _dimensions.z;

        public float3 ToFloat3() => _dimensions;
    }

    public readonly struct float4 {
        public float x { get; }
        public float y { get; }
        public float z { get; }
        public float w { get; }

        public float4(float X, float Y, float Z, float W) { x = X; y = Y; z = Z; w = W; }
    }

    public abstract class QuaternionMath {
        private static QuaternionMath _calculator = null;

        public static void ProvideCalculator(QuaternionMath calculator) {
            if (_calculator != null) {
                throw new InvalidOperationException($"Cannot overwrite existing {typeof(QuaternionMath).Name} calculator!");
            }

            _calculator = calculator;
        }

        public static void ReleaseCalculator() {
            if (_calculator == null) {
                throw new InvalidOperationException($"No existing {typeof(QuaternionMath).Name} instance to release!");
            }

            _calculator = null;
        }

        public static QuaternionMath Calculator => _calculator;

        public abstract float4 Euler(float x, float y, float z);
        public float4 Euler(in float3 euler) => Euler(euler.x, euler.y, euler.z);
        public abstract float4 identity { get; }
    }

    public readonly struct DurationProfile<TUnit> where TUnit : struct {
        public TUnit Duration { get; }
        public TUnit Variance { get; }

        public DurationProfile(in TUnit duration, in TUnit variance = default) {
            Duration = duration;
            Variance = variance;
        }
    }
}
