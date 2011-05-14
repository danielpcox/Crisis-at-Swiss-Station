using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrisisAtSwissStation.Common;

namespace CrisisAtSwissStation
{
    [Serializable]
    public class SavedGame
    {
        public List<int> disabledOptions = new List<int>();
        //public int currentRoom = 0;
        public bool[,] roomsBeatenBitmap = new bool[4,5]{
        {false, false, false, false, false},
        {false, false, false, false, false},
        {false, false, false, false, false},
        {false, false, false, false, false}
        };
        public void SaveGame()
        {
            //string currdir = (Directory.GetCurrentDirectory()).Replace("bin\\x86\\Debug", "Content").Replace("bin\\x86\\Release", "Content").Replace("\\Worlds", "");
            Serializer.Serialize(this, GameEngine.GetCurrDir() + "\\" + Constants.SAVED_GAME_FILENAME);
        }
        public bool LoadGame()
        {
            //string currdir = (Directory.GetCurrentDirectory()).Replace("bin\\x86\\Debug", "Content").Replace("bin\\x86\\Release", "Content").Replace("\\Worlds", "");
            if (File.Exists(GameEngine.GetCurrDir() + "\\" + Constants.SAVED_GAME_FILENAME))
            {
                SavedGame tmp = Serializer.DeSerialize(GameEngine.GetCurrDir() + "\\" + Constants.SAVED_GAME_FILENAME, true);
                disabledOptions = tmp.disabledOptions;
                //currentRoom = tmp.currentRoom;
                roomsBeatenBitmap = tmp.roomsBeatenBitmap;
                return true;
            }
            return false;
        }

        public int GetCurrentFloor(int numoptions = 200)
        {
            int low_water_mark = numoptions; // starts as the maximum number of options, or "infinity" (200 is pretty close)
            foreach (int floor in disabledOptions)
            {
                if (floor < low_water_mark)
                {
                    low_water_mark = floor;
                }
            }
            return low_water_mark - 1;
        }
    }
}
