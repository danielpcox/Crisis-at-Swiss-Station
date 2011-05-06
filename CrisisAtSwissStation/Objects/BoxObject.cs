using System;
using Box2DX.Collision;
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
    [Serializable]
    public class BoxObject : PhysicsObject
    {
       
        // The box texture
        [NonSerialized]
        protected Texture2D texture;

        /**
         * Creates a new box object
         */
        /*
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

        */

        public BoxObject(World world, string textureName, float density, float friction, float restitution, float myScale, bool isPulley)
            : base(world)
        {
            // Initialize
            //Console.WriteLine(textureName);
            this.texture = GameEngine.TextureList[textureName];
            TextureFilename = textureName;
            Height = texture.Height * myScale;
            Width = texture.Width * myScale;
            boundingBox = new Rectangle((int)Position.X, (int)Position.Y, (int)Width, (int)Height);

            scale = myScale;

            if (isPulley)
                BodyDef.FixedRotation = true;
            
            // Determine dimensions
            float halfWidth = (float)texture.Width / (2 * CASSWorld.SCALE) * scale ;
            float halfHeight = (float)texture.Height / (2 * CASSWorld.SCALE) * scale;

            // Create the collision shape
            PolygonDef shape = new PolygonDef();
            shape.SetAsBox(halfWidth, halfHeight);
            shape.Density = density;
            shape.Friction = friction;
            shape.Restitution = restitution;
            shape.UserData = textureName; // for DEBUGging purposes
            shapes.Add(shape);
        }

        public void reloadNonSerializedAssets()
        {
            //Console.WriteLine(TextureFilename);
            this.texture = GameEngine.TextureList[TextureFilename];
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

            if (TextureFilename == "Art\\Objects\\BoxObjects\\bottomTexture2273")
            {
                spriteBatch.Draw(texture, CASSWorld.SCALE * Position, null, Color.White, Angle, origin, new Vector2(scale, 1), SpriteEffects.None, 0);
            }
            else
            {
                spriteBatch.Draw(texture, CASSWorld.SCALE * Position, null, Color.White, Angle, origin, scale, SpriteEffects.None, 0);
            }

            spriteBatch.End();
        }

    }
}
