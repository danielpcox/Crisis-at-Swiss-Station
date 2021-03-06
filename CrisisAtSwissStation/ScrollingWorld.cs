﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input; // TODO : figure out if this should be here
using Color = Microsoft.Xna.Framework.Color;

using Box2DX.Collision;
using Box2DX.Dynamics;
using CrisisAtSwissStation.Common;
// NOTE: Much of the code has been taken from programming lab 3

namespace CrisisAtSwissStation
{

    [Serializable]
    public class ScrollingWorld : CASSWorld
    {

        

        // Dimensions of the game world
        public const float WIDTH = 80.0f; //16.0f originally, then 20f, now changed for side scrolling
        public const float HEIGHT = 15.0f; //12.0f
        private const float GRAVITY = 9.8f * 1.5f; // now with 50% more gravity!
        public const int SCREEN_WIDTH = GameEngine.SCREEN_WIDTH;
        public const int SCREEN_HEIGHT = GameEngine.SCREEN_HEIGHT;

        public const float PAINTING_GRANULARITY = 20f; // how far apart points in a painting need to be for us to store them both

        // Content in the game world
        // HACK - these are all deprecated by GameEngine.TextureList

        [NonSerialized]
        private static Texture2D groundTexture; 
        [NonSerialized]
        private static Texture2D dudeTexture;
        [NonSerialized]
        private static Texture2D armTexture;
        [NonSerialized]
        private static Texture2D dudeObjectTexture;
        [NonSerialized]
        private static Texture2D winTexture;
        [NonSerialized]
        private static Texture2D ropeBridgeTexture;
        [NonSerialized]
        private static Texture2D barrierTexture;
        [NonSerialized]
        private static Texture2D barrierTexture2;
        [NonSerialized]
        private static Texture2D paintTexture;
        [NonSerialized]
        private static Texture2D paintedSegmentTexture;
        [NonSerialized]
        private static Texture2D crosshairTexture;
        [NonSerialized]
        private static Texture2D background;
        [NonSerialized]
        private static Texture2D backgroundTerrible;

        public string backgroundName;
        public string musicName;

        [NonSerialized]
        private static Texture2D laserAnimTexture;

        [NonSerialized]
        private static Texture2D bigBoxTexture;
        [NonSerialized]
        private static Texture2D littleBoxTexture;
        [NonSerialized]
        private static Texture2D leftPipeTexture;
        [NonSerialized]
        private static Texture2D rightPipeTexture;
        [NonSerialized]
        private static Texture2D platformTexture;
        [NonSerialized]
        private static Texture2D bottom1Texture;
        [NonSerialized]
        private static Texture2D bottom2Texture;

        [NonSerialized]
        private static Texture2D holeTexture;
        [NonSerialized]
        private static Texture2D holeObjTexture;

        [NonSerialized]
        private static Texture2D movingPlatformTexture;
        [NonSerialized]
        private static Texture2D brokenMovingPlatformTexture;
        [NonSerialized]
        private static Texture2D brokenMovingPlatformAnimTexture;

        [NonSerialized]
        private static Texture2D pipeAssemblyTexture;
        [NonSerialized]
        private static Texture2D window1Texture;
        [NonSerialized]
        private static Texture2D window2Texture;
        [NonSerialized]
        private static Texture2D window3Texture;
        [NonSerialized]
        private static Texture2D window4Texture;
        [NonSerialized]
        private static Texture2D window5Texture;
        [NonSerialized]
        private static Texture2D straightPipeTexture;
        [NonSerialized]
        private static Texture2D straightPipeTileTexture;
        [NonSerialized]
        private static Texture2D groundPlatformTexture;
        [NonSerialized]
        private static Texture2D pulleyPlatformTexture;
        [NonSerialized]
        private static Texture2D pulleyPlatformLongTexture;
        [NonSerialized]
        private static Texture2D pulleyChainTexture;
        [NonSerialized]
        private static Texture2D pistonAssemblyTexture;
        [NonSerialized]
        private static Texture2D pistonHeadTexture;
        [NonSerialized]
        private static Texture2D tableTexture;
        [NonSerialized]
        private static Texture2D fanAnimTexture;
        [NonSerialized]
        private static Texture2D fanTexture;
        [NonSerialized]
        private static Texture2D switchAnimTexture;
        [NonSerialized]
        private static Texture2D switchDeathAnimTexture;
        [NonSerialized]
        private static Texture2D rollingpinTexture;
        [NonSerialized]
        private static Texture2D rollingpinAnimTexture;
        [NonSerialized]
        private static Texture2D switchObjectTexture;
        [NonSerialized]
        private static Texture2D deathObjectTexture;
        [NonSerialized]
        private static Texture2D failButtonObjectTexture;
        [NonSerialized]
        private static Texture2D holeAnimTexture;
        [NonSerialized]
        private static Texture2D holeObjectTexture;
        [NonSerialized]
        private static Texture2D movingObjectTexture;
        [NonSerialized]
        private static Texture2D horizontalmovingObjectTexture;

        [NonSerialized]
        private static Texture2D lampTexture;
        [NonSerialized]
        private static Texture2D lampAnimTexture;

        [NonSerialized]
        private static Texture2D winDoorAnimTexture;

        [NonSerialized]
        private static Texture2D ballroomBackground;

        private bool movPlat1;
        private bool pistonMove;
        //private bool movPlat2;
        //private bool mov;
        //[NonSerialized]
        //private static Texture2D bottomTexture;

        /*
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
        */
        private static Vector2 winDoorPos = new Vector2(60f, 13.65f);

        //private static Vector2 spinPlatformPos = new Vector2(7.0f, 6.0f);

        private static Vector2 dudePosition = new Vector2(4f, 6f); //was 2.5 now 55
        private static string dudeSensorName = "Dude Ground Sensor";

        private static Vector2 screenOffset = new Vector2(0, 0); // The location of the screen origin in the Game World
        /*
        private static Vector2 bigBoxPosition = new Vector2(2.5f, 4f);
        private BoxObject bigBox;
        */
        private static Vector2 littleBoxPosition = new Vector2(5.85f, 5.65f);
        private BoxObject littleBox;
        /*
        private static Vector2 leftPipePosition = new Vector2(8.9f, 5.7f);
        private BoxObject leftPipe;

        private static Vector2 rightPipePosition = new Vector2(16.35f, 6.37f);
        private BoxObject rightPipe;
        */

        private static Vector2 platformPosition = new Vector2(18.2f, 5.48f);
        private BoxObject platform;

        private static Vector2 bottom1Position = new Vector2(23.2f, 15f);//22.75
        private static Vector2 bottom2Position = new Vector2(65f, 15f);//65.58
        private BoxObject bottom1, bottom2;

        private static Vector2 straightPipe1Position = new Vector2(9.5f, 9.3f);
        private static Vector2 straightPipe2Position = new Vector2(3.1f, 5.5f);
        private static Vector2 straightPipe3Position = new Vector2(12.85f, 4.35f);
        private static Vector2 straightPipe4Position = new Vector2(15.7f, 3.4f);
        private static Vector2 straightPipe5Position = new Vector2(18.0f, 4.5f);
        private static Vector2 straightPipe6Position = new Vector2(13.6f, 7f);
        private static Vector2 straightPipe7Position = new Vector2(12.8f, 12f);
        private static Vector2 straightPipe8Position = new Vector2(15.8f, 11.5f);
        private static Vector2 straightPipe9Position = new Vector2(19.3f, 9.35f);
        private static Vector2 straightPipe10Position = new Vector2(20.5f, 11f);
        private static Vector2 straightPipe11Position = new Vector2(25f, 13.3f);
        private BoxObject straightPipe1, straightPipe2, straightPipe3, straightPipe4, straightPipe5, straightPipe6, straightPipe7;
        private BoxObject straightPipe8, straightPipe9, straightPipe10, straightPipe11;

        private static Vector2 pulleyPipe1Position = new Vector2(16.8f, 4.35f);
        private static Vector2 pulleyPipe2Position = new Vector2(18.2f, 12f);
        private BoxObject pulleyPipe1,pulleyPipe2;

        private static Vector2 topPosition = new Vector2(41f, 0f);
        private BoxObject top;

        private static Vector2 hole1Position = new Vector2(47f, 15f);//14.7
        private BoxObject hole1;
        
        private static Vector2 pistonHeadPosition = new Vector2(12f, 13.2f);
        private BoxObject pistonHead;

        private static Vector2 pistonPosition = new Vector2(12f, 13.2f);
        private PistonObject piston;

        //private static Vector2 movPlatform1Position = new Vector2(10f, 10f);
        //private BoxObject movPlatform1;

        private static Vector2 movPlatform1Position = new Vector2(8.3f, 10f);
        private MovingObject movPlatform1;

        private static Vector2 movPlatform2Position = new Vector2(35f,14f);
        private HorizontalMovingObject movPlatform2;

        private static Vector2 tablePosition = new Vector2(15.6f, 13f);
        private CircleObject table;

        private static Vector2 fan1Position = new Vector2(20.33f, 2f);
        private AnimationObject fan1;

        private static Vector2 lamp1Position = new Vector2(3.1f, 1.32f);
        private AnimationObject lamp1;        

        private static Vector2 brokenMovingPlatform1Position = new Vector2(1f, 14.3f);
        private SwitchObject brokenMovingPlatform1;

        private static Vector2 pillarPosition = new Vector2(0.035f, 7f);
        private BoxObject pillar;
        private BoxObject pillar2;        
     

        int gameLevelWidth;
        int gameLevelHeight;
        [NonSerialized]
        Vector2 mousePosition;
        List<Vector2> dotPositions = new List<Vector2>();
        [NonSerialized]
        Vector2 halfdotsize;
        [NonSerialized]
        MouseState prevms;

        Rectangle cursorSrcRect;
        int cursorWidth = 32;
        public static float numDrawLeft;
        public static float drawLimit = 4096;
        float totalInstaSteelInWorld;
        float lengthCurDrawing = 0; // The length of the drawing so far that the player is currently drawing
        Vector2 prevMousePos;
        bool mouseWasInbounds = false;
        bool finishDraw = false;
        bool drawingInterrupted = false; // true when we're creating the object due to occlusion, false otherwise

        List<HoleObject> holeList;
        bool allHolesFilled = false;

        DudeObject dude;
        BoxObject arm;
        WinDoorObject winDoor;
        public LaserObject laser;

        

        public ScrollingWorld(string backgroundname = "background")
            : base(WIDTH, HEIGHT, new Vector2(0, GRAVITY))
        {
            backgroundName = "Art\\Backgrounds\\" + backgroundname;
            musicName = backgroundname;
            background = GameEngine.TextureList[backgroundName];

            movPlat1 = true;
            //movPlat2 = true;
            //mov = true;

            gameLevelWidth = background.Width;
            gameLevelHeight = background.Height;

            cursorSrcRect = new Rectangle(cursorWidth, cursorWidth, cursorWidth, cursorWidth);
            numDrawLeft = 0; // reset the amount of instasteel when loading the level
            totalInstaSteelInWorld = 0;

            // Create win door
	    // HACK HACK - this will break door animation until a fix is created
            //winDoor = new SensorObject(World, winDoorAnimTexture, winTexture,93,99,20,5);
            winDoor = new WinDoorObject(World, "door_strip", "WinDoor", 93, 99, 20, 5);
            winDoor.Position = winDoorPos;
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

            // Create laser
            laser = new LaserObject(World, dude, "paintedsegment", 10);

            // Create dude
            //dude = new DudeObject(World, dudeTexture, dudeObjectTexture, armTexture, dudeSensorName);
            //dude = new DudeObject(World, this, dudeTexture, dudeObjectTexture, armTexture, laser, dudeSensorName);
            dude = new DudeObject(World, this, "newDudeFilmstrip", "Dude", "arm", laser, dudeSensorName);
            dude.Position = dudePosition;
            AddObject(dude);

            // Create the dude's arm
            /*
            arm = new BoxObject(World, armTexture, 0, .1f, 0);
            arm.Position = dudePosition;
            AddObject(arm);
            */

            //Left pillar (walls)
            pillar = new BoxObject(World, "Barrier", 0, .1f, 0,1,false);
            pillar.Position = pillarPosition;
            AddObject(pillar);
            /*
            pillar = new BoxObject(World, barrierTexture, 0, .1f, 0,1,false);
            pillar.Position = pillarPosition + new Vector2(0, -3.7f);
            AddObject(pillar);
            pillar = new BoxObject(World, barrierTexture, 0, .1f, 0,1,false);
            pillar.Position = pillarPosition + new Vector2(0, -7.4f);
            AddObject(pillar);
            pillar = new BoxObject(World, barrierTexture, 0, .1f, 0,1,false);
            pillar.Position = pillarPosition + new Vector2(0, -11.1f);
            AddObject(pillar);
            pillar = new BoxObject(World, barrierTexture, 0, .1f, 0,1,false);
            pillar.Position = pillarPosition + new Vector2(0, -14.8f);
            AddObject(pillar);
             */

            //right pillar now
            pillar2 = new BoxObject(World, "Barrier", 0, .1f, 0,1,false);
            pillar2.Position = pillarPosition + new Vector2(81.9f, 0);
            AddObject(pillar2);
            /*
            pillar = new BoxObject(World, barrierTexture, 0, .1f, 0,1,false);
            pillar.Position = pillarPosition + new Vector2(0, -3.7f);
            AddObject(pillar);
            pillar = new BoxObject(World, barrierTexture, 0, .1f, 0,1,false);
            pillar.Position = pillarPosition + new Vector2(0, -7.4f);
            AddObject(pillar);
            pillar = new BoxObject(World, barrierTexture, 0, .1f, 0,1,false);
            pillar.Position = pillarPosition + new Vector2(0, -11.1f);
            AddObject(pillar);
            pillar = new BoxObject(World, barrierTexture, 0, .1f, 0,1,false);
            pillar.Position = pillarPosition + new Vector2(0, -14.8f);
            AddObject(pillar);*/
            

/*
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
            */
            platform = new BoxObject(World, "platformTexture", 0, .1f, 0, 1, false);
            platform.Position = platformPosition + new Vector2(61.5f, 0f); 
          //  AddObject(platform);        

            straightPipe1 = new BoxObject(World, "straight_pipe", 0, .5f, 0,1,false);
            straightPipe1.Position = straightPipe1Position;            
            straightPipe1.Angle = 1.57f;
            //straightPipe1.Angle = 3.14f;
           // AddObject(straightPipe1);

            straightPipe2 = new BoxObject(World, "ground_like_platform", 0, .5f, 0,.6f,false);
            straightPipe2.Position = straightPipe2Position;                        
          //  AddObject(straightPipe2);

            straightPipe3 = new BoxObject(World, "straight_pipe", 0, .5f, 0, .6f,false);
            straightPipe3.Position = straightPipe3Position;
          //  AddObject(straightPipe3);

            straightPipe4 = new BoxObject(World, "straight_pipe_tile", 0, .5f, 0, .8f,false);
            straightPipe4.Position = straightPipe4Position;
            straightPipe4.Angle = 1.57f;
           // AddObject(straightPipe4);

            straightPipe5 = new BoxObject(World, "straight_pipe", 0, .5f, 0, 1f,false);
            straightPipe5.Position = straightPipe5Position;
            straightPipe5.Angle = 1.57f;
           // AddObject(straightPipe5);

            straightPipe6 = new BoxObject(World, "straight_pipe", 0, .5f, 0, .45f,false);
            straightPipe6.Position = straightPipe6Position;
          //  AddObject(straightPipe6);

            straightPipe7 = new BoxObject(World, "straight_pipe", 0, .5f, 0, .58f,false);
            straightPipe7.Position = straightPipe7Position;
          //  AddObject(straightPipe7);

            straightPipe8 = new BoxObject(World, "straight_pipe_tile", 0, .5f, 0, .4f,false);
            straightPipe8.Position = straightPipe8Position;
            straightPipe8.Angle = 1.57f;
            //AddObject(straightPipe8);

            straightPipe9 = new BoxObject(World, "straight_pipe_tile", 0, .5f, 0, 1f,false);
            straightPipe9.Position = straightPipe9Position;
          //  AddObject(straightPipe9);

            straightPipe10 = new BoxObject(World, "straight_pipe_tile", 0, .5f, 0, 1.3f, false);
            straightPipe10.Position = straightPipe10Position;
            straightPipe10.Angle = 1.57f;
          //  AddObject(straightPipe10);

            straightPipe11 = new BoxObject(World, "straight_pipe_tile", 0, .5f, 0, 1.3f, false);
            straightPipe11.Position = straightPipe11Position;
            straightPipe11.Angle = 1.57f;
           // AddObject(straightPipe11);

            pulleyPipe1 = new BoxObject(World, "pulley_platform", 1f, .5f, 0, .7f,true);
            pulleyPipe1.Position = pulleyPipe1Position;
           // pulleyPipe1.Body.
         //   AddObject(pulleyPipe1);

            pulleyPipe2 = new BoxObject(World, "pulley_platform_long", 1f, .5f, 0, .35f,true);
            pulleyPipe2.Position = pulleyPipe2Position;            
        //    AddObject(pulleyPipe2);
            
       //     PulleyJointDef jointDef1 = new PulleyJointDef();
        //    Box2DX.Common.Vec2 anchor1 = pulleyPipe1.Body.GetWorldCenter();
       //     Box2DX.Common.Vec2 anchor2 = pulleyPipe2.Body.GetWorldCenter();
       //     Box2DX.Common.Vec2 groundAnchor1 = pulleyPipe1.Body.GetWorldCenter() + Utils.Convert(new Vector2(0, -2f));
       //     Box2DX.Common.Vec2 groundAnchor2 = pulleyPipe2.Body.GetWorldCenter() + Utils.Convert(new Vector2(0, -5f));
            //Box2DX.Common.Vec2 groundAnchor2 = Utils.Convert(new Vector2(18.2f, 10.0f));
       //     jointDef1.Initialize(pulleyPipe1.Body,pulleyPipe2.Body, groundAnchor1, groundAnchor2, anchor1, anchor2, 1f);
            
            /*
            PulleyJointDef jointDef1 = new PulleyJointDef();
            Box2DX.Common.Vec2 anchor1 = pulleyPipe2.Body.GetWorldCenter();
            Box2DX.Common.Vec2 anchor2 = pulleyPipe1.Body.GetWorldCenter();
            Box2DX.Common.Vec2 groundAnchor1 = pulleyPipe2.Body.GetWorldCenter() + Utils.Convert(new Vector2(0, -5f));
            Box2DX.Common.Vec2 groundAnchor2 = pulleyPipe1.Body.GetWorldCenter();
            //Box2DX.Common.Vec2 groundAnchor2 = Utils.Convert(new Vector2(18.2f, 10.0f));
            jointDef1.Initialize(pulleyPipe2.Body, pulleyPipe1.Body, groundAnchor1, groundAnchor2, anchor1, anchor2, .5f);
            */
        //    jointDef1.Length1 = 2f;
        //    jointDef1.Length2 = 5f;
         //   jointDef1.MaxLength1 = 5f;
         //   jointDef1.MaxLength2 = 5f;
            
            //Console.WriteLine("{0} {1}", pulleyPipe1.Body.GetMass(), pulleyPipe2.Body.GetMass());
            //Console.WriteLine("{0} {1}", jointDef1.Length1, jointDef1.Length2);
            //Console.WriteLine("{0} {1}", pulleyPipe2.Body.GetWorldCenter().X, pulleyPipe2.Body.GetWorldCenter().Y);
            //Console.WriteLine("{0}",  getGameCoords(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)));
         //   World.CreateJoint(jointDef1);
            /*
            pistonHead = new BoxObject(World, "Art\\Objects\\PistonObjects\\piston_moving", 0, .5f, 0, .5f, false);
            pistonHead.Position = pistonHeadPosition;
            AddObject(pistonHead);
            */

            piston = new PistonObject(World, .5f, .5f, 12f, 13f, 9.7f, 12.6f, .01f, .2f, pistonPosition);
            piston.Position = pistonPosition;
          //  AddObject(piston);

            //private static Vector2 pistonPosition = new Vector2(12f, 13.2f);

            bottom1 = new BoxObject(World, "bottomTexture2273", 0, .5f, 0,1,false);
            bottom1.Position = bottom1Position;
            //AddObject(bottom1);

            bottom2 = new BoxObject(World, "bottomTexture1636", 0, .5f, 0,1,false);
            bottom2.Position = bottom2Position;
           // AddObject(bottom2);            
          

            //omgar its a ceiling!!
            top = new BoxObject(World, "Barrier1", 0, .5f, 0,1,false);
            top.Position = topPosition;
            AddObject(top);
            /*
            top = new BoxObject(World, bottomTexture, 0, .5f, 0,1,false);
            top.Position = topPosition + new Vector2(20.3f, 0f);
            AddObject(top);

            top = new BoxObject(World, bottomTexture, 0, .5f, 0,1,false);
            top.Position = topPosition + new Vector2(40.6f, 0f);
            AddObject(top);

            top = new BoxObject(World, bottomTexture, 0, .5f, 0,1,false);
            top.Position = topPosition + new Vector2(60.9f, 0f);
            AddObject(top);*/

            table = new CircleObject(World, "table", 1f, .5f, 0,.3f);
            table.Position = tablePosition;
          //  AddObject(table);

            fan1 = new AnimationObject(World, "fan_strip", "fan", 200, 200, 20, 7);
            fan1.Position = fan1Position;
           // AddObject(fan1);

            //hole1 = new HoleObject(World, "big_hole_strip", "hole_tile_sealed");
            hole1 = new HoleObject(World, "Art\\Objects\\HoleObjects\\hole_strip", "Art\\Objects\\HoleObjects\\hole");
            hole1.Position = hole1Position;
          //  AddObject(hole1);
            
            //movPlatform1 = new MovingObject(World, "moving platform", 0, .5f, 0,1,false);
            //movPlatform2 = new BoxObject(World, movingPlatformTexture, 0, .5f, 0);          
            //movPlatform1.Position = movPlatform1Position;
            //AddObject(movPlatform1);
            //movPlatform1.Position = movPlatform1Position;
            //AddObject(movPlatform1);

            //DEBUG
            /*
            littleBox = new BoxObject(World, littleBoxTexture, 10f, .1f, 0, 1, false);
            littleBox.Position = new Vector2(16.7f, 1.46f);// dudePosition + new Vector2(0, -5f); 
            AddObject(littleBox);
            
            PulleyJointDef jointDef2 = new PulleyJointDef();
            Box2DX.Common.Vec2 anchora = movPlatform1.Body.GetWorldCenter();
            Box2DX.Common.Vec2 anchorb = littleBox.Body.GetWorldCenter();
            Box2DX.Common.Vec2 groundAnchora = movPlatform1.Body.GetWorldCenter() + Utils.Convert(new Vector2(0, -5f));
            Box2DX.Common.Vec2 groundAnchorb = littleBox.Body.GetWorldCenter() + Utils.Convert(new Vector2(0, -5f));
            jointDef2.Initialize(movPlatform1.Body, littleBox.Body, groundAnchorb, groundAnchorb, anchora, anchorb, 1f);

            jointDef2.Length1 = 4f;
            jointDef2.Length2 = 4f;
            //jointDef1.MaxLength1 = 2.65f;
            //jointDef2.MaxLength2 = 4f;
            World.CreateJoint(jointDef2);
            */
            // END DEBUG
            
            //public AnimationObject( World world, Texture2D mytexture, Texture2D objectTexture, int sprWidth, int sprHeight, int animInt, int myNumFrames)

            /*
            Vector2 seesawposition = new Vector2(5f,8f);
            SeeSawObject seesaw = new SeeSawObject(World, "straight_pipe",.4f,seesawposition );
            seesaw.Position = seesawposition;
            AddObject(seesaw);
            
            
            RevoluteJointDef jointDef2 = new RevoluteJointDef();
            jointDef2.Initialize( World.GetGroundBody(),seesaw.Body, Common.Utils.Convert(seesaw.Position));
            Console.WriteLine(Common.Utils.Convert(seesaw.Position));
            //jointDef.EnableMotor = true;
            //jointDef.MotorSpeed = 1f;
            World.CreateJoint(jointDef2);
            */

            brokenMovingPlatform1 = new SwitchObject(World, "Art\\Objects\\SwitchObjects\\button_strip", "Art\\Objects\\SwitchObjects\\button", 181, 84, 20, 2);
            brokenMovingPlatform1.Position = brokenMovingPlatform1Position;
         //   AddObject(brokenMovingPlatform1);

            movPlatform1 = new MovingObject(World, "Art\\Objects\\MovingPlatformObjects\\moving_platform", 1000f, .5f, 0, 1, false, brokenMovingPlatform1, new Vector2(0, -11500), 4.5f, 14.2f);
            movPlatform2 = new HorizontalMovingObject(World, "Art\\Objects\\HorizontalMovingPlatformObjects\\moving_platform", 0f, 0.5f, 0, 1, false, null, new Vector2(0, -11500), 32f, 38f);
            //movPlatform2 = new BoxObject(World, movingPlatformTexture, 0, .5f, 0);          
            //movPlatform1.Position = movPlatform1Position;
            //AddObject(movPlatform1);
            //movPlatform1.Body.SetBullet(true);
            movPlatform1.Position = movPlatform1Position;
            movPlatform2.Position = movPlatform2Position;
          //  AddObject(movPlatform1);
          //  AddObject(movPlatform2);

            lamp1 = new AnimationObject(World, "light_strip", "light", 312, 120, 20, 8);
            lamp1.Position = lamp1Position;
           // AddObject(lamp1);


	    /*
            platform = new BoxObject(World, "platformTexture", 0, .1f, 0);
            platform.Position = platformPosition;
            AddObject(platform);

            bottom = new BoxObject(World, "bottomTexture", 0, .5f, 0);
            bottom.Position = bottomPosition;
            AddObject(bottom);
	    */

            // Create laser
            laser = new LaserObject(World, dude, "Art\\spray2_strip", 10);


            //creating insta steel already in the level

            //group1

            
            float startx = .3f, endx = 1.5f, starty = 4f, spacing = .5f;
            List<Vector2> blobs = new List<Vector2>();
            blobs.Add(new Vector2(startx, starty));
            blobs.Add(new Vector2(endx,starty));
            //AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, blobs));
            //AddObject(new PaintedObject(World, "paint", "paintedsegment", blobs));
            blobs.Clear();
            blobs.Add(new Vector2(startx, starty + .2f));
            blobs.Add(new Vector2(endx, starty + .2f));
            //AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, blobs));
            //AddObject(new PaintedObject(World, "paint", "paintedsegment", blobs));
            blobs.Clear();
            blobs.Add(new Vector2(startx, starty + .4f));
            blobs.Add(new Vector2(endx, starty + .4f));
            //AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, blobs));
            //AddObject(new PaintedObject(World, "paint", "paintedsegment", blobs));
            blobs.Clear();
            blobs.Add(new Vector2(startx, starty + .6f));
            blobs.Add(new Vector2(endx, starty + .6f));
            //AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, blobs));
            //AddObject(new PaintedObject(World, "paint", "paintedsegment", blobs));
            blobs.Clear();

            /* a TON of insta-steel
            blobs.Add(new Vector2(startx, starty + .6f));
            blobs.Add(new Vector2(endx, starty + .6f));
            //AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, blobs));
            kaintedObject po = new PaintedObject(World, "paint", "paintedsegment", blobs);
            po.Length = 200000; // DEBUG
            AddObject(po);
            blobs.Clear();
            */

            /*
            float startx = .3f, endx = 1.5f, starty = 4f, spacing = .5f;
            List<Vector2> blobs = new List<Vector2>();
            for (float x = startx; x <= endx; x += spacing)
            {
                blobs.Add(new Vector2(x, starty));
            }
            AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, blobs));

            blobs.Clear();
           
            for (float x = startx; x <= endx; x += spacing)
            {
                blobs.Add(new Vector2(x, starty+.2f));
            }
            AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, blobs));

            blobs.Clear();

            for (float x = startx; x <= endx; x += spacing)
            {
                blobs.Add(new Vector2(x, starty+.4f));
            }
            AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, blobs));

            blobs.Clear();

            for (float x = startx; x <= endx; x += spacing)
            {
                blobs.Add(new Vector2(x, starty+.6f));
            }
            AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, blobs));
            */
            //group2
            startx = 3.5f; endx = 5.2f; starty = 13f;
            blobs.Add(new Vector2(startx, starty));
            blobs.Add(new Vector2(endx, starty));
            //AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, blobs));
      //      AddObject(new PaintedObject(World, "paint", "paintedsegment", blobs));
            blobs.Clear();
            blobs.Add(new Vector2(startx, starty + .2f));
            blobs.Add(new Vector2(endx, starty + .2f));
            //AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, blobs));
      //      AddObject(new PaintedObject(World, "paint", "paintedsegment", blobs));
            blobs.Clear();
            blobs.Add(new Vector2(startx, starty + .4f));
            blobs.Add(new Vector2(endx, starty + .4f));
            //AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, blobs));
     //       AddObject(new PaintedObject(World, "paint", "paintedsegment", blobs));
            blobs.Clear();
            blobs.Add(new Vector2(startx, starty + .6f));
            blobs.Add(new Vector2(endx, starty + .6f));
            //AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, blobs));
       //     AddObject(new PaintedObject(World, "paint", "paintedsegment", blobs));
            /*
            startx = 3.5f; endx = 5.2f; starty = 13f;
            blobs.Clear();
            for (float x = startx; x <= endx; x += spacing)
            {
                blobs.Add(new Vector2(x, starty));
            }
            AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, blobs));

            blobs.Clear();

            for (float x = startx; x <= endx; x += spacing)
            {
                blobs.Add(new Vector2(x, starty + .2f));
            }
            AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, blobs));

            blobs.Clear();

            for (float x = startx; x <= endx; x += spacing)
            {
                blobs.Add(new Vector2(x, starty + .4f));
            }
            AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, blobs));

            blobs.Clear();

            for (float x = startx; x <= endx; x += spacing)
            {
                blobs.Add(new Vector2(x, starty + .6f));
            }
            AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, blobs));
          */


            /*
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
                    Vector2 addition = getGameCoords(new Vector2(x, y));
                    Console.WriteLine("{0}", addition);
                    blobs.Add(addition);
                    i++;
                }
                i++;
            }
            //Console.WriteLine("{0}", blobs.Count);
            //AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, blobs));
            AddObject(new PaintedObject(World, "paint", "paintedsegment", blobs));
            */

            // Create rope bridge
            //AddObject(new RopeBridge(World, ropeBridgeTexture, 8.1f, 5.5f, 11.5f, 1, 0, 0));

            // Create spinning platform
            /** BoxObject spinPlatform = new BoxObject(World, barrierTexture, 25,0,0);
             spinPlatform.Position = spinPlatformPos;
             AddObject(spinPlatform);
            
             // Create a joint to affix the platform to the world.
             ////////////////////////////////////////////
            RevoluteJointDef joint = new RevoluteJointDef();
            joint.Initialize(spinPlatform.Body, World.GetGroundBody(), Common.Utils.Convert(spinPlatform.Position));
            World.CreateJoint(joint); */

            ////////////////////////////////////////////


            World.SetContactListener(new PlatformContactListener(this));
            World.SetBoundaryListener(new PlatformBoundaryListener(this));

            paintTexture = GameEngine.TextureList["paint"];

            halfdotsize = new Vector2(paintTexture.Width / 2, paintTexture.Height / 2);


            //PLAYS THE SONG!!!  (It resets at the beginning of the level)
            AudioManager audio = GameEngine.AudioManager;
            //audio.Play(AudioManager.MusicSelection.Destruction);

        }

        public ScrollingWorld(string savedroomname, bool loadingWorldFromFile)
            : base(WIDTH, HEIGHT, new Vector2(0, GRAVITY))
        {
            SavedRoom sr = Serializer.DeSerialize(savedroomname);
            World = sr.world;
            backgroundName = sr.backgroundName; // "Art\\Backgrounds\\" + backgroundname;
            Objects = sr.objects;
            this.World.Gravity = Utils.Convert(new Vector2(0, GRAVITY)); // RESET GRAVITY
            foreach (PhysicsObject obj in sr.objects)
            {
                //obj.AddToWorld();
                //obj.SetupJoints(this.World);
                obj.world = this.World;
                //obj.Body.SetWorld(this.World);
                //obj.AddToWorld();
                //Objects.Add(obj);
                //AddObject(obj);
                List<Body> tobedeleted = new List<Body>();
                for (Body bd = World.GetBodyList(); bd != null; bd = bd.GetNext() )
                {
                    if (!Objects.Contains(bd.GetUserData()))
                    {
                        //World.DestroyBody(bd);
                        //tobedeleted.Add(bd);
                        World.DestroyBody(bd);
                    }
                    else if (bd.GetUserData() is DudeObject || bd.GetUserData() is WinDoorObject)
                    {
                        World.DestroyBody(bd);
                    }
                }
                //foreach (Body bd in tobedeleted)
                    //World.DestroyBody(bd);
            }

            musicName = sr.musicName;
            background = GameEngine.TextureList[backgroundName];

            gameLevelWidth = background.Width;
            gameLevelHeight = background.Height;

            cursorSrcRect = new Rectangle(cursorWidth, cursorWidth, cursorWidth, cursorWidth);
            numDrawLeft = 0; // reset the amount of instasteel when loading the level
            totalInstaSteelInWorld = 0;

            winDoor = new WinDoorObject(World, "door_strip", "WinDoor", 93, 99, 20, 5);
            winDoor.Position = sr.winDoor.Position; // winDoorPos;
            winDoor.world = this.World;
            AddObject(winDoor);

            // Create laser
            laser = new LaserObject(World, dude, "paintedsegment", 10);

            dude = new DudeObject(World, this, "newDudeFilmstrip", "Dude", "arm", laser, dudeSensorName);
            dude.Position = sr.dude.Position; // dudePosition;
            dude.world = this.World;
            AddObject(dude);

            laser = new LaserObject(World, dude, "Art\\spray2_strip", 10);

            World.SetContactListener(new PlatformContactListener(this));
            World.SetBoundaryListener(new PlatformBoundaryListener(this));

            paintTexture = GameEngine.TextureList["paint"];

            halfdotsize = new Vector2(paintTexture.Width / 2, paintTexture.Height / 2);


            //PLAYS THE SONG!!!  (It resets at the beginning of the level)
            AudioManager audio = GameEngine.AudioManager;
            //audio.Play(AudioManager.MusicSelection.Destruction);
        }

        public void reloadNonSerializedAssets()
        {
            paintTexture = GameEngine.TextureList["paint"];
            AudioManager audio = GameEngine.AudioManager;
            //audio.Play(AudioManager.MusicSelection.Basement);
            audio.Play(musicName);
            //Console.WriteLine(musicName);
            background = GameEngine.TextureList[backgroundName];
            holeList = new List<HoleObject>();
            foreach (PhysicsObject obj in Objects)
            {
                if (obj is PaintedObject)
                {
                    totalInstaSteelInWorld += ((PaintedObject)obj).getAmountOfInstasteel();
                }
                if (obj is HoleObject)
                {
                    holeList.Add((HoleObject)obj);
                }
                if (obj is WinDoorObject)
                {
                    winDoor = (WinDoorObject)obj;
                }
                if (obj is DudeObject)
                {
                    dude = (DudeObject)obj;
                }
            }
            halfdotsize = new Vector2(paintTexture.Width / 2, paintTexture.Height / 2);
            numDrawLeft = 0; // reset the amount of instasteel we have when we load the level

            GameEngine.resetVictoryAnimation();
        }

        // NO LONGER USED. TALK TO DANIEL
        public static void LoadContent(ContentManager content)
        {
            // all of this is obsoleted by GameEngine.TextureList

            groundTexture = content.Load<Texture2D>("EarthTile02");
            //dudeTexture = content.Load<Texture2D>("Dude");
            dudeTexture = content.Load<Texture2D>("newDudeFilmstrip");
            armTexture = content.Load<Texture2D>("arm");
            dudeObjectTexture = content.Load<Texture2D>("DudeObject");
            winTexture = content.Load<Texture2D>("WinDoor");
            winDoorAnimTexture = content.Load<Texture2D>("door_strip");
            //ropeBridgeTexture = content.Load<Texture2D>("RopeBridge");
            barrierTexture = content.Load<Texture2D>("Barrier");
            barrierTexture2 = content.Load<Texture2D>("Barrier1");
            //paintTexture = content.Load<Texture2D>("paint");
            paintTexture = content.Load<Texture2D>("paint");
            paintedSegmentTexture = content.Load<Texture2D>("paintedsegment");
            crosshairTexture = content.Load<Texture2D>("Crosshair");
            background = content.Load<Texture2D>("background");
            backgroundTerrible = content.Load<Texture2D>("RonniesBestWork");
            ballroomBackground = content.Load<Texture2D>("ballroom");

            /* //our new platforms
             bigBoxTexture = content.Load<Texture2D>("bigBoxTexture");
             */
             littleBoxTexture = content.Load<Texture2D>("littleBoxTexture");
            /*
             leftPipeTexture = content.Load<Texture2D>("leftPipeTexture");
             rightPipeTexture = content.Load<Texture2D>("rightPipeTexture");
            */
             platformTexture = content.Load<Texture2D>("platformTexture");
            bottom1Texture = content.Load<Texture2D>("bottomTexture2273");
            bottom2Texture = content.Load<Texture2D>("bottomTexture1636");


            holeTexture = content.Load<Texture2D>("big_hole_strip");
            holeObjTexture = content.Load<Texture2D>("hole_tile_sealed");

            movingPlatformTexture = content.Load<Texture2D>("moving platform");
            brokenMovingPlatformTexture = content.Load<Texture2D>("broken_moving_platform");
            brokenMovingPlatformAnimTexture = content.Load<Texture2D>("broken_strip");

            straightPipeTexture = content.Load<Texture2D>("straight_pipe");
            straightPipeTileTexture = content.Load<Texture2D>("straight_pipe_tile");
            groundPlatformTexture = content.Load<Texture2D>("ground_like_platform");
            pulleyPlatformTexture = content.Load<Texture2D>("pulley_platform");
            pulleyPlatformLongTexture = content.Load<Texture2D>("pulley_platform_long");
            pulleyChainTexture = content.Load<Texture2D>("pulleytrack");
            pistonAssemblyTexture = content.Load<Texture2D>("piston_unmoving");
            pistonHeadTexture = content.Load<Texture2D>("piston_moving");
            tableTexture = content.Load<Texture2D>("table");

            lampTexture = content.Load<Texture2D>("light");
            lampAnimTexture = content.Load<Texture2D>("light_strip");

            laserAnimTexture = content.Load<Texture2D>("Art\\spray2_strip");
       

            pipeAssemblyTexture = content.Load<Texture2D>("pipe_steam_part");
            window1Texture = content.Load<Texture2D>("window1");
            window2Texture = content.Load<Texture2D>("window2");
            window3Texture = content.Load<Texture2D>("window3");
            window4Texture = content.Load<Texture2D>("window4");
            window5Texture = content.Load<Texture2D>("window5");

            fanAnimTexture =  content.Load<Texture2D>("fan_strip");
            fanTexture = content.Load<Texture2D>("fan");
       
            switchAnimTexture = content.Load<Texture2D>("Art\\Objects\\SwitchObjects\\button_strip");
            switchDeathAnimTexture = content.Load<Texture2D>("Art\\Objects\\SwitchObjects\\button_death_strip");
            switchObjectTexture = content.Load<Texture2D>("Art\\Objects\\SwitchObjects\\button");
            rollingpinAnimTexture = content.Load<Texture2D>("Art\\Objects\\AnimationObjects\\rollingpin_strip");
            rollingpinTexture = content.Load<Texture2D>("Art\\Objects\\AnimationObjects\\rollingpin");
            deathObjectTexture = content.Load<Texture2D>("Art\\Objects\\SwitchObjects\\death");
            failButtonObjectTexture = content.Load<Texture2D>("Art\\Objects\\SwitchObjects\\fail_button");

            holeAnimTexture = content.Load<Texture2D>("Art\\Objects\\HoleObjects\\hole_strip");
            holeObjectTexture = content.Load<Texture2D>("Art\\Objects\\HolehObjects\\hole");

            movingObjectTexture = content.Load<Texture2D>("Art\\Objects\\MovingPlatformObjects\\moving_platform");
            movingObjectTexture = content.Load<Texture2D>("Art\\Objects\\MovingPlatformObjects\\switchmoving_platform");

            horizontalmovingObjectTexture = content.Load<Texture2D>("Art\\Objects\\HorizontalMovingPlatformObjects\\moving_platform");
            horizontalmovingObjectTexture = content.Load<Texture2D>("Art\\Objects\\MovingPlatformObjects\\switchmoving_platform");
        }





        public Vector2 getScreenOrigin()
        {
            if (dude.Position.X * CASSWorld.SCALE <= SCREEN_WIDTH / 2)
                return new Vector2(0, 0);
            else if (dude.Position.X * CASSWorld.SCALE >= gameLevelWidth - SCREEN_WIDTH / 2)
            {
                return new Vector2((gameLevelWidth - SCREEN_WIDTH) / CASSWorld.SCALE, 0);
            }
            else
            {
                return new Vector2(dude.Position.X - ((SCREEN_WIDTH / 2) / CASSWorld.SCALE), 0);
            }

        }

        //for Game to screen, subtract getscreenorigin from your game coords, and then multiply by scale
        public Vector2 getScreenCoords(Vector2 myGameCoords)
        {

            return (myGameCoords - getScreenOrigin()) * CASSWorld.SCALE;

        }

        //for screen to game, use getScreenOrigin() plus scaled screen coords
        public Vector2 getGameCoords(Vector2 myScreenCoords)
        {
            return (getScreenOrigin() + (myScreenCoords / CASSWorld.SCALE));

        }

        public Vector2 getCameraCoords()
        {
            if (dude.Position.X * CASSWorld.SCALE <= SCREEN_WIDTH / 2)
            {
                return new Vector2(0, 0);
            }
            else if (dude.Position.X * CASSWorld.SCALE >= gameLevelWidth - SCREEN_WIDTH / 2)
            {
                return new Vector2((-gameLevelWidth + (SCREEN_WIDTH)), 0);
            }
            else
            {
                return new Vector2(-dude.Position.X * CASSWorld.SCALE + (SCREEN_WIDTH / 2), 0);
            }
        }

        public override void Simulate(float dt)
        {
            //pulleyPipe1.Position = new Vector2(16.8f,pulleyPipe1.Position.Y);
            //pulleyPipe2.Position = new Vector2(18.2f, pulleyPipe2.Position.Y);         


            //Console.WriteLine("{0}", getGameCoords(new Vector2(Mouse.GetState().X, Mouse.GetState().Y)));
            //ronnies 2 line attempt at fixing drawing
            //float guyPos = -dude.Position.X * CASSWorld.SCALE + (GameEngine.GAME_WINDOW_WIDTH / 2);
            // screenOffset = new Vector2(guyPos, 0);

            //hackish code to make the platform move
            /*
            if (movPlat)
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
            //if (mov==false)
            //{ movPlat2 = true; }
              */




          

            //Hackie Moving Platform Code
            /*
            if (movPlat1 == true)
            {

                movPlatform2.Position = movPlatform2.Position + new Vector2(.035f, 0);
               // dude.Position = dude.Position + new Vector2(0.05f, 0);
                if (movPlatform2.Position.X > 36){

                   
                    movPlat1 = false;
                  
                }

            }
            else
            {

                movPlatform2.Position = movPlatform2.Position - new Vector2(.035f, 0);
             //   dude.Position = dude.Position - new Vector2(0.05f, 0);
                if (movPlatform2.Position.X < 34f)
                {
                    
                    movPlat1 = true;
                }
            }
            */





            //new Vector2(14.5f, 13.3f);
            /*
            if (pistonMove == true)
            {
                pistonHead.Position = pistonHead.Position - new Vector2(.01f, 0);
                if (pistonHead.Position.X < 12f)
                    pistonMove = false;
            }
            else
            {
                pistonHead.Position = pistonHead.Position + new Vector2(.2f, 0);                
                if (pistonHead.Position.X > 13f)
                    pistonMove= true;
            }
            */

            dude.Grounded = false; // unrelated to the following
            dude.OnSlope = false; // unrelated to the following

            // code for erasing a painted object
            MouseState mouse = Mouse.GetState();
            bool mouseinbounds = mouse.X > 0 && mouse.X < GameEngine.SCREEN_WIDTH && mouse.Y < GameEngine.SCREEN_HEIGHT && mouse.Y > 0;
            Vector2 scaledMousePosition = new Vector2(mouse.X / CASSWorld.SCALE, mouse.Y / CASSWorld.SCALE);
            Vector2 mouseGamePosition = getScreenOrigin() + scaledMousePosition;
            //Console.WriteLine("{0} {1} {2} {3}", dude.Position.X, dude.Position.Y, mouseGamePosition.X, mouseGamePosition.Y);
            AudioManager audio = GameEngine.AudioManager;
            
            //ERASING
            if (mouse.RightButton == ButtonState.Pressed && laser.canErase())
            {
                //ronnie added for laser
                laser.startErasing();



                // if the right button is pressed, remove any painted objects under the cursor from the world
                // Query a small box around the mouse
                AABB aabb = new AABB();
                aabb.LowerBound = Common.Utils.Convert(mouseGamePosition - new Vector2(0.1f));
                aabb.UpperBound = Common.Utils.Convert(mouseGamePosition + new Vector2(0.1f));

                Shape[] shapes = new Shape[1];
                int nHit = World.Query(aabb, shapes, 1);

                if (nHit > 0)
                {
                    Body body = shapes[0].GetBody();
                    PhysicsObject po = (PhysicsObject)body.GetUserData();
                    if (po is PaintedObject)
                    {
                        this.RemoveObject(po);
                        //Implementing deconstruct sfx
                        audio.Play(CrisisAtSwissStation.AudioManager.SFXSelection.Deconstruct);
                        PaintedObject painto = (PaintedObject)po;
                        numDrawLeft += painto.getAmountOfInstasteel();
                    }
                }
                // laser.finishErasing();
            }
            else
            {
                laser.finishErasing();
            }

            if (mouse.LeftButton == ButtonState.Released && laser.canDraw())
                drawingInterrupted = false;

            if (mouse.LeftButton == ButtonState.Pressed && laser.canDraw() && !drawingInterrupted && mouseinbounds && numDrawLeft > PAINTING_GRANULARITY && lengthCurDrawing < drawLimit)
            {
                //random ronnie addition for laser
                laser.startDrawing();
                
               
                /* Still working on this nasty loop
                audio.Play(AudioManager.SFXSelection.Charge); */

                // if we're holding down the mouse button
                //Vector2 mousepos = new Vector2(mouse.X, mouse.Y);
                
                if (dotPositions.Count == 0 || ( getScreenCoords(mouseGamePosition) - getScreenCoords(dotPositions[dotPositions.Count - 1])).Length() > PAINTING_GRANULARITY)
                { // according to the granularity constraint for paintings,
                    float drawDist = 0;
                    Vector2 drawPos = mouseGamePosition;

                    // make sure there is enough insta-steel to make it to the next point. if not, adjust the point
                    if (dotPositions.Count > 0)
                    {
                        drawDist = Vector2.Distance(drawPos * CASSWorld.SCALE, prevMousePos);
                        if (drawDist > numDrawLeft)
                        {
                            Vector2 relativePos = prevMousePos - (drawPos * CASSWorld.SCALE);
                            relativePos = relativePos * (numDrawLeft / drawDist);
                            drawPos = (prevMousePos - relativePos) / CASSWorld.SCALE;
                            drawDist = numDrawLeft;
                        }
                    }
                    dotPositions.Add(drawPos);
                    if (dotPositions.Count == 1)
                    {
                        lengthCurDrawing = 0;
                    }
                    else
                    {
                        numDrawLeft -= drawDist;
                        lengthCurDrawing += drawDist;
                    }
                    
                    prevMousePos = drawPos * CASSWorld.SCALE;
                    finishDraw = true;
                }

                
                //other random ronnie addition for laser
                //laser.finishDrawing();

            }
            else if (((mouse.LeftButton == ButtonState.Released || !laser.canDraw() || (mouseWasInbounds && prevms.LeftButton == ButtonState.Pressed)) && (numDrawLeft > PAINTING_GRANULARITY || finishDraw)) && (prevms.LeftButton == ButtonState.Pressed || drawingInterrupted) && (mouseinbounds || mouseWasInbounds))
            {
                if (!laser.canDraw())
                    drawingInterrupted = true;
                Box2DX.Dynamics.Body overlapped = null;
                PhysicsObject po = null;
                foreach (Vector2 pos in dotPositions)
                {
                    // Query a small box around the mouse
                    AABB aabb = new AABB();
                    //Vector2 gamepos = new Vector2(pos.X / CASSWorld.SCALE, pos.Y / CASSWorld.SCALE) + screenOffset;
                    //Vector2 gamepos = new Vector2(pos.X, pos.Y);
                    aabb.LowerBound = Common.Utils.Convert(pos - new Vector2(0.1f));
                    aabb.UpperBound = Common.Utils.Convert(pos + new Vector2(0.1f));

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

                if (dotPositions.Count > 1)
                {
                    audio.Play(AudioManager.SFXSelection.Construct);
                }

                laser.finishDrawing();

                List<Vector2> dp2 = new List<Vector2>();
                // hack to make the drawing fit the offset
                //foreach (Vector2 pos in dotPositions)
                //{
                //    //Console.WriteLine(dude.Position.X * CASSWorld.SCALE);
                //    dp2.Add(pos + new Vector2(dude.Position.X * CASSWorld.SCALE, 0));
                //}

                // create the painting as an object in the world
                if (dotPositions.Count > 1)
                    //this.AddObject(new PaintedObject(World, paintTexture, paintedSegmentTexture, dp2));
                    this.AddObject(new PaintedObject(World, "paint", "paintedsegment", dotPositions));
                // clear the way for another painting
                dotPositions = new List<Vector2>(); // 
                lengthCurDrawing = 0;
                finishDraw = false;
            }
            // end painting code (except for prevms = ms below)

            prevms = mouse;
            mouseWasInbounds = mouseinbounds;

            laser.Update(scaledMousePosition.X, scaledMousePosition.Y, getCameraCoords(), gameLevelWidth);

            // update cursor insta-steel level
            float fractionInstasteel = numDrawLeft / totalInstaSteelInWorld;
            int cursorPic = (int)(fractionInstasteel * 8);
            
            cursorSrcRect = new Rectangle(cursorPic * cursorWidth, 0, cursorWidth, cursorWidth);

            base.Simulate(dt);
            screenOffset = new Vector2(0, 0); // TODO Diana: Change this!

            allHolesFilled = true;

            foreach (HoleObject hole in holeList)
            {
                allHolesFilled = allHolesFilled && (hole.Filled >= HoleObject.MAX_FILL);
            }
        }

        public Texture2D Background
        {
            get
            {
                return background;
            }
            set
            {
                background = value;
            }
        }

        /**
         * Listens to contacts to apply demo level-specific things
         */
        [Serializable]
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

                // set the normal vector from the object to the dude sensor - used for jumping
                if (!(
                    (shape1.GetBody().GetUserData() == world.dude && (ScrollingWorld.dudeSensorName).Equals(shape2.UserData))
                    ||
                    (shape2.GetBody().GetUserData() == world.dude && (ScrollingWorld.dudeSensorName).Equals(shape1.UserData))
                    ))
                {
                    if ((ScrollingWorld.dudeSensorName).Equals(shape1.UserData))
                    {
                        Vector2 b2dnormal = -Utils.Convert(point.Normal);
                        b2dnormal.Normalize();
                        world.dude.Normal = b2dnormal;
                    }
                    else if ((ScrollingWorld.dudeSensorName).Equals(shape2.UserData))
                    {
                        Vector2 b2dnormal = Utils.Convert(point.Normal);
                        b2dnormal.Normalize();
                        world.dude.Normal = b2dnormal;
                    }
                }

                if ((ScrollingWorld.dudeSensorName+"SLOPE").Equals(shape2.UserData) || ScrollingWorld.dudeSensorName.Equals(shape2.UserData))
                {
                    Shape temp = shape1;
                    shape1 = shape2;
                    shape2 = temp;
                }
                if (ScrollingWorld.dudeSensorName.Equals(shape1.UserData) &&
                    (world.dude != shape2.GetBody().GetUserData()))
                {
                    world.dude.Grounded = true;
                }
                if ((ScrollingWorld.dudeSensorName + "SLOPE").Equals(shape1.UserData) &&
                    (world.dude != shape2.GetBody().GetUserData()) && (shape1.GetBody() != shape2.GetBody()))
                {
                    //Console.WriteLine(shape2.UserData); // DEBUG
                    world.dude.OnSlope = true;
                }
                Dictionary<String, List<PhysicsObject>> objsDict = new Dictionary<String, List<PhysicsObject>>();
                objsDict.Add("BoxObject", new List<PhysicsObject>()); 
                objsDict.Add("PolygonObject", new List<PhysicsObject>());
                objsDict.Add("CircleObject", new List<PhysicsObject>());
                objsDict.Add("DudeObject", new List<PhysicsObject>());
                objsDict.Add("PaintedObject", new List<PhysicsObject>());
                objsDict.Add("SensorObject", new List<PhysicsObject>());
                objsDict.Add("HoleObject", new List<PhysicsObject>());
                objsDict.Add("AnimationObject", new List<PhysicsObject>());
                objsDict.Add("MovingObject", new List<PhysicsObject>());
                objsDict.Add("HorizontalMovingObject", new List<PhysicsObject>());
                objsDict.Add("SwitchObject", new List<PhysicsObject>());
                objsDict.Add("DeathObject", new List<PhysicsObject>());
                objsDict.Add("FailButtonObject", new List<PhysicsObject>());
                objsDict.Add("WinDoorObject", new List<PhysicsObject>());
                objsDict.Add("PistonObject", new List<PhysicsObject>());
                
                foreach (PhysicsObject po in this.world.Objects)
                {
                    if (po is BoxObject)
                    {
                        objsDict["BoxObject"].Add(po);
                    }
                    else if (po is PolygonObject)
                    {
                        objsDict["PolygonObject"].Add(po);
                    }
                    else if (po is CircleObject)
                    {
                        objsDict["CircleObject"].Add(po);
                    }
                    
                    if (po is DudeObject)
                    {
                        objsDict["DudeObject"].Add(po);
                    }
                    else if (po is PaintedObject)
                    {
                        objsDict["PaintedObject"].Add(po);
                    }
                    else if (po is SensorObject)
                    {
                        objsDict["SensorObject"].Add(po);
                    }
                    else if (po is HoleObject)
                    {
                        objsDict["HoleObject"].Add(po);
                    }
                    else if (po is AnimationObject)
                    {
                        objsDict["AnimationObject"].Add(po);
                    }
                    else if (po is MovingObject)
                    {
                        objsDict["MovingObject"].Add(po);
                    }
                    else if (po is HorizontalMovingObject)
                    {
                        objsDict["HorizontalMovingObject"].Add(po);
                    }
                    else if (po is SwitchObject)
                    {
                        objsDict["SwitchObject"].Add(po);
                    }
                    else if (po is DeathPlatform)
                    {
                        objsDict["DeathObject"].Add(po);
                    }
                    else if (po is FailButtonObject)
                    {
                        objsDict["FailButtonObject"].Add(po);
                    }
                    else if (po is WinDoorObject)
                    {
                        objsDict["WinDoorObject"].Add(po);
                    }
                    else if (po is PistonObject)
                    {
                        objsDict["PistonObject"].Add(po);
                    }
                }
                foreach (PhysicsObject switchObj in objsDict["AnimationObject"])
                {
                    if ((object1 == switchObj && object2 == world.dude) ||
                        (object2 == switchObj && object1 == world.dude))
                    //(object1 == switchObj || object2 == switchObj)
                    {
                      AnimationObject temp = (AnimationObject)switchObj;
                      if (temp.checkDeathObject())
                        {

                           world.dude.setDead();
                          world.Fail();
                        }
                    }
                }
                
                foreach (PhysicsObject switchObj in objsDict["SwitchObject"])
                {
                    if ((object1 == switchObj && object2 == world.dude) ||
                        (object2 == switchObj && object1 == world.dude))
                        //(object1 == switchObj || object2 == switchObj)
                    {
                        ((SwitchObject)switchObj).switchOn = true;
                    }
                }
                foreach (PhysicsObject switchObj in objsDict["DeathObject"])
                {
                    if ((object1 == switchObj && object2 == world.dude) ||
                        (object2 == switchObj && object1 == world.dude))
                    {
                        ((DeathPlatform)switchObj).deadCosmo = true;
                        world.dude.setDead();
                        world.Fail();
                    }
                }
                foreach (PhysicsObject switchObj in objsDict["CircleObject"])
                {
                    if ((object1 == switchObj && object2 == world.dude) ||
                        (object2 == switchObj && object1 == world.dude))
                    {
                        if (((CircleObject)switchObj).TextureFilename.Equals("Art\\Objects\\CircleObjects\\gear"))
                        {
                            world.dude.setDead();
                            world.Fail();
                        }
                    }
                }
                foreach (PhysicsObject switchObj in objsDict["FailButtonObject"])
                {
                    if ((object1 == switchObj && object2 == world.dude) ||
                        (object2 == switchObj && object1 == world.dude))
                    {
                        ((FailButtonObject)switchObj).deadCosmo = true;
                        world.dude.setDead();
                        world.Fail();
                    }
                }

                foreach (PhysicsObject hMove in objsDict["HorizontalMovingObject"])
                {
                    if ((object1 == hMove && object2 == world.dude) ||
                        (object2 == hMove && object1 == world.dude))
                    {
                        if (world.dude.Grounded)
                        {
                            
                                if (((HorizontalMovingObject)hMove).mySwitch != null)
                                {
                                    if (((HorizontalMovingObject)hMove).mySwitch.switchOn)
                                    {
                                        if (((HorizontalMovingObject)hMove).isMoving)
                                        {
                                            world.dude.Body.ApplyForce(Utils.Convert(new Vector2(10.5f, 0)), world.dude.Body.GetWorldCenter());
                                            // world.dude.Position += new Vector2(.035f, 0);

                                        }

                                        else
                                        {
                                            world.dude.Body.ApplyForce(Utils.Convert(new Vector2(-10.5f, 0)), world.dude.Body.GetWorldCenter());
                                            // world.dude.Position += new Vector2(-0.05f, 0);
                                        }

                                    }
                                }
                                else
                                {

                                    if (((HorizontalMovingObject)hMove).isMoving)
                                    {
                                        world.dude.Body.ApplyForce(Utils.Convert(new Vector2(10.5f, 0)), world.dude.Body.GetWorldCenter());
                                        // world.dude.Position += new Vector2(.035f, 0);

                                    }

                                    else
                                    {
                                        world.dude.Body.ApplyForce(Utils.Convert(new Vector2(-10.5f, 0)), world.dude.Body.GetWorldCenter());
                                        // world.dude.Position += new Vector2(-0.05f, 0);
                                    }
                                }


                            
                        }
                    }
                }

                bool allHolesFilled = true;
                foreach (PhysicsObject hole in objsDict["HoleObject"])
                {
                    if (((HoleObject)hole).Filled < HoleObject.MAX_FILL)
                    {
                        allHolesFilled = false;
                    }
                }

                foreach (PhysicsObject door in objsDict["WinDoorObject"])
                {
                    if ((object1 == door && object2 == world.dude) ||
                        (object2 == door && object1 == world.dude))
                    {
                        if (allHolesFilled)
                        {
                            ((WinDoorObject)door).makeAnimate();
                            world.Win();
                        }
                    }
                }

                foreach (PhysicsObject piston in objsDict["PistonObject"])
                {
                    foreach (PhysicsObject circle in objsDict["CircleObject"])
                    {
                        if ((object1 == piston && object2 == circle) ||
                            (object1 == piston && object2 == circle))
                        {
                            if ((MathHelper.ToDegrees(object1.Angle) > -5) && (MathHelper.ToDegrees(object1.Angle) < 5))
                            { circle.Body.ApplyForce(Utils.Convert(new Vector2(500, 0)), circle.Body.GetWorldCenter()); }
                            else if ((MathHelper.ToDegrees(object1.Angle) > 85) && (MathHelper.ToDegrees(object1.Angle) < 95))
                            { circle.Body.ApplyForce(Utils.Convert(new Vector2(0, 500)), circle.Body.GetWorldCenter()); }
                            else if ((MathHelper.ToDegrees(object1.Angle) > -95) && (MathHelper.ToDegrees(object1.Angle) < -85))
                            { circle.Body.ApplyForce(Utils.Convert(new Vector2(0, -500)), circle.Body.GetWorldCenter()); }
                            else if ((MathHelper.ToDegrees(object1.Angle) > 175) && (MathHelper.ToDegrees(object1.Angle) < 185))
                            { circle.Body.ApplyForce(Utils.Convert(new Vector2(-500, 0)), circle.Body.GetWorldCenter()); }
                        }
                    }
                }
            }
        }

        private void DrawCrosshair()
        {
            MouseState mouse = Mouse.GetState();
            crosshairTexture = GameEngine.TextureList["Art\\crosshair_strip"]; // HACK HACK HACK handle this in a more sustainable way soon
            GameEngine.Instance.SpriteBatch.Draw(crosshairTexture, new Vector2(mouse.X, mouse.Y),
                cursorSrcRect, Color.White, 0, new Vector2(cursorWidth / 2, cursorWidth / 2), 1,
                SpriteEffects.None, 0);
        }

        public override void Draw(GraphicsDevice device, Matrix camera)
        {
            float guyPos = getCameraCoords().X;
            Matrix cameraTransform = Matrix.CreateTranslation(guyPos, 0.0f, 0.0f);

            GameEngine.Instance.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default,
                              RasterizerState.CullCounterClockwise, null, cameraTransform);
            GameEngine.Instance.SpriteBatch.Draw(background, new Vector2(0, 0), Color.White);

            //nonfunctional art stuff
            //GameEngine.Instance.SpriteBatch.Draw(brokenMovingPlatformTexture, new Vector2(.1f*CASSWorld.SCALE, 13.6f*CASSWorld.SCALE), Color.White);

            /* // DEBUG - replace these in a more sustainable way later
            GameEngine.Instance.SpriteBatch.Draw(pulleyChainTexture, new Vector2(16.2f * CASSWorld.SCALE, 4.5f * CASSWorld.SCALE), Color.White);
            GameEngine.Instance.SpriteBatch.Draw(pulleyChainTexture, new Vector2(17.6f * CASSWorld.SCALE, 9.7f * CASSWorld.SCALE), Color.White);
            GameEngine.Instance.SpriteBatch.Draw(pipeAssemblyTexture, new Vector2(0f * CASSWorld.SCALE, 5.9f * CASSWorld.SCALE), Color.White);
            GameEngine.Instance.SpriteBatch.Draw(window1Texture, new Vector2(5.5f * CASSWorld.SCALE, .5f * CASSWorld.SCALE), Color.White);
            GameEngine.Instance.SpriteBatch.Draw(window2Texture, new Vector2(25f * CASSWorld.SCALE, .5f * CASSWorld.SCALE), Color.White);
            GameEngine.Instance.SpriteBatch.Draw(window3Texture, new Vector2(38f * CASSWorld.SCALE, .5f * CASSWorld.SCALE), Color.White);
            GameEngine.Instance.SpriteBatch.Draw(window4Texture, new Vector2(51f * CASSWorld.SCALE, .5f * CASSWorld.SCALE), Color.White);
            GameEngine.Instance.SpriteBatch.Draw(pistonAssemblyTexture, new Vector2(9.7f * CASSWorld.SCALE, 12.6f * CASSWorld.SCALE), null, Color.White, 0, new Vector2(0, 0), .5f, SpriteEffects.None, 0);
            */

            //(texture, CASSWorld.SCALE * Position, null, Color.White, Angle, origin, scale, SpriteEffects.None, 0);
            GameEngine.Instance.SpriteBatch.End();

            base.Draw(device, cameraTransform);

            GameEngine.Instance.SpriteBatch.Begin();
            paintTexture = GameEngine.TextureList["paint"]; // HACK HACK HACK handle this in a more sustainable way soon
            DrawCrosshair();
            foreach (Vector2 dotpos in dotPositions)
            {
                GameEngine.Instance.SpriteBatch.Draw(paintTexture, getScreenCoords(dotpos) - halfdotsize - screenOffset, Color.Red);
            }
            GameEngine.Instance.SpriteBatch.End();

            laser.Draw();
            if (allHolesFilled)
            {
                winDoor.makeAnimate();
            }
        }

        /**
         * Returns the location of the dude, which should be
         * used as the center of the visible screen.
         */
        public override Vector2 GetVisualTargetPosition()
        {
            return dude.Position;
        }

        [Serializable]
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

                // code to fill up a hole
                int fillradius = 3;
                if (obj is PaintedObject)
                {
                    foreach (PhysicsObject hole in this.world.Objects)
                    {
                        if (hole is HoleObject && obj.Position.X < hole.Position.X + fillradius && obj.Position.X > hole.Position.X - fillradius )
                        {
                            ((HoleObject)hole).Filled += ((PaintedObject)obj).Length;
                            //Console.WriteLine("Filled: "); Console.WriteLine(((HoleObject)hole).Filled); // DEBUG
                        }
                    }
                }
                

                // code to kill the object that fell off, and to fail you if that object was you
                obj.Die();
                if (obj == world.dude)
                    world.Fail();
            }
        }

    }
}
