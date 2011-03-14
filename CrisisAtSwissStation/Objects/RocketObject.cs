using Box2DX.Dynamics;
using Box2DX.Dynamics.Controllers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Diagnostics;

namespace CrisisAtSwissStation
{
    /**
     * Defines a rocket
     */
    public class RocketObject : BoxObject
    {
        int animationTimer = 0;
        bool timerDirection = true;
        Texture2D flameEffect;

        public RocketObject(World world, Texture2D texture, Texture2D effect)
            : base(world, texture, 1.0f, 0.1f, 0.4f)
        {
            controllers.Add(new RocketController());
            flameEffect = effect;

            //    Prevent body from rotating
            ////////////////////////////////////

            BodyDef.FixedRotation = true;

            ////////////////////////////////////

        }

        /**
         * Controller for a rocket
         */
        private class RocketController : Controller
        {

            public RocketController()
            {
            }

            public override void Step(TimeStep step)
            {
                Body rocket = _bodyList.body;
                Debug.Assert(rocket != null);

                Vector2 force = new Vector2();

                KeyboardState ks = Keyboard.GetState();

                //   Add force to X-component based on whether left/right
                //   arrows are pressed.  Add force to the Y-component if
                //   up is pressed.
                ///////////////////////////////////////////////

                if (ks.IsKeyDown(Keys.Up))
                    force.Y -= 20;
                if (ks.IsKeyDown(Keys.Left))
                    force.X -= 5;
                else if (ks.IsKeyDown(Keys.Right))
                    force.X += 5;

                ///////////////////////////////////////////////

                force = Vector2.Transform(force, Matrix.CreateRotationZ(rocket.GetAngle()));

                //   Apply force to the rocket
                ///////////////////////////////////////////////

                rocket.ApplyForce(new Box2DX.Common.Vec2(force.X, force.Y), rocket.GetPosition());

                ///////////////////////////////////////////////
            }
        }

        public override void Draw(Vector2 offset)
        {
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;

            // Picks which frame of the rocket's fire effect to use
            if (Body.GetLinearVelocity().Y < -1)
            {
                if (animationTimer == 0)
                    animationTimer = 1;
                if (animationTimer == 3)
                    timerDirection = false;
                if (animationTimer == 1)
                    timerDirection = true;

                if (timerDirection)
                    animationTimer++;
                else
                    animationTimer--;
            }
            else
            {
                animationTimer = 0;
            }

            Rectangle sourceRectangle = new Rectangle(0 + (animationTimer * 55), 0, 50, flameEffect.Height);

            SpriteBatch spriteBatch = GameEngine.Instance.SpriteBatch;
            spriteBatch.Begin();

            spriteBatch.Draw(texture, DemoWorld.SCALE * Position, null, Microsoft.Xna.Framework.Color.White, Angle, origin, 1, SpriteEffects.None, 0);
            spriteBatch.Draw(flameEffect, DemoWorld.SCALE * Position, sourceRectangle, Microsoft.Xna.Framework.Color.White, Angle, origin, 1, SpriteEffects.None, 0);

            spriteBatch.End();
        }
    }
}