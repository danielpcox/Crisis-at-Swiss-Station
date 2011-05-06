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
        public static void Serialize(SavedRoom sr, string filename)
        {
                FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);

                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, sr);
                }
                finally
                {
                    fs.Close();
                    Console.WriteLine("Saved!");
                }
            }
        
        public static SavedRoom DeSerialize(string filename)
        {
            SavedRoom sr;

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
                    sr = (SavedRoom) formatter.Deserialize(fs);
   
                }
                finally
                {
                    fs.Close();
                }
            return sr;

         }


        public static void Serialize(SavedGame sg, string filename)
        {
                FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write);

                try
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, sg);
                }
                finally
                {
                    fs.Close();
                    Console.WriteLine("Saved!");
                }
            }
        
        public static SavedGame DeSerialize(string filename, bool throwaway)
        {
            SavedGame sg;

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
                    sg = (SavedGame) formatter.Deserialize(fs);
   
                }
                finally
                {
                    fs.Close();
                }
            return sg;

         }

        public static ScrollingWorld OLDDeSerialize(string filename)
        {
            ScrollingWorld world;

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
                    world = (ScrollingWorld) formatter.Deserialize(fs);
   
                }
                finally
                {
                    fs.Close();
                }
            return world;

         }
    }
}
