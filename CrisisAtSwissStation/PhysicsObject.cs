using Box2DX.Collision;
using Box2DX.Dynamics;
using Box2DX.Dynamics.Controllers;

using Microsoft.Xna.Framework;

using System.Collections.Generic;

namespace CrisisAtSwissStation
{
    public abstract class PhysicsObject
    {
        // The world we live in
        private World world;

        // Our body definition
        private BodyDef bodyDef;

        // Our body
        private Body body;

        // Our shapes
        protected List<ShapeDef> shapes = new List<ShapeDef>();

        // Our controllers
        protected List<Controller> controllers = new List<Controller>();

        // Our children
        protected List<PhysicsObject> children = new List<PhysicsObject>();

        // Are we dead?
        private bool isDead;

        /**
         * Creates a new physics object
         */
        public PhysicsObject(World world)
        {
            this.world = world;
            isDead = false;
            bodyDef = new BodyDef();
        }

        /**
         * Draws the object
         */
        public virtual void Draw(Matrix cameraTransform)
        {
            foreach (PhysicsObject child in children)
                child.Draw(cameraTransform);
        }

        /**
         * Updates the object's game logic.
         * objects by default have no game logic significance.
         */
        public virtual void Update(CASSWorld world, float dt)
        {
            foreach (PhysicsObject child in children)
                child.Update(world, dt);
        }

        public void AddToWorld()
        {
            body = world.CreateBody(bodyDef);
            foreach (ShapeDef shape in shapes)
                body.CreateShape(shape);
            body.SetMassFromShapes();
            body.SetUserData(this);

            foreach (Controller controller in controllers)
            {
                controller.AddBody(body);
                world.AddController(controller);
            }

            foreach (PhysicsObject child in children)
                child.AddToWorld();
        }

        public void RemoveFromWorld()
        {
            foreach (PhysicsObject child in children)
                child.RemoveFromWorld();

            world.DestroyBody(body);
        }

        public void Die()
        {
            isDead = true;
        }

        public virtual void SetupJoints(World world)
        {
            foreach (PhysicsObject child in children)
                child.SetupJoints(world);
        }

        protected BodyDef BodyDef
        { get { return bodyDef; } }

        public Body Body
        {
            get { return body; }
        }

        public bool Dead
        { get { return isDead; } }

        /**
         * Gets or sets the physics object's position
         */
        public Vector2 Position
        {
            get
            {
                if (body == null)
                    return Utils.Convert(bodyDef.Position);
                return Utils.Convert(body.GetPosition());
            }
            set
            {
                SetTransform(value, Angle);
            }
        }

        /**
         * Gets or sets the physics object's angle
         */
        public float Angle
        {
            get
            {
                if (body == null)
                    return bodyDef.Angle;
                return body.GetAngle();
            }
            set
            {
                SetTransform(Position, value);
            }
        }

        /**
         * Explicitly sets the body's position and rotation
         */
        public void SetTransform(Vector2 position, float rotation)
        {
            if (body == null)
            {
                bodyDef.Position = Utils.Convert(position);
                bodyDef.Angle = rotation;
            }
            else
                body.SetXForm(Utils.Convert(position), rotation);
        }
    }
}