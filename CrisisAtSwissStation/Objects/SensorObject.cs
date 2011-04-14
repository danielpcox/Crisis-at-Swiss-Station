using System;

using Box2DX.Collision;
using Box2DX.Dynamics;
using Box2DX.Dynamics.Controllers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;

using System.Diagnostics;

namespace CrisisAtSwissStation
{
    [Serializable]
    public class SensorObject : BoxObject
    {
        private Rectangle sourceRect;
        private Vector2 origin;       
        private int xFrame;
        private int yFrame;
        private int spriteWidth;
        private int spriteHeight;     
        private int numFrames;
        [NonSerialized]
        private Texture2D animTexture;
        private int myGameTime, animateTimer, animateInterval;
        private bool animate;

        public string animTextureName;


        //public SensorObject(World world, Texture2D texture)
        //    : base(world, texture, 0, 0, 0,1,false)
        public SensorObject(World world, string animTexturename, string objectTextureName, int sprWidth, int sprHeight, int animInt, int myNumFrames)
            : base(world, objectTextureName, 0, 0, 0, 1, false)
        {
            TextureFilename = objectTextureName;

            animTexture = GameEngine.TextureList[animTexturename];
            //Texture2D objectTexture = GameEngine.TextureList[objectTextureName];

            //Texture2D texture = GameEngine.TextureList[texturename];
            //TextureFilename = texturename;
            //boundingBox = new Rectangle((int)Position.X, (int)Position.Y, (int)Height, (int)Width);

            shapes[0].IsSensor = true;

            animate = false;
            myGameTime = 0;
            animateTimer = 0;
            animateInterval = animInt;
            xFrame = 0;
            yFrame = 0;
            numFrames = myNumFrames;
            spriteWidth = sprWidth;
            spriteHeight = sprHeight;
            sourceRect = new Rectangle(xFrame * spriteWidth, yFrame * spriteHeight, spriteWidth, spriteHeight);
            origin = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);
        }

        public void reloadNonSerializedAssets()
        {
            animTexture = GameEngine.TextureList[animTextureName];
        }

        public void makeAnimate()
        {
            animate = true;
        }

        public override void Update(CASSWorld world, float dt)
        {
            if (animTexture != null)
            {
                if(animate == true)
                {
                    if (xFrame != 4)
                    {
                        //animation stuff
                        myGameTime++;
                        sourceRect = new Rectangle(xFrame * spriteWidth, yFrame * spriteHeight, spriteWidth, spriteHeight);

                        animateTimer += myGameTime;

                        if (animateTimer > animateInterval)
                        {
                            xFrame++;

                            if (xFrame > numFrames - 1)
                            {
                                xFrame = 0;
                            }


                            myGameTime = 0;
                            animateTimer = 0;

                        }
                    }
                }
            }

            base.Update(world, dt);

        }


        public override void Draw(Matrix cameraTransform)
        {
       
            
                //animation stuff
                Vector2 screenOffset = (CASSWorld.SCALE * Position);
                SpriteBatch spriteBatch = GameEngine.Instance.SpriteBatch;
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default,
                                  RasterizerState.CullCounterClockwise, null, cameraTransform);
                spriteBatch.Draw(animTexture, screenOffset, sourceRect, Color.White, Angle, origin, 1, SpriteEffects.None, 0);

                spriteBatch.End();
            
            


        }

    }
}
