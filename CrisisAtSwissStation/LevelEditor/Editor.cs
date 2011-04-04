/* TODO:
 * 
 * Beta:     
 *      Smooth rotation.
 * 
 *      Change background
 * */

using System;
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
        private CASSWorld world;

        string currdir = CurrDirHack();

        //The object that the user just selected. May be null!
        //private SpaceObject currentlySelectedObject;
        private PhysicsObject currentlySelectedObject;

        //The box inside which the user must click in order to resize the object.
        private RectangleF resizeObjectBox;

        public Editor()
        {
            InitializeComponent();

            Size pbLevelSize = new Size(GameEngine.GAME_WINDOW_WIDTH, GameEngine.GAME_WINDOW_HEIGHT);

            pb_Level.MaximumSize = (pb_Level.MinimumSize = (pb_Level.Size = pbLevelSize));
            this.FormBorderStyle = FormBorderStyle.FixedSingle;  

            currentTool = EditorTool.Insertion;
            ClearToolMenu();
            tool_Insertion.Checked = true;

            // Disable object properties by default
            tb_Damage.Enabled = false;
            tb_Rotation.Enabled = false;
            b_ApplyProperties.Enabled = false;
            b_Front.Enabled = false;
            tb_SLevel.Enabled = false;


            //Only enable this menu item if the currently selected object is a door. When we start,
            // we don't have a currently selected item, so disable this menu item.
            menu_door.Enabled = false;
            menu_room.Enabled = false;
            menu_debug.Enabled = false;
        }


        //------The following are callbacks for when the user selects a new type of object in the radio button
        // list. They populate the list of textures.
        private void rb_WallStatic_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_WallStatic.Checked)
            {
                textureDir = "Art\\Static Walls\\";
                PopulateTextureList(textureDir);
            }
        }

        // Hardcoded to only show the player's icon
        private void rb_Player_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Player.Checked)
            {
                textureDir = "Art\\Players\\";
                //PopulateTextureList(textureDir);

                this.lb_TextureList.Items.Clear();

                if (File.Exists(currdir + "\\Art\\Players\\player.png"))
                    lb_TextureList.Items.Add("player.png");

                if (lb_TextureList.Items.Count != 0)
                {
                    lb_TextureList.SelectedIndex = 0;
                }
            }
        }

        private void rb_WallDynamic_CheckedChanged_1(object sender, EventArgs e)
        {
            if (rb_WallDynamic.Checked)
            {
                textureDir = "Art\\Dynamic Walls\\";
                PopulateTextureList(textureDir);
            }
        }

        private void rb_HazardStatic_CheckedChanged_1(object sender, EventArgs e)
        {
            if (rb_HazardStatic.Checked)
            {
                textureDir = "Art\\Static Hazards\\";
                PopulateTextureList(textureDir);
            }
        }

        private void rb_HazardDynamic_CheckedChanged_1(object sender, EventArgs e)
        {
            if (rb_HazardDynamic.Checked)
            {
                textureDir = "Art\\Dynamic Hazards\\";
                PopulateTextureList(textureDir);
            }
        }

        private void rb_Handlebars_CheckedChanged_1(object sender, EventArgs e)
        {
            if (rb_Handlebars.Checked)
            {
                textureDir = "Art\\Handlebars\\";
                PopulateTextureList(textureDir);
            }
        }

        private void rb_Parts_CheckedChanged_1(object sender, EventArgs e)
        {
            if (rb_Parts.Checked)
            {
                textureDir = "Art\\Parts\\";
                PopulateTextureList(textureDir);
            }
        }

        private void rb_Survivors_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Survivors.Checked)
            {
                textureDir = "Art\\Survivors\\";
                PopulateTextureList(textureDir);
            }
        }

        //This is slightly different than all the others. There is no "texture"
        //for doors, you see, so we're going to just add one item in the
        //list at the bottom, and make it selected.
        private void rb_Doors_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_Doors.Checked)
            {
                lb_TextureList.Items.Clear();

                lb_TextureList.Items.Add("Door");

                lb_TextureList.SelectedIndex = 0;
            }
        }

        private void rb_VictoryTest_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_VictoryTest.Checked)
            {
                textureDir = "Art\\Victory Tests\\";
                PopulateTextureList(textureDir);
            }

        }

        private void rb_SavePoint_CheckedChanged(object sender, EventArgs e)
        {
            if (rb_SavePoint.Checked)
            {
                textureDir = "Art\\Save Points\\";
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
                    x_drag_transform = (int)(e.X - currentlySelectedObject.Position.X);
                    y_drag_transform = (int)(e.Y - currentlySelectedObject.Position.Y);

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
                        new Vector2(new_X, new_Y);
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
                tb_Damage.Enabled = false; // Only true if this is a hazard (below)
                tb_Rotation.Enabled = true;
                b_ApplyProperties.Enabled = true;
                b_Front.Enabled = true;
                tb_SLevel.Enabled = false;
                /*
                tb_Rotation.Text = (currentlySelectedObject.Rotation * 180.0f / MathHelper.Pi).ToString();

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

                SetScriptingFields();
            }
            else
            {
                // Clear all the properties fields
                tb_Damage.Text = "";
                tb_Rotation.Text = "";
                tb_SLevel.Text = "";
                b_ApplyProperties.Enabled = false;
                b_Front.Enabled = false;
                tb_Damage.Enabled = false;
                tb_Rotation.Enabled = false;
                tb_SLevel.Enabled = false;

                tb_Script.Text = "";
                tb_Script.Enabled = false;
                cbox_Scripted.Checked = false;
                cbox_Scripted.Enabled = false;
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
                        world.Objects.Remove(obj);

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
                if (rb_Doors.Checked)
                {
                    makeDoor(mouse);
                }
                else
                {
                    object textureName = lb_TextureList.SelectedItem;

                    if (textureName != null)
                    {
                        texture = Image.FromFile(currdir + "\\" + textureDir + textureName.ToString());

                        MakeSpaceObject(textureDir + textureName.ToString(), texture, mouse);
                    }
                }
            }

        }

        // Create object in room upon user click
        private void MakeSpaceObject(string texName, Image tex, DrawPoint mp)
        {
            texName = texName.Replace(".png", "");

            //int numberOfFrames = SpaceObject.GetNumberOfFrames(texName);

            //Much repeated code to account for each possible object
            if (rb_WallStatic.Checked)
            {
                //mp.X -= (texture.Width / (numberOfFrames * 2));
                mp.X -= (texture.Width / 2);
                mp.Y -= (texture.Height / 2);

                BoxObject platform = new BoxObject(world.World, GameEngine.TextureList[texName], 0, .1f, 0);
                platform.Position = Conversion.DrawPointToVector2(mp);

                world.AddObject(platform);
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



        //-----------------Functions dealing with saving and loading levels-----------------------------------
        // More specifically, these are the callbacks and functions related to the menu bar in the top.


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

                    verifyInvariants(false);

                    Serializer.Serialize(world, currentFileName);
                }
            }
        }

        //Allows the user to choose the name of the current file to save.
        private void mi_Save_As_Click(object sender, EventArgs e)
        {
            if (currentState != State.NO_EDITS)
            {
                verifyInvariants(false);

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


                dialog.InitialDirectory = ".";
                dialog.Title = "Choose the file to save.";


                DialogResult result = dialog.ShowDialog();

                //If the result was ok, load the resultant file, otherwise, just return.
                if (result == DialogResult.OK)
                {
                    if (currentState == State.EDITING_WORLD)
                    {
                        Serializer.Serialize(world, dialog.FileName);
                    }
                    currentFileName = dialog.FileName;
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
            dialog.InitialDirectory = ".";
            dialog.Title = title;

            DialogResult result = dialog.ShowDialog();

            //If the result was ok, load the resultant file, otherwise, just return.
            if (result == DialogResult.OK)
            {
                if (loadWorld)
                {
                    world = Serializer.DeSerialize(dialog.FileName);
                    switchRooms(world.CurrentRoom);
                }
                else
                {
                    currentlySelectedRoom = Serializer.DeserializeRoom(dialog.FileName);
                    switchRooms(currentlySelectedRoom);
                }

                currentFileName = dialog.FileName;

                enableEditing(loadWorld);

                pb_Level.Refresh();
            }
        }

        //Creates a new game world and allows editing
        private void mi_New_World_Click(object sender, EventArgs e)
        {
            //Get the new room from the user.
            StringPromptDialog dialog = new StringPromptDialog("Enter the name of the first room: ");

            //Create the world.
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                world = new GameWorld(dialog.UserInput);

                enableEditing(true);

                switchRooms(world.CurrentRoom);
            }
        }

        //Creates a new empty room and allows editing
        private void mi_New_Room_Click(object sender, EventArgs e)
        {
            //Get the new room from the user.
            StringPromptDialog dialog = new StringPromptDialog("Enter the name of the room: ");

            //Create the world.
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                currentlySelectedRoom = new Room(dialog.UserInput);

                enableEditing(false);

                switchRooms(currentlySelectedRoom);
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
                menu_room.Enabled = true;
                mi_goToRoom.Enabled = true;
                mi_Insert_Room.Enabled = true;
                mi_merge_room.Enabled = true;
                mi_link_door.Enabled = true;
                deleteRoomFromWorldToolStripMenuItem.Enabled = true;
                rb_Player.Enabled = true;
            }
            else
            {
                currentState = State.EDITING_ROOM;
                menu_room.Enabled = true;
                mi_goToRoom.Enabled = false;
                mi_Insert_Room.Enabled = false;
                mi_merge_room.Enabled = false;
                mi_link_door.Enabled = false;
                deleteRoomFromWorldToolStripMenuItem.Enabled = false;
                rb_Player.Enabled = false;
            }

            menu_debug.Enabled = true;

        }
        //------------End of saving/loading functions-------------------------------------------------------


        //------------Functions for working with doors------------------------------------------------------

        //If the currently selected object is a door, then pops up a prompt asking for the new name for the
        //   door, verifies that it is unique, and renames the door.
        private void renameDoorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentlySelectedObject != null &&
                currentlySelectedObject is Door)
            {
                StringPromptDialog dialog = new StringPromptDialog("Enter the new name: ");

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (isDoorNameUnique(dialog.UserInput))
                    {
                        ((Door)currentlySelectedObject).name = dialog.UserInput;
                    }
                    else
                    {
                        showMessage("Uniqueness Error", "A door with that name already exists in the room");
                    }
                }
            }
        }

        //Call back for linking the 
        private void linkDoorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentState == State.EDITING_WORLD &&
                currentlySelectedObject != null &&
                currentlySelectedObject is Door)
            {
                Door A = ((Door)currentlySelectedObject);

                //First, pop up the menu and get whichever door the user selects.
                string title = "Currently selected door: " +
                    A.name + " in room " + currentlySelectedRoom.name;

                RoomSelectDialog dialog = new RoomSelectDialog(world, false, title);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Door B = dialog.SelectedDoor;

                    A.Link = null;
                    B.Link = null;

                    A.Link = B;
                    B.Link = A;
                }
            }
        }

        /// <summary>
        /// Returns true if the door is uniquly named in the room it is in.
        /// </summary>
        private bool isDoorNameUnique(string name)
        {
            foreach (Door door in currentlySelectedRoom.Doors)
            {
                if (door.name == name)
                {
                    return false;
                }
            }

            return true;
        }

        //Creates a door at the location of the user click.
        private void makeDoor(DrawPoint pt)
        {
            //First, get name for door
            StringPromptDialog dialog = new StringPromptDialog("Please enter the name of the door");

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (isDoorNameUnique(dialog.UserInput))
                {
                    //Create the door
                    Vector2 postition = new Vector2(
                        pt.X - (EditorConstants.DEFAULT_DOOR_SIZE / 2),
                        pt.Y - (EditorConstants.DEFAULT_DOOR_SIZE / 2));

                    string boundsFile = currdir + "\\Art\\Doors\\Door.tri";

                    Door door = new Door(postition, boundsFile, dialog.UserInput, currentlySelectedRoom);


                    //Add it to the current room
                    currentlySelectedRoom.AddObject(door);

                }
                else
                {
                    showMessage("Name Entry Error",
                        "A door already exists in the current room with the name " + dialog.UserInput);
                }
            }
        }

        //Draws a door
        private void drawDoor(Door door, PaintEventArgs e)
        {
            SolidBrush brush =
                new SolidBrush(
                    Color.FromArgb(
                        Convert.ToInt32(
                            EditorConstants.DOOR_COLOR, 16)));

            RectangleF rect =
                Conversion.RectToRect(door.getBBRelativeToWorld());

            e.Graphics.FillRectangle(brush, rect);

            e.Graphics.DrawImage(Image.FromFile(currdir + "\\Art\\Players\\target.png"),
                door.TargetPosition.X - EditorConstants.DOOR_TARGET_WIDTH / 2.0f,
                door.TargetPosition.Y - EditorConstants.DOOR_TARGET_WIDTH / 2.0f,
                EditorConstants.DOOR_TARGET_WIDTH,
                EditorConstants.DOOR_TARGET_WIDTH);
        }

        private void showDoorStatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentState != State.NO_EDITS)
            {
                string message = "";

                if (currentState == State.EDITING_WORLD)
                {
                    foreach (Room room in world.Rooms)
                    {
                        message += getDoorStatesInRoom(room);
                    }
                }
                else
                {
                    message += getDoorStatesInRoom(currentlySelectedRoom);
                }


                if (message == "")
                    message = "No doors in the world!";

                showMessage("Door states", message);
            }
        }

        private string getDoorStatesInRoom(Room room)
        {
            string message = "";

            foreach (Door door in room.Doors)
            {
                message += door.toString(room.name) + "\n";
            }

            return message;
        }

        //------------End of functions for working with doors-----------------------------------------------



        //------------Functions for creating, switching, renaming, etc  rooms-------------------------------
        
        private void deleteRoomFromWorldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(currentState == State.EDITING_WORLD){
                if(world.Rooms.Count <= 1){
                    showMessage("Room Delete Error", "There is only one room, you cannot delete any more!");

                }else{
                    RoomSelectDialog dialog = new RoomSelectDialog(world, true, "Select room to delete.");

                    if(dialog.ShowDialog() == DialogResult.OK)
                    {
                        Room toDelete = dialog.SelectedRoom;

                        //First, remove the room from the list if rooms.
                        world.Rooms.Remove(toDelete);

                        //Next, clear all the links into this room from other doors.
                        foreach(Door door in toDelete.Doors)
                        {
                            if(door.Link != null)
                            {
                                door.Link.Link = null;
                            }
                        }

                        //Finally, if this was the room with the player, set the world's
                        // starting room to null.
                        if(toDelete == world.CurrentRoom)
                        {
                            world.CurrentRoom = null;
                        }
                    }
                }
            }
        }

        private void goToRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentState == State.EDITING_WORLD)
            {
                string title = "Currently Selected Room: " + currentlySelectedRoom.name;
                RoomSelectDialog dialog = new RoomSelectDialog(world, true, title);

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    switchRooms(dialog.SelectedRoom);
                    pb_Level.Refresh();
                }
            }
        }

        //Trys and creates a new room. Pops up a RoomCreateDialog.
        private void createNewRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentState == State.EDITING_WORLD)
            {
                StringPromptDialog dialog =
                    new StringPromptDialog("Please enter the name of the room");

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (isRoomNameUnique(dialog.UserInput))
                    {

                        Room room = new Room(dialog.UserInput);
                        world.Rooms.Add(room);
                        switchRooms(room);
                    }
                    else
                    {
                        MessageBox.Show(
                            "A room already exists with the name " + dialog.UserInput,
                            "Name Entry Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation);
                    }
                }
            }
        }

        //Switches the current room to the room toSwitchTo. This involves repainting the paint screen, 
        //changing the title of the editor, making sure our currentlySelectedObject is null so we don't get
        //null pointers, and update the room being edited.
        private void switchRooms(Room toSwitchTo)
        {
            currentlySelectedObject = null;
            menu_door.Enabled = false;

            currentlySelectedRoom = toSwitchTo;
            updateTitle();
            pb_Level.Refresh();
        }


        //Renames the current room. The callback for the "Rename current room..." option under
        //the room menu
        private void renameCurrentRoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentState != State.NO_EDITS)
            {
                StringPromptDialog dialog = new StringPromptDialog("Please enter the new name:");

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (currentState == State.EDITING_ROOM || isRoomNameUnique(dialog.UserInput))
                    {
                        currentlySelectedRoom.name = dialog.UserInput;
                        updateTitle();
                    }
                    else
                    {
                        showMessage("Name Entry Error", "A room already exists with the name " + dialog.UserInput);
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if there is a room in the world with a name of the passed arguement.
        /// </summary>
        private bool isRoomNameUnique(string name)
        {
            foreach (Room room in world.Rooms)
            {
                if (room.name == name)
                {
                    return false;
                }
            }

            return true;
        }

        private void mi_merge_room_Click(object sender, EventArgs e)
        {
            //Get the room we want to load
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Room Files | *.room";
            dialog.InitialDirectory = ".";
            dialog.Title = "Choose the room you want to merge";

            DialogResult result = dialog.ShowDialog();

            //If the result was ok, load the resultant file, otherwise, just return.
            if (result == DialogResult.OK)
            {
                Room toMerge = Serializer.DeserializeRoom(dialog.FileName);

                //Check if it's name is unique
                while (!isRoomNameUnique(toMerge.name))
                {
                    StringPromptDialog name_dialog =
                        new StringPromptDialog(
                            "Room named "
                            + toMerge.name
                            + " name already exists, enter new name!");

                    if (name_dialog.ShowDialog() == DialogResult.OK)
                    {
                        toMerge.name = name_dialog.UserInput;
                    }
                    else
                    {
                        return;
                    }
                }

                //Add the room to the list.
                world.Rooms.Add(toMerge);

                switchRooms(toMerge);
            }
        }

        private void menu_change_background_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "PNG Files | *.png";
            dialog.InitialDirectory = ".";
            dialog.Title = "Choose the new background image";

            if(dialog.ShowDialog() == DialogResult.OK)
            {
                string filename = dialog.FileName;

                if (!filename.Contains(".png"))
                {
                    MessageBox.Show("Background image must be .png.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (!filename.Contains("Art\\Backgrounds\\"))
                {
                    MessageBox.Show("Background image must be in the Art\\Backgrounds folder.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int startindex = filename.IndexOf("Art\\Backgrounds\\");
                filename = filename.Substring(startindex).Replace(".png", "");
                Console.WriteLine("New background: " + filename);
                currentlySelectedRoom.BackgroundFilename = filename;
            }
        }


        //------------ End of Functions for creating and switching rooms----------------------------



        //------------ Functions for drawing on the drawing pane------------------------------------

        //Paints all the objects on the drawing pane.
        private void pb_Level_Paint(object sender, PaintEventArgs e)
        {
            if (currentState != State.NO_EDITS)
            {
                //Draw the background image

                string background_file = currdir + "\\" + 
                    currentlySelectedRoom.BackgroundFilename + ".png";

                //System.Drawing.Image image = Image.FromFile(background_file);
                //e.Graphics.DrawImage(image, 0, 0, pb_Level.Width, pb_Level.Height);

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
                foreach (SpaceObject obj in currentlySelectedRoom.Objects)
                {
                    if (obj is Door)
                    {
                        drawDoor((Door)obj, e);
                    }
                    else
                    {
                        drawSpaceObject(obj, e);
                    }
                }

                //Finally, if the room is the current room of the world, then draw the player, if it exists.
                if (currentState == State.EDITING_WORLD && world.CurrentRoom == currentlySelectedRoom
                    && world.player != null)
                {
                    drawSpaceObject(world.player, e);
                }
            }

            else
            {
                e.Graphics.FillRectangle(Brushes.Black, pb_Level.Bounds);
            }
        }



        //Draws an arbitrary space object to the drawing pane. 
        // Note: Won't work for doors.
        private void drawSpaceObject(SpaceObject obj, PaintEventArgs e)
        {
            Image img = Image.FromFile(currdir + "\\" + obj.TextureName + ".png");

            //In order to do rotations and scaling in windows forms, we need to determine where
            // the upper right, upper left, and lower left corners map to. We determine these, and
            // tell the graphics to draw the image.


            //Find the new point for the upper-left corner
            DrawPoint upper_left = Conversion.Vector2ToDrawPoint(obj.mapPointOnImage(0, 0));


            //Find the new point for the upper-right corner
            float x_ur = obj.getOriginalWidth();
            float y_ur = 0;

            DrawPoint upper_right = Conversion.Vector2ToDrawPoint(
                obj.mapPointOnImage(x_ur, y_ur));


            //Find the new point for the lower-left corner
            float x_ll = 0;
            float y_ll = obj.getOriginalHeight();

            DrawPoint lower_left = Conversion.Vector2ToDrawPoint(obj.mapPointOnImage(x_ll, y_ll));


            //Define the point mapping.
            DrawPoint[] destmapping = {upper_left, upper_right, lower_left};
            DrawRect srcrect = new DrawRect(0,0,img.Width / obj.NumberOfFrames,img.Height);

            //Draw the image with the specified position and scaling.
            e.Graphics.DrawImage(img, destmapping, srcrect, GraphicsUnit.Pixel);
        }

        //-------End of painting functions------------------------------------------------------------

        /// <summary>
        /// Changes the title of the main editor screen to display the name of the selected room.
        /// </summary>
        private void updateTitle()
        {
            this.Text = "Currently Selected Room: " + currentlySelectedRoom.name;
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
            return (Directory.GetCurrentDirectory()).Replace("bin\\x86\\Debug", "Content").Replace("bin\\x86\\Release", "Content");
        }

        private void showMessage(string title, string text)
        {

            MessageBox.Show(text, title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Exclamation);
        }



        //---------Vefification code -------------------------------------------------------------------
        //These functions below are used to make sure that certain key invariants of the game are enforced.
        //Whenever the user saves or clicks "verify" under the debug menu, these functions find any violations
        //of these invariants, and displays them in a pop-up message. 
        //Right now, these invariants are:
        //
        //A player must exist.
        //No door may be unlinked.
        //Doors must be linked to each other.

        /// <summary>
        /// The start of the invariant checking process.
        /// </summary>
        /// <param name="displayIfOK">Whether or not to display a message if no invariants are broken. True
        /// means display.</param>
        private void verifyInvariants(bool displayIfOK)
        {
            string errors = "";

            if (currentState != State.NO_EDITS)
            {
                if (currentState == State.EDITING_WORLD && world.player == null)
                {
                    errors += "There is no player in the world! The game will not run without a player. \n\n";
                }

                //Check for badly linked doors.
                errors += verifyDoors();

                //Display the errors if they exist
                if (errors != "")
                {
                    showMessage("Invariants broken!", errors);

                }
                else if (displayIfOK)
                {
                    showMessage("No broken invariants!", "No broken invariants!");
                }
            }
        }

        /// <summary>
        /// Checks the doors invariants.
        /// </summary>
        /// <returns>A human-readable list of all door invariants broken. Returns the empty string
        /// if there are no broken invariants.</returns>
        private string verifyDoors()
        {
            string errors = "";

            if (currentState == State.EDITING_WORLD)
            {
                //Look at every door in every room.
                foreach (Room room in world.Rooms)
                {
                    errors += verifyDoorsInRoom(room);
                }
            }
            else if (currentState == State.EDITING_ROOM)
            {
                errors += verifyDoorsInRoom(currentlySelectedRoom);
            }


            return errors;
        }


        private string verifyDoorsInRoom(Room room)
        {
            string errors = "";

            foreach (Door door in room.Doors)
            {
                //The link is null
                if (door.Link == null)
                {
                    errors += "Door " + door.name + " in room " + room.name + " has a null link!\n\n";

                }
                else if (door.Link.Link != door)
                {
                    errors += "Door " + door.name +
                        " in room " + room.name +
                        " links to door " + door.Link.name +
                        " which does not link back!";
                }
            }
            return errors;
        }

        private void verifyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            verifyInvariants(true);
        }

        //------------End of verification stuff----------------------------------------------------------------

        /// <summary>
        /// Apply properties in text box to currently selected object.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void b_ApplyProperties_Click(object sender, EventArgs e)
        {
            // If we've selected an object and text is valid,
            // apply these new settings
            if (currentlySelectedObject != null && AreObjPropertiesValid())
            {
                if (currentlySelectedObject is HazardStatic)
                {
                    ((HazardStatic)currentlySelectedObject).Damage = int.Parse(tb_Damage.Text);
                }
                else if (currentlySelectedObject is HazardDynamic)
                {
                    ((HazardDynamic)currentlySelectedObject).Damage = int.Parse(tb_Damage.Text);
                }
                else if (currentlySelectedObject is Survivor)
                {
                    ((Survivor)currentlySelectedObject).SLevel = int.Parse(tb_SLevel.Text);
                }
                else if (currentlySelectedObject is VanishWall)
                {
                    ((VanishWall)currentlySelectedObject).VLevel = int.Parse(tb_SLevel.Text);
                }

                float newRotation = float.Parse(tb_Rotation.Text);
                currentlySelectedObject.Rotate(currentlySelectedObject.Rotation + MathHelper.ToRadians(newRotation));
                if (currentlySelectedObject is Dynamic)
                    ((Dynamic)currentlySelectedObject).OriginalRotation = -currentlySelectedObject.Rotation;
            }

            if (cbox_Scripted.Enabled)
            {
                ParseScript();
            }

           pb_Level.Refresh();
        }

        // Predicate<string> delegate to remove empty strings
        public static bool IsEmptyString(string s)
        {
            return s == "";
        }

        /// <summary>
        /// Parse script line by line.
        /// </summary>
        private void ParseScript()
        {
            Dynamic dyn = (Dynamic)currentlySelectedObject;
            int atindex = -1; // If -1, @ token was not found
            dyn.Instructions.Clear();

            // Mark flag to enable scripted movement
            dyn.IsScripted = cbox_Scripted.Checked;

            // Parse instructions and send to object
            Regex reg = new Regex(EditorConstants.WINFORM_NEWLINE);
            List<string> instructions = new List<string>(reg.Split(tb_Script.Text));
            instructions.RemoveAll(IsEmptyString);

            for (int i = 0; i < instructions.Count; i++)
            {
                string line = instructions[i];
                Instruction inst = new Instruction();

                if (line[0] == '@')
                {
                    atindex = dyn.Instructions.Count;
                    line = line.Substring(1);
                    if (inst.Initialize(line))
                        dyn.Instructions.Add(inst);

                }
                else if (line == "goto @" && atindex > -1)
                {
                    line = "goto " + atindex;
                    if (inst.Initialize(line))
                        dyn.Instructions.Add(inst);
                }
                else
                {
                    if (inst.Initialize(line))
                        dyn.Instructions.Add(inst);
                }

            }
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

            bool rotationCorrect = float.TryParse(tb_Rotation.Text, out dummyFloat);
            bool slevelCorrect = int.TryParse(tb_SLevel.Text, out dummyInt2) || 
                (!(currentlySelectedObject is Survivor || currentlySelectedObject is VanishWall));
            bool damageCorrect = int.TryParse(tb_Damage.Text, out dummyInt) ||
                (!(currentlySelectedObject is HazardStatic)
                && !(currentlySelectedObject is HazardDynamic));

            return rotationCorrect && damageCorrect && slevelCorrect;
        }

        private void tb_Damage_Leave(object sender, EventArgs e)
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

        private void cbox_Scripted_CheckedChanged(object sender, EventArgs e)
        {
            tb_Script.Enabled = cbox_Scripted.Checked;
        }

        private void b_Front_Click(object sender, EventArgs e)
        {
            if (currentlySelectedObject != null)
            {
                currentlySelectedRoom.Objects.Remove(currentlySelectedObject);
                currentlySelectedRoom.Objects.Add(currentlySelectedObject);
            }
        }
    }
}