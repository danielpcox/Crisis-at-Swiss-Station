using Box2DX.Common;

using Microsoft.Xna.Framework;

using System;

namespace CrisisAtSwissStation
{
    public class Utils
    {
        public const float EPSILON = 0.001f;

        private static Random random;
        public static Random Random
        {
            get
            {
                if (random == null)
                    random = new Random();
                return random;
            }
        }

        public static Vector2 Convert(Vec2 vec)
        {
            return new Vector2(vec.X, vec.Y);
        }

        public static Vec2 Convert(Vector2 vec)
        {
            return new Vec2(vec.X, vec.Y);
        }
    }
}