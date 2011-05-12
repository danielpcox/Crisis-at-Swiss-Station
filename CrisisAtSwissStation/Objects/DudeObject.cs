using Box2DX.Collision;
using Box2DX.Dynamics;
using Box2DX.Dynamics.Controllers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color; // Stupid Box2DX name collision!
using CrisisAtSwissStation.Common;

using System;
using System.Diagnostics;

namespace CrisisAtSwissStation
{
    /**
     * Defines a platform character, the super-fancy
     * technical name of which is a "Dude."
     * Our dude can move left and right, jump, and draw and erase.
     */
    [Serializable]
    public class DudeObject : BoxObject
    {

        private static bool lockDude;
        private bool amDead;

        private Vector2 normal = new Vector2(0,-1); // when the dude is in contact with a surface, this points from the surface to the dude
       

        //dude's jump impulse
        //public static float jumpImpulse = -5f;
        public static float jumpImpulse = 5f;


        private const int JUMP_COOLDOWN = 30;

        private const float DUDE_DENSITY = 0.7f;
        private const float DUDE_FORCE = 30.0f;   //was 20, raised so horizonal moving plats are a bit smoother, How much force to apply to get the dude moving
        private const float UNGROUNDED_DUDE_FORCE = 6.0f;
        private const float DUDE_DAMPING = 10.0f; // was 10, How hard the brakes are applied to get a dude to stop moving
        private const float DUDE_MAXSPEED = 6.0f; //was 6, Upper limit on dude left-right movement.  Does NOT apply to vertical movement.

        // Whether or not this dude is touching the ground
        private bool isGrounded;
        private bool isSloped;

        // Cooldown values
        private int jumpCooldown;

        // Lets us know which direction we're facing
        private bool facingRight;

        //animation stuff
        private Rectangle sourceRect;
        private Vector2 animOrigin, armOrigin;
        private float walkTimer;
        private float walkInterval;
        private int xFrame;
        private int yFrame;
        private int spriteWidth;
        private int spriteHeight;

        private int prevxFrame;
        private int prevyFrame;

        [NonSerialized]
        private Texture2D animTexture;
        protected string animTextureName;
        [NonSerialized]
        private Texture2D armTexture;
        protected string armTextureName;
        [NonSerialized]
        private Texture2D deadTexture;

        private int myGameTime;
        //private LaserObject myLaser; //for arm rotation only
        private float armAngle;

        ScrollingWorld myWorld;

        /**
         * Creates a new dude
         */
        //public DudeObject(World world, Texture2D texture, Texture2D objectTexture, Texture2D armTexture, string groundSensorName)
        public DudeObject(World world, ScrollingWorld theWorld, string texturename, string objectTexturename, string armTexturename, LaserObject Laser, string groundSensorName)
        : base(world, objectTexturename, DUDE_DENSITY, 0.0f, 0.0f, 1, false) //: base(world, texture, 1.0f, 0.0f, 0.0f)
        {
            this.armTextureName = armTexturename;
            this.animTextureName = texturename;

            Texture2D objectTexture = GameEngine.TextureList[objectTexturename];
            Texture2D texture = GameEngine.TextureList[texturename];
            Texture2D armTexture = GameEngine.TextureList[armTexturename];
            deadTexture = GameEngine.TextureList["Art\\failure_cosmo"];

            Height = objectTexture.Height;
            Width = objectTexture.Width;

            boundingBox = new Rectangle((int)(Position.X * CASSWorld.SCALE), (int)(Position.Y * CASSWorld.SCALE), (int)Height, (int)Width);

            amDead = false;

            // Initialize
            isGrounded = false;
            isSloped = false;

            TextureFilename = objectTexturename;
            // BodyDef options
            BodyDef.FixedRotation = true;

            // Make a dude controller
            controllers.Add(new DudeController());

            armAngle = 0;

            myWorld = theWorld;

            //animation stuff
            myGameTime = 0;
            animTexture = texture;
            this.armTexture = armTexture;
            walkTimer = 0;
            walkInterval = 5;
            xFrame = 0;
            yFrame = 0;
            spriteWidth = 100;
            spriteHeight = 100;
            sourceRect = new Rectangle(xFrame * spriteWidth, yFrame * spriteHeight, spriteWidth, spriteHeight);
            animOrigin = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);
            armOrigin = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2) - new Vector2(10,18);


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
            //Vector2 sensorCenter = new Vector2(0, halfHeight); 
            Vector2 sensorCenter = new Vector2(0, halfHeight - (5f / CASSWorld.SCALE)); 

            // Create collision shape of the ground sensor
            PolygonDef groundSensor = new PolygonDef();
            groundSensor.Density = 0.0f;
            groundSensor.IsSensor = true;
            groundSensor.UserData = groundSensorName;
            //groundSensor.SetAsBox(halfWidth*0.6f, halfHeight*0.5f, Utils.Convert(sensorCenter), 0);
            groundSensor.SetAsBox(2f/CASSWorld.SCALE, halfWidth/2f, Utils.Convert(sensorCenter), 0);
            shapes.Add(groundSensor);

            /* CIRCULAR GROUND SENSOR - just comment above out and uncomment this
            CircleDef groundSensor = new CircleDef();
            groundSensor.Radius = halfWidth * 0.95f;
            //groundSensor.Radius = (15f/2f) / CASSWorld.SCALE;
            groundSensor.LocalPosition = Utils.Convert(sensorCenter);
            groundSensor.UserData = groundSensorName;
            groundSensor.IsSensor = true;
            shapes.Add(groundSensor);
            */

            PolygonDef slopeSensor = new PolygonDef();
            slopeSensor.Density = 0.0f;
            slopeSensor.IsSensor = true;
            slopeSensor.UserData = groundSensorName + "SLOPE";
            slopeSensor.SetAsBox(halfWidth * 1.1f, 0.1f, Utils.Convert(sensorCenter + new Vector2(0, 0)), 0);
            shapes.Add(slopeSensor);

            lockDude = false;

            //animation stuff
            //base.(world, texture, 1.0f, 0.0f, 0.0f);
        }

        public void reloadNonSerializedAssets()
        {
            this.texture = GameEngine.TextureList[animTextureName];
            this.animTexture = GameEngine.TextureList[animTextureName];
            this.armTexture = GameEngine.TextureList[armTextureName];
            deadTexture = GameEngine.TextureList["Art\\failure_cosmo"];
            base.reloadNonSerializedAssets();
            lockDude = false;
        }



        public static void locked()
        {
            lockDude = true;
        }

        public void setDead()
        {
            amDead = true;
        }

        /**
         * Updates dude game logic - jumping cooldown
         */
        public override void Update(CASSWorld world, float dt)
        {
            
            // Apply cooldowns
            jumpCooldown = Math.Max(0, jumpCooldown - 1);

            MouseState mouse = Mouse.GetState();
            Vector2 start = myWorld.getScreenCoords(Position);
            Vector2 end = new Vector2(mouse.X,mouse.Y);
            //Console.WriteLine("{0} {1} {2} {3}", end.Y, start.Y, end.X, start.X);
            armAngle = (float)Math.Atan((end.Y - start.Y) / (end.X - start.X));
            //Console.WriteLine("{0}",armAngle);
            //animation stuff
            myGameTime++;
            sourceRect = new Rectangle(xFrame * spriteWidth, yFrame * spriteHeight, spriteWidth, spriteHeight);
            //origin = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);
            

            base.Update(world, dt);
        }

        /**
         * Draws the dude
         */
        public override void Draw(Matrix cameraTransform)
        {
            //animation stuff

            //sourceRect = new Rectangle(xFrame * spriteWidth, yFrame * spriteHeight, spriteWidth, spriteHeight);
            //origin = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);

            Vector2 screenOffset = (CASSWorld.SCALE * Position);
            SpriteEffects flip = facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            SpriteBatch spriteBatch = GameEngine.Instance.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default,
                              RasterizerState.CullCounterClockwise, null, cameraTransform);

            //Console.WriteLine("X {0} Y {1}",sourceRect.Width, sourceRect.Height);
            if (amDead)
            {
                spriteBatch.Draw(deadTexture, screenOffset, null, Color.White, 0, new Vector2(deadTexture.Width / 2, deadTexture.Height / 2), 1, SpriteEffects.None, 0);
            }
            else
            {
                //spriteBatch.Draw(animTexture, screenOffset, sourceRect, Color.White, Angle, origin, 1, flip, 0);
                spriteBatch.Draw(animTexture, screenOffset, sourceRect, Color.White, Angle, animOrigin, 1, flip, 0);

                //arm code
                if (facingRight)
                    spriteBatch.Draw(armTexture, screenOffset + new Vector2(-10, 0), null, Color.White, armAngle, armOrigin, .8f, flip, 0);
                else
                {
                    if (armAngle < -1.5) armAngle = -armAngle;//for corner cases
                    // if (armAngle > 1.5) armAngle = -armAngle;
                    spriteBatch.Draw(armTexture, screenOffset + new Vector2(0, 0), null, Color.White, armAngle, armOrigin, .8f, flip, 0);
                    // spriteBatch.Draw(armTexture, screenOffset + new Vector2(0, 0), null, Color.White, armAngle, armOrigin - new Vector2(-10, -10), .8f, flip, 0);
                }
                //Console.WriteLine("{0}", armAngle);
            }

            spriteBatch.End();


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

        /**
         * Is the dude on a surface at all or not?
         */
        public bool OnSlope
        {
            get
            {
                /*
                double angle = 0;
                angle = (Math.PI /2) - Math.Atan2(Math.Abs(Normal.Y), Math.Abs(Normal.X)); 
                //angle = (angle > Math.PI/2f) ? (Math.PI - angle) : angle ;
                //Console.WriteLine(angle / Math.PI);
                return Grounded && (angle < Constants.STEEPEST_SLOPE);
                */
                return isSloped;
            }
            set
            {
                isSloped = value;
            }
        }

        public Vector2 Normal
        {
            get { return normal; }
            set { normal = value; }
        }

        public bool isRight()
        {
            return facingRight;
        }

        public int getTime()
        {
            return myGameTime;
        }

        public ScrollingWorld getWorld()
        {
            return myWorld;
        }

        /**
         * Controller for a dude
         */
        [Serializable]
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
                
                KeyboardState ks = Keyboard.GetState();

                if ((ks.IsKeyDown(Keys.Left) || ks.IsKeyDown(Keys.A)) && !lockDude) //&& !CASSWorld.getFailed())
                {
               
                    
                    if (dudeObject.Grounded)
                    {
                        moveForce.X = -DUDE_FORCE;
                    }
                    else 
                    {
                        moveForce.X = -UNGROUNDED_DUDE_FORCE;
                    }

                    //dudeObject.walkAnimation(dudeObject.getTime());
                    /*
                        if (dudeObject.OnSlope) 
                        dudeObject.walkAnimation();
                        else
                        dudeObject.fallAnimation();
                    */
                    if (dudeObject.OnSlope)
                        dudeObject.walkAnimation();
                    else
                        dudeObject.fallAnimation();
                }
                else if ((ks.IsKeyDown(Keys.Right) || ks.IsKeyDown(Keys.D)) &&!lockDude) //&& !CASSWorld.getFailed())
                {

                    //moveForce.X += DUDE_FORCE;
                    if (dudeObject.Grounded)
                    {
                        moveForce.X = +DUDE_FORCE;
                    }
                    else 
                    {
                        moveForce.X = +UNGROUNDED_DUDE_FORCE;
                    }

                    //dudeObject.walkAnimation(dudeObject.getTime());
                    /*
                    if (dudeObject.OnSlope)
                        dudeObject.walkAnimation();
                    else
                        dudeObject.fallAnimation();
                    */
                    if (dudeObject.OnSlope)
                        dudeObject.walkAnimation();
                    else
                        dudeObject.fallAnimation();

                }
                else
                {
                    /*
                    if (dudeObject.OnSlope)
                        dudeObject.standAnimation();
                    else
                        dudeObject.fallAnimation();
                    */
                    if (dudeObject.OnSlope)
                        dudeObject.standAnimation();
                    else
                        dudeObject.fallAnimation();
                    
                }
                if (dudeObject.Body.GetLinearVelocity().X == 0 && dudeObject.Body.GetLinearVelocity().Y == 0)
                    dudeObject.standAnimation();


                if ((ks.IsKeyDown(Keys.Up) || ks.IsKeyDown(Keys.W)) && !lockDude)// && !CASSWorld.getFailed())
                    jump = true;
                // --------------------

                Vector2 vel = Utils.Convert(dude.GetLinearVelocity());

                // Which way are we facing  (add in a is X(mouse) > X(dude) condition??)
                //Vector2 screenOffset = (CASSWorld.SCALE * dudeObject.Position);
                
                /*
                int offsetX = (int)((dudeObject.Position.X * CASSWorld.SCALE) / 1024); 
                Console.WriteLine("{0}  {1}", offsetX, Mouse.GetState().X);
                if (dudeObject.Position.X * CASSWorld.SCALE < (Mouse.GetState().X + offsetX*1024))
                    dudeObject.facingRight = true;
                else if (dudeObject.Position.X * CASSWorld.SCALE >= (Mouse.GetState().X + offsetX *1024))
                    dudeObject.facingRight = false;
                 */

                //making him flip towards the cursor, works with sidescrolling, need to fix magic numbers
                if (dudeObject.Position.X * CASSWorld.SCALE >= (.5* GameEngine.SCREEN_WIDTH) && dudeObject.Position.X * CASSWorld.SCALE <= 3584)
                {
                    if ((.5 * GameEngine.SCREEN_WIDTH) < Mouse.GetState().X)
                        dudeObject.facingRight = true;
                    else if ((.5 * GameEngine.SCREEN_WIDTH) >= Mouse.GetState().X)
                        dudeObject.facingRight = false;
                }
                else
                {
                    float dudescreenX = (dudeObject.Position.X * CASSWorld.SCALE) % (GameEngine.SCREEN_WIDTH); 
                    if (dudescreenX < Mouse.GetState().X)
                        dudeObject.facingRight = true;
                    else if (dudescreenX >= Mouse.GetState().X)
                        dudeObject.facingRight = false;
                }



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
               // else
              //  {
                    dude.ApplyForce(Utils.Convert(moveForce), dude.GetPosition());
                   
                    
               // }

                // Jump!
                if (dudeObject.jumpCooldown == 0 && jump && dudeObject.Grounded)
                {
                    //animation stuff
                    //Vector2 impulse = new Vector2(0, -2.1f);
                    Vector2 impulse = new Vector2(0, -jumpImpulse);

                    //Vector2 impulse;
                    //double normalAngle = (Math.PI /2) - Math.Atan2(Math.Abs(dudeObject.Normal.Y), Math.Abs(dudeObject.Normal.X));

                    //Console.WriteLine(normalAngle * 180 / Math.PI); // DEBUG
                    // if slope is pretty shallow, just jump straight up
                    /*
                    if (normalAngle < Constants.FLAT_ENOUGH)
                    {
                        impulse = new Vector2(0, -jumpImpulse);
                        Console.WriteLine("STANDING ON FLAT GROUND"); // DEBUG
                    }
                    else
                    {
                        impulse = dudeObject.Normal * jumpImpulse;
                        Console.WriteLine((normalAngle / Math.PI) * 180f); // DEBUG
                        Console.WriteLine(dudeObject.Normal.Length()); // DEBUG
                    }
                    */

                    //dudeObject.Position += new Vector2(0.05f,-0.05f); // HACK HACK HACK
                    dude.ApplyImpulse(Utils.Convert(impulse), dude.GetPosition());
                    dudeObject.jumpCooldown = JUMP_COOLDOWN;
                }

            }
        }

        private void standAnimation()
        {
            xFrame = 0;
            yFrame = 2;
           // Console.WriteLine("Stand called");
            //myGameTime = 0;
        }

        private void fallAnimation()
        {
            prevxFrame = xFrame;
            prevyFrame = yFrame;
            xFrame = 2;
            yFrame = 2;
            // Console.WriteLine("Stand called");
            //myGameTime = 0;
        }

        //animation stuff
        private void walkAnimation()//int gameTime)
        {
            xFrame = prevxFrame;
            yFrame = prevyFrame;
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
                else if (yFrame == 2)
                {
                    xFrame = 0;
                    yFrame = 0;
                }

                // -= (int)walkInterval;
                myGameTime = 0;
                walkTimer = 0;

                prevxFrame = xFrame;
                prevyFrame = yFrame;

            }



        }
    }
}
