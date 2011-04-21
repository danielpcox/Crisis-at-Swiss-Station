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
    public class HoleObject : BoxObject
    {

        public const float MAX_FILL = 300f; // the amount of instasteel you must drop into the hole before it is completely filled

        //private static Texture2D thisTexture;

        //animation stuff
        private Rectangle sourceRect;
        private Vector2 origin;
        private int xFrame;
        private int yFrame;
        private int spriteWidth;
        private int spriteHeight;
        [NonSerialized]
        private Texture2D animTexture;      
        private int myGameTime, animateTimer, animateInterval;

        private float filled = 0;

        string animTextureName;

        public HoleObject(World world, string animTexturename, string objectTexturename)
            : base(world, objectTexturename, 0f, .5f, 0.0f,1,false)
        {
            //thisTexture = mytexture;
            texture = GameEngine.TextureList[objectTexturename];
            TextureFilename = objectTexturename;
            animTexture = GameEngine.TextureList[animTexturename];
            animTextureName = animTexturename;

            //animation stuff
            myGameTime = 0;
            animateTimer = 0;
            animateInterval = 40;
            xFrame = 0;
            yFrame = 0;
            spriteWidth = 800;
            spriteHeight = 500;
            sourceRect = new Rectangle(xFrame * spriteWidth, yFrame * spriteHeight, spriteWidth, spriteHeight);
            origin = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);
            
        }

        public void reloadNonSerializedAssets()
        {
            texture = GameEngine.TextureList[TextureFilename];
            animTexture = GameEngine.TextureList[animTextureName];
        }

        public float Filled
        {
            get
            {
                return filled;
            }
            set
            {
                filled = value;
            }
        }

        bool destroyedBody = false;
        Body tempBody;
        bool replacedBody = false;

        public override void Update(CASSWorld world, float dt)
        {

          
            //animation stuff
            myGameTime++;
            sourceRect = new Rectangle(xFrame * spriteWidth, yFrame * spriteHeight, spriteWidth, spriteHeight);

            animateTimer += myGameTime;

            if (animateTimer > animateInterval)
            {
                xFrame++;

                if (xFrame > 3 )
                {
                    xFrame = 0;                    
                }               

                // -= (int)walkInterval;
                myGameTime = 0;
                animateTimer = 0;

            }

            // gotta delete the hole Body so we fall through the hole
            if (!destroyedBody)
            {
                tempBody = Body;
                //world.World.DestroyBody(Body);
                RemoveFromWorld();
                destroyedBody = true;
            }
            else if (Filled >= MAX_FILL && !replacedBody)
            {
                AddToWorld();
                Position = Position + new Vector2(0.35f, 0.2f);
                replacedBody = true;
                if (Filled != MAX_FILL)
                {
                    float temp1 = Filled - MAX_FILL;
                    Filled = MAX_FILL;
                    ScrollingWorld.numDrawLeft += temp1;
                    
                }
            }

            //base.Update(world, dt);

        }

        public override void Draw(Matrix cameraTransform)
        {
            //animation stuff
            Vector2 screenOffset = (CASSWorld.SCALE * Position);
            SpriteBatch spriteBatch = GameEngine.Instance.SpriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default,
                              RasterizerState.CullCounterClockwise, null, cameraTransform);           
            if (Filled < MAX_FILL)
            {
                
                spriteBatch.Draw(animTexture, screenOffset+ (new Vector2(0,-4.325f) * CASSWorld.SCALE), sourceRect, Color.White, Angle, origin, 1, SpriteEffects.None, 0);            
            }
            else
            {
                origin = new Vector2(texture.Width/2, texture.Height/2);

               // Console.WriteLine(origin);
                spriteBatch.Draw(texture, screenOffset  , null, Color.White, Angle, origin, 1, SpriteEffects.None, 0);            

            }

            spriteBatch.End();       
        
        }


    }
}
