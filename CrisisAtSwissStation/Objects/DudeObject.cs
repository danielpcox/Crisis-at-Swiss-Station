using Box2DX.Collision;
using Box2DX.Dynamics;
using Box2DX.Dynamics.Controllers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color; // Stupid Box2DX name collision!

using System;
using System.Diagnostics;

namespace CrisisAtSwissStation
{
    /**
     * Defines a platform character, the super-fancy
     * technical name of which is a "Dude."
     * Our dude can move left and right, jump, and shoot
     * bullets.
     */
    public class DudeObject : BoxObject
    {

        //dude's jump impulse
        public static float jumpImpulse = -2.7f;


        private const int JUMP_COOLDOWN = 30;

        private const float DUDE_FORCE = 20.0f;   // How much force to apply to get the dude moving
        private const float DUDE_DAMPING = 10.0f; // How hard the brakes are applied to get a dude to stop moving
        private const float DUDE_MAXSPEED = 6.0f; // Upper limit on dude left-right movement.  Does NOT apply to vertical movement.

        // Whether or not this dude is touching the ground
        private bool isGrounded;

        // Cooldown values
        private int jumpCooldown;

        // Lets us know which direction we're facing
        private bool facingRight;

        //animation stuff
        private Rectangle sourceRect;
        private Vector2 origin;
        private float walkTimer;
        private float walkInterval;
        private int xFrame;
        private int yFrame;
        private int spriteWidth;
        private int spriteHeight;
        private Texture2D animTexture;
        private int myGameTime;
      

        /**
         * Creates a new dude
         */
        public DudeObject(World world, Texture2D texture, Texture2D objectTexture, string groundSensorName)
        : base(world, objectTexture, .5f, 0.0f, 0.0f) //: base(world, texture, 1.0f, 0.0f, 0.0f)
        {
            // Initialize
            isGrounded = false;            

            // BodyDef options
            BodyDef.FixedRotation = true;

            // Make a dude controller
            controllers.Add(new DudeController());

            //animation stuff
            myGameTime = 0;
            animTexture = texture;
            walkTimer = 0;
            walkInterval = 5;
            xFrame = 0;
            yFrame = 0;
            spriteWidth = 100;
            spriteHeight = 100;
            sourceRect = new Rectangle(xFrame * spriteWidth, yFrame * spriteHeight, spriteWidth, spriteHeight);
            origin = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);


            // Ground Sensor
            // -------------
            //   We only allow the dude to jump when he's
            // on the ground.  After all, how can you jump
            // when you're flying in the air?  (Unless you
            // can double jump)
            //   To determine whether or not the dude is on
            // the ground, we create a thin sensor under his
            // feet, which reports collisions with the world
            // but has no collision response.
            //   Game logic in PlatformWorld tells the dude
            // whether or not he is grounded.


            //animation stuff
            /*
            // Compute dimensions of the ground sensor
            float halfWidth = (float)texture.Width / (2 * CASSWorld.SCALE);
            float halfHeight = (float)texture.Height / (2 * CASSWorld.SCALE);
            Vector2 sensorCenter = new Vector2(0, halfHeight);
             */
            float halfWidth = (float)objectTexture.Width / (2 * CASSWorld.SCALE);
            float halfHeight = (float)objectTexture.Height / (2 * CASSWorld.SCALE);
            Vector2 sensorCenter = new Vector2(0, halfHeight); 

            // Create collision shape of the ground sensor
            PolygonDef groundSensor = new PolygonDef();
            groundSensor.Density = 1.0f;
            groundSensor.IsSensor = true;
            groundSensor.UserData = groundSensorName;
            groundSensor.SetAsBox(halfWidth / 2, 0.05f, Utils.Convert(sensorCenter), 0);
            shapes.Add(groundSensor);

            //animation stuff
            //base.(world, texture, 1.0f, 0.0f, 0.0f);
        }

        /**
         * Updates dude game logic - jumping cooldown
         */
        public override void Update(CASSWorld world, float dt)
        {
            
            // Apply cooldowns
            jumpCooldown = Math.Max(0, jumpCooldown - 1);

            //animation stuff
            myGameTime++;
            sourceRect = new Rectangle(xFrame * spriteWidth, yFrame * spriteHeight, spriteWidth, spriteHeight);
            origin = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);


            base.Update(world, dt);
        }

        /**
         * Draws the dude
         */
        public override void Draw(Vector2 offset)
        {
            //animation stuff

            //sourceRect = new Rectangle(xFrame * spriteWidth, yFrame * spriteHeight, spriteWidth, spriteHeight);
            //origin = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);

            Vector2 screenOffset = (CASSWorld.SCALE * Position) - offset;
            SpriteEffects flip = facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            //SpriteBatch spriteBatch = GameEngine.Instance.SpriteBatch;
            //spriteBatch.Begin();
            GameEngine.Instance.SpriteBatch.Begin();

            //spriteBatch.Draw(animTexture, screenOffset, sourceRect, Color.White, Angle, origin, 1, flip, 0);
            GameEngine.Instance.SpriteBatch.Draw(animTexture, screenOffset, sourceRect, Color.White, Angle, origin, 1, flip, 0);

            //spriteBatch.End();
            GameEngine.Instance.SpriteBatch.End();


            /*
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;
            Vector2 screenOffset = (CASSWorld.SCALE * Position) - offset;

            SpriteEffects flip = facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            SpriteBatch spriteBatch = GameEngine.Instance.SpriteBatch;
            spriteBatch.Begin();

            spriteBatch.Draw(texture, screenOffset, null, Color.White, Angle, origin, 1, flip, 0);

            spriteBatch.End();*/
        }

        /**
         * Is the dude on the ground or not
         */
        public bool Grounded
        {
            get { return isGrounded; }
            set { isGrounded = value; }
        }

        public bool isRight()
        {
            return facingRight;
        }

        public int getTime()
        {
            return myGameTime;
        }

        /**
         * Controller for a dude
         */
        private class DudeController : Controller
        {
            /**
             * Performs a timestep
             */
            public override void Step(TimeStep step)
            {
                Body dude = _bodyList.body;
                DudeObject dudeObject = dude.GetUserData() as DudeObject;

                Debug.Assert(dude != null);

                Vector2 moveForce = new Vector2();
                bool jump = false;
               

                // TODO: XBox controls
                // --------------------
                KeyboardState ks = Keyboard.GetState();
                if (ks.IsKeyDown(Keys.Left) || ks.IsKeyDown(Keys.A))
                { 
                    moveForce.X -= DUDE_FORCE;
                    //dudeObject.walkAnimation(dudeObject.getTime());
                    if(dudeObject.Grounded)
                    dudeObject.walkAnimation();
                }
                else if (ks.IsKeyDown(Keys.Right) || ks.IsKeyDown(Keys.D))
                {
                    moveForce.X += DUDE_FORCE;
                    //dudeObject.walkAnimation(dudeObject.getTime());
                    if(dudeObject.Grounded)
                    dudeObject.walkAnimation();
                }
                
                if (ks.IsKeyDown(Keys.Up) || ks.IsKeyDown(Keys.W))
                    jump = true;
                // --------------------

                Vector2 vel = Utils.Convert(dude.GetLinearVelocity());

                // Which way are we facing  (add in a is X(mouse) > X(dude) condition??)
                if (moveForce.X < 0)
                    dudeObject.facingRight = false;
                else if (moveForce.X > 0)
                    dudeObject.facingRight = true;

                // Don't want to be moving - damp out player motion
                if (moveForce.X == 0.0f)
                {
                    Vector2 dampForce = new Vector2(-DUDE_DAMPING*vel.X, 0);
                    dude.ApplyForce(Utils.Convert(dampForce), dude.GetPosition());
                }

                // Velocity too high, clamp it
                if (Math.Abs(vel.X) >= DUDE_MAXSPEED)
                {
                    vel.X = Math.Sign(vel.X) * DUDE_MAXSPEED;
                    dude.SetLinearVelocity(Utils.Convert(vel));
                }
                else
                {
                    dude.ApplyForce(Utils.Convert(moveForce), dude.GetPosition());
                }

                // Jump!
                if (dudeObject.jumpCooldown == 0 && jump && dudeObject.Grounded)
                {
                    //animation stuff
                    //Vector2 impulse = new Vector2(0, -2.1f);
                   Vector2 impulse = new Vector2(0, jumpImpulse);
                    dude.ApplyImpulse(Utils.Convert(impulse), dude.GetPosition());
                    dudeObject.jumpCooldown = JUMP_COOLDOWN;
                }

            }
        }

        //animation stuff
        private void walkAnimation()//int gameTime)
        {
            //walkTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            //walkTimer += (float)gameTime;
            walkTimer += myGameTime;

            if (walkTimer > walkInterval)
            {
                xFrame++;

                if (xFrame > 7 && yFrame == 0)
                { 
                    xFrame = 0;                  
                    yFrame = 1;
                }
                else if (xFrame > 7 && yFrame == 1)
                {
                    xFrame = 0;
                    yFrame = 0;
                }

                // -= (int)walkInterval;
                myGameTime = 0;
                walkTimer = 0;

            }



        }
    }
}