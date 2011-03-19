using Box2DX.Collision;
using Box2DX.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace CrisisAtSwissStation
{
    public class HackObject : PhysicsObject
    {
        private int width, height, x, y;

        public HackObject(World world,  int myWidth, int myHeight, int myX, int myY, float myFriction)
            : base(world)
        {
            width = myWidth;
            height = myHeight;
            x = myX;
            y = myY;

            float halfWidth = width / (2 * CASSWorld.SCALE);
            float halfHeight = height / (2 * CASSWorld.SCALE);
        
            PolygonDef shape = new PolygonDef();
            shape.SetAsBox(halfWidth, halfHeight);
            shape.Friction = myFriction;
            shapes.Add(shape);

            BodyDef.Position.X = x;
            BodyDef.Position.Y = y;       
                       
        }

    }
}
