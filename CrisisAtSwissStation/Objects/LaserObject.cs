using System;
using Color = Microsoft.Xna.Framework.Color;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using CrisisAtSwissStation.Common;

using Box2DX.Collision;

namespace CrisisAtSwissStation
{
    [Serializable]
    public class LaserObject
    {
        private Color IN_SIGHT = Color.LimeGreen;
        private Color OUT_SIGHT = Color.Firebrick;

        private Box2DX.Dynamics.World world;
        private float SCALE;
        private DudeObject dude;
        private bool canIDraw, canIErase, amDrawing, amErasing;
        private Vector2 original, adjustment, end, offset, startpoint, endpoint;
        private float mouseX, mouseY, guyPos,lambda;
        private int currentSection, numSections, sectionTimer;
        Box2DX.Collision.Shape interference;
        //Box2DX.Collision.Shape[] interference;
        private Vector2[] sections;

        //animation stuff
        private Rectangle sourceRect;
        private Vector2 origin;
        private int xFrame;
        private int yFrame;
        private int spriteWidth;
        private int spriteHeight;
        private int numFrames;
        private int myGameTime, animateTimer, animateInterval;


        [NonSerialized]
        PrimitiveBatch primitiveBatch;
        [NonSerialized]
        private Texture2D sectionTex;
        private string sectionTextureName;

        public LaserObject(Box2DX.Dynamics.World myWorld, DudeObject myDude, string sectionTexturename, int amnSections)
        {               
            world = myWorld;
            dude = myDude;
            sectionTex = GameEngine.TextureList[sectionTexturename];
            sectionTextureName = sectionTexturename;
            numSections = amnSections;
            sections = new Vector2[numSections];
            currentSection = 0;
            sectionTimer = 0;
            lambda = 0;
            amDrawing = false;
            amErasing = false;
            //interference = new Box2DX.Collision.Shape[20];

            SCALE = CASSWorld.SCALE;
            primitiveBatch = new PrimitiveBatch(GameEngine.Instance.GraphicsDevice);

            //animation stuff
            myGameTime = 0;
            animateTimer = 0;
            animateInterval = 20;
            xFrame = 0;
            yFrame = 0;
            numFrames = 8;
            spriteWidth = 50;
            spriteHeight = 512;
            sourceRect = new Rectangle(xFrame * spriteWidth, yFrame * spriteHeight, spriteWidth, spriteHeight);
            origin = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);



            
        }

        public void reloadNonSerializedAssets()
        {
            primitiveBatch = new PrimitiveBatch(GameEngine.Instance.GraphicsDevice);
            sectionTex = GameEngine.TextureList[sectionTextureName];
        }

        public bool canDraw()
        { return canIDraw; }

        public bool canErase()
        { return canIErase; }

        public void startDrawing()
        { amDrawing = true; }

        public void finishDrawing()
        { amDrawing = false; }

        public void startErasing()
        { amErasing = true; }

        public void finishErasing()
        { amErasing = false; }

        public float getLambda()
        { return lambda; }

        public Vector2 getStart()
        { return startpoint; }

        public Vector2 getEnd()
        { return endpoint; }

        public void Update(float mX, float mY, Vector2 offset, int levelWidth)
        {
            if (amDrawing || amErasing)
            {
                //animation object
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



            //Console.WriteLine("{0} {1}", mX, mY);

            mouseX = mX * SCALE;
            mouseY = mY * SCALE;
            if (dude.isRight() == true)
                adjustment = new Vector2(12.5f, -6f);
            else
                adjustment = new Vector2(-12.5f, -6f);

            int guyScreenPos = 0;
            if (dude.Position.X * CASSWorld.SCALE <= GameEngine.SCREEN_WIDTH / 2)
                guyScreenPos = (int)(dude.getWorld().getScreenCoords(dude.Position).X);
            else if (dude.Position.X * CASSWorld.SCALE >= levelWidth - GameEngine.SCREEN_WIDTH / 2)
            {
                guyScreenPos = (int)(dude.getWorld().getScreenCoords(dude.Position).X); ;
            }
            else
            {
                guyScreenPos = GameEngine.SCREEN_WIDTH / 2;
            }

            original = new Vector2(dude.Position.X * SCALE, dude.Position.Y * SCALE);
            end = new Vector2((mX+ dude.Position.X)*SCALE - guyScreenPos, mY * SCALE);
            //end = new Vector2(mX*SCALE, mY * SCALE);

            Vector2 gunpos = original; //+ adjustment;

            Box2DX.Collision.Segment myseg = new Segment();
            myseg.P1 = new Box2DX.Common.Vec2(gunpos.X / SCALE, gunpos.Y / SCALE);
            myseg.P2 = new Box2DX.Common.Vec2(end.X / SCALE, end.Y / SCALE);

            lambda = 1;
            Box2DX.Common.Vec2 normal;

            //for (int i = 0; i < interference.Length; i++)
           // {
           //     interference[i] = null;
           // }

            interference = world.RaycastOne(myseg, out lambda, out normal, false, null);   
            //int numShapes = world.Raycast(myseg, interference, 20, false, null);   
            //Console.WriteLine( "{0}", interference[0]);
            //if(interference[0] != null)
            //Physic

            if (interference == null || interference.IsSensor)
            {
                canIDraw = true;
                canIErase = true;
                endpoint = new Vector2(mouseX, mouseY);
            }           
            else if ((PhysicsObject)interference.GetBody().GetUserData() is PaintedObject)
            {

                
                Box2DX.Common.Vec2 p = (((1 - lambda) * (myseg.P1)) + (lambda * (myseg.P2)));

                endpoint = Common.Utils.Convert(p) *SCALE;
/**                Box2DX.Collision.Segment tempseg = new Segment();
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
                 */
                canIDraw = false;
                canIErase = true;

            }
            else
            {
                Box2DX.Common.Vec2 p = ((((1 - lambda) * (myseg.P1)) + (lambda * (myseg.P2))))*SCALE;
                end.X = SCALE * p.X;
                end.Y = SCALE * p.Y;
                canIDraw = false;
                canIErase = false;
                endpoint = Common.Utils.Convert(p);
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

            //original animation stuff
            //startpoint = original + adjustment + offset;
            startpoint = original + offset;
            endpoint = new Vector2(mouseX, mouseY);

            // move the start point to the end of his gun
            Vector2 cursorDirection = (endpoint - startpoint);
            cursorDirection.Normalize();
            startpoint = startpoint + (Constants.HALF_GUN * cursorDirection);

            /*
            int totalx = (int)(endpoint.X - startpoint.X);
            int totaly = (int)(endpoint.Y - startpoint.Y);
            int xincr = totalx/numSections;
            int yincr = totaly/numSections;
            for (int i = 0; i < numSections; i++)
            {
                sections[i] = new Vector2(startpoint.X+ (i * xincr), startpoint.Y + (i * yincr));
            }

            */
        }

        public void Draw()
        {           
            
            primitiveBatch.Begin(PrimitiveType.LineList);            

            if (interference != null)
            {
                primitiveBatch.AddVertex(startpoint, OUT_SIGHT);
                primitiveBatch.AddVertex(endpoint, OUT_SIGHT);
            }
            else
            {
                primitiveBatch.AddVertex(startpoint, IN_SIGHT);
                primitiveBatch.AddVertex(endpoint, IN_SIGHT);
            }

            primitiveBatch.End();

            if (amDrawing || amErasing)
            {

                //Vector2 screenOffset = (CASSWorld.SCALE * Position);
                //SpriteBatch spriteBatch = GameEngine.Instance.SpriteBatch;
                //spriteBatch.Begin();
                //spriteBatch.Draw(animTexture, screenOffset, sourceRect, Color.White, Angle, origin, 1, SpriteEffects.None, 0);

                //spriteBatch.End();

                GameEngine.Instance.SpriteBatch.Begin();                
                //Console.WriteLine("{0}");
                Common.Utils.stretchForLaser(GameEngine.Instance.SpriteBatch, sectionTex, startpoint, endpoint, Color.White,sourceRect);
                GameEngine.Instance.SpriteBatch.End();


                //original animation stuff
                /*
                GameEngine.Instance.SpriteBatch.Begin();
                //Console.WriteLine("{0} {1}", sections[currentSection].X, sections[currentSection].Y);
                Common.Utils.DrawLine(GameEngine.Instance.SpriteBatch, sectionTex, sections[currentSection], sections[currentSection + 1], PaintedObject.INSTASTEEL_COLOR);
                GameEngine.Instance.SpriteBatch.End();

                sectionTimer++;
                if (sectionTimer > 5)
                {
                    sectionTimer = 0;
                    currentSection++;
                    if (currentSection > numSections - 2)
                        currentSection = 0;
                }
                  */
            }
        }

      
    }

}
