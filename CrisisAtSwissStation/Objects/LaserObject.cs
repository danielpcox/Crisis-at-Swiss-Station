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
        private bool canIDraw;
        private Vector2 original, adjustment, end;
        Box2DX.Collision.Shape interference;
        
        PrimitiveBatch primitiveBatch;

        public LaserObject(Box2DX.Dynamics.World myWorld, DudeObject myDude)
        {               
            world = myWorld;
            dude = myDude;

            SCALE = CASSWorld.SCALE;
            primitiveBatch = new PrimitiveBatch(GameEngine.Instance.GraphicsDevice);
            
        }

        public bool canDraw()
        { return canIDraw; }

        public void Update(float mX, float mY)
        {
            if (dude.isRight() == true)
                adjustment = new Vector2(12.5f, -6f);
            else
                adjustment = new Vector2(-12.5f, -6f);

            original = new Vector2(dude.Position.X * SCALE, dude.Position.Y * SCALE);
            end = new Vector2(mX * SCALE, mY * SCALE);

            Vector2 gunpos = original + adjustment;

            Box2DX.Collision.Segment myseg = new Segment();
            myseg.P1 = new Box2DX.Common.Vec2(gunpos.X / SCALE, gunpos.Y / SCALE);
            myseg.P2 = new Box2DX.Common.Vec2(end.X / SCALE, end.Y / SCALE);

            float lambda = 1;
            Box2DX.Common.Vec2 normal;

            interference = world.RaycastOne(myseg, out lambda, out normal, false, null);            

            if (interference != null)
            {
                Box2DX.Common.Vec2 p = (((1 - lambda) * myseg.P1) + (lambda * myseg.P2));
                end.X = SCALE * p.X;
                end.Y = SCALE * p.Y;
                canIDraw = false;
            }
            else { canIDraw = true; }

        }

        public void Draw()
        {
            primitiveBatch.Begin(PrimitiveType.LineList);

            if (interference != null)
            {
                primitiveBatch.AddVertex(original + adjustment, OUT_SIGHT);
                primitiveBatch.AddVertex(end, OUT_SIGHT);
            }
            else
            {
                primitiveBatch.AddVertex(original + adjustment, IN_SIGHT);
                primitiveBatch.AddVertex(end, IN_SIGHT);
            }

            primitiveBatch.End();

        }

      
    }

}
