using Box2DX.Collision;
using Box2DX.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color; // Stupid Box2DX name collision!

namespace CrisisAtSwissStation
{
    /**
     * A circularly shaped object in the world.
     * Dimension is taken from the width of the texture.
     * Texture provided should be square to avoid distortion.
     */
    public class CircleObject : PhysicsObject
    {
        // The circle texture
        protected Texture2D texture;

        /**
         * Creates a new circle object.
         */
        public CircleObject(World world, Texture2D texture, float density, float friction, float restitution)
            : base(world)
        {
            // Initialize
            this.texture = texture;

            // Determine dimension
            float radius = (float)texture.Width / (2 * CASSWorld.SCALE);

            // Create collision shape
            CircleDef shape = new CircleDef();
            shape.Radius = radius;
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
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;
            Vector2 screenOffset = (CASSWorld.SCALE * Position);// -offset;

            SpriteBatch spriteBatch = GameEngine.Instance.SpriteBatch;
            Matrix cameraTransform = Matrix.CreateTranslation(new Vector3(offset, 0));
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default,
                              RasterizerState.CullCounterClockwise, null, cameraTransform);

            spriteBatch.Draw(texture, screenOffset, null, Color.White, Angle, origin, 1, SpriteEffects.None, 0);

            spriteBatch.End();

            base.Draw(offset);
        }

    }
}