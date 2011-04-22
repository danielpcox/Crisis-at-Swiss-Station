using Box2DX.Common;
using Box2DX.Collision;
using Box2DX.Dynamics;
using Box2DX.Dynamics.Controllers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color; // Stupid Box2DX name collision!
using CrisisAtSwissStation.Common;

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
    [Serializable]
    public class InstasteelObject : PaintedObject
    {
        protected static string blobtexturename ="paint";
        protected static string segmenttexturename = "paintsegment";
        protected static List<Vector2> blobs = new List<Vector2>();

        /**
         * Creates a new drawn object
         */
        public InstasteelObject(World world, string textureName, float amountofis, float density, float friction, float restitution, float myScale, bool isPulley)
            : base(world, blobtexturename, segmenttexturename, blobs)
        {

            amountOfInstasteel = amountofis;

            this.texture = GameEngine.TextureList[textureName];
            TextureFilename = textureName;
            Height = texture.Height * myScale;
            Width = texture.Width * myScale;
            boundingBox = new Rectangle((int)Position.X, (int)Position.Y, (int)Width, (int)Height);

            scale = myScale;

            if (isPulley)
                BodyDef.FixedRotation = true;
            
            // Determine dimensions
            float halfWidth = (float)texture.Width / (2 * CASSWorld.SCALE) * scale ;
            float halfHeight = (float)texture.Height / (2 * CASSWorld.SCALE) * scale;

            // Create the collision shape
            PolygonDef shape = new PolygonDef();
            shape.SetAsBox(halfWidth, halfHeight);
            shape.Density = density;
            shape.Friction = friction;
            shape.Restitution = restitution;
            shapes.Add(shape);
        }
        
        public void reloadNonSerializedAssets()
        {
            this.texture = GameEngine.TextureList[TextureFilename];
            amountOfInstasteel = amountOfInstasteel * (float)System.Math.Pow(scale,2);
        }

        public override void Draw(Matrix cameraTransform)
        {
            //base.Draw(cameraTransform);

            Vector2 origin = new Vector2(texture.Width, texture.Height) / 2;
           
            SpriteBatch spriteBatch = GameEngine.Instance.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default,
                              RasterizerState.CullCounterClockwise, null, cameraTransform);

            spriteBatch.Draw(texture, CASSWorld.SCALE * Position, null, Color.White, Angle, origin, scale, SpriteEffects.None, 0);

            spriteBatch.End();
        }
        public float getAmountOfInstasteel()
        {
            return amountOfInstasteel;
        }

        /**
         * Creates a ShapeDef for each triangle
         */
        private void CreateShapes(List<Vector2[]> triangles, float density, float friction, float restitution)
        {
            foreach (Vector2[] triangle in triangles)
            {
                PolygonDef polygon = new PolygonDef();
                polygon.Vertices[0] = Common.Utils.Convert(triangle[0]);
                polygon.Vertices[1] = Common.Utils.Convert(triangle[1]);
                polygon.Vertices[2] = Common.Utils.Convert(triangle[2]);
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
                        if (System.Math.Abs(d) < Common.Utils.EPSILON)
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
