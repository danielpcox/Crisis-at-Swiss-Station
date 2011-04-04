using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input; // TODO : figure out if this should be here
using Color = Microsoft.Xna.Framework.Color;

using Box2DX.Collision;
using Box2DX.Dynamics;
// NOTE: Much of the code has been taken from programming lab 3

namespace CrisisAtSwissStation
{
    public class ScrollingWorld : CASSWorld
    {
        // Dimensions of the game world
        public const float WIDTH = 80.0f; //16.0f originally, then 20f, now changed for side scrolling
        public const float HEIGHT = 15.0f; //12.0f
        private const float GRAVITY = 9.8f;
        public const int GAME_WIDTH = GameEngine.GAME_WINDOW_WIDTH; // how big the game is in pixels, regardless of the size of the game window
        public const int GAME_HEIGHT = GameEngine.GAME_WINDOW_HEIGHT; // how big the game is in pixels, regardless of the size of the game window

        public const float PAINTING_GRANULARITY = 20f; // how far apart points in a painting need to be for us to store them both

        // Content in the game world
        private static Texture2D groundTexture;
        private static Texture2D dudeTexture;
        private static Texture2D armTexture;
        private static Texture2D dudeObjectTexture;
        private static Texture2D winTexture;
        private static Texture2D ropeBridgeTexture;
        private static Texture2D barrierTexture;
        private static Texture2D paintTexture;
        private static Texture2D paintedSegmentTexture;
        private static Texture2D crosshairTexture;
        private static Texture2D background;

        private static Texture2D bigBoxTexture;
        private static Texture2D littleBoxTexture;
        private static Texture2D leftPipeTexture;
        private static Texture2D rightPipeTexture;
        private static Texture2D platformTexture;
        private static Texture2D bottomTexture;

        private static Texture2D holeTexture;

        private static Texture2D movingPlatformTexture;
        private bool movPlat;

        

        // Wall vertices
        private static Vector2[] wall1 = new Vector2[]
        {
          new Vector2(8,  0), new Vector2(8,  1),
          new Vector2(1,  1), new Vector2(1, 12),
          new Vector2(0, 12), new Vector2(0,  0)
        };
        private static Vector2[] wall2 = new Vector2[]
        {
          new Vector2(16,   0), new Vector2(16, 12),
          new Vector2(15,  12), new Vector2(15,  1),
          new Vector2( 8,  1),  new Vector2( 8,  0)
        }; 

        private static Vector2 winDoorPos = new Vector2(19f, 4.38f);

        private static Vector2 spinPlatformPos = new Vector2(7.0f, 6.0f);

        private static Vector2 dudePosition = new Vector2(10f, 10f);
        private static string dudeSensorName = "Dude Ground Sensor";

        private static Vector2 screenOffset = new Vector2(0, 0); // The location of the screen origin in the Game World

        private static Vector2 bigBoxPosition = new Vector2(2.5f, 4f);
        private BoxObject bigBox;

        private static Vector2 littleBoxPosition = new Vector2(5.85f, 5.65f);
        private BoxObject littleBox;

        private static Vector2 leftPipePosition = new Vector2(8.9f, 5.7f);
        private BoxObject leftPipe;

        private static Vector2 rightPipePosition = new Vector2(16.35f, 6.37f);
        private BoxObject rightPipe;

        private static Vector2 platformPosition = new Vector2(18.2f, 5.48f);
        private BoxObject platform;

        private static Vector2 bottomPosition = new Vector2(10.3f, 15f);
        private BoxObject bottom1,bottom2,bottom3,bottom4;

        private static Vector2 hole1Position = new Vector2(12f, 14.7f);
        private  BoxObject hole1;

        private static Vector2 movPlatform1Position = new Vector2(10f, 13f);
        private BoxObject movPlatform1;
        /*
        private static Vector2[][] platforms = new Vector2[][]
        {
            new Vector2[]
            { new Vector2(1,9), new Vector2(4, 9), new Vector2(4,9.5f), new Vector2(1, 9.5f) },
            new Vector2[]
            { new Vector2(4,8), new Vector2(5, 8), new Vector2(5,9.5f), new Vector2(4, 9.5f) },
            new Vector2[]
            { new Vector2(12,8), new Vector2(15, 8), new Vector2(15,9.5f), new Vector2(12, 9.5f) },
            /*new Vector2[]
            { new Vector2(13,7), new Vector2(14, 7), new Vector2(14,7.5f), new Vector2(13, 7.5f) },
            new Vector2[]
            { new Vector2(14,6), new Vector2(15, 6), new Vector2(15,6.5f), new Vector2(14, 6.5f) },
            new Vector2[]
            { new Vector2(12,5), new Vector2(13, 5), new Vector2(13,5.5f), new Vector2(12, 5.5f) },
            new Vector2[]
            { new Vector2(10,4), new Vector2(12, 4), new Vector2(12,4.5f), new Vector2(10, 4.5f) },
            new Vector2[]
            { new Vector2(1,4), new Vector2(4, 4), new Vector2(4,4.5f), new Vector2(1, 4.5f) },
            //new Vector2[] // Daniel-added
            //{ new Vector2(10,10), new Vector2(11, 10), new Vector2(11,11f), new Vector2(10, 11f) },
        }; 
        */
        Vector2 mousePosition;
        List<Vector2> dotPositions = new List<Vector2>();
        Vector2 halfdotsize;
        MouseState prevms;
        public static int numDrawLeft = 0; //yeah yeah, bad coding style...im tired :(
        bool finishDraw = false;
        bool drawingInterrupted = false; // true when we're creating the object due to occlusion, false otherwise

        DudeObject dude;
        BoxObject arm;
        SensorObject winDoor;
        LaserObject laser;

        public ScrollingWorld()
            : base(WIDTH, HEIGHT, new Vector2(0, GRAVITY))
        {
            movPlat = true;

            numDrawLeft = 0; // HACK HACK HACK
            // Create win door
            winDoor = new SensorObject(World, winTexture);
            winDoor.Position = winDoorPos + new Vector2(61.5f, .05f); 
            AddObject(winDoor);

            // Create ground pieces
            //AddObject(new PolygonObject(World, wall1, groundTexture, 0, 0.0f, 0.1f));
            //AddObject(new PolygonObject(World, wall2, groundTexture, 0, 0.0f, 0.1f));

            
            // Create platforms
            //foreach(Vector2[] platform in platforms)
            //    AddObject(new PolygonObject(World, platform, groundTexture, 0, 0.1f, 0.0f));
            

            //new platforms
            //float s = CASSWorld.SCALE;
           // AddObject(new HackObject(World, (int)(1024/s), (int)(38/s), 0, (int)(730/s),.1f));


            // Create dude
            dude = new DudeObject(World, dudeTexture, dudeObjectTexture, armTexture, dudeSensorName);
            dude.Position = dudePosition;
            AddObject(dude);

            // Create the dude's arm
            /*
            arm = new BoxObject(World, armTexture, 0, .1f, 0);
            arm.Position = dudePosition;
            AddObject(arm);
            */

            //create bottom platforms
            bigBox = new BoxObject(World, bigBoxTexture, 0, .1f, 0);
            bigBox.Position = bigBoxPosition + new Vector2(61.5f, .05f); 
            AddObject(bigBox);

            littleBox = new BoxObject(World, littleBoxTexture, 0, .1f, 0);
            littleBox.Position = littleBoxPosition + new Vector2(61.5f, 0f); 
            AddObject(littleBox);

            leftPipe = new BoxObject(World, leftPipeTexture, 0, .1f, 0);
            leftPipe.Position = leftPipePosition + new Vector2(61.5f, 0f); 
            AddObject(leftPipe);

            rightPipe = new BoxObject(World, rightPipeTexture, 0, .1f, 0);
            rightPipe.Position = rightPipePosition + new Vector2(61.5f, 0f); 
            AddObject(rightPipe);

            platform = new BoxObject(World, platformTexture, 0, .1f, 0);
            platform.Position = platformPosition + new Vector2(61.5f, 0f); 
            AddObject(platform);

            bottom1 = new BoxObject(World, bottomTexture, 0, .5f, 0);
            bottom1.Position = bottomPosition;
            AddObject(bottom1);

            bottom2 = new BoxObject(World, bottomTexture, 0, .5f, 0);
            bottom2.Position = bottomPosition + new Vector2(20.3f, 0f);
            AddObject(bottom2);

            bottom3 = new BoxObject(World, bottomTexture, 0, .5f, 0);
            bottom3.Position = bottomPosition + new Vector2(40.6f, 0f); 
            AddObject(bottom3);

            bottom4 = new BoxObject(World, bottomTexture, 0, .5f, 0);
            bottom4.Position = bottomPosition + new Vector2(60.9f, 0f); 
            AddObject(bottom4);

            hole1 = new BoxObject(World, holeTexture, 0, .5f, 0);
            hole1.Position = hole1Position;
            AddObject(hole1);

            movPlatform1 = new BoxObject(World, movingPlatformTexture, 0, .5f, 0);
            movPlatform1.Position = movPlatform1Position;
            AddObject(movPlatform1);

            // Create laser
            laser = new LaserObject(World, dude);

            // create a DEBUG painted object
            List<Vector2> blobs = new List<Vector2>(500);

            int i = 0;
            for (int y = 250; y >= 50; y-= 20)
            {
                for (int x = 3422; x <= 3522; x += 20)//(int x = 350; x <= 450; x+=20)
                {
                    //Vector2 temp = new Vector2();
                    //temp.X = x;
                   // temp.Y = y;
                    //blobs[i] = temp;
                    //blobs.Insert(i,new Vector2(x, y));
                    blobs.Add(new Vector2(x, y));
                    i++;
                }
                i++;
            }
            Console.WriteLine("{0}", blobs.Count);
            AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, blobs));


            // Create rope bridge
            //AddObject(new RopeBridge(World, ropeBridgeTexture, 8.1f, 5.5f, 11.5f, 1, 0, 0));

           // Create spinning platform
           /** BoxObject spinPlatform = new BoxObject(World, barrierTexture, 25,0,0);
            spinPlatform.Position = spinPlatformPos;
            AddObject(spinPlatform);
            
            // Create a joint to affix the platform to the world.
            ////////////////////////////////////////////

            RevoluteJointDef joint = new RevoluteJointDef();
            joint.Initialize(spinPlatform.Body, World.GetGroundBody(), Utils.Convert(spinPlatform.Position));
            World.CreateJoint(joint); */

            ////////////////////////////////////////////


            World.SetContactListener(new PlatformContactListener(this));
            World.SetBoundaryListener(new PlatformBoundaryListener(this));

            halfdotsize = new Vector2(paintTexture.Width / 2, paintTexture.Height / 2);

            
            //PLAYS THE SONG!!!  (It resets at the beginning of the level)
            AudioManager audio = GameEngine.AudioManager;
            audio.Play(AudioManager.MusicSelection.EarlyLevelv2);

        }

        public static void LoadContent(ContentManager content)
        {
            groundTexture = content.Load<Texture2D>("EarthTile02");
            //dudeTexture = content.Load<Texture2D>("Dude");
            dudeTexture = content.Load<Texture2D>("newDudeFilmstrip");
            armTexture = content.Load<Texture2D>("arm");
            dudeObjectTexture = content.Load<Texture2D>("DudeObject");
            winTexture = content.Load<Texture2D>("WinDoor");
            ropeBridgeTexture = content.Load<Texture2D>("RopeBridge");
            barrierTexture = content.Load<Texture2D>("Barrier");
            //paintTexture = content.Load<Texture2D>("paint");
            paintTexture = content.Load<Texture2D>("paint");
            paintedSegmentTexture = content.Load<Texture2D>("paintedsegment");
            crosshairTexture = content.Load<Texture2D>("Crosshair");
            background = content.Load<Texture2D>("background");
            
            //our new platforms
            bigBoxTexture = content.Load<Texture2D>("bigBoxTexture");
            littleBoxTexture = content.Load<Texture2D>("littleBoxTexture");
            leftPipeTexture = content.Load<Texture2D>("leftPipeTexture");
            rightPipeTexture = content.Load<Texture2D>("rightPipeTexture");
            platformTexture = content.Load<Texture2D>("platformTexture");
            bottomTexture = content.Load<Texture2D>("bottomTexture");

            holeTexture = content.Load<Texture2D>("hole_tile");

            movingPlatformTexture = content.Load<Texture2D>("moving platform");
          
        }

        public override void Simulate(float dt)
        {
            if (movPlat == true)
            {
                movPlatform1.Position = movPlatform1.Position + new Vector2(.05f, 0);
                if (movPlatform1.Position.X > 20)
                    movPlat = false;
            }
            else
            {
                movPlatform1.Position = movPlatform1.Position - new Vector2(.05f, 0);
                if (movPlatform1.Position.X <10 )
                    movPlat = true;
            }

            dude.Grounded = false; // unrelated to the following

            // code for erasing a painted object
            MouseState mouse = Mouse.GetState();
            bool mouseinbounds = mouse.X > 0 && mouse.X < GameEngine.GAME_WINDOW_WIDTH && mouse.Y < GameEngine.GAME_WINDOW_HEIGHT && mouse.Y > 0;
            mousePosition = new Vector2(mouse.X / CASSWorld.SCALE, mouse.Y / CASSWorld.SCALE);
            //ERASING
            if (mouse.RightButton == ButtonState.Pressed && laser.canErase())
            { // if the right button is pressed, remove any painted objects under the cursor from the world
                // Query a small box around the mouse
                AABB aabb = new AABB();
                aabb.LowerBound = Utils.Convert(mousePosition - new Vector2(0.1f));// + screenOffset); //TODO Diana: fix this
                aabb.UpperBound = Utils.Convert(mousePosition + new Vector2(0.1f));// + screenOffset); //TODO Diana: fix this

                Shape[] shapes = new Shape[1];
                int nHit = World.Query(aabb, shapes, 1);

                if (nHit > 0)
                {
                    Body body = shapes[0].GetBody();
                    PhysicsObject po = (PhysicsObject)body.GetUserData();
                    if (po is PaintedObject)
                    {
                        this.RemoveObject(po);
                        PaintedObject painto = (PaintedObject)po;
                        numDrawLeft += painto.getNumBlobs();
                    }
                }
            }

            if (mouse.LeftButton == ButtonState.Released && laser.canDraw())
                drawingInterrupted = false;

            if (mouse.LeftButton == ButtonState.Pressed && laser.canDraw() && !drawingInterrupted && mouseinbounds && numDrawLeft > 0)
            {// if we're holding down the mouse button
                Vector2 mousepos = new Vector2(mouse.X, mouse.Y);
                if (dotPositions.Count == 0 || (mousepos - dotPositions[dotPositions.Count - 1]).Length() > PAINTING_GRANULARITY)
                { // according to the granularity constraint for paintings,
                    dotPositions.Add(new Vector2(mouse.X, mouse.Y) + screenOffset); // add a point in a new painting
                    numDrawLeft--;
                    finishDraw = true;
                }
                
            }
            else if (((mouse.LeftButton == ButtonState.Released || !laser.canDraw()) && (numDrawLeft > 0 || finishDraw)) && (prevms.LeftButton == ButtonState.Pressed || drawingInterrupted) && mouseinbounds)
            {
                if (!laser.canDraw())
                    drawingInterrupted = true;
                Box2DX.Dynamics.Body overlapped = null;
                PhysicsObject po = null;
                foreach (Vector2 pos in dotPositions)
                {
                    // Query a small box around the mouse
                    AABB aabb = new AABB();
                    Vector2 gamepos = new Vector2(pos.X / CASSWorld.SCALE, pos.Y / CASSWorld.SCALE) + screenOffset;
                    aabb.LowerBound = Utils.Convert(gamepos - new Vector2(0.1f));
                    aabb.UpperBound = Utils.Convert(gamepos + new Vector2(0.1f));

                    Box2DX.Collision.Shape[] shapes = new Box2DX.Collision.Shape[1];
                    int nHit = World.Query(aabb, shapes, 1);

                    if (nHit > 0)
                    {
                        Body body = shapes[0].GetBody();
                        po = (PhysicsObject)body.GetUserData();
                        if (po is PaintedObject)
                        {
                            overlapped = body;
                        }
                        break;
                    }
                }

                // DEBUG : uncomment next line (and delete "false)") to attempt connecting of painted objects
                // TODO Diana: add something about screen offset?
                if (false)//overlapped != null)
                {
                    foreach (Vector2 pos in dotPositions)
                    {
                        ((PaintedObject)po).AddToShapes(dotPositions);
                    }
                }
                else
                {

                    List<Vector2> dp2 = new List<Vector2>();
                    // hack to make the drawing fit the offset
                    //foreach (Vector2 pos in dotPositions)
                    //{
                    //    //Console.WriteLine(dude.Position.X * CASSWorld.SCALE);
                    //    dp2.Add(pos + new Vector2(dude.Position.X * CASSWorld.SCALE, 0));
                    //}

                    // create the painting as an object in the world
                    if (dotPositions.Count>1)
                        //this.AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, dp2));
                        this.AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, dotPositions));
                }
                // clear the way for another painting
                dotPositions = new List<Vector2>(); // 
                finishDraw = false;
            }
            // end painting code (except for prevms = ms below)

            prevms = mouse;

            laser.Update(mousePosition.X, mousePosition.Y);

            base.Simulate(dt);
            screenOffset = new Vector2(0, 0); // TODO Diana: Change this!
        }

        /**
         * Listens to contacts to apply demo level-specific things
         */
        private class PlatformContactListener : ContactListener
        {
            ScrollingWorld world;

            public PlatformContactListener(ScrollingWorld world)
            {
                this.world = world;
            }
            
            public override void Persist(ContactPoint point)
            {
                // Test player collision with ground
                Shape shape1 = point.Shape1;
                Shape shape2 = point.Shape2;

                PhysicsObject object1 = shape1.GetBody().GetUserData() as PhysicsObject;
                PhysicsObject object2 = shape2.GetBody().GetUserData() as PhysicsObject;

                if (ScrollingWorld.dudeSensorName.Equals(shape2.UserData))
                {
                    Shape temp = shape1;
                    shape1 = shape2;
                    shape2 = temp;
                }
                if (ScrollingWorld.dudeSensorName.Equals(shape1.UserData) &&
                    (world.dude != shape2.GetBody().GetUserData()))
                    world.dude.Grounded = true;

                if ((object1 == world.winDoor && object2 == world.dude) ||
                    (object2 == world.winDoor && object1 == world.dude))
                    world.Win();

                //ronnie added as hole test
               //if (object1 == world.hole1 && object2 == world.dude)
                   // world.Fail();
                
            }
        }

        public override void Draw(GraphicsDevice device, Matrix camera)
        {
            float guyPos = -dude.Position.X * CASSWorld.SCALE + (GameEngine.GAME_WINDOW_WIDTH / 2);
            Matrix cameraTransform = Matrix.CreateTranslation(guyPos, 0.0f, 0.0f);

            GameEngine.Instance.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default,
                              RasterizerState.CullCounterClockwise, null, cameraTransform);
            GameEngine.Instance.SpriteBatch.Draw(background, new Vector2(0, 0), Color.White);
            GameEngine.Instance.SpriteBatch.End();

            base.Draw(device, cameraTransform);

            GameEngine.Instance.SpriteBatch.Begin();                   
            GameEngine.Instance.SpriteBatch.Draw(crosshairTexture, mousePosition * CASSWorld.SCALE,
                null, Color.White, 0, new Vector2(crosshairTexture.Width / 2, crosshairTexture.Height / 2), 1,
                SpriteEffects.None, 0);
            foreach (Vector2 dotpos in dotPositions)
            {
                GameEngine.Instance.SpriteBatch.Draw(paintTexture, dotpos - halfdotsize - screenOffset, Color.Red);
            }
            GameEngine.Instance.SpriteBatch.End();
            
            laser.Draw(new Vector2(guyPos, 0));
        }

        /**
         * Returns the location of the dude, which should be
         * used as the center of the visible screen.
         */
        public override Vector2 GetVisualTargetPosition()
        {
            return dude.Position;
        }

        private class PlatformBoundaryListener : BoundaryListener
        {
            ScrollingWorld world;

            public PlatformBoundaryListener(ScrollingWorld world)
            {
                this.world = world;
            }

            public override void Violation(Body body)
            {
                PhysicsObject obj = body.GetUserData() as PhysicsObject;
                obj.Die();

                if (obj == world.dude)
                    world.Fail();
            }
        }
    }
}