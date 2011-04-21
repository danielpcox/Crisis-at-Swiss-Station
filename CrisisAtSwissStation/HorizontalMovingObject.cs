using Box2DX.Collision;
using Box2DX.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color; // Stupid Box2DX name collision!
using CrisisAtSwissStation.Common;
using System;
namespace CrisisAtSwissStation
{
    /**
    * A BoxObject is a physical object in the world
    * that has a rectangular shape.  Its dimensions
    * are determined by the size of the texture provided.
    */
    [Serializable]
    public class HorizontalMovingObject : PhysicsObject
    {
        public bool isMoving;
        private Vector2 myForce;
        public SwitchObject mySwitch;
        // The box texture
        [NonSerialized]
        protected Texture2D texture;
        private float scale;
        public float bound1;//lower bound
        public float bound2;//upper bound
        
        /**
         * Creates a new box object
         */
        public HorizontalMovingObject(World world, string texturename, float density, float friction, float restitution, float myScale, bool isPulley, SwitchObject mySwitch, Vector2 myForce, float bound1, float bound2)
            : base(world)
        {
            texture = GameEngine.TextureList[texturename];
            TextureFilename = texturename;

            Height = texture.Height * myScale;
            Width = texture.Width * myScale;
            boundingBox = new Rectangle((int)Position.X, (int)Position.Y, (int)Width, (int)Height);

            this.bound1 = bound1;
            this.bound2 = bound2;
            this.myForce = myForce;
            this.mySwitch = mySwitch;
            BodyDef.IsBullet = true;
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

            isMoving = true;

        }


        public void reloadNonSerializedAssets()
        {
            texture = GameEngine.TextureList[TextureFilename];
        }

        public override void Update(CASSWorld world, float dt)
        {
            
            //hoizontal objects not bound by switches atm

            if (mySwitch != null)
            {
                if (mySwitch.switchOn)
                {

                    if (isMoving == true)
                    {
                        Position = Position + new Vector2(.035f, 0);
                        //movPlatform1.Position = movPlatform1.Position - new Vector2(0, 0.05f);
                        if (this.Position.X > bound2)
                        {
                            isMoving = false;
                             mySwitch.switchOn = false;//right end of path, switch turns off automatically
                        }

                    }
                    else
                    {
                        //movPlatform1.Position = movPlatform1.Position + new Vector2(0, 0.05f);
                        Position = Position - new Vector2(.035f, 0);
                        if (this.Position.X < bound1)
                            // mySwitch.switchOn = true;
                            isMoving = true;
                    }

                }
            }
            else
            {
                if (isMoving == true)
                {
                    Position = Position + new Vector2(.035f, 0);
                    //movPlatform1.Position = movPlatform1.Position - new Vector2(0, 0.05f);
                    if (this.Position.X > bound2)
                    {
                        isMoving = false;
                        // mySwitch.switchOn = false;
                    }

                }
                else
                {
                    //movPlatform1.Position = movPlatform1.Position + new Vector2(0, 0.05f);
                    Position = Position - new Vector2(.035f, 0);
                    if (this.Position.X < bound1)
                        // mySwitch.switchOn = true;
                        isMoving = true;
                }
            }

            base.Update(world, dt);

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
