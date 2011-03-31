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
    public class BoxObject : PhysicsObject
    {
        // The box texture
        protected Texture2D texture;

        /**
         * Creates a new box object
         */
        public BoxObject(World world, Texture2D texture, float density, float friction, float restitution)
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
        public override void Draw(Vector2 offset)
        {
            base.Draw(offset);

            

            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;
            Vector2 screenOffset = (CASSWorld.SCALE * Position);// -offset;

            SpriteBatch spriteBatch = GameEngine.Instance.SpriteBatch;
            Matrix cameraTransform = Matrix.CreateTranslation(new Vector3(offset, 0));
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default,
                              RasterizerState.CullCounterClockwise, null, cameraTransform);

            spriteBatch.Draw(texture, screenOffset, null, Color.White, Angle, origin, 1, SpriteEffects.None, 0);

            spriteBatch.End();
        }

    }
}