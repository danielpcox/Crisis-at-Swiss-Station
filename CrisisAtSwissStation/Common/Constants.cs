using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CrisisAtSwissStation.Common
{
    class Constants
    {
        public const string NEW_GAME_NAME = "introduction.world";

        public const string SAVED_GAME_FILENAME = "SAVEDGAME.sav";

        public static List<MenuCommand> floors = new List<MenuCommand> { MenuCommand.LoadGenesis, MenuCommand.LoadExodus, MenuCommand.LoadLeviticus, MenuCommand.LoadNumbers};

        public const float HALF_GUN = 30f;

        public const float STEEPEST_SLOPE =(float) Math.PI / 6f; // in radians, the steepest slope Cosmo can walk up comfortably
    }
}
