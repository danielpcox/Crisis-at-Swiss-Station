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
        private bool pistonMove, adjusted;
        private float endXPos, endYPos, headScale, endScale, headMaxVal, headMinVal,trueInc, falseInc;
        [NonSerialized]       
        private Texture2D pistonHead = GameEngine.TextureList["Art\\Objects\\PistonObjects\\piston_moving"];
        [NonSerialized]
        private Texture2D pistonEnd = GameEngine.TextureList["Art\\Objects\\PistonObjects\\piston_unmoving"];

        public PistonObject(World world, float myEndScale, float myHeadScale, float myHeadMinVal, float myHeadMaxVal, float myEndXPos, float myEndYPos, float myTrueInc, float myFalseInc, Vector2 headPos)//, float headXPos,float headYPos)
            : base(world, "Art\\Objects\\PistonObjects\\piston_moving", 0f, .5f, 0.0f, myHeadScale, false)
        {
            TextureFilename = "Art\\Objects\\PistonObjects\\piston_moving";

            //po = new PistonObject(world.World, .5f, .5f, 12f, 13f, 9.7f, 12.6f, .01f, .2f, gameposition);
            // private static Vector2 pistonPosition = new Vector2(12f, 13.2f);
            /*
            endXPos = myEndXPos;
            endYPos = myEndYPos;
             */
            endXPos = headPos.X - 2.3f;
            endYPos = headPos.Y - .6f;
            //headScale = myHeadScale; needed?
            //endScale = myEndScale;
            endScale = myEndScale;
           // headMinVal = myHeadMinVal;
            //headMaxVal = myHeadMaxVal;
            headMinVal = headPos.X;
            headMaxVal = headPos.X + .6f;
            trueInc = myTrueInc;
            falseInc = myFalseInc;
            adjusted = false;


        }

        public void reloadNonSerializedAssets()
        {
            pistonHead = GameEngine.TextureList["Art\\Objects\\PistonObjects\\piston_moving"];
            pistonEnd = GameEngine.TextureList["Art\\Objects\\PistonObjects\\piston_unmoving"];         
        }


        public override void Update(CASSWorld world, float dt)
        {
            if (Angle == MathHelper.ToRadians(90))
            {
                if(!adjusted)
                {
                endXPos = Position.X + .6f;
                endYPos = Position.Y - 2.5f;
                headMinVal = Position.Y;
                headMaxVal = Position.Y + .6f;
                adjusted = true;
                }

                if (pistonMove == true)
                {
                    Position = Position - new Vector2(0, trueInc);
                    if (Position.Y < headMinVal)
                        pistonMove = false;
                }
                else
                {
                    Position = Position + new Vector2(0, falseInc);
                    if (Position.Y > headMaxVal)
                        pistonMove = true;
                }

            }
            else if (Angle == MathHelper.ToRadians(-90))
            {
                if (!adjusted)
                {
                    endXPos = Position.X - .6f;
                    endYPos = Position.Y + 2.5f;
                    headMinVal = Position.Y;
                    headMaxVal = Position.Y - .6f;
                    adjusted = true;
                }

                if (pistonMove == true)
                {
                    Position = Position + new Vector2(0, trueInc);
                    if (Position.Y > headMinVal)
                        pistonMove = false;
                }
                else
                {
                    Position = Position - new Vector2(0, falseInc);
                    if (Position.Y < headMaxVal)
                        pistonMove = true;
                }

            }
            else if (Angle == MathHelper.ToRadians(0))
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
            }
            else if (Angle == MathHelper.ToRadians(180))
            {        
                if (!adjusted)
                {
                    endXPos = Position.X + 2.3f;
                    endYPos = Position.Y + .6f;
                    headMinVal = Position.X;
                    headMaxVal = Position.X - .6f;
                    adjusted = true;
                }

                if (pistonMove == true)
                {
                    Position = Position + new Vector2(trueInc, 0);
                    if (Position.X > headMinVal)
                        pistonMove = false;
                }
                else
                {
                    Position = Position - new Vector2(falseInc, 0);
                    if (Position.X < headMaxVal)
                        pistonMove = true;
                }
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
            spriteBatch.Draw(pistonEnd, new Vector2(endXPos * CASSWorld.SCALE, endYPos * CASSWorld.SCALE), null, Color.White, Angle, new Vector2(0, 0), scale, SpriteEffects.None, 0);
            spriteBatch.End();

             base.Draw(cameraTransform);
        }
    }


}