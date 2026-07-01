using System;

namespace OmniWorld
{
    public struct Vec2
    {
        public float X;
        public float Y;

        public Vec2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vec2 Zero
        {
            get { return new Vec2(0f, 0f); }
        }

        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X + b.X, a.Y + b.Y);
        }

        public static Vec2 operator -(Vec2 a, Vec2 b)
        {
            return new Vec2(a.X - b.X, a.Y - b.Y);
        }

        public static Vec2 operator *(Vec2 a, float value)
        {
            return new Vec2(a.X * value, a.Y * value);
        }
    }

    public static class GameMath
    {
        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public static int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public static float Approach(float current, float target, float maxDelta)
        {
            if (current < target)
            {
                current += maxDelta;
                if (current > target) current = target;
            }
            else if (current > target)
            {
                current -= maxDelta;
                if (current < target) current = target;
            }

            return current;
        }

        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
    }

    public enum GameScreen
    {
        CourseSelect,
        Playing,
        Paused,
        CourseComplete,
        GameOver,
        Victory
    }

    public enum TileType
    {
        Empty,
        Ground,
        Breakable,
        Spike
    }

    public enum CollectibleKind
    {
        EnergyOrb,
        FluxCore,
        Heart,
        BurstCell
    }

    public enum EnemyKind
    {
        Roller,
        Hopper,
        Seeker
    }
}
