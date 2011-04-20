using Box2DX.Common;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System;

namespace CrisisAtSwissStation.Common
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

            spriteBatch.Draw(spr, a, null, col, angle, Origin, Scale, SpriteEffects.None, 0f);
        }

        public static System.Drawing.Image Texture2Image(Texture2D texture)
        {
            if (texture == null)
            {
                return null;
            }

            if (texture.IsDisposed)
            {
                return null;
            }

            //Memory stream to store the bitmap data.
            MemoryStream ms = new MemoryStream();

            //Save the texture to the stream.
            texture.SaveAsPng(ms, texture.Width, texture.Height);

            //Seek the beginning of the stream.
            ms.Seek(0, SeekOrigin.Begin);

            //Create an image from a stream.
            System.Drawing.Image bmp2 = System.Drawing.Bitmap.FromStream(ms);

            //Close the stream, we nolonger need it.
            ms.Close();
            ms = null;
            return bmp2;
        }

        public static void Image2Texture(System.Drawing.Image image, 
                                 GraphicsDevice graphics,
                                 ref Texture2D texture)
        {
            if (image == null)
            {
                return;
            }

            if (texture == null || texture.IsDisposed ||
                texture.Width != image.Width ||
                texture.Height != image.Height ||
                texture.Format != SurfaceFormat.Color)
            {
                if (texture != null && !texture.IsDisposed)
                {
                    texture.Dispose();
                }

                texture = new Texture2D(graphics, image.Width, image.Height, false, SurfaceFormat.Color);
            }
            else
            {
                for (int i = 0; i < 16; i++)
                {
                    if (graphics.Textures[i] == texture)
                    {
                        graphics.Textures[i] = null;
                        break;
                    }
                }
            }

            //Memory stream to store the bitmap data.
            MemoryStream ms = new MemoryStream();

            //Save to that memory stream.
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            //Go to the beginning of the memory stream.
            ms.Seek(0, SeekOrigin.Begin);

            //Fill the texture.
            texture = Texture2D.FromStream(graphics, ms, image.Width, image.Height, false);

            //Close the stream.
            ms.Close();
            ms = null;
        }


    }
}