using Box2DX.Collision;
using Box2DX.Dynamics;
using Box2DX.Dynamics.Controllers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Color = Microsoft.Xna.Framework.Color;

using System;
using System.Diagnostics;

namespace CrisisAtSwissStation
{
    [Serializable]
    public class AnimationObject : BoxObject
    {
        //animation stuff
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
        private bool hasBody;

        string animTextureName;

        public AnimationObject( World world, string animTexturename, string objectTexturename, int sprWidth, int sprHeight, int animInt, int myNumFrames)
            : base(world, objectTexturename, 0f, .5f, 0.0f, 1, false)
        {
            animTextureName = animTexturename;
            //Console.WriteLine("***" + animTexturename + "***");
            animTexture = GameEngine.TextureList[animTexturename];

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

            hasBody = true;


        }
        public AnimationObject(World world, string animTexturename, string objectTexturename, int sprWidth, int sprHeight, int animInt, int myNumFrames, bool hasBody)
            : base(world, objectTexturename, 0f, .5f, 0.0f, 1, false)
        {
            animTextureName = animTexturename;
            //Console.WriteLine("***" + animTexturename + "***");
            animTexture = GameEngine.TextureList[animTexturename];

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

            this.hasBody = hasBody;
         


        }

        public void reloadNonSerializedAssets()
        {
            animTexture = GameEngine.TextureList[animTextureName];
        }

        public override void Update(CASSWorld world, float dt)
        {
            //animation stuff
            myGameTime++;
            sourceRect = new Rectangle(xFrame * spriteWidth, yFrame * spriteHeight, spriteWidth, spriteHeight);

            animateTimer += myGameTime;

            if (animateTimer > animateInterval)
            {
                xFrame++;

                if (xFrame > numFrames-1)
                {
                    xFrame = 0;
                }


                myGameTime = 0;
                animateTimer = 0;

            }

            if (!hasBody) { RemoveFromWorld(); hasBody = true; }

            base.Update(world, dt);

        }

        public override void Draw(Matrix cameraTransform)
        {

            //animation stuff
            Vector2 screenOffset = (CASSWorld.SCALE * Position);
            SpriteBatch spriteBatch = GameEngine.Instance.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default,
                              RasterizerState.CullCounterClockwise, null, cameraTransform);

            spriteBatch.Draw(animTexture, screenOffset, sourceRect, Color.White, Angle, origin,scale, SpriteEffects.None, 0);

            spriteBatch.End();



        }


    }
}