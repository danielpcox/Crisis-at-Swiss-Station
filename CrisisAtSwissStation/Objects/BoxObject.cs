﻿using Box2DX.Collision;
using Box2DX.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color; // Stupid Box2DX name collision!

namespace CrisisAtSwissStation
{
    /**
     * A BoxObject is a physical object in the world
     * that has a rectangular shape.  Its dimensions
     * are determined by the size of the texture provided.
     */
    public class BoxObject : PhysicsObject
    {
        // The box texture
        protected Texture2D texture;
        private float scale;

        /**
         * Creates a new box object
         */
        public BoxObject(World world, Texture2D texture, float density, float friction, float restitution, float myScale, bool isPulley)
            : base(world)
        {
            // Initialize
            this.texture = texture;

            scale = myScale;

            if (isPulley)
                BodyDef.FixedRotation = true;
            
            // Determine dimensions
            float halfWidth = ((float)texture.Width / (2 * CASSWorld.SCALE)) * scale;
            float halfHeight = ((float)texture.Height / (2 * CASSWorld.SCALE)) * scale;

            // Create the collision shape
            PolygonDef shape = new PolygonDef();
            shape.SetAsBox(halfWidth, halfHeight);
            shape.Density = density;
            shape.Friction = friction;
            shape.Restitution = restitution;
            shapes.Add(shape);
        }

        public BoxObject(World world, string textureName, float density, float friction, float restitution)
            : base(world)
        {
            // Initialize
            this.texture = texture;
            
            // Determine dimensions
            float halfWidth = (float)texture.Width / (2 * CASSWorld.SCALE);
            float halfHeight = (float)texture.Height / (2 * CASSWorld.SCALE);

            // Create the collision shape
            PolygonDef shape = new PolygonDef();
            shape.SetAsBox(halfWidth, halfHeight);
            shape.Density = density;
            shape.Friction = friction;
            shape.Restitution = restitution;
            shapes.Add(shape);
        }

        /**
         * Draws the object on the screen using a sprite batch.
         */
        public override void Draw(Matrix cameraTransform)
        {
            base.Draw(cameraTransform);

            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;
           
            SpriteBatch spriteBatch = GameEngine.Instance.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default,
                              RasterizerState.CullCounterClockwise, null, cameraTransform);
            
            spriteBatch.Draw(texture, CASSWorld.SCALE * Position, null, Color.White, Angle, origin, scale, SpriteEffects.None, 0);

            spriteBatch.End();
        }

    }
}
