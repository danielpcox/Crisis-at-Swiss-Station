using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;

namespace CrisisAtSwissStation
{
    public abstract class CASSWorld
    {
        // Scale from game space -> screen space
        public const float SCALE = 50.0f;

        // Amount of offgame area covered by the world AABB
        protected const float MARGIN = 2.0f;

        // All the objects in the world
        private List<PhysicsObject> objects = new List<PhysicsObject>();
        private List<PhysicsObject> tempObjects = new List<PhysicsObject>();

        // The Box2D world
        private World world;

        // Have we won yet?
        private bool succeeded;
        private bool failed;

        public CASSWorld(float width, float height, Vector2 gravity)
        {
            // Create the world's axis-aligned bounding box
            AABB aabb = new AABB();
            aabb.LowerBound = new Vec2(-MARGIN, -MARGIN);
            aabb.UpperBound = new Vec2(width + MARGIN, height + MARGIN);

            world = new World(aabb, Utils.Convert(gravity), true);

            succeeded = failed = false;
        }

        public void AddObject(PhysicsObject obj)
        {
            objects.Add(obj);
            obj.AddToWorld();
            obj.SetupJoints(world);
        }

        public void RemoveObject(PhysicsObject obj)
        {
            objects.Remove(obj);
            obj.RemoveFromWorld();
        }

        public virtual void Simulate(float dt)
        {
            int iterations = 10;

            world.Step(dt, iterations, iterations);
            
            // Update game logic
            tempObjects.AddRange(objects);
            foreach (PhysicsObject obj in tempObjects)
            {
                if (obj.Dead)
                    RemoveObject(obj);
                else
                    obj.Update(this, dt);
            }
            tempObjects.Clear();
        }

        /**
         * Draws all objects in the physics world
         */
        public virtual void Draw(GraphicsDevice device, Vector3 eye, Matrix view, Matrix proj)
        {
            Vector2 offset = new Vector2(0, 0);
            foreach (PhysicsObject obj in objects)
                obj.Draw(offset);
        }

        /**
         * Returns the location to of what should be
         * the center of the visible screen.
         */
        public virtual Vector2 GetVisualTargetPosition()
        {
            return new Vector2(0, 0);
        }

        /**
         * Report that the player has won the level
         */
        public void Win()
        {
            succeeded = true;
        }

        /**
         * Report that the player has died/lost
         */
        public void Fail()
        {
            failed = true;
        }

        /**
         * Gets the Box2DX physics world
         */
        public World World
        { get { return world; } }

        /**
         * Whether or not we've beaten this world
         */
        public bool Succeeded
        { get { return succeeded; } }

        /**
         * Whether or not we died trying
         */
        public bool Failed
        { get { return failed; } }
    }
}