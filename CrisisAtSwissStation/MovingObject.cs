using System;
using Box2DX.Collision;
using Box2DX.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color; // Stupid Box2DX name collision!
using CrisisAtSwissStation.Common;
namespace CrisisAtSwissStation
{
     /**
     * A BoxObject is a physical object in the world
     * that has a rectangular shape.  Its dimensions
     * are determined by the size of the texture provided.
     */
    [Serializable]
    public class MovingObject : PhysicsObject
    {
        private bool isMoving = true;
        private Vector2 myForce;
        private SwitchObject mySwitch;
        // The box texture
        [NonSerialized]
        protected Texture2D texture;
        private float scale;
        private float bound1;//lower bound
        private float bound2;//upper bound

        private string textureName;

        /**
         * Creates a new box object
         */
        public MovingObject(World world, string texturename, float density, float friction, float restitution, float myScale, bool isPulley, SwitchObject mySwitch, Vector2 myForce, float bound1, float bound2)
            : base(world)
        {
            texture = GameEngine.TextureList[texturename];

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
           
        }

        public void reloadNonSerializedAssets()
        {
            texture = GameEngine.TextureList[textureName];
        }

        public override void Update(CASSWorld world, float dt)
        {
           
            if (mySwitch.switchOn)
            {
                
                
                    if (isMoving== true)
                    {
                        this.Body.ApplyForce(Utils.Convert(myForce), this.Body.GetWorldCenter());
                        //movPlatform1.Position = movPlatform1.Position - new Vector2(0, 0.05f);
                        if (this.Position.Y < bound1)
                        {
                            isMoving = false;
                            mySwitch.switchOn = false;
                        }

                    }
                    else
                    {
                        //movPlatform1.Position = movPlatform1.Position + new Vector2(0, 0.05f);
                        this.Body.ApplyForce(Utils.Convert(myForce/3), this.Body.GetWorldCenter());
                        if (this.Position.Y > bound2)
                            mySwitch.switchOn = true;
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
