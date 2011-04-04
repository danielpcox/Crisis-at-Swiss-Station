using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing;

namespace CrisisAtSwissStation.LevelEditor
{
    public enum EditorTool
    {
        Insertion,
        Selection
    }

    class EditorConstants
    {

        //The maximum allowable size of an object.
        public const int MAX_WIDTH = 500;
        public const int MAX_HEIGHT = 500;

        /*//NOTE: Changing these will NOT change the result in the combo box.
        // In fact, everything will break. Since we auto-generated the rest of
        // this class by visual studio, the positions are hard-coded. These just
        // reflect the positions.
        public const int INDEX_FOR_SELECTION = 1;
        public const int INDEX_FOR_INSERTION = 0;*/


        //Defines the width of the bounding box to draw around the selected items, in pixels
        public const int BOUNDING_BOX_WIDTH = 1;

        //Defines the extra space around the background of the selected box
        public const int SELECTION_BOX_SLACK = 10;

        //Defines the width of the little box in the corner of the
        // selected object's bounding box, used to re-size the image
        public const int RESIZE_BOX_DIMS = 5;


        //The color to draw doors.
        public const string DOOR_COLOR = "80ADEAEA";

        //The default size of a door
        public const int DEFAULT_DOOR_SIZE = 25;

        // Scale of target for door
        public const float DOOR_TARGET_WIDTH = 30.0f;

        // Windows forms terminating string
        public const string WINFORM_NEWLINE = "\r\n";
    }
}
