using System;
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
    [Serializable]
    public class CircleObject : PhysicsObject
    {
        // The circle texture
        [NonSerialized]
        protected Texture2D texture;
        private float scale;

        string textureName;

        /**
         * Creates a new circle object.
         */
        public CircleObject(World world, string texturename, float density, float friction, float restitution,float myScale)
            : base(world)
        {
            textureName = texturename;

            // Initialize
            this.texture = GameEngine.TextureList[texturename];
            TextureFilename = texturename;

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
            this.texture = GameEngine.TextureList[TextureFilename];
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

            base.Draw(cameraTransform);
        }

    }
}