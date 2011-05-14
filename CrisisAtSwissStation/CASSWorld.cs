using System;
using Box2DX.Collision;
using Box2DX.Common;
using Box2DX.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;
using CrisisAtSwissStation.Common;

namespace CrisisAtSwissStation
{
    [Serializable]
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
            Objects.Add(obj);
            obj.AddToWorld();
            obj.SetupJoints(world);
        }

        public void RemoveObject(PhysicsObject obj)
        {
            Objects.Remove(obj);
            obj.RemoveFromWorld();
        }

        public List<PhysicsObject> Objects
        {
            get
            {
                return objects;
            }
            set
            {
                objects = value;
            }
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
        public virtual void Draw(GraphicsDevice device, Matrix cameraTransform)
        {
            foreach (PhysicsObject obj in objects)
            {
                if (obj is BackgroundObject)
                    obj.Draw(cameraTransform);
                if (obj is WinDoorObject) obj.Draw(cameraTransform);
                if (obj is PaintedObject)
                    obj.Draw(cameraTransform);
            }
            foreach (PhysicsObject obj in objects)
            {
                if (!(obj is BackgroundObject) && !(obj is PaintedObject) && !(obj is WinDoorObject))
                    obj.Draw(cameraTransform);
            }
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
            if (succeeded == false)
            {
                DudeObject.locked();
                failed = true;
            }
        }

        /**
         * Gets the Box2DX physics world
         */
        public World World
        {
            get { return world; }
            set { world = value; }
        }

        /**
         * Whether or not we've beaten this world
         */
        public bool Succeeded
        {
            get { return succeeded; }
            set
            {
                succeeded = value;
            }
        }

        /**
         * Whether or not we died trying
         */
        public bool Failed
        { get { return failed; } }

        public void reloadNonSerializedAssets()
        {
            foreach (PhysicsObject obj in objects)
            {

                if (obj is BoxObject)
                {
                    ((BoxObject)obj).reloadNonSerializedAssets();
                }
                else if (obj is PolygonObject)
                {
                    ((PolygonObject)obj).reloadNonSerializedAssets();
                }
                else if (obj is CircleObject)
                {
                    ((CircleObject)obj).reloadNonSerializedAssets();
                }

                if (obj is DudeObject)
                {
                    ((DudeObject)obj).reloadNonSerializedAssets();
                }
                else if (obj is InstasteelObject)
                {
                    ((InstasteelObject)obj).reloadNonSerializedAssets();
                }
                else if (obj is InstasteelCircleObject)
                {
                    ((InstasteelCircleObject)obj).reloadNonSerializedAssets();
                }
                else if (obj is PaintedObject)
                {
                    ((PaintedObject)obj).reloadNonSerializedAssets();
                }
                else if (obj is SensorObject)
                {
                    ((SensorObject)obj).reloadNonSerializedAssets();
                }
                
                else if (obj is AnimationObject)
                {
                    ((AnimationObject)obj).reloadNonSerializedAssets();
                }
                else if (obj is MovingObject)
                {
                    ((MovingObject)obj).reloadNonSerializedAssets();
                }
                else if (obj is HorizontalMovingObject)
                {
                    ((HorizontalMovingObject)obj).reloadNonSerializedAssets();
                }
                else if (obj is SwitchObject)
                {
                    ((SwitchObject)obj).reloadNonSerializedAssets();
                }
                else if (obj is PistonObject)
                {
                    ((PistonObject)obj).reloadNonSerializedAssets();
                }
                else if (obj is WinDoorObject)
                {
                    ((WinDoorObject)obj).reloadNonSerializedAssets();
                }
                else if (obj is DeathPlatform)
                {
                    ((DeathPlatform)obj).reloadNonSerializedAssets();
                }
                else if (obj is FailButtonObject)
                {
                    ((FailButtonObject)obj).reloadNonSerializedAssets();
                }
                else if (obj is HoleObject)
                {
                    ((HoleObject)obj).reloadNonSerializedAssets();
                }
            }

            // misc
            if (this is ScrollingWorld)
            {
                ((ScrollingWorld)this).laser.reloadNonSerializedAssets();
                ((ScrollingWorld)this).reloadNonSerializedAssets();
            }
        }

    }
}
