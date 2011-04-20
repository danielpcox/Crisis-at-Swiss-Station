using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrisisAtSwissStation.Common
{
    public class Conversion
    {
        //Converts between the XNA points and the Drawing points
        public static Microsoft.Xna.Framework.Point PointToPoint(System.Drawing.Point pt){
            return (new Microsoft.Xna.Framework.Point(pt.X, pt.Y));
        }

       public static System.Drawing.Point PointToPoint(Microsoft.Xna.Framework.Point pt){
            return (new System.Drawing.Point(pt.X, pt.Y));
        }

        //Converts between XNA rectangles and drawing RectangleFs
        public static System.Drawing.RectangleF RectToRect(Microsoft.Xna.Framework.Rectangle rect){
            return (new System.Drawing.RectangleF(rect.X, rect.Y, rect.Width, rect.Height));
        }


        //Converts from a Vector2 to an XnaPoint
        public static Microsoft.Xna.Framework.Point Vector2ToXnaPoint(Microsoft.Xna.Framework.Vector2 toConvert)
        {
            return (new Microsoft.Xna.Framework.Point((int) toConvert.X, (int) toConvert.Y));
        }


        //Converts from a Point to a Vector2
        public static Microsoft.Xna.Framework.Vector2 DrawPointToVector2(System.Drawing.Point toConvert)
        {
            return (new Microsoft.Xna.Framework.Vector2(toConvert.X, toConvert.Y));
        }

        //Converts from a Vector2 to a DrawPoint
        public static System.Drawing.Point Vector2ToDrawPoint(Microsoft.Xna.Framework.Vector2 toConvert)
        {
            return (new System.Drawing.Point((int) toConvert.X, (int) toConvert.Y));
        }
    }
}
