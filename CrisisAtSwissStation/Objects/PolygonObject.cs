using Box2DX.Collision;
using Box2DX.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using CrisisAtSwissStation.Common;

namespace CrisisAtSwissStation
{
    public class PolygonObject : PhysicsObject
    {
        // Drawable vertices
        VertexPositionTexture[] vertices;

        // Texture
        Texture2D texture;

        /**
         * Creates a new polygon object.
         * Uses a divide-and-conquer algorithm
         * to triangulate the polygon, since triangles
         * are guaranteed to be convex, a Box2DX
         * requirement.  Colinear consecutive points
         * are okay, but the polygon must be simple -
         * i.e. no crossing edges.
         */
        public PolygonObject(World world, Vector2[] points, Texture2D texture, float density, float friction, float restitution)
            : base(world)
        {
            this.texture = texture;
            Height = texture.Height;
            Width = texture.Width;

            Debug.Assert(points.Length >= 3);

            LinkedList<int> polygon = new LinkedList<int>();
            for (int i = 0; i < points.Length; i++)
                polygon.AddLast(i);

            // Triangles generated
            List<Vector2[]> triangles = new List<Vector2[]>();

            Split(polygon, points, triangles);

            CreateShapes(triangles, density, friction, restitution);
            CreateDrawable(triangles);
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
                        if (Math.Abs(d) < Utils.EPSILON)
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
                    ourColinearity = Math.Max(ourColinearity, Math.Abs(GetColinearity(points[t1.Value], p1, dir)));

                    LinkedListNode<int> t2 = i1.Next;
                    if (t2 == null) t2 = polygon.First;
                    ourColinearity = Math.Max(ourColinearity, GetColinearity(points[t2.Value], p1, dir));

                    LinkedListNode<int> t3 = i2.Previous;
                    if (t3 == null) t3 = polygon.Last;
                    ourColinearity = Math.Max(ourColinearity, GetColinearity(points[t3.Value], p2, dir));

                    LinkedListNode<int> t4 = i2.Next;
                    if (t4 == null) t4 = polygon.First;
                    ourColinearity = Math.Max(ourColinearity, GetColinearity(points[t4.Value], p2, dir));

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

        private float GetColinearity(Vector2 testPoint, Vector2 origin, Vector2 direction)
        {
            Vector2 dir = testPoint - origin;
            dir.Normalize();

            return Math.Abs(Vector2.Dot(dir, direction));
        }
        public override void Draw(Matrix cameraTransform)
        {
            PolygonDrawer drawer = GameEngine.Instance.PolygonDrawer;
            drawer.DrawPolygons(CASSWorld.SCALE * Position, Angle, CASSWorld.SCALE, texture, vertices, BlendState.AlphaBlend);

            base.Draw(cameraTransform);
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

        /**
         * Creates vertices that can be sent to the
         * graphics card for drawing
         */
        private void CreateDrawable(List<Vector2[]> triangles)
        {
            // Loop over triangles to create vertices
            vertices = new VertexPositionTexture[triangles.Count * 3];
            for (int i = 0; i < triangles.Count; i++)
            {
                Vector2[] points = triangles[i];

                // Loop over points in triangle to create vertices
                for (int j = 0; j < 3; j++)
                {
                    Vector3 v = new Vector3(points[j], 0);

                    // Generate texture coordinate around the center of the texture
                    Vector2 t = CASSWorld.SCALE * points[j] / new Vector2(texture.Width, texture.Height) +
                        new Vector2(0.5f);

                    vertices[3 * i + j] = new VertexPositionTexture(v, t);
                }
            }
        }

    }
}