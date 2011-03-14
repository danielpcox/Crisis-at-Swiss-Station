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
        private const int JUMP_COOLDOWN = 30;
        private const int SHOOT_COOLDOWN = 40;
        private const float BULLET_SPEED = 20.0f;

        private const float DUDE_FORCE = 20.0f;   // How much force to apply to get the dude moving
        private const float DUDE_DAMPING = 10.0f; // How hard the brakes are applied to get a dude to stop moving
        private const float DUDE_MAXSPEED = 6.0f; // Upper limit on dude left-right movement.  Does NOT apply to vertical movement.

        // Texture of the bullets we shoot
        private Texture2D bulletTexture;

        // Whether or not this dude is touching the ground
        private bool isGrounded;

        // Cooldown values
        private int shootCooldown;
        private int jumpCooldown;

        // Lets us know which direction we're facing
        private bool facingRight;

        /**
         * Creates a new dude
         */
        public DudeObject(World world, Texture2D texture, Texture2D bulletTexture, string groundSensorName)
            : base(world, texture, 1.0f, 0.0f, 0.0f)
        {
            // Initialize
            isGrounded = false;
            this.bulletTexture = bulletTexture;

            // BodyDef options
            BodyDef.FixedRotation = true;

            // Make a dude controller
            controllers.Add(new DudeController());

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

            // Compute dimensions of the ground sensor
            float halfWidth = (float)texture.Width / (2 * CASSWorld.SCALE);
            float halfHeight = (float)texture.Height / (2 * CASSWorld.SCALE);
            Vector2 sensorCenter = new Vector2(0, halfHeight);

            // Create collision shape of the ground sensor
            PolygonDef groundSensor = new PolygonDef();
            groundSensor.Density = 1.0f;
            groundSensor.IsSensor = true;
            groundSensor.UserData = groundSensorName;
            groundSensor.SetAsBox(halfWidth / 2, 0.05f, Utils.Convert(sensorCenter), 0);
            shapes.Add(groundSensor);
        }

        /**
         * Updates dude game logic - shooting, jumping cooldowns
         */
        public override void Update(CASSWorld world, float dt)
        {
            /*
            // Just fired a shot
            if (shootCooldown == SHOOT_COOLDOWN)
            {
                BulletObject bullet = new BulletObject(world.World, bulletTexture, 10.0f, 0, 0);
                float offset = (texture.Width + bulletTexture.Width + 1) / (2 * DemoWorld.SCALE);
                float speed = BULLET_SPEED;
                if (!facingRight)
                {
                    offset = -offset;
                    speed = -speed;
                }

                bullet.Position = Position + new Vector2(offset, 0);
                world.AddObject(bullet);
                bullet.Body.SetLinearVelocity(Utils.Convert(new Vector2(speed, 0)));
            }
            */
            // Apply cooldowns
            shootCooldown = Math.Max(0, shootCooldown - 1);
            jumpCooldown = Math.Max(0, jumpCooldown - 1);


            base.Update(world, dt);
        }

        /**
         * Draws the dude
         */
        public override void Draw(Vector2 offset)
        {
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;
            Vector2 screenOffset = (CASSWorld.SCALE * Position) - offset;

            SpriteEffects flip = facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            SpriteBatch spriteBatch = GameEngine.Instance.SpriteBatch;
            spriteBatch.Begin();

            spriteBatch.Draw(texture, screenOffset, null, Color.White, Angle, origin, 1, flip, 0);

            spriteBatch.End();
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
                bool shoot = false;

                // TODO: XBox controls
                // --------------------
                KeyboardState ks = Keyboard.GetState();
                if (ks.IsKeyDown(Keys.Left) || ks.IsKeyDown(Keys.A))
                    moveForce.X -= DUDE_FORCE;
                else if (ks.IsKeyDown(Keys.Right) || ks.IsKeyDown(Keys.D))
                    moveForce.X += DUDE_FORCE;
                if (ks.IsKeyDown(Keys.Space))
                    shoot = true;
                if (ks.IsKeyDown(Keys.Up) || ks.IsKeyDown(Keys.W))
                    jump = true;
                // --------------------

                Vector2 vel = Utils.Convert(dude.GetLinearVelocity());

                // Which way are we facing
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
                    Vector2 impulse = new Vector2(0, -2.1f);
                    dude.ApplyImpulse(Utils.Convert(impulse), dude.GetPosition());
                    dudeObject.jumpCooldown = JUMP_COOLDOWN;
                }

                // Shoot!
                if (dudeObject.shootCooldown == 0 && shoot)
                {
                    dudeObject.shootCooldown = SHOOT_COOLDOWN;
                }
            }
        }
    }
}