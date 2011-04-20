using System;
using Box2DX.Collision;
using Box2DX.Dynamics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color; // Stupid Box2DX name collision!

namespace CrisisAtSwissStation
{
    [Serializable]
    public class SwitchObject : BoxObject
    {
        public bool switchOn;
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

        private string animTextureName;


        public SwitchObject(World world, string mytexturename, string objectTexturename, int sprWidth, int sprHeight, int animInt, int myNumFrames)
            : base(world, objectTexturename, 0f, .5f, 0.0f, 1, false)
        {

            animTexture = GameEngine.TextureList[mytexturename];
            animTextureName = mytexturename;

            switchOn = false;

            myGameTime = 0;
            animateTimer = 0;
            animateInterval = animInt;
            //animTexture = mytexture;
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


        public override void Update(CASSWorld world, float dt)
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
