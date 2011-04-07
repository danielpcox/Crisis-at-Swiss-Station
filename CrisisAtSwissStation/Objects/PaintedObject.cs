using Box2DX.Common;
using Box2DX.Collision;
using Box2DX.Dynamics;
using Box2DX.Dynamics.Controllers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color; // Stupid Box2DX name collision!

using System;
using System.Resources;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;

namespace CrisisAtSwissStation
{
    /**
     * Defines an object drawn by the user with the mouse.
     */
    public class PaintedObject : CircleObject
    {
        // physical parameters for each blob in a PaintedObject
        private const float POB_DENSITY = 1.0f;
        private const float POB_FRICTION = 0.5f;
        private const float POB_RESTITUTION = 0.0f;
        private Color INSTASTEEL_COLOR = Color.Gray;

        List<Vec2> vertices = new List<Vec2>(); // we need this for drawing between the vertices

        Texture2D segmentTexture;

        int numBlobs = 0;

        float radius = 0f;

        /**
         * Creates a new drawn object
         */
        public PaintedObject(World world, Texture2D blobtexture, Texture2D segmenttexture, List<Vector2> blobs)
            : base(world, blobtexture, POB_DENSITY, POB_FRICTION, POB_RESTITUTION)
        {
          
            segmentTexture = segmenttexture;
            //Position = blobs[0] / CASSWorld.SCALE; // position of the painting is the first blob in it
            Position = blobs[0];
            radius = (float)blobtexture.Width / (2 * CASSWorld.SCALE);

            //foreach (Vector2 blobpos in blobs)
            for (int i=0; i<blobs.Count-1; i++)
            {
               // Vector2 localpos = (blobs[i] / CASSWorld.SCALE) - Position;
                //Vector2 localpos2 = (blobs[i+1] / CASSWorld.SCALE) - Position;
                Vector2 localpos = blobs[i]  - Position;
                Vector2 localpos2 = blobs[i+1]- Position;

                // add a circle fixture to this object at each point
                /*
                CircleDef circle = new CircleDef();
                circle.LocalPosition = Utils.Convert(localpos);
                circle.Radius = radius;
                circle.Density = POB_DENSITY;
                circle.Friction = POB_FRICTION;
                circle.Restitution = POB_RESTITUTION;
                shapes.Add(circle);
                */

                float scaledTextureWidth = segmentTexture.Width / CASSWorld.SCALE;

                Vector2 shapeVec = localpos2 - localpos;
                shapeVec.Normalize();
                Vector2 cornerOrtho = new Vector2(-shapeVec.Y, shapeVec.X);// * segmentTexture.Width / 2;
                // manually calculate the positions of each corner of the box/line between localpos and localpos2
                Vector2[] cornerpoints = new Vector2[] {               // if box were lying flat...
                    localpos - cornerOrtho * scaledTextureWidth / 2, // lower-left corner
                    localpos2 - cornerOrtho * scaledTextureWidth / 2, // lower-right corner
                    localpos2 + cornerOrtho * scaledTextureWidth / 2, // upper-right corner
                    localpos + cornerOrtho * scaledTextureWidth / 2 // upper-left corner
                };

                LinkedList<int> polygon = new LinkedList<int>();
                for (int j = 0; j < cornerpoints.Length; j++)
                    polygon.AddLast(j);
                // Triangles generated
                List<Vector2[]> triangles = new List<Vector2[]>();
                Split(polygon, cornerpoints, triangles);
                CreateShapes(triangles, POB_DENSITY, POB_FRICTION, POB_RESTITUTION);

                vertices.Add(Utils.Convert(localpos)); // that is, vertices of the curve approximation
                numBlobs++;
            }
            // add the last vertex
            Vector2 lastlocalpos = blobs[blobs.Count - 1]- Position;
            vertices.Add(Utils.Convert(lastlocalpos));
            numBlobs++;
        }

        public int Length
        {
            get
            {
                return numBlobs;
            }
            set
            {
                numBlobs = value;
            }
        }
        //Constructor for insta-steel generated in the level
        public PaintedObject(World world, Texture2D blobtexture, Texture2D segmenttexture, List<Vector2> blobs, int numBlobs)
            : base(world, blobtexture, POB_DENSITY, POB_FRICTION, POB_RESTITUTION)
        {
            this.numBlobs = numBlobs;
            segmentTexture = segmenttexture;
            //Position = blobs[0] / CASSWorld.SCALE; // position of the painting is the first blob in it
            Position = blobs[0];
            radius = (float)blobtexture.Width / (2 * CASSWorld.SCALE);

            //foreach (Vector2 blobpos in blobs)
            for (int i = 0; i < blobs.Count - 1; i++)
            {
                // Vector2 localpos = (blobs[i] / CASSWorld.SCALE) - Position;
                //Vector2 localpos2 = (blobs[i+1] / CASSWorld.SCALE) - Position;
                Vector2 localpos = blobs[i] - Position;
                Vector2 localpos2 = blobs[i + 1] - Position;

                // add a circle fixture to this object at each point
                /*
                CircleDef circle = new CircleDef();
                circle.LocalPosition = Utils.Convert(localpos);
                circle.Radius = radius;
                circle.Density = POB_DENSITY;
                circle.Friction = POB_FRICTION;
                circle.Restitution = POB_RESTITUTION;
                shapes.Add(circle);
                */

                float scaledTextureWidth = segmentTexture.Width / CASSWorld.SCALE;

                Vector2 shapeVec = localpos2 - localpos;
                shapeVec.Normalize();
                Vector2 cornerOrtho = new Vector2(-shapeVec.Y, shapeVec.X);// * segmentTexture.Width / 2;
                // manually calculate the positions of each corner of the box/line between localpos and localpos2
                Vector2[] cornerpoints = new Vector2[] {               // if box were lying flat...
                    localpos - cornerOrtho * scaledTextureWidth / 2, // lower-left corner
                    localpos2 - cornerOrtho * scaledTextureWidth / 2, // lower-right corner
                    localpos2 + cornerOrtho * scaledTextureWidth / 2, // upper-right corner
                    localpos + cornerOrtho * scaledTextureWidth / 2 // upper-left corner
                };

                LinkedList<int> polygon = new LinkedList<int>();
                for (int j = 0; j < cornerpoints.Length; j++)
                    polygon.AddLast(j);
                // Triangles generated
                List<Vector2[]> triangles = new List<Vector2[]>();
                Split(polygon, cornerpoints, triangles);
                CreateShapes(triangles, POB_DENSITY, POB_FRICTION, POB_RESTITUTION);

                vertices.Add(Utils.Convert(localpos)); // that is, vertices of the curve approximation
                numBlobs++;
            }
            // add the last vertex
            Vector2 lastlocalpos = blobs[blobs.Count - 1] - Position;
            vertices.Add(Utils.Convert(lastlocalpos));
            numBlobs++;
        }


        //Not fixed for side scrolling!!!!!!!!!!!!
        // DEBUG : so, Box2DX for some reason didn't want to add shapes to an object on the fly.
        // temporary solution: blast the old object and create a whole new one
        public void AddToShapes(List<Vector2> blobs)
        {
            //List<Vector2> totalblobs = new List<Vector2>();

            //foreach (CircleDef shape in shapes)
            //{
            //    totalblobs.Add(Utils.Convert(shape.LocalPosition));
            //}

            float radius = (float)texture.Width / (2 * CASSWorld.SCALE);

            foreach (Vector2 blobpos in blobs)
            {
                Vector2 localpos = (blobpos / CASSWorld.SCALE) - Position;

                // add a circle fixture to this object at each point
                CircleDef circle = new CircleDef();
                circle.LocalPosition = Utils.Convert(localpos);
                circle.Radius = radius;
                circle.Density = POB_DENSITY;
                circle.Friction = POB_FRICTION;
                circle.Restitution = POB_RESTITUTION;               
                shapes.Add(circle);
                Body.CreateShape(circle);
            }
            Body.SetMassFromShapes();
        }



        /**
         * Draws the painted object
         */
        public override void Draw(Matrix cameraTransform)
        {
            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;

            SpriteBatch spriteBatch = GameEngine.Instance.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default,
                              RasterizerState.CullCounterClockwise, null, cameraTransform);

            /*foreach (Vec2 blobpos in vertices)//(CircleDef blobpos in shapes)
            {
                Vector2 localpos = Utils.Convert(blobpos); //Utils.Convert(blobpos.LocalPosition);

                double theta = System.Math.Atan2(localpos.Y, localpos.X) + Angle;
                Vector2 rotatedpos = new Vector2((float)(localpos.Length() * System.Math.Cos(theta)), (float)(localpos.Length() * System.Math.Sin(theta)));                spriteBatch.Draw(texture, screenOffset, null, Color.White, Angle, origin, 1, 0, 0);
            }*/

            //DEBUG
            /*
            vertices = new List<Vec2>();
            foreach (CircleDef cd in shapes)
            {
                vertices.Add(cd.LocalPosition);
            }
            */

            for (int i = 0; i < vertices.Count - 1; i++)
            {
                Vector2 localpos = Utils.Convert(vertices[i]);
                Vector2 localpos2 = Utils.Convert(vertices[i + 1]);
                double theta = System.Math.Atan2(localpos.Y, localpos.X) + Angle;
                double theta2 = System.Math.Atan2(localpos2.Y, localpos2.X) + Angle;
                Vector2 rotatedpos = new Vector2((float)(localpos.Length() * System.Math.Cos(theta)), (float)(localpos.Length() * System.Math.Sin(theta)));
                Vector2 rotatedpos2 = new Vector2((float)(localpos2.Length() * System.Math.Cos(theta2)), (float)(localpos2.Length() * System.Math.Sin(theta2)));
                Vector2 screenpos = ((Position + rotatedpos) * CASSWorld.SCALE);
                Vector2 screenpos2 = ((Position + rotatedpos2) * CASSWorld.SCALE);
                // Draw blob at point
                spriteBatch.Draw(texture, screenpos, null, INSTASTEEL_COLOR, Angle, origin, 1, 0, 0);

                // Draw segment between points
                CrisisAtSwissStation.Utils.DrawLine(spriteBatch, segmentTexture, screenpos, screenpos2, INSTASTEEL_COLOR);
            }
            // Draw blob at last point
            Vector2 lastlocalpos = Utils.Convert(vertices[vertices.Count - 1]);
            double lasttheta = System.Math.Atan2(lastlocalpos.Y, lastlocalpos.X) + Angle;
            Vector2 lastrotatedpos = new Vector2((float)(lastlocalpos.Length() * System.Math.Cos(lasttheta)), (float)(lastlocalpos.Length() * System.Math.Sin(lasttheta)));
            Vector2 lastscreenpos = ((Position + lastrotatedpos) * CASSWorld.SCALE);
            // Draw blob at point
            spriteBatch.Draw(texture, lastscreenpos, null, INSTASTEEL_COLOR, Angle, origin, 1, 0, 0);

            spriteBatch.End();
        }

        public int getNumBlobs()
        {
            return numBlobs;
        }

        /**
         * Creates a ShapeDef for each triangle
         */
        private void CreateShapes(List<Vector2[]> triangles, float density, float friction, float restitution)
        {
            foreach (Vector2[] triangle in triangles)
            {
                PolygonDef polygon = new PolygonDef();
                polygon.Vertices[0] = Utils.Convert(triangle[0]);
                polygon.Vertices[1] = Utils.Convert(triangle[1]);
                polygon.Vertices[2] = Utils.Convert(triangle[2]);
                polygon.VertexCount = 3;
                polygon.Density = density;
                polygon.Friction = friction;
                polygon.Restitution = restitution;
                shapes.Add(polygon);
            }
        }

        private float GetColinearity(Vector2 testPoint, Vector2 origin, Vector2 direction)
        {
            Vector2 dir = testPoint - origin;
            dir.Normalize();

            return System.Math.Abs(Vector2.Dot(dir, direction));
        }

        private void Split(LinkedList<int> polygon, Vector2[] points, List<Vector2[]> triangles)
        {
            // It's a triangle - make it and terminate recursion
            if (polygon.Count == 3)
            {
                Vector2[] v = new Vector2[3];
                v[0] = points[polygon.First.Value];
                v[1] = points[polygon.First.Next.Value];
                v[2] = points[polygon.First.Next.Next.Value];
                triangles.Add(v);
                return;
            }

            float smallestColinearity = float.MaxValue; // Colinearity measure for best diagonal
            LinkedListNode<int> diag1 = null; // Points in the diagonal
            LinkedListNode<int> diag2 = null;

            // Look at all diagonals in the polygon
            for (LinkedListNode<int> i1 = polygon.First; i1.Next != null; i1 = i1.Next)
            {
                Vector3 testEdge1 = new Vector3(points[i1.Next.Value] - points[i1.Value], 0);
                LinkedListNode<int> prev = i1.Previous;
                if (prev == null)
                    prev = polygon.Last;
                Vector3 testEdge2 = new Vector3(points[prev.Value] - points[i1.Value], 0);

                for (LinkedListNode<int> i2 = i1.Next.Next; i2 != null; i2 = i2.Next)
                {
                    Vector2 p1 = points[i1.Value];
                    Vector2 p2 = points[i2.Value];

                    // Indicates that the segment travels through
                    //   some empty space, so it's not a diagonal edge
                    Vector3 edge = new Vector3(p2 - p1, 0);
                    if (Vector3.Cross(testEdge1, edge).Z <= 0)
                        continue;
                    if (Vector3.Cross(edge, testEdge2).Z <= 0)
                        continue;

                    // If no polygon edge intersects our edge, 
                    //   then our edge is guaranteed to be a diagonal
                    //   find out whether or not this is a diagonal
                    bool isDiagonal = true;
                    for (LinkedListNode<int> i3 = polygon.First; isDiagonal && i3 != null; i3 = i3.Next)
                    {
                        LinkedListNode<int> i4 = i3.Next;
                        if (i4 == null)
                            i4 = polygon.First;

                        // Ignore edges eminating from one of our vertices
                        if (i3 == i1 || i3 == i2 || i4 == i1 || i4 == i2)
                            continue;

                        Vector2 p3 = points[i3.Value];
                        Vector2 p4 = points[i4.Value];

                        // Test for intersection
                        Vector2 d1 = p2 - p1;
                        Vector2 d2 = p4 - p3;

                        Vector2 b = p3 - p1;
                        float d = d1.X * d2.Y - d2.X * d1.Y;
                        if (System.Math.Abs(d) < Utils.EPSILON)
                            continue;
                        d = 1.0f / d;

                        float t = d2.Y * b.X - d2.X * b.Y;
                        float s = d1.Y * b.X - d1.X * b.Y;
                        t *= d;
                        s *= d;

                        // Just found an intersection - this is not a diagonal!
                        if (s > 0 && s < 1 && t > 0 && t < 1)
                            isDiagonal = false;
                    }

                    // Don't consider it if it's not a diagonal
                    if (!isDiagonal)
                        continue;

                    Vector2 dir = p1 - p2;
                    dir.Normalize();

                    // Measure colinearity with nearby points
                    float ourColinearity = 0;

                    LinkedListNode<int> t1 = i1.Previous;
                    if (t1 == null) t1 = polygon.Last;
                    ourColinearity = System.Math.Max(ourColinearity, System.Math.Abs(GetColinearity(points[t1.Value], p1, dir)));

                    LinkedListNode<int> t2 = i1.Next;
                    if (t2 == null) t2 = polygon.First;
                    ourColinearity = System.Math.Max(ourColinearity, GetColinearity(points[t2.Value], p1, dir));

                    LinkedListNode<int> t3 = i2.Previous;
                    if (t3 == null) t3 = polygon.Last;
                    ourColinearity = System.Math.Max(ourColinearity, GetColinearity(points[t3.Value], p2, dir));

                    LinkedListNode<int> t4 = i2.Next;
                    if (t4 == null) t4 = polygon.First;
                    ourColinearity = System.Math.Max(ourColinearity, GetColinearity(points[t4.Value], p2, dir));

                    // If our colinearity is smaller than our best bet, make
                    //   us into the new best bet!
                    if (ourColinearity < smallestColinearity)
                    {
                        smallestColinearity = ourColinearity;
                        diag1 = i1;
                        diag2 = i2;
                    }
                }
            }

            Debug.Assert(diag1 != null);

            LinkedList<int> left = new LinkedList<int>();
            for (LinkedListNode<int> i = diag1; i != diag2; i = (i.Next != null) ? i.Next : polygon.First)
                left.AddLast(i.Value);
            left.AddLast(diag2.Value);

            LinkedList<int> right = new LinkedList<int>();
            for (LinkedListNode<int> i = diag2; i != diag1; i = (i.Next != null) ? i.Next : polygon.First)
                right.AddLast(i.Value);
            right.AddLast(diag1.Value);

            Split(left, points, triangles);
            Split(right, points, triangles);
        }
        
    }
}