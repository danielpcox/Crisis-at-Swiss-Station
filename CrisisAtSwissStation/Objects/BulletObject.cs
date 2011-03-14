using Box2DX.Dynamics;
using Box2DX.Dynamics.Controllers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrisisAtSwissStation
{
    /**
     * A Bullet.  Bullets are unaffected by gravity
     * and can travel very fast.
     */
    public class BulletObject : CircleObject
    {
        /**
         * Creates a new bullet
         */
        public BulletObject(World world, Texture2D texture, float density, float friction, float restitution)
            : base(world, texture, density, friction, restitution)
        {
            BodyDef.IsBullet = true;

            controllers.Add(new BulletController(Utils.Convert(world.Gravity)));
        }

        /**
         * BulletController - applies negative gravity to cancel out the effect
         * of positive gravity.
         */
        private class BulletController : Controller
        {
            Vector2 gravity;

            public BulletController(Vector2 gravity)
            {
                this.gravity = gravity;
            }

            public override void Step(TimeStep step)
            {
                // Force = mass * gravity_constant
                Body bullet = this._bodyList.body;
                bullet.ApplyForce(Utils.Convert(-bullet.GetMass()*gravity), bullet.GetPosition());
            }
        }
    }
}