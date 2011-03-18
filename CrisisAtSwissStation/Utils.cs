using Box2DX.Common;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public static void DrawLine(SpriteBatch spriteBatch, Texture2D spr, Vector2 a, Vector2 b, Color col)
        {
            Vector2 Origin = new Vector2(0.5f * spr.Width, 0.0f);
            Vector2 diff = b - a;
            float angle;
            Vector2 Scale = new Vector2(1.0f, diff.Length() / spr.Height);

            angle = (float)(System.Math.Atan2(diff.Y, diff.X)) - MathHelper.PiOver2;

            spriteBatch.Draw(spr, a, null, col, angle, Origin, Scale, SpriteEffects.None, 1.0f);
        }
    }
}