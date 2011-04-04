using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;

namespace CrisisAtSwissStation.Common
{
    class Serializer
    {

        //-------------Serialization for the world-----------------------------------------------
        public static void Serialize(CASSWorld world, string filename)
        {
                FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);

                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, world );
                }
                finally
                {
                    fs.Close();
                    Console.WriteLine("Saved!");
                }
            }
        
        public static CASSWorld DeSerialize(string filename)
        {
            CASSWorld world;

            // Check for existance of file
            if (!File.Exists(filename))
            {
                MessageBox.Show("File \"" + filename + "\" does not exist!",
                    "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            
                try
                {
                    //The actual game serialization couldn't handle the upgrade to 4.0 :(
                    BinaryFormatter formatter = new BinaryFormatter();
                    world = (CASSWorld) formatter.Deserialize(fs);
   
                }
                finally
                {
                    fs.Close();
                }
            return world;

         }



        //-------------Serialization for rooms-----------------------------------------------------
        /*
        public static void SerializeRoom(Room room, string filename)
        {
                FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);

                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, room );
                }
                finally
                {
                    fs.Close();
                    Console.WriteLine("Saved!");
                }
            }
        
        public static Room DeserializeRoom(string filename)
        {
            Room room;

            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            
                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    room = (Room) formatter.Deserialize(fs);
   
                }
                finally
                {
                    fs.Close();
                }
            return room;

         }
        */
    }
}
