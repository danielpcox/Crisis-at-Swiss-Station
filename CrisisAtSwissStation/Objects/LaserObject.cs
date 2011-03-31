using System;
using Color = Microsoft.Xna.Framework.Color;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Box2DX.Collision;

namespace CrisisAtSwissStation
{
    public class LaserObject
    {
        private Color IN_SIGHT = Color.Red;
        private Color OUT_SIGHT = Color.Yellow;

        private Box2DX.Dynamics.World world;
        private float SCALE;
        private DudeObject dude;
        private bool canIDraw, canIErase;
        private Vector2 original, adjustment, end;
        private float mouseX, mouseY;
        Box2DX.Collision.Shape interference;
        //Box2DX.Collision.Shape[] interference;
        
        PrimitiveBatch primitiveBatch;

        public LaserObject(Box2DX.Dynamics.World myWorld, DudeObject myDude)
        {               
            world = myWorld;
            dude = myDude;

            //interference = new Box2DX.Collision.Shape[20];

            SCALE = CASSWorld.SCALE;
            primitiveBatch = new PrimitiveBatch(GameEngine.Instance.GraphicsDevice);
            
        }

        public bool canDraw()
        { return canIDraw; }

        public bool canErase()
        { return canIErase; }

        public void Update(float mX, float mY)
        {
            mouseX = mX * SCALE;
            mouseY = mY * SCALE;
            if (dude.isRight() == true)
                adjustment = new Vector2(12.5f, -6f);
            else
                adjustment = new Vector2(-12.5f, -6f);

            original = new Vector2(dude.Position.X * SCALE, dude.Position.Y * SCALE);
            end = new Vector2((mX+ dude.Position.X)*SCALE - 512, mY * SCALE);

            Vector2 gunpos = original + adjustment;

            Box2DX.Collision.Segment myseg = new Segment();
            myseg.P1 = new Box2DX.Common.Vec2(gunpos.X / SCALE, gunpos.Y / SCALE);
            myseg.P2 = new Box2DX.Common.Vec2(end.X / SCALE, end.Y / SCALE);

            float lambda = 1;
            Box2DX.Common.Vec2 normal;

            //for (int i = 0; i < interference.Length; i++)
           // {
           //     interference[i] = null;
           // }

            interference = world.RaycastOne(myseg, out lambda, out normal, false, null);   
            //int numShapes = world.Raycast(myseg, interference, 20, false, null);   
            //Console.WriteLine( "{0}", interference[0]);
            //if(interference[0] != null)
            //PhysicsObject po = (PhysicsObject)interference.GetBody().GetUserData();

            if (interference == null || interference.IsSensor)
            {
                canIDraw = true;
                canIErase = true;
            }           
            else if ((PhysicsObject)interference.GetBody().GetUserData() is PaintedObject)
            {

                Box2DX.Common.Vec2 p = (((1 - lambda) * myseg.P1) + (lambda * myseg.P2));

                Box2DX.Collision.Segment tempseg = new Segment();
                tempseg.P1 = new Box2DX.Common.Vec2(end.X / SCALE, end.Y / SCALE);
                tempseg.P2 = p;
                interference = world.RaycastOne(tempseg, out lambda, out normal, false, null);

                if (interference == null)
                {
                    end.X = SCALE * p.X;
                    end.Y = SCALE * p.Y;
                    canIDraw = false;
                    canIErase = true;
                }
                else
                {
                    end.X = SCALE * p.X;
                    end.Y = SCALE * p.Y;
                    canIDraw = false;
                    canIErase = false;
                }
            }
            else
            {
                Box2DX.Common.Vec2 p = (((1 - lambda) * myseg.P1) + (lambda * myseg.P2));
                end.X = SCALE * p.X;
                end.Y = SCALE * p.Y;
                canIDraw = false;
                canIErase = false;

            }

            //Console.WriteLine("{0}  {1}", canIDraw, canIErase);
                /*
            else if (interference != null && interference.IsSensor != true)//&& !((interference.UserData).Equals("I am a painted object")))
            {
                Box2DX.Common.Vec2 p = (((1 - lambda) * myseg.P1) + (lambda * myseg.P2));
                end.X = SCALE * p.X;
                end.Y = SCALE * p.Y;
                canIDraw = false;
                canIErase = false;
            }
            else { canIDraw = true; }*/
            
        }

        public void Draw(Vector2 offset)
        {
            primitiveBatch.Begin(PrimitiveType.LineList);

            if (interference != null)
            {
                primitiveBatch.AddVertex(original + adjustment +offset, OUT_SIGHT);
                primitiveBatch.AddVertex(new Vector2(mouseX,mouseY), OUT_SIGHT);
            }
            else
            {
                primitiveBatch.AddVertex(original + adjustment +offset, IN_SIGHT);
                primitiveBatch.AddVertex(new Vector2(mouseX,mouseY), IN_SIGHT);
            }

            primitiveBatch.End();

        }

      
    }

}
