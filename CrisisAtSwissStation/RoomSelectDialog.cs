using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Bounce.Common;

namespace Bounce.LevelEditor
{
    public partial class RoomSelectDialog : Form
    {
        private GameWorld world;

        //The room that is currently selected.
        public Room SelectedRoom
        {
            get;
            set;
        }

        //The door that is currently selected.
        public Door SelectedDoor
        {
            get;
            set;
        }

        //Whether we are selecting a door or a room.
        bool selectRoom;


        //Creates a new RoomSelectDialog, and populates the list of rooms with all the rooms in the world.
        public RoomSelectDialog(GameWorld wld, bool selectRoom, string title)
        {
            world = wld;
            this.selectRoom = selectRoom;

            InitializeComponent();

            this.Text = title;

            //If we are just selecting a room, then hide the door stuff.
            if (selectRoom)
            {
                this.doors_ListBox.Enabled = false;
                this.exit_Label.Enabled = false;
            }


            //Populate the list of rooms in the world
            foreach (Room room in world.Rooms)
            {
                room_ListBox.Items.Add(room.name);
            }

            room_ListBox.SelectedIndex = 0;
        }




        //----------Helper functions---------------------------------------------------------------

        //Clears the listBox of the exits in the selected room, and repopulates it with every Door in 
        private void repopulateExits(Room room)
        {
            doors_ListBox.Items.Clear();

            foreach (Door door in room.Doors)
            {
                doors_ListBox.Items.Add(door.name);
            }

            if (doors_ListBox.Items.Count > 0)
            {
                doors_ListBox.SelectedIndex = 0;
            }
        }


        //Finds the first room in the world's room list named 'name'.
        private Room findNamedRoom(string name)
        {
            foreach (Room room in world.Rooms)
            {
                if (room.name == name)
                {
                    return room;
                }
            }

            return null;
        }

        //Finds the first room in the currently selected room with the name of the given string
        private Door findNamedDoor(string name)
        {
            foreach (Door door in SelectedRoom.Doors)
            {
                if (door.name == name)
                {
                    return door;
                }
            }

            return null;
        }

        //--------Callbacks for the changes in the components.

        //The callback for a change in the selection of the ListBox for the rooms. It updates
        //the field "SelectedRoom" and maybe the list of the doors.
        private void room_ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectedRoom = findNamedRoom((string)room_ListBox.SelectedItem);

            //Populate the list of doors.
            repopulateExits(this.SelectedRoom);
            
        }

        //Callback for change in selection of the door. It updates the "SelectedDoor" field.
        private void doors_ListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.SelectedDoor = findNamedDoor((string)doors_ListBox.SelectedItem);
        }

        private void ok_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void cancel_Button_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
