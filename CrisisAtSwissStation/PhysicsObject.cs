using System;
using Box2DX.Collision;
using Box2DX.Dynamics;
using Box2DX.Dynamics.Controllers;

using Microsoft.Xna.Framework;

using System.Collections.Generic;

using CrisisAtSwissStation.Common;

namespace CrisisAtSwissStation
{
    [Serializable]
    public abstract class PhysicsObject
    {
        // The world we live in
        private World world;

        // Our body definition
        private BodyDef bodyDef;

        // Our body
        private Body body;

        // Our shapes
        public List<ShapeDef> shapes = new List<ShapeDef>();

        // Our controllers
        protected List<Controller> controllers = new List<Controller>();

        // Our children
        protected List<PhysicsObject> children = new List<PhysicsObject>();

        // Are we dead?
        private bool isDead;

        float width = 10f;
        float height = 10f; // 10 for DEBUGging purposes
        public float scale = 1f;

        string textureFilename = null;
        protected Rectangle boundingBox;

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
        
        public float Height
        {
            get
            {
                return this.height;
            }
            set
            {
                this.height = value;
            }
        }

        public float Width
        {
            get
            {
                return this.width;
            }
            set
            {
                this.width = value;
            }
        }

        public string TextureFilename
        {
            get
            {
                return textureFilename;
            }
            set
            {
                textureFilename = value;
            }
        }

        public Point getCenterPoint(){
            Point center = getBBRelativeToWorld().Center;

            return (new Point(center.X, center.Y));
        }

        /**
         * Explicitly sets the body's position and rotation
         */
        public void SetTransform(Vector2 position, float rotation)
        {
            bodyDef.Position = Utils.Convert(position);
            bodyDef.Angle = rotation;
            if (body != null)
            {
                body.SetXForm(Utils.Convert(position), rotation);
            }
        }

        //Maps a point on the original image to a point in the world. Takes into account rotation and scaling.
        //Scale. Rotate. Translate.
        public Vector2 mapPointOnImage(Vector2 point)
        {

            Vector2 newPoint = new Vector2(point.X, point.Y);

            //Scale
            //newPoint = newPoint * ScaleVector;

            //The next thing we do is rotate. Since the origin for the rotation is the center, we need to 
            //untranslate the image (we did this in the line above), and then retranslate it to the center.
            //float cos_theta = (float)System.Math.Cos(Angle);
            //float sin_theta = (float)System.Math.Sin(Angle);

            Vector2 center = new Vector2(boundingBox.Center.X*scale, boundingBox.Center.Y*scale);
            newPoint = newPoint - center;
            
            //unscale before rotating
            newPoint = newPoint / scale;

            double theta = System.Math.Atan2(newPoint.Y, newPoint.X) + Angle;
            Vector2 rotatedpos = new Vector2((float)(newPoint.Length() * System.Math.Cos(theta)), (float)(newPoint.Length() * System.Math.Sin(theta)));
            newPoint = rotatedpos;

            //rescale after rotating
            newPoint = newPoint * scale;

            /*
            newPoint = new Vector2(
                newPoint.X * cos_theta - newPoint.Y * sin_theta,
                newPoint.X * sin_theta + newPoint.Y * cos_theta
                );
             * */

            newPoint = newPoint + center;


            newPoint = newPoint + (Position * CASSWorld.SCALE);

            return newPoint;
        }


        //A convience function for the above function.
        public Vector2 mapPointOnImage(float x, float y)
        {
            return (mapPointOnImage(new Vector2(x, y)));
        }


        //The stretch in the horizontal direction. Scales the y-axis of the bounding box
        protected float horizontalScale = 1.0f;

        //The axis-aligned bounding box of the sprite. This provides a very loose bound for the sprite object.

        public Rectangle getBBRelativeToWorld()
        {

            List<Vector2> newPts = new List<Vector2>();

            Vector2 halfDim = new Vector2(Width / 2, Height / 2); // this is a hack to place the objects at their center correctly
            
            newPts.Add(mapPointOnImage(boundingBox.X, boundingBox.Y) - halfDim);
            newPts.Add(mapPointOnImage(boundingBox.X, Height + boundingBox.Y) - halfDim);
            newPts.Add(mapPointOnImage(Width + boundingBox.X, boundingBox.Y) - halfDim);
            newPts.Add(mapPointOnImage(Width + boundingBox.X, Height + boundingBox.Y) - halfDim);

            float max_y = newPts[0].Y;
            float min_y = newPts[0].Y;
            float max_x = newPts[0].X;
            float min_x = newPts[0].X;

            foreach (Vector2 pt in newPts)
            {
                if (pt.X > max_x)
                {
                    max_x = pt.X;
                }
                else if (pt.X < min_x)
                {
                    min_x = pt.X;
                }

                if (pt.Y > max_y)
                {
                    max_y = pt.Y;
                }
                else if (pt.Y < min_y)
                {
                    min_y = pt.Y;
                }
            }

            min_x -= boundingBox.X; // *horizontalScale;
            min_y -= boundingBox.Y; // *verticalScale;
            max_x -= boundingBox.X; // *horizontalScale;
            max_y -= boundingBox.Y; // *verticalScale;

            return (new Rectangle(
                (int)min_x,
                (int)min_y,
                (int)(max_x - min_x),
                (int)(max_y - min_y)));
        }
    }
}