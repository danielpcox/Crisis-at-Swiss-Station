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

        PrimitiveBatch primitiveBatch;

        public LaserObject(Box2DX.Dynamics.World myWorld, DudeObject myDude, float myScale)
        {   
            SCALE = myScale;
            world = myWorld;
            dude = myDude;

            primitiveBatch = new PrimitiveBatch(GameEngine.Instance.GraphicsDevice);
            
        }

        public void Create(float mX, float mY)
        {
            primitiveBatch.Begin(PrimitiveType.LineList);

            //I know magic numbers suck, i just wanted this to look pretty to begin with
            Vector2 adjustment;
            if (dude.isRight() == true)
                adjustment = new Vector2(12.5f, -6f);
            else
                adjustment = new Vector2(-12.5f, -6f);

            Vector2 original = new Vector2(dude.Position.X * SCALE, dude.Position.Y * SCALE);
            Vector2 end = new Vector2(mX * SCALE, mY * SCALE);

            Vector2 gunpos = original + adjustment;

            Box2DX.Collision.Segment myseg = new Segment();
            myseg.P1 = new Box2DX.Common.Vec2(gunpos.X / SCALE, gunpos.Y / SCALE);
            myseg.P2 = new Box2DX.Common.Vec2(end.X / SCALE, end.Y / SCALE);

            float lambda = 1;
            Box2DX.Common.Vec2 normal;

            Box2DX.Collision.Shape interference = world.RaycastOne(myseg, out lambda, out normal, false, null);

            Box2DX.Common.Vec2 p = (((1 - lambda) * myseg.P1) + (lambda * myseg.P2));
            if (interference != null)
            {
                end.X = SCALE * p.X;
                end.Y = SCALE * p.Y;

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
