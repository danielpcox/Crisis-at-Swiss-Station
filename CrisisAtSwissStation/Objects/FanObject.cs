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

    public class FanObject : BoxObject
    {

        //animation stuff
        private Rectangle sourceRect;
        private Vector2 origin;
        private int xFrame;
        private int yFrame;
        private int spriteWidth;
        private int spriteHeight;
        private Texture2D animTexture;
        private int myGameTime, animateTimer, animateInterval;



      
         public FanObject(World world, Texture2D mytexture, Texture2D objectTexture)
            : base(world, objectTexture, 0f, .5f, 0.0f, 1, false)
         {
           
            //animation stuff
            myGameTime = 0;
            animateTimer = 0;
            animateInterval = 20;
            animTexture = mytexture;                   
            xFrame = 0;
            yFrame = 0;
            spriteWidth = 200;
            spriteHeight = 200;
            sourceRect = new Rectangle(xFrame * spriteWidth, yFrame * spriteHeight, spriteWidth, spriteHeight);
            origin = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);


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

                if (xFrame > 6 )
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



