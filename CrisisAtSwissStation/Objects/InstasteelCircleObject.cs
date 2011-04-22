using Box2DX.Common;
using Box2DX.Collision;
using Box2DX.Dynamics;
using Box2DX.Dynamics.Controllers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color; // Stupid Box2DX name collision!
using CrisisAtSwissStation.Common;

using System;
using System.Resources;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace CrisisAtSwissStation
{
    /**
     * A circularly shaped object in the world.
     * Dimension is taken from the width of the texture.
     * Texture provided should be square to avoid distortion.
     */
    [Serializable]
    public class InstasteelCircleObject : PaintedObject
    {

        protected static string blobtexturename ="paint";
        protected static string segmenttexturename = "paintsegment";
        protected static List<Vector2> blobs = new List<Vector2>();

        /**
         * Creates a new circle object.
         */
        public InstasteelCircleObject(World world, string texturename, float amountofis, float density, float friction, float restitution,float myScale)
            : base(world, blobtexturename, segmenttexturename, blobs)
        {
            shapes.Clear();

            amountOfInstasteel = amountofis;

            textureName = texturename;

            // Initialize
            this.texture = GameEngine.TextureList[texturename];
            TextureFilename = texturename;

            Width = texture.Width * myScale;
            Height = Width;

            scale = myScale;

            // Determine dimension
            float radius = (float)texture.Width / (2 * CASSWorld.SCALE) * scale;

            // Create collision shape
            CircleDef shape = new CircleDef();
            shape.Radius = radius;
            shape.Density = density;
            shape.Friction = friction;
            shape.Restitution = restitution;
            shapes.Add(shape);
        }

        public void reloadNonSerializedAssets()
        {
            this.texture = GameEngine.TextureList[textureName];
            amountOfInstasteel = amountOfInstasteel * scale;
        }

        /**
         * Draws the object on the screen using a sprite batch.
         */
        public override void Draw(Matrix cameraTransform)
        {
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;

            SpriteBatch spriteBatch = GameEngine.Instance.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default,
                              RasterizerState.CullCounterClockwise, null, cameraTransform);

            spriteBatch.Draw(texture, CASSWorld.SCALE * Position, null, Color.White, Angle, origin, scale, SpriteEffects.None, 0);

            spriteBatch.End();

            //base.Draw(cameraTransform);
        }

    }
}