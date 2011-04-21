using System;
using Box2DX.Collision;
using Box2DX.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace CrisisAtSwissStation
{
    [Serializable]
    public class PistonObject : BoxObject
    {
        private bool pistonMove;
        private float endXPos, endYPos, headScale, endScale, headMaxVal, headMinVal,trueInc, falseInc;
        [NonSerialized]       
        private Texture2D pistonHead = GameEngine.TextureList["Art\\Objects\\PistonObjects\\piston_moving"];
        [NonSerialized]
        private Texture2D pistonEnd = GameEngine.TextureList["Art\\Objects\\PistonObjects\\piston_unmoving"];

        public PistonObject(World world, float myEndScale, float myHeadScale, float myHeadMinVal, float myHeadMaxVal, float myEndXPos, float myEndYPos, float myTrueInc, float myFalseInc)//, float headXPos,float headYPos)
            : base(world, "Art\\Objects\\PistonObjects\\piston_moving", 0f, .5f, 0.0f, myHeadScale, false)
        {
            TextureFilename = "Art\\Objects\\PistonObjects\\piston_moving";

            endXPos = myEndXPos;
            endYPos = myEndYPos;
            //headScale = myHeadScale; needed?
            endScale = myEndScale;
            headMinVal = myHeadMinVal;
            headMaxVal = myHeadMaxVal;
            trueInc = myTrueInc;
            falseInc = myFalseInc;


        }

        public void reloadNonSerializedAssets()
        {
            pistonHead = GameEngine.TextureList["Art\\Objects\\PistonObjects\\piston_moving"];
            pistonEnd = GameEngine.TextureList["Art\\Objects\\PistonObjects\\piston_unmoving"];         
        }


        public override void Update(CASSWorld world, float dt)
        {

            if (pistonMove == true)
            {
                Position = Position - new Vector2(trueInc, 0);
                if (Position.X < headMinVal)
                    pistonMove = false;
            }
            else
            {
                Position = Position + new Vector2(falseInc, 0);
                if (Position.X > headMaxVal)
                    pistonMove = true;
            }

            base.Update(world, dt);
        }


        public override void Draw(Matrix cameraTransform)
        {  
            SpriteBatch spriteBatch = GameEngine.Instance.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default,
                              RasterizerState.CullCounterClockwise, null, cameraTransform);
            //GameEngine.Instance.SpriteBatch.Draw(pistonAssemblyTexture, new Vector2(9.7f * CASSWorld.SCALE, 12.6f * CASSWorld.SCALE), null, Color.White, 0, new Vector2(0, 0), .5f, SpriteEffects.None, 0);
            //spriteBatch.Draw(texture, CASSWorld.SCALE * Position, null, Color.White, Angle, origin, scale, SpriteEffects.None, 0);
            spriteBatch.Draw(pistonEnd, new Vector2(endXPos * CASSWorld.SCALE, endYPos * CASSWorld.SCALE), null, Color.White, 0, new Vector2(0, 0), endScale, SpriteEffects.None, 0);
            spriteBatch.End();

             base.Draw(cameraTransform);
        }
    }


}