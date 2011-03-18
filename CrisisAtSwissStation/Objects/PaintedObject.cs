using Box2DX.Common;
using Box2DX.Collision;
using Box2DX.Dynamics;
using Box2DX.Dynamics.Controllers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color; // Stupid Box2DX name collision!

using System;
using System.Resources;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace CrisisAtSwissStation
{
    /**
     * Defines an object drawn by the user with the mouse.
     */
    public class PaintedObject : CircleObject
    {
        // physical parameters for each blob in a PaintedObject
        private const float POB_DENSITY = 1.0f;
        private const float POB_FRICTION = 0.5f;
        private const float POB_RESTITUTION = 0.0f;

        int numBlobs = 0;

        /**
         * Creates a new drawn object
         */
        public PaintedObject(World world, Texture2D texture, List<Vector2> blobs)
            : base(world, texture, POB_DENSITY, POB_FRICTION, POB_RESTITUTION)
        {
            Position = blobs[0] / CASSWorld.SCALE; // position of the painting is the first blob in it

            float radius = (float)texture.Width / (2 * CASSWorld.SCALE);

            foreach (Vector2 blobpos in blobs)
            {
                Vector2 localpos = (blobpos / CASSWorld.SCALE) - Position;

                // add a circle fixture to this object at each point
                CircleDef circle = new CircleDef();
                circle.LocalPosition = Utils.Convert(localpos);
                circle.Radius = radius;
                circle.Density = POB_DENSITY;
                circle.Friction = POB_FRICTION;
                circle.Restitution = POB_RESTITUTION;
                shapes.Add(circle);
                numBlobs++;
            }
        }

        // DEBUG : so, Box2DX for some reason didn't want to add shapes to an object on the fly.
        // temporary solution: blast the old object and create a whole new one
        public void AddToShapes(List<Vector2> blobs)
        {
            //List<Vector2> totalblobs = new List<Vector2>();

            //foreach (CircleDef shape in shapes)
            //{
            //    totalblobs.Add(Utils.Convert(shape.LocalPosition));
            //}

            float radius = (float)texture.Width / (2 * CASSWorld.SCALE);

            foreach (Vector2 blobpos in blobs)
            {
                Vector2 localpos = (blobpos / CASSWorld.SCALE) - Position;

                // add a circle fixture to this object at each point
                CircleDef circle = new CircleDef();
                circle.LocalPosition = Utils.Convert(localpos);
                circle.Radius = radius;
                circle.Density = POB_DENSITY;
                circle.Friction = POB_FRICTION;
                circle.Restitution = POB_RESTITUTION;
                shapes.Add(circle);
                Body.CreateShape(circle);
            }
            Body.SetMassFromShapes();
        }

        /**
         * Draws the painted object
         */
        public override void Draw(Vector2 offset)
        {
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;

            SpriteBatch spriteBatch = GameEngine.Instance.SpriteBatch;
            spriteBatch.Begin();
            //foreach (Vector2 blobpos in blobs)
            //{
            //    spriteBatch.Draw(texture, (Position + blobpos) * DemoWorld.SCALE, null, Color.White, Angle, origin, 1, 0, 0);
            //}
            foreach (CircleDef blobpos in shapes)
            {
                Vector2 localpos = Utils.Convert(blobpos.LocalPosition);

                double theta = System.Math.Atan2(localpos.Y, localpos.X) + Angle;
                Vector2 rotatedpos = new Vector2((float)(localpos.Length() * System.Math.Cos(theta)), (float)(localpos.Length() * System.Math.Sin(theta)));
                Vector2 screenOffset = ((Position + rotatedpos) * CASSWorld.SCALE) - offset;
                spriteBatch.Draw(texture, screenOffset, null, Color.White, Angle, origin, 1, 0, 0);
            }
            
            spriteBatch.End();
        }

        public int getNumBlobs()
        {
            return numBlobs;
        }
    }
}