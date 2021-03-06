﻿using System;
using System.IO;
using System.Text.RegularExpressions; // For string tokenizing
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;

using Box2DX.Collision;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Color = System.Drawing.Color;

using CrisisAtSwissStation.Common;

using DrawPoint = System.Drawing.Point;
using XnaPoint = Microsoft.Xna.Framework.Point;
using DrawRect = System.Drawing.Rectangle;

using XnaRectangle = Microsoft.Xna.Framework.Rectangle;

namespace CrisisAtSwissStation.LevelEditor
{
    public partial class Editor : Form
    {
        //An enum representing the current state of the editor.
        private enum State
        {
            NO_EDITS, //Used when we don't have anything in the level editor
            EDITING_WORLD, //Used when we have a world loaded, as opposed to a room.
        }

        private State currentState = State.NO_EDITS;


        private string currentFileName = "";
        
        private EditorTool currentTool;

        private Image texture;
        private string textureDir;
        private ScrollingWorld world;

        //string currdir = CurrDirHack();
        string currdir = GameEngine.GetCurrDir() + "\\Content\\";

        //The object that the user just selected. May be null!
        //private SpaceObject currentlySelectedObject;
        private PhysicsObject currentlySelectedObject;

        //The box inside which the user must click in order to resize the object.
        private RectangleF resizeObjectBox;

        public Editor()
        {
            InitializeComponent();

            //Size pbLevelSize = new Size(GameEngine.SCREEN_WIDTH, GameEngine.SCREEN_HEIGHT);
            Size pbLevelSize = new Size(4096, 768);

            pb_Level.MaximumSize = (pb_Level.MinimumSize = (pb_Level.Size = pbLevelSize));
            this.FormBorderStyle = FormBorderStyle.FixedSingle;  

            currentTool = EditorTool.Insertion;
            ClearToolMenu();
            tool_Insertion.Checked = true;

            // Disable object properties by default
            tb_Density.Enabled = false;
            tb_Rotation.Enabled = false;
            b_ApplyProperties.Enabled = false;
            b_Front.Enabled = false;
            tb_Scale.Enabled = false;
            tb_bound1.Enabled = false;
            tb_bound2.Enabled = false;
            cBox_StaticObject.Enabled = false;

        }


        //------The following are callbacks for when the user selects a new type of object in the radio button
        // list. They populate the list of textures.
        private void rb_BoxObjects_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_BoxObjects.Checked)
            {
                textureDir = "Art\\Objects\\BoxObjects\\";
                PopulateTextureList(textureDir);
            }
        }

        private void rb_AnimationObjects_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_AnimationObjects.Checked)
            {
                /*
                textureDir = "Art\\Players\\";
                //PopulateTextureList(textureDir);

                this.lb_TextureList.Items.Clear();

                if (File.Exists(currdir + "\\Art\\Players\\player.png"))
                    lb_TextureList.Items.Add("player.png");

                if (lb_TextureList.Items.Count != 0)
                {
                    lb_TextureList.SelectedIndex = 0;
                }
                */
                textureDir = "Art\\Objects\\AnimationObjects\\";
                PopulateTextureList(textureDir);
            }
        }

        private void rb_WinDoorObject_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rb_PistonObject_CheckChanged(object sender, EventArgs e)
        {
            textureDir = "Art\\Objects\\PistonObjects\\";
            PopulateTextureList(textureDir);
        }
        
        private void rb_SeeSawObject_CheckedChanged(object sender, EventArgs e)
        {
            //if (rb_SeeSawObject.Checked)
            //{
            
                textureDir = "Art\\Objects\\SeeSawObjects\\";
                PopulateTextureList(textureDir);
            //}
        }

        private void rb_MovingPlatform_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_MovingPlatform.Checked)
            {
                textureDir = "Art\\Objects\\MovingPlatformObjects\\";
                PopulateTextureList(textureDir);
            }
        }

        private void rb_CircleObjects_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_CircleObjects.Checked)
            {
                textureDir = "Art\\Objects\\CircleObjects\\";
                PopulateTextureList(textureDir);
            }
        }

        private void rb_HorizontalMovingPlatform_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_HorizontalMovingPlatform.Checked)
            {
                textureDir = "Art\\Objects\\HorizontalMovingPlatformObjects\\";
                PopulateTextureList(textureDir);
            }
        }


        private void rb_SwitchObject_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_SwitchObject.Checked)
            {
                textureDir = "Art\\Objects\\SwitchObjects\\";
                PopulateTextureList(textureDir);
            }
        }

        private void rb_PaintedObjects_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_PaintedObjects.Checked)
            {
                textureDir = "Art\\Objects\\PaintedObjects\\";
                PopulateTextureList(textureDir);
            }
        }

        //This is slightly different than all the others. There is no "texture"
        //for doors, you see, so we're going to just add one item in the
        //list at the bottom, and make it selected.
        private void rb_Door_CheckedChanged(object sender, EventArgs e)
        {/*
            if (rb_HorizontalMovingPlatform.Checked)
            {
                lb_TextureList.Items.Clear();

                lb_TextureList.Items.Add("Door");

                lb_TextureList.SelectedIndex = 0;
            }
          */
        }

        private void rb_BackgroundObjects_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_BackgroundObjects.Checked)
            {
                textureDir = "Art\\Objects\\BackgroundObjects\\";
                PopulateTextureList(textureDir);
            }

        }

        private void rb_HoleObject_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_HoleObject.Checked)
            {
                textureDir = "Art\\Objects\\HoleObjects\\";
                PopulateTextureList(textureDir);
            }

        }

        private void rb_Vanish_Walls_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Vanish_Walls.Checked)
            {
                textureDir = "Art\\Static Walls\\";
                PopulateTextureList(textureDir);
            }
        }

        private void PopulateTextureList(string dir)
        {
            this.lb_TextureList.Items.Clear();

            DirectoryInfo di = new DirectoryInfo(currdir + "\\" + dir);
            FileInfo[] fileList = di.GetFiles("*.png");

            foreach (FileInfo file in fileList)
            {
                lb_TextureList.Items.Add(file.Name);
            }

            if (lb_TextureList.Items.Count != 0)
            {
                lb_TextureList.SelectedIndex = 0;
            }
        }
        //------End of the texture callbacks


        //----------------------------------------------------------------------------------------------------
        //These methods and variables deal with clicking on the paint area. If we are in insertion mode, and
        // the user left clicks, insert the object that is selected by the radio buttons to the left. 
        // If the user right clicks, then remove the object under the cursor if there is one. 
        // If we are in selection mode and the user clicks, select the object under the cursor.
        // If the user clicks and drags, move the selected object.

        private bool toDrag = false;
        private bool toResize = false;

        private int y_drag_transform = 0;
        private int x_drag_transform = 0;

        //Callback for when the user clicks on the painting area. We need to insert or select an object
        private void pb_Level_MouseClick(object sender, MouseEventArgs e)
        {

            if (currentState != State.NO_EDITS)
            {
                if (currentTool == EditorTool.Insertion)
                {
                    painting_clicked_insert(e);

                }
                else if (currentTool == EditorTool.Selection && !toResize)
                {

                    painting_clicked_select(e);

                }

                pb_Level.Refresh();
            }
        }

        //The callback for when the user presses down the mouse inside the painting area. This means the
        //user wants to start dragging an object.
        private void pb_Level_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && currentlySelectedObject != null)
            {
                bool isInResizeBox = resizeObjectBox.Contains(e.Location.X, e.Location.Y);

                if (isInResizeBox)
                {
                    toResize = true;
                }
                else
                {
                    x_drag_transform = (int)(e.X - (currentlySelectedObject.Position.X * CASSWorld.SCALE));
                    y_drag_transform = (int)(e.Y - (currentlySelectedObject.Position.Y * CASSWorld.SCALE));

                    toDrag = true;

                }
            }
        }

        //The user wants to stop dragging.
        private void pb_Level_MouseUp(object sender, MouseEventArgs e)
        {
            toDrag = false;
            toResize = false;
        }

        //The user may have dragged the object
        private void pb_Level_MouseMove(object sender, MouseEventArgs e)
        {
            if (currentlySelectedObject != null &&
                currentTool == EditorTool.Selection)
            {

                if (toDrag)
                {
                    //Update the object's position
                    int new_X = (e.Location.X - x_drag_transform);
                    int new_Y = (e.Location.Y - y_drag_transform);

                    currentlySelectedObject.Position =
                        new Vector2(new_X / CASSWorld.SCALE, new_Y / CASSWorld.SCALE);
                }
                else if (toResize)
                {
                    //Update the object's horizontal and vertical scaling.
                    //SCALING WILL ONLY WORK BEFORE ROTATING.  DO NOT SCALE AFTER ROTATING.  You have been warned.
                    /* Scaling removed for the moment... -- DANIEL
                    Vector2 pos = currentlySelectedObject.Position;

                    currentlySelectedObject.HorizontalScale =
                        Math.Abs(pos.X - e.Location.X) / currentlySelectedObject.getOriginalWidth();

                    currentlySelectedObject.VerticalScale =
                        Math.Abs(pos.Y - e.Location.Y) / currentlySelectedObject.getOriginalHeight();            
                    */
                }

                refreshResizeObjectBox();

                //Refresh the painting area so we can dynamic drawing
                pb_Level.Refresh();
            }
        }



        //End of the callbacks, beginning of the helper fuctions


        //Used when the user clicks in the painting area while using the select tool.
        // This will select the object who's center is closest to the mouse.
        private void painting_clicked_select(MouseEventArgs e)
        {

            XnaPoint mousePosition = Conversion.PointToPoint(pb_Level.PointToClient(MousePosition));
            currentlySelectedObject = null;

            //Find all possible objects that we might be selecting.
            List<PhysicsObject> candidates = new List<PhysicsObject>();

            foreach (PhysicsObject obj in world.Objects)
            {
                if (obj.getBBRelativeToWorld().Contains(mousePosition))
                {
                    candidates.Add(obj);
                }
            }

            //If the room is the currently selected room, the player is not null, and the click is close enough
            // to the player's location, add the player to the list of possible candidates.
            /*
            if(currentState == State.EDITING_WORLD && 
                currentlySelectedRoom == world.CurrentRoom &&
                world.player != null &&
                world.player.getBBRelativeToWorld().Contains(mousePosition))
            {
                candidates.Add(world.player);
            }
            */

            //Find the object whose center is closest to the current mouse position. If there is no 
            // possible selection, then the currentlySelectedObject continues to be null.
            foreach (PhysicsObject cand in candidates)
            {
                if (currentlySelectedObject == null ||
                        dist(mousePosition, cand.getCenterPoint()) < dist(mousePosition, currentlySelectedObject.getCenterPoint()))
                {

                    currentlySelectedObject = cand;

                    refreshResizeObjectBox();
                }
            }

            //If the currently selected item is a door, enable the door menu item. Otherwise, disable it.
            /*
            if (currentlySelectedObject != null &&
                currentlySelectedObject is Door)
            {

                menu_door.Enabled = true;
            }
            else
            {
                menu_door.Enabled = false;
            }
            */

            // Set fields in Object Properties panel
            if (currentlySelectedObject != null)
            {
                tb_Density.Enabled = true;
                tb_Rotation.Enabled = true;
                tb_bound1.Enabled = true;
                tb_bound2.Enabled = true;
                b_ApplyProperties.Enabled = true;
                b_Front.Enabled = true;
                tb_Scale.Enabled = true;
                cBox_StaticObject.Enabled = true;
                tb_Rotation.Text = (currentlySelectedObject.Angle * 180.0f / MathHelper.Pi).ToString();
                tb_Density.Text = currentlySelectedObject.Body.GetMass().ToString();
                tb_Scale.Text = currentlySelectedObject.scale.ToString();
                tb_bound1.Text = currentlySelectedObject.scale.ToString();
                tb_bound2.Text = currentlySelectedObject.scale.ToString();
                cBox_StaticObject.Checked = (currentlySelectedObject.Body.GetMass() == 0);

                /*
                if (currentlySelectedObject is HazardStatic)
                {
                    tb_Damage.Text = ((HazardStatic)currentlySelectedObject).Damage.ToString();
                    tb_Damage.Enabled = true;
                }
                else if (currentlySelectedObject is HazardDynamic)
                {
                    // Other 
                    tb_Damage.Text = ((HazardDynamic)currentlySelectedObject).Damage.ToString();
                    tb_Damage.Enabled = true;
                }
                else if (currentlySelectedObject is Survivor || currentlySelectedObject is VanishWall)
                {
                    tb_SLevel.Enabled = true;
                }
                */

                //SetScriptingFields();
            }
            else
            {
                // Clear all the properties fields
                tb_Density.Text = "";
                tb_Rotation.Text = "";
                tb_bound1.Text = "";
                tb_bound2.Text = "";
                tb_Scale.Text = "";
                b_ApplyProperties.Enabled = false;
                b_Front.Enabled = false;
                tb_Density.Enabled = false;
                tb_Rotation.Enabled = false;
                tb_bound1.Enabled = false;
                tb_bound2.Enabled = false;
                tb_Scale.Enabled = false;

                tb_Script.Text = "";
                tb_Script.Enabled = false;
                cbox_Scripted.Checked = false;
                cbox_Scripted.Enabled = false;
                cBox_StaticObject.Checked = false;
                cBox_StaticObject.Enabled = false;
            }
        }

        //The distance between two points.
        private double dist(XnaPoint p, XnaPoint center)
        {
            return Math.Sqrt(Math.Pow(p.X - center.X, 2) + Math.Pow(p.Y - center.Y, 2));
        }


        //Used when the user clicks in the painting area while using the insert tool.
        // This will create a new object when the user left-clicks, and delete the object
        // it is currently on when it is right clicked.
        private void painting_clicked_insert(MouseEventArgs e)
        {
            DrawPoint mouse = pb_Level.PointToClient(MousePosition);

            // Remove the object under the cursor.
            if (e.Button == MouseButtons.Right)
            {
                foreach (PhysicsObject obj in world.Objects.Reverse<PhysicsObject>())
                {

                    XnaRectangle bb = obj.getBBRelativeToWorld();

                    /*texture = Image.FromFile(currdir + "\\" + obj.TextureName + ".png");
                    if (mouse.X > obj.Position.X && mouse.X < (obj.Position.X + texture.Width) &&
                        mouse.Y > obj.Position.Y && mouse.Y < (obj.Position.Y + texture.Height))*/
                    if (bb.Contains(Conversion.PointToPoint(mouse)))
                    {
                        //world.Objects.Remove(obj);
                        if (!(obj is DudeObject))
                        {
                            world.RemoveObject(obj);
                        }

                        if (obj == currentlySelectedObject)
                        {
                            currentlySelectedObject = null;
                        }
                        break;
                    }
                }
            }

            // Add an object.
            if (e.Button == MouseButtons.Left)
            {
                object textureName = lb_TextureList.SelectedItem;

                if (textureName != null)
                {
                    //Console.WriteLine(currdir + "\\" + textureDir + textureName.ToString());
                    texture = Image.FromFile(currdir + "\\" + textureDir + textureName.ToString());

                    MakeSpaceObject(textureDir + textureName.ToString(), texture, mouse);
                }
            }

        }

        // Create object in room upon user click
        private void MakeSpaceObject(string texName, Image tex, DrawPoint mp)
        {
            texName = texName.Replace(".png", "");

            string[] lastname_ary= (string[])texName.Split('\\');
            string lastname = lastname_ary[lastname_ary.Length-1];

            string texStripName = texName.Replace(lastname,"strips\\"+lastname) + "_strip";

            //int numberOfFrames = SpaceObject.GetNumberOfFrames(texName);

            Vector2 screenposition = Conversion.DrawPointToVector2(mp);
            Vector2 gameposition = new Vector2(screenposition.X / CASSWorld.SCALE, screenposition.Y / CASSWorld.SCALE);

            if (rb_AnimationObjects.Checked)
            {
                AnimationObject ao;
                // TODO : i'm not a fan of these ifs, since the only thing that's changing are those last two numbers
                //        having to do with the animation. is there a way they could be passed in, or even better,
                //        derived from the strip image?
                if (lastname == "fan")
                    ao = new AnimationObject(world.World, texStripName, texName, tex.Width, tex.Height, 20, 7);
                else if (lastname == "broken_platform")
                    ao = new AnimationObject(world.World, texStripName, texName, tex.Width, tex.Height, 20, 8);
                else if (lastname == "light" || lastname == "fire")
                {
                    ao = new AnimationObject(world.World, texStripName, texName, tex.Width, tex.Height, 20, 8,false, false);
                    //ao.RemoveFromWorld();
                    //world.World.DestroyBody(ao.Body);
                }
                else if (lastname == "rollingpin")
                {
                    ao = new AnimationObject(world.World, texStripName, texName, tex.Width, tex.Height, 20, 8, true, true);
                }
                else
                {
                    ao = new AnimationObject(world.World, texStripName, texName, tex.Width, tex.Height, 20, 8);
                }
                ao.Position = gameposition;
                world.AddObject(ao);
            }
            else if (rb_BoxObjects.Checked)
            {
                BoxObject bo;
                bo = new BoxObject(world.World, texName, 0, .5f, 0, 1, false);
                bo.Position = gameposition;
                world.AddObject(bo);
            }
            else if (rb_CircleObjects.Checked)
            {
                CircleObject co;
                co = new CircleObject(world.World, texName, 1, .5f, 0, 1);
                co.Position = gameposition;
                world.AddObject(co);
            }
            else if (rb_WinDoorObject.Checked)
            {
                WinDoorObject so;
                // HACK - hard-coded for the win-door
                so = new WinDoorObject(world.World, texStripName, texName, tex.Width, tex.Height, 20, 5);
                so.Position = gameposition;
                world.AddObject(so);
            }
            else if (rb_PistonObject.Checked)
            {
                PistonObject po;
                Console.WriteLine("{0}", gameposition);
                po = new PistonObject(world.World, .5f, .5f, 12f, 13f, 9.7f, 12.6f, .01f, .1f, gameposition);
                po.Position = gameposition;
                world.AddObject(po);  
            }
            else if(rb_SeeSawObject.Checked)
            {
                SeeSawObject ss;
                ss = new SeeSawObject(world.World, texName, 1.5f, gameposition);
                ss.Position = gameposition;
                world.AddObject(ss);

            }
            else if (rb_SwitchObject.Checked)
            {
                SwitchObject ss1;
         //       ss1 = new SwitchObject(world.World, "Art\\Objects\\SwitchObjects\\button_strip","Art\\Objects\\SwitchObjects\\button", 181, 84, 20, 2);
                //brokenMovingPlatform1 = new SwitchObject(World, "broken_strip", "broken_moving_platform", 89, 32, 20, 8);
         //       ss1.Position = gameposition;
              //  world.AddObject(ss1);

                if (lastname == "button")
                {
                    ss1 = new SwitchObject(world.World, "Art\\Objects\\SwitchObjects\\button_strip", "Art\\Objects\\SwitchObjects\\button", 181, 84, 20, 2);

                    ss1.Position = gameposition;
                    world.AddObject(ss1);
                }
                else if (lastname == "death")
                {

                    DeathPlatform ss2;
                    ss2 = new DeathPlatform(world.World, "Art\\Objects\\SwitchObjects\\button_death_strip", "Art\\Objects\\SwitchObjects\\death", 181, 84, 20, 2);

                    //brokenMovingPlatform1 = new SwitchObject(World, "broken_strip", "broken_moving_platform", 89, 32, 20, 8);
                    ss2.Position = gameposition;
                    world.AddObject(ss2);
                }
                else
                {
                    FailButtonObject ss2;
                    ss2 = new FailButtonObject(world.World, "Art\\Objects\\SwitchObjects\\button_strip", "Art\\Objects\\SwitchObjects\\fail_button", 181, 84, 20, 2);

                    //brokenMovingPlatform1 = new SwitchObject(World, "broken_strip", "broken_moving_platform", 89, 32, 20, 8);
                    ss2.Position = gameposition;
                    world.AddObject(ss2);
                }

              
            }
            else if (rb_HoleObject.Checked)
            {
                HoleObject ss;
                ss = new HoleObject(world.World, "Art\\Objects\\HoleObjects\\hole_strip", "Art\\Objects\\HoleObjects\\hole");
                //hole1 = new HoleObject(World, "Art\\Objects\\HoleObjects\\hole_strip", "Art\\Objects\\HoleObjects\\hole");
                ss.Position = gameposition;
                world.AddObject(ss);

            }
            else if (rb_MovingPlatform.Checked)
            {
                MovingObject ss;
              //  ss = new MovingObject(world.World, "Art\\Objects\\MovingPlatformObjects\\moving_platform", 1000, 0.5f, 0, 1, false, null, new Vector2(0, -11500), 4.5f, 14.2f);
               // movPlatform1 = new MovingObject(World, "moving platform", 1000f, .5f, 0, 1, false, brokenMovingPlatform1, new Vector2(0, -11500), 4.5f, 14.2f);
               // ss.Position = gameposition;
              //  world.AddObject(ss);


                if (lastname == "moving_platform")
                    ss = new MovingObject(world.World, "Art\\Objects\\MovingPlatformObjects\\moving_platform", 0, 0.5f, 0, 1, false, null, new Vector2(0, -11500), 4.5f, 14.2f);
                else if (lastname == "gate")
                {
                    SwitchObject ssfine;
                    ssfine = new SwitchObject(world.World, "Art\\Objects\\SwitchObjects\\button_strip", "Art\\Objects\\SwitchObjects\\button", 181, 84, 20, 2);
                    ss = new MovingObject(world.World, "Art\\Objects\\MovingPlatformObjects\\gate", 0, 0.5f, 0, 1, false, ssfine, new Vector2(0, -11500), 4.5f, 14.2f);
                    //brokenMovingPlatform1 = new SwitchObject(World, "broken_strip", "broken_moving_platform", 89, 32, 20, 8);
                    ssfine.Position = gameposition;
                    world.AddObject(ssfine);
                }
                else
                {

                    SwitchObject ss1;
                    ss1 = new SwitchObject(world.World, "Art\\Objects\\SwitchObjects\\button_strip", "Art\\Objects\\SwitchObjects\\button", 181, 84, 20, 2);
                    ss = new MovingObject(world.World, "Art\\Objects\\MovingPlatformObjects\\switchmoving_platform", 0, 0.5f, 0, 1, false, ss1, new Vector2(0, -11500), 4.5f, 14.2f);
                    //brokenMovingPlatform1 = new SwitchObject(World, "broken_strip", "broken_moving_platform", 89, 32, 20, 8);
                    ss1.Position = gameposition;
                    world.AddObject(ss1);
                }

                ss.Position = gameposition;
                world.AddObject(ss);
                 

            }
            else if (rb_HorizontalMovingPlatform.Checked)
            {
                HorizontalMovingObject ss;
              //  ss = new HorizontalMovingObject(world.World, "Art\\Objects\\HorizontalMovingPlatformObjects\\moving_platform", 0, 0.5f, 0, 1, false, null, new Vector2(0, -11500), 4.5f, 14.2f);
               // movPlatform2 = new HorizontalMovingObject(World, "Art\\Objects\\HorizontalMovingPlatformObjects\\moving_platform", 0f, 0.5f, 0, 1, false, null, new Vector2(0, -11500), 32f, 38f);


                if (lastname == "moving_platform")
                    ss = new HorizontalMovingObject(world.World, "Art\\Objects\\HorizontalMovingPlatformObjects\\moving_platform", 0, 0.5f, 0, 1, false, null, new Vector2(0, -11500), 4.5f, 14.2f);
                else
                {

                    SwitchObject ss1;
                    ss1 = new SwitchObject(world.World, "Art\\Objects\\SwitchObjects\\button_strip", "Art\\Objects\\SwitchObjects\\button", 181, 84, 20, 2);
                    ss = new HorizontalMovingObject(world.World, "Art\\Objects\\HorizontalMovingPlatformObjects\\switchmoving_platform", 0, 0.5f, 0, 1, false, ss1, new Vector2(0, -11500), 4.5f, 14.2f);
                    //brokenMovingPlatform1 = new SwitchObject(World, "broken_strip", "broken_moving_platform", 89, 32, 20, 8);
                    ss1.Position = gameposition;
                    world.AddObject(ss1);
                }
                
                
                ss.Position = gameposition;
                world.AddObject(ss);

            }
            else if (rb_PaintedObjects.Checked)
            {
                switch (lastname)
                {
                    case "block_long":
                        InstasteelObject io;
                        io = new InstasteelObject(world.World, texName, 580, 1, .5f, 0, 1, false);
                        io.Position = gameposition;
                        world.AddObject(io);
                        break;
                    case "disk_long":
                        InstasteelCircleObject ico;
                        ico = new InstasteelCircleObject(world.World, texName, 480, 1, .5f, 0, 1);
                        ico.Position = gameposition;
                        world.AddObject(ico);
                        break;
                    case "line_long":
                        InstasteelObject ilo;
                        ilo = new InstasteelObject(world.World, texName, 190, 1, .5f, 0, 1, false);
                        ilo.Position = gameposition;
                        world.AddObject(ilo);
                        break;
                    default:
                        break;
                }
            }
                /*
            else if (rb_PaintedObjects.Checked)
            {
                PaintedObject po;
                List<Vector2> blobs = new List<Vector2>();
                float radius;
                float sidelength;
                Vector2 centeroff;
                switch (lastname)
                {
                    case "line_short":
                        float linelength_short = (30f/60f) * 1.2f;
                        blobs.Add(gameposition + new Vector2(linelength_short/2f, 0f));
                        blobs.Add(gameposition + new Vector2(-linelength_short/2f, 0f));
                        centeroff = new Vector2(0,0);
                        break;
                    case "line":
                        float linelength = 1.2f;
                        blobs.Add(gameposition + new Vector2(linelength/2, 0f));
                        blobs.Add(gameposition + new Vector2(-linelength/2f, 0f));
                        centeroff = new Vector2(0,0);
                        break;
                    case "line_long":
                        float linelength_long = (200f/60f) * 1.2f;
                        blobs.Add(gameposition + new Vector2(linelength_long/2f, 0f));
                        blobs.Add(gameposition + new Vector2(-linelength_long/2f, 0f));
                        centeroff = new Vector2(0,0);
                        break;
                    case "disk_short":
                        radius =  (30f/60f) *0.6f;
                        blobs = paintedCircle(radius, gameposition); // radius of 0.6f
                        centeroff = -1 * new Vector2(radius, radius);
                        centeroff = new Vector2(0,0);
                        break;
                    case "disk_small":
                        radius =  (40f/60f) *0.6f;
                        blobs = paintedCircle(radius, gameposition); // radius of 0.6f
                        centeroff = -1 * new Vector2(radius, radius);
                        centeroff = new Vector2(0,0);
                        break;
                    case "disk":
                        radius =  0.6f;
                        blobs = paintedCircle(radius, gameposition); // radius of 0.6f
                        centeroff = -1 * new Vector2(radius, radius);
                        centeroff = new Vector2(0,0);
                        break;
                    case "disk_long":
                        radius =  (150f/60f) * 0.6f;
                        blobs = paintedCircle(radius, gameposition); // radius of 0.6f
                        centeroff = -1 * new Vector2(radius, radius);
                        centeroff = new Vector2(0,0);
                        break;
                    case "block_short":
                        sidelength =  (30f/60f) *0.6f;
                        blobs = paintedSquare((30f/60f) * 1.2f, gameposition); // sidelength of 1.2f
                        centeroff = -1 * new Vector2(sidelength, sidelength);
                        break;
                    case "block_small":
                        sidelength =  (40f/60f) *0.6f;
                        blobs = paintedSquare((40f/60f) * 1.2f, gameposition); // sidelength of 1.2f
                        centeroff = -1 * new Vector2(sidelength, sidelength);
                        break;
                    case "block":
                        sidelength =  0.6f;
                        blobs = paintedSquare(1.2f, gameposition); // sidelength of 1.2f
                        centeroff = -1 * new Vector2(sidelength, sidelength);
                        break;
                    case "block_long":
                        sidelength =  (150f/60f) * 0.6f;
                        blobs = paintedSquare((150f/60f) * 1.2f, gameposition); // sidelength of 1.2f
                        centeroff = -1 * new Vector2(sidelength, sidelength);
                        break;
                    default:
                        blobs.Add(gameposition + new Vector2(0.6f, 0f));
                        blobs.Add(gameposition + new Vector2(-0.6f, 0f));
                        centeroff = new Vector2(0,0);
                        break;
                }
                po = new PaintedObject(world.World, "paint", "paintedsegment", blobs);
                po.TextureFilename = "Art\\Objects\\PaintedObjects\\" + lastname;
                po.Position = gameposition + centeroff;
                world.AddObject(po);
            }
            */
            else if (rb_BackgroundObjects.Checked)
            {
                BackgroundObject bo;
                bo = new BackgroundObject(world.World, world, texName,gameposition);
                bo.Position = gameposition;
                world.AddObject(bo); 

            }
            
                /*
            else if (rb_Player.Checked)
            {
                mp.X -= (int)((texture.Width * Constants.PLAYER_SCALE) / 2);
                mp.Y -= (int)((texture.Height * Constants.PLAYER_SCALE) / 2);

                world.player = new Player(
                    Conversion.DrawPointToVector2(mp),
                    currdir + "\\" + texName + ".tri"
                    );

                world.CurrentRoom = currentlySelectedRoom;
                world.player.TextureName = texName;
            }
            */
        }
        //End of the clicking on paint area callbacks
        //----------------------------------------------------------------------------------------------------


        private List<Vector2> paintedCircle(float radius, Vector2 gameposition)
        {
            int numpoints = 10;
            List<Vector2> blobs = new List<Vector2>();
            for (int i = 0; i < numpoints; i++)
            {
                float angle = ((float)i * 2f * (float)Math.PI / (float)numpoints ) + (float)Math.PI;
                blobs.Add(gameposition + new Vector2((float)radius * (float)Math.Sin(angle), (float)radius * (float)Math.Cos(angle)));
            }
            return blobs;
        }

        private List<Vector2> paintedSquare(float sidelength, Vector2 gameposition)
        {
            int numpoints = 10;
            List<Vector2> blobs = new List<Vector2>();
            Vector2 right = (sidelength / 2) * new Vector2(1, 0); Vector2 left = (sidelength / 2) * new Vector2(-1, 0);
            Vector2 top = (sidelength / 2) * new Vector2(0, -1); Vector2 bottom = (sidelength / 2) * new Vector2(0, 1);
            Vector2 centeroff = new Vector2(-sidelength * 2f, -sidelength * 2f);
            for (int i = 0; i < numpoints / 4; i++)
            {
                float progress = i * sidelength / ((float)numpoints / 4f) - (sidelength / 2f);
                blobs.Add(gameposition + top + new Vector2(progress, 0) + centeroff);
            }
            for (int i = 0; i < numpoints / 4; i++)
            {
                float progress = i * sidelength / ((float)numpoints / 4f) - (sidelength / 2f);
                blobs.Add(gameposition + right + new Vector2(0, progress) + centeroff);
            }
            for (int i = 0; i < numpoints / 4; i++)
            {
                float progress = i * sidelength / ((float)numpoints / 4f) - (sidelength / 2f);
                blobs.Add(gameposition + bottom + new Vector2(-progress, 0) + centeroff);
            }
            for (int i = 0; i < numpoints / 4; i++)
            {
                float progress = i * sidelength / ((float)numpoints / 4f) - (sidelength / 2f);
                blobs.Add(gameposition + left + new Vector2(0, -progress) + centeroff);
            }
            return blobs;
        }


        //-----------------Functions dealing with saving and loading levels-----------------------------------
        // More specifically, these are the callbacks and functions related to the menu bar in the top.

        private void SaveRoom()
        {
            SavedRoom sr = new SavedRoom();
            sr.objects = world.Objects;
            sr.backgroundName = world.backgroundName;
            sr.musicName = world.musicName;
            foreach (PhysicsObject obj in world.Objects)
            {
                if (obj is DudeObject)
                {
                    sr.dude = (DudeObject)obj;
                }
                else if (obj is WinDoorObject)
                {
                    sr.winDoor = (WinDoorObject)obj;
                }
            }
            sr.objects.Remove(sr.dude);
            //sr.world.DestroyBody(sr.dude.Body);
            sr.objects.Remove(sr.winDoor);
            //sr.world.DestroyBody(sr.winDoor.Body);

            sr.world = world.World;

            Serializer.Serialize(sr, currentFileName);

            sr.objects.Add(sr.dude);
            sr.objects.Add(sr.winDoor);
        }

        //Saves the current file, using the currentFileName as the name of the file.
        private void mi_Save_Click(object sender, EventArgs e)
        {
            if (currentState != State.NO_EDITS)
            {
                if (currentFileName == "")
                {
                    mi_Save_As_Click(sender, e);
                }
                else if (currentState == State.EDITING_WORLD)
                {
                    //verifyInvariants(false);
                    SaveRoom();
                }
            }
        }

        //Allows the user to choose the name of the current file to save.
        private void mi_Save_As_Click(object sender, EventArgs e)
        {
            if (currentState != State.NO_EDITS)
            {
                //verifyInvariants(false);

                //Allow the user to choose a name and a location
                SaveFileDialog dialog = new SaveFileDialog();

                if (currentState == State.EDITING_WORLD)
                {
                    dialog.Filter = "World Files | *.world";
                }
                else
                {
                    dialog.Filter = "Room Files | *.room";
                }

                //dialog.InitialDirectory = ".";
                dialog.InitialDirectory = GameEngine.GetCurrDir() + "\\Levels";
                dialog.Title = "Choose the file to save.";


                DialogResult result = dialog.ShowDialog();

                //If the result was ok, load the resultant file, otherwise, just return.
                if (result == DialogResult.OK)
                {
                    currentFileName = dialog.FileName;
                    if (currentState == State.EDITING_WORLD)
                    {
                        //Serializer.Serialize(world, dialog.FileName);
                        SaveRoom();
                    }
                }
            }
        }


        //Loads the specified file.
        private void mi_Load_World_Click(object sender, EventArgs e)
        {
            load_file(true, "World Files | *.world", "Select a world file.");
        }


        private void mi_Load_Room_Click(object sender, EventArgs e)
        {
            load_file(false, "Room Files | *.room", "Select a room file.");
        }

        private void load_file(bool loadWorld, string filter, string title)
        {
            //First, choose the file we want to load.
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = filter;
            string currdir = GameEngine.GetCurrDir() + "\\Levels";
            //Console.WriteLine("Current Directory " + currdir);
            dialog.InitialDirectory = currdir; //".";
            dialog.Title = title;

            DialogResult result = dialog.ShowDialog();

            //If the result was ok, load the resultant file, otherwise, just return.
            if (result == DialogResult.OK)
            {
                if (loadWorld)
                {
                    //world = Serializer.OLDDeSerialize(dialog.FileName); // HACK so we can convert the old levels into the new format
                    world = new ScrollingWorld(dialog.FileName, true);
                    world.Background = GameEngine.TextureList[world.backgroundName];
                    switchRooms();
                }
                Size pbLevelSize = new Size(world.Background.Width, world.Background.Height);
                pb_Level.MaximumSize = (pb_Level.MinimumSize = (pb_Level.Size = pbLevelSize));
                    /*
                else
                {
                    currentlySelectedRoom = Serializer.DeserializeRoom(dialog.FileName);
                    switchRooms(currentlySelectedRoom);
                }
                */

                currentFileName = dialog.FileName;

                enableEditing(loadWorld);

                Texture2D backgroundTexture = GameEngine.TextureList[world.backgroundName];

                pb_Level.Width = backgroundTexture.Width;
                pb_Level.Height = backgroundTexture.Height;
                pb_Level.Refresh();
            }
        }

        //Creates a new game world and allows editing
        private void mi_New_World_Click(object sender, EventArgs e)
        {
            //Get the background name from the user
            StringPromptDialog dialog = new StringPromptDialog("Enter background/music", "background");

            //Create the world.
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                world = new ScrollingWorld(dialog.UserInput);
                Size pbLevelSize = new Size(world.Background.Width, world.Background.Height);
                pb_Level.MaximumSize = (pb_Level.MinimumSize = (pb_Level.Size = pbLevelSize));

                enableEditing(true);

                switchRooms();
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        //Set various boolean values to their appropriate state
        private void enableEditing(bool editingWorld)
        {
            if (editingWorld)
            {
                currentState = State.EDITING_WORLD;
                rb_AnimationObjects.Enabled = true;
            }

        }
        //------------End of saving/loading functions-------------------------------------------------------


        //Switches the current room to the room toSwitchTo. This involves repainting the paint screen, 
        //changing the title of the editor, making sure our currentlySelectedObject is null so we don't get
        //null pointers, and update the room being edited.
        //private void switchRooms(Room toSwitchTo)
        private void switchRooms()
        {
            currentlySelectedObject = null;

            updateTitle();
            pb_Level.Refresh();
        }


        //------------ Functions for drawing on the drawing pane------------------------------------

        //Paints all the objects on the drawing pane.
        private void pb_Level_Paint(object sender, PaintEventArgs e)
        {
            if (currentState != State.NO_EDITS)
            {
                //Draw the background image
                string background_file = currdir + world.backgroundName + ".png";

                System.Drawing.Image image = Image.FromFile(background_file);
                e.Graphics.DrawImage(image, 0, 0, pb_Level.Width, pb_Level.Height);

                //Draw some stuff to draw around the currently selected object, if there is one
                if (currentlySelectedObject != null)
                {
                    XnaRectangle BB = currentlySelectedObject.getBBRelativeToWorld();

                    //First, draw a big red box around the object, with a little bit of slack around the edges.
                    System.Drawing.RectangleF selectionRect = new System.Drawing.RectangleF(
                        BB.X - EditorConstants.SELECTION_BOX_SLACK,
                        BB.Y - EditorConstants.SELECTION_BOX_SLACK,
                        BB.Width + 2 * EditorConstants.SELECTION_BOX_SLACK,
                        BB.Height + 2 * EditorConstants.SELECTION_BOX_SLACK
                        );

                    e.Graphics.FillRectangle(
                        Brushes.Red, selectionRect);

                    //Now, draw the actual bounding box in white.
                    Pen pen = new Pen(Color.White, EditorConstants.BOUNDING_BOX_WIDTH);
                    e.Graphics.DrawRectangle(pen, BB.X, BB.Y, BB.Width, BB.Height);

                    //Finally, draw the little box in the corner of the bounding box where the user
                    // can click to drag.
                    e.Graphics.FillRectangle(Brushes.White, resizeObjectBox);
                }


                //Now actually draw the images on the screen.
                foreach (PhysicsObject obj in world.Objects)
                {
                    drawPhysicsObject(obj, e);
                }

                //Finally, if the room is the current room of the world, then draw the player, if it exists.
                /*
                if (currentState == State.EDITING_WORLD && world.CurrentRoom == currentlySelectedRoom
                    && world.player != null)
                {
                    drawPhysicsObject(world.player, e);
                }
                */
            }

            else
            {
                e.Graphics.FillRectangle(Brushes.Black, pb_Level.Bounds);
            }
        }



        //Draws an arbitrary space object to the drawing pane. 
        private void drawPhysicsObject(PhysicsObject obj, PaintEventArgs e)
        {
            //Console.WriteLine(obj.TextureFilename + ".png");
            //Console.WriteLine(currdir + "\\" + obj.TextureFilename + ".png");
            Image img = Image.FromFile(currdir + "\\" + obj.TextureFilename + ".png");

            //In order to do rotations and scaling in windows forms, we need to determine where
            // the upper right, upper left, and lower left corners map to. We determine these, and
            // tell the graphics to draw the image.

            // unscale before rotating
            //obj.Height = obj.Height / obj.scale;
            //obj.Width = obj.Width / obj.scale;

            // Hack to make the image representing a painted object draw correctly
            if (obj is PaintedObject && !(obj is InstasteelObject) && !(obj is InstasteelCircleObject))
            {
                ((PaintedObject)obj).Height = img.Height;
                ((PaintedObject)obj).Width = img.Width;
            }

            XnaRectangle bb = obj.getBBRelativeToWorld();

            float s = obj.scale;

            //Find the new point for the upper-left corner
            DrawPoint upper_left = Conversion.Vector2ToDrawPoint(new Vector2(bb.X + bb.Width/2f, bb.Y + bb.Height/2f) + RotateVector2(new Vector2(-obj.Width/2f, -obj.Height/2f), obj.Angle));

            DrawPoint upper_right = Conversion.Vector2ToDrawPoint(new Vector2(bb.X + bb.Width/2f, bb.Y + bb.Height/2) + RotateVector2(new Vector2(obj.Width - obj.Width/2f, -obj.Height/2f), obj.Angle));

            DrawPoint lower_left = Conversion.Vector2ToDrawPoint(new Vector2(bb.X + bb.Width/2f, bb.Y + bb.Height/2) + RotateVector2(new Vector2(-obj.Width/2f, obj.Height - obj.Height/2f), obj.Angle));

            // rescale after rotating
            //obj.Height = obj.Height * obj.scale;
            //obj.Width = obj.Width * obj.scale;

            /*
            DrawPoint upper_left = Conversion.Vector2ToDrawPoint(RotateVector2(new Vector2(bb.X, bb.Y), obj.Angle));

            DrawPoint upper_right = Conversion.Vector2ToDrawPoint(RotateVector2(new Vector2(bb.X + bb.Width, bb.Y), obj.Angle));

            DrawPoint lower_left = Conversion.Vector2ToDrawPoint(RotateVector2(new Vector2(bb.X, bb.Y + bb.Height), obj.Angle));
            */

            //Define the point mapping.
            DrawPoint[] destmapping = {upper_left, upper_right, lower_left};
            DrawRect srcrect = new DrawRect(0,0,img.Width,img.Height);

            //Draw the image with the specified position and scaling.
            e.Graphics.DrawImage(img, destmapping, srcrect, GraphicsUnit.Pixel);
        }


        public static Vector2 RotateVector2(Vector2 point, float radians)
        {
            float cosRadians = (float)Math.Cos(radians);
            float sinRadians = (float)Math.Sin(radians);

            return new Vector2(
                point.X * cosRadians - point.Y * sinRadians,
                point.X * sinRadians + point.Y * cosRadians);
        }

        //-------End of painting functions------------------------------------------------------------

        /// <summary>
        /// Changes the title of the main editor screen to display the name of the selected room.
        /// </summary>
        private void updateTitle()
        {
            this.Text = "TEXT!";
        }

        private void refreshResizeObjectBox()
        {
            XnaRectangle BB = currentlySelectedObject.getBBRelativeToWorld();

            resizeObjectBox = new RectangleF(
                BB.X - EditorConstants.RESIZE_BOX_DIMS + BB.Width,
                BB.Y - EditorConstants.RESIZE_BOX_DIMS + BB.Height,
                EditorConstants.RESIZE_BOX_DIMS * 2,
                EditorConstants.RESIZE_BOX_DIMS * 2);
        }



        /// <summary>
        /// Returns up through "Content folder
        /// </summary>
        /// <returns></returns>
        public static string CurrDirHack()
        {
            //return (Directory.GetCurrentDirectory()).Replace("bin\\x86\\Debug", "Content").Replace("bin\\x86\\Release", "Content").Replace("\\Worlds", "");
            return GameEngine.GetCurrDir();
        }

        private void showMessage(string title, string text)
        {

            MessageBox.Show(text, title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
        }


        /// <summary>
        /// Apply properties in text box to currently selected object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_ApplyProperties_Click(object sender, EventArgs e)
        {
            
            // apply these new settings
            if (currentlySelectedObject != null && AreObjPropertiesValid())
            {
                float newScale = float.Parse(tb_Scale.Text);
                currentlySelectedObject.Width = (currentlySelectedObject.Width / currentlySelectedObject.scale) * newScale;

                if ((currentlySelectedObject is InstasteelCircleObject || (currentlySelectedObject is CircleObject && !(currentlySelectedObject is PaintedObject))))
                {
                    currentlySelectedObject.Height = currentlySelectedObject.Width;

                    float radius = (float)currentlySelectedObject.Width / (2 * CASSWorld.SCALE);

                    // circle objects only
                    currentlySelectedObject.RemoveFromWorld();
                    CircleDef shape = new CircleDef();
                    shape.Radius = radius;
                    // HACK HACK HACK - this won't work for objects that have more than one shape!
                    shape.Density = currentlySelectedObject.shapes[0].Density;
                    shape.Friction = currentlySelectedObject.shapes[0].Friction;
                    shape.Restitution = currentlySelectedObject.shapes[0].Restitution;
                    currentlySelectedObject.shapes.Clear(); // get rid of the old, unscaled shape
                    currentlySelectedObject.shapes.Add(shape); // add the new one
                    currentlySelectedObject.AddToWorld();
                }
                else if (currentlySelectedObject.TextureFilename == "Art\\Objects\\BoxObjects\\bottomTexture2273")
                {
                    float halfWidth = (float)currentlySelectedObject.Width / (2 * CASSWorld.SCALE);
                    float halfHeight = (float)currentlySelectedObject.Height / (2 * CASSWorld.SCALE);

                    // box object only...
                    currentlySelectedObject.RemoveFromWorld();
                    // Create the collision shape
                    PolygonDef shape = new PolygonDef();
                    shape.SetAsBox(halfWidth, halfHeight);
                    // HACK HACK HACK - this won't work for objects that have more than one shape!
                    shape.Density = currentlySelectedObject.shapes[0].Density;
                    shape.Friction = currentlySelectedObject.shapes[0].Friction;
                    shape.Restitution = currentlySelectedObject.shapes[0].Restitution;
                    currentlySelectedObject.shapes.Clear(); // get rid of the old, unscaled shape
                    currentlySelectedObject.shapes.Add(shape); // add the new one
                    currentlySelectedObject.AddToWorld();
                }
                else if (currentlySelectedObject is InstasteelObject || !(currentlySelectedObject is PaintedObject))
                {
                    currentlySelectedObject.Height = (currentlySelectedObject.Height / currentlySelectedObject.scale) * newScale;
                    // Determine dimensions
                    float halfWidth = (float)currentlySelectedObject.Width / (2 * CASSWorld.SCALE);
                    float halfHeight = (float)currentlySelectedObject.Height / (2 * CASSWorld.SCALE);

                    // box object only...
                    currentlySelectedObject.RemoveFromWorld();
                    // Create the collision shape
                    PolygonDef shape = new PolygonDef();
                    shape.SetAsBox(halfWidth, halfHeight);
                    // HACK HACK HACK - this won't work for objects that have more than one shape!
                    shape.Density = currentlySelectedObject.shapes[0].Density;
                    shape.Friction = currentlySelectedObject.shapes[0].Friction;
                    shape.Restitution = currentlySelectedObject.shapes[0].Restitution;
                    currentlySelectedObject.shapes.Clear(); // get rid of the old, unscaled shape
                    currentlySelectedObject.shapes.Add(shape); // add the new one
                    currentlySelectedObject.AddToWorld();
                }

                if ((bool)cBox_StaticObject.Checked)
                {
                    MassData mass = new MassData();
                    mass.Mass = 0f;
                    currentlySelectedObject.Body.SetMass(mass);
                }
                else if (currentlySelectedObject.Body.GetMass()==0)
                {
                    MassData mass = new MassData();
                    mass.Mass = 1f;
                    currentlySelectedObject.Body.SetMass(mass);
                    currentlySelectedObject.Body.GetShapeList().Density = 1f;
                    currentlySelectedObject.Body.SetMassFromShapes();
                }

                float newRotation = float.Parse(tb_Rotation.Text);
                currentlySelectedObject.Angle = MathHelper.ToRadians(newRotation);

                currentlySelectedObject.scale = newScale;

                float newbound1 = float.Parse(tb_bound1.Text);
                float newbound2 = float.Parse(tb_bound2.Text);
                if (currentlySelectedObject is MovingObject)
                {
                   
                    MovingObject temp = (MovingObject)currentlySelectedObject;
                    temp.bound1 = temp.Position.Y - newbound1;
                    temp.bound2 = temp.Position.Y + newbound2;
                    Console.WriteLine(temp.bound1 + " " + temp.bound2);
                    currentlySelectedObject = temp;
                    
                }
                else if (currentlySelectedObject is HorizontalMovingObject)
                {
                    HorizontalMovingObject temp = (HorizontalMovingObject)currentlySelectedObject;
                    temp.bound1 = temp.Position.X - newbound1;
                    temp.bound2 = temp.Position.X + newbound2;
                    currentlySelectedObject = temp;
                }
                
            }

           pb_Level.Refresh();
        }

        // Predicate<string> delegate to remove empty strings
        public static bool IsEmptyString(string s)
        {
            return s == "";
        }

        private void tb_Rotation_Leave(object sender, EventArgs e)
        {
            b_ApplyProperties.Enabled = AreObjPropertiesValid();
        }

        /// <summary>
        /// Are all the object properties entered by the user valid?
        /// </summary>
        /// <returns></returns>
        private bool AreObjPropertiesValid()
        {
            float dummyFloat;
            int dummyInt;
            int dummyInt2;

            /*
            bool rotationCorrect = float.TryParse(tb_Rotation.Text, out dummyFloat);
            bool slevelCorrect = int.TryParse(tb_SLevel.Text, out dummyInt2) || 
                (!(currentlySelectedObject is Survivor || currentlySelectedObject is VanishWall));
            bool damageCorrect = int.TryParse(tb_Damage.Text, out dummyInt) ||
                (!(currentlySelectedObject is HazardStatic)
                && !(currentlySelectedObject is HazardDynamic));
            */

            //return rotationCorrect && damageCorrect && slevelCorrect;
            return true;
        }

        private void tb_Density_Leave(object sender, EventArgs e)
        {
            b_ApplyProperties.Enabled = AreObjPropertiesValid();
        }

        private void tool_Insertion_Click(object sender, EventArgs e)
        {
            ClearToolMenu();
            tool_Insertion.Checked = true;

            currentTool = EditorTool.Insertion;
        }

        private void tool_Selection_Click(object sender, EventArgs e)
        {
            ClearToolMenu();
            tool_Selection.Checked = true;

            currentTool = EditorTool.Selection;
        }

        /// <summary>
        /// Set all tool squares to "unchecked."
        /// </summary>
        private void ClearToolMenu()
        {
            tool_Insertion.Checked = false;
            tool_Selection.Checked = false;
        }

        //This handles scripts for dynamic objects - basic string parsing
        /*
        private void SetScriptingFields()
        {
            tb_Script.Text = "";
            tb_Script.Enabled = false;
            cbox_Scripted.Checked = false;
            cbox_Scripted.Enabled = false;

            if (currentlySelectedObject == null ||
                !(currentlySelectedObject is Dynamic))
                return;

            Dynamic dyn = (Dynamic)currentlySelectedObject;

            cbox_Scripted.Enabled = true;

            cbox_Scripted.Checked = dyn.IsScripted;
            tb_Script.Enabled = dyn.IsScripted;

            List<string> lines = new List<string>();

            foreach (Instruction inst in dyn.Instructions)
            {
                string line = "";
                string terminate = "\r\n";

                switch (inst.Type)
                {
                    case InstructionType.SetPosition:
                        line = "pos " + inst.Value1 + " " + inst.Value2;
                        break;

                    case InstructionType.SetVelocity:
                        line = "vel " + inst.Value1 + " " + inst.Value2;
                        break;

                    case InstructionType.Move:
                        line = "move for " + inst.Value1;
                        break;

                    case InstructionType.Stop:
                        line = "stop";
                        break;

                    case InstructionType.Goto:
                        line = "goto @";
                        lines[(int)inst.Value1] = "@" + lines[(int)inst.Value1];
                        break;

                    case InstructionType.Wait:
                        line = "wait for " + inst.Value1;
                        break;

                    case InstructionType.LoadPos:
                        line = "loadpos";
                        break;

                    case InstructionType.SavePos:
                        line = "savepos";
                        break;

                    case InstructionType.SetRotation:
                        line = "rot " + (inst.Value1 / MathHelper.Pi * 180.0f);
                        break;

                    case InstructionType.MoveForever:
                        line = "move forever";
                        break;
                }

                lines.Add(line + terminate);
            }

            foreach (string s in lines)
            {
                tb_Script.Text += s;
            }
        }
        */

        private void cbox_Scripted_CheckedChanged(object sender, EventArgs e)
        {
            tb_Script.Enabled = cbox_Scripted.Checked;
        }

        private void b_Front_Click(object sender, EventArgs e)
        {
            if (currentlySelectedObject != null)
            {
                world.Objects.Remove(currentlySelectedObject);
                world.Objects.Add(currentlySelectedObject);
            }
        }

        private void Editor_FormClosing(object sender, FormClosingEventArgs e)
        {
            GameEngine.level_editor_open = false;
        }

       

    }
}