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
// NOTE: Much of the code has been taken from programing lab 3

namespace CrisisAtSwissStation
{
    public class ScrollingWorld : CASSWorld
    {
        // Dimensions of the game world
        public const float WIDTH = 16.0f;
        public const float HEIGHT = 12.0f;
        private const float GRAVITY = 9.8f;
        public const int GAME_WIDTH = GameEngine.GAME_WINDOW_WIDTH; // how big the game is in pixels, regardless of the size of the game window
        public const int GAME_HEIGHT = GameEngine.GAME_WINDOW_HEIGHT; // how big the game is in pixels, regardless of the size of the game window

        // Content in the game world
        private static Texture2D groundTexture;
        private static Texture2D dudeTexture;
        private static Texture2D dudeObjectTexture;
        private static Texture2D winTexture;
        private static Texture2D ropeBridgeTexture;
        private static Texture2D barrierTexture;
        private static Texture2D paintTexture;
        private static Texture2D crosshairTexture;
        private static Texture2D background;
        private static AudioManager audioman;

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

        private static Vector2 winDoorPos = new Vector2(2.5f, 3.2f);

        private static Vector2 spinPlatformPos = new Vector2(5.5f, 4.0f);

        private static Vector2 dudePosition = new Vector2(2.5f, 7);
        private static string dudeSensorName = "Dude Ground Sensor";

        private static Vector2 screenOffset = new Vector2(0, 0); // The location of the screen origin in the Game World

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
            { new Vector2(12,5), new Vector2(13, 5), new Vector2(13,5.5f), new Vector2(12, 5.5f) },*/
            new Vector2[]
            { new Vector2(10,4), new Vector2(12, 4), new Vector2(12,4.5f), new Vector2(10, 4.5f) },
            new Vector2[]
            { new Vector2(1,4), new Vector2(4, 4), new Vector2(4,4.5f), new Vector2(1, 4.5f) },
            //new Vector2[] // Daniel-added
            //{ new Vector2(10,10), new Vector2(11, 10), new Vector2(11,11f), new Vector2(10, 11f) },
        };

        Vector2 mousePosition;
        List<Vector2> dotPositions = new List<Vector2>();
        Vector2 halfdotsize;
        float PAINTING_GRANULARITY = 5f; // how far apart points in a painting need to be for us to store them both
        MouseState prevms;
        int numDrawLeft = 105;
        bool finishDraw = false;

        DudeObject dude;
        SensorObject winDoor;
        LaserObject laser;

        public ScrollingWorld()
            : base(WIDTH, HEIGHT, new Vector2(0, GRAVITY))
        {
            // Create win door
            winDoor = new SensorObject(World, winTexture);
            winDoor.Position = winDoorPos;
            AddObject(winDoor);

            // Create ground pieces
            AddObject(new PolygonObject(World, wall1, groundTexture, 0, 0.0f, 0.1f));
            AddObject(new PolygonObject(World, wall2, groundTexture, 0, 0.0f, 0.1f));

            // Create platforms
            foreach(Vector2[] platform in platforms)
                AddObject(new PolygonObject(World, platform, groundTexture, 0, 0.1f, 0.0f));

            // Create dude
            dude = new DudeObject(World, dudeTexture, dudeObjectTexture,dudeSensorName);
            dude.Position = dudePosition;
            AddObject(dude);

            // Create laser
            laser = new LaserObject(World, dude);

            // create a DEBUG painted object
            List<Vector2> blobs = new List<Vector2> { new Vector2(400, 300), new Vector2(410, 310), new Vector2(420, 300) };
            //AddObject(new PaintedObject(World, paintTexture, blobs));

            // Create rope bridge
            AddObject(new RopeBridge(World, ropeBridgeTexture, 8.1f, 5.5f, 11.5f, 1, 0, 0));

           /* // Create spinning platform
            BoxObject spinPlatform = new BoxObject(World, barrierTexture, 10,0,0);
            spinPlatform.Position = spinPlatformPos;
            AddObject(spinPlatform);
            
            // Create a joint to affix the platform to the world.
            ////////////////////////////////////////////

            RevoluteJointDef joint = new RevoluteJointDef();
            joint.Initialize(spinPlatform.Body, World.GetGroundBody(), Utils.Convert(spinPlatform.Position));
            World.CreateJoint(joint);*/

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
            dudeTexture = content.Load<Texture2D>("DudeFilmstrip");
            dudeObjectTexture = content.Load<Texture2D>("DudeObject");
            winTexture = content.Load<Texture2D>("WinDoor");
            ropeBridgeTexture = content.Load<Texture2D>("RopeBridge");
            barrierTexture = content.Load<Texture2D>("Barrier");
            paintTexture = content.Load<Texture2D>("paint");
            crosshairTexture = content.Load<Texture2D>("Crosshair");
            background = content.Load<Texture2D>("background");
          
        }

        public override void Simulate(float dt)
        {
            dude.Grounded = false; // unrelated to the following

            // code for erasing a painted object
            MouseState mouse = Mouse.GetState();
            bool mouseinbounds = mouse.X > 0 && mouse.X < GameEngine.GAME_WINDOW_WIDTH && mouse.Y < GameEngine.GAME_WINDOW_HEIGHT && mouse.Y > 0;
            mousePosition = new Vector2(mouse.X / CASSWorld.SCALE, mouse.Y / CASSWorld.SCALE);
            if (mouse.RightButton == ButtonState.Pressed)
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

            if (mouse.LeftButton == ButtonState.Pressed && mouseinbounds && numDrawLeft > 0)
            {// if we're holding down the mouse button
                Vector2 mousepos = new Vector2(mouse.X, mouse.Y);
                if (dotPositions.Count == 0 || (mousepos - dotPositions[dotPositions.Count - 1]).Length() > PAINTING_GRANULARITY)
                { // according to the granularity constraint for paintings,
                    dotPositions.Add(new Vector2(mouse.X, mouse.Y) + screenOffset); // add a point in a new painting
                    numDrawLeft--;
                    finishDraw = true;
                }
                
            }
            else if ((mouse.LeftButton == ButtonState.Released && (numDrawLeft > 0 || finishDraw)) && prevms.LeftButton == ButtonState.Pressed && mouseinbounds)
            {
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
                    // create the painting as an object in the world
                    this.AddObject(new PaintedObject(World, paintTexture, dotPositions));
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
                
            }
        }

        public override void Draw(GraphicsDevice device, Vector3 eye, Matrix view, Matrix proj)
        {
            GameEngine.Instance.SpriteBatch.Begin();
            Vector2 backgroundOffset = new Vector2(0, 0) - screenOffset;
            GameEngine.Instance.SpriteBatch.Draw(background, backgroundOffset, Color.White);
            GameEngine.Instance.SpriteBatch.End();

            base.Draw(device, eye, view, proj);

            GameEngine.Instance.SpriteBatch.Begin();                   
            GameEngine.Instance.SpriteBatch.Draw(crosshairTexture, mousePosition * CASSWorld.SCALE,
                null, Color.White, 0, new Vector2(crosshairTexture.Width / 2, crosshairTexture.Height / 2), 1,
                SpriteEffects.None, 0);
            foreach (Vector2 dotpos in dotPositions)
            {
                GameEngine.Instance.SpriteBatch.Draw(paintTexture, dotpos - halfdotsize - screenOffset, Color.White);
            }
            GameEngine.Instance.SpriteBatch.End();
            
            laser.Draw();
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