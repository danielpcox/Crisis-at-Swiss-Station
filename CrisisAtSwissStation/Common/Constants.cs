using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CrisisAtSwissStation.Common
{
    class Constants
    {
        public const float MAX_DRAWING_LENGTH = 800; // NOT USED YET

        public const string NEW_GAME_NAME = "introduction1.world";

        public const string SAVED_GAME_FILENAME = "SAVEDGAME.sav";

        public static List<MenuCommand> floors = new List<MenuCommand> { MenuCommand.LoadGenesis, MenuCommand.LoadExodus, MenuCommand.LoadLeviticus, MenuCommand.LoadNumbers};

        public const float HALF_GUN = 30f;

        public const float STEEPEST_SLOPE =(float) Math.PI / 3f; // in radians, the steepest slope Cosmo can walk up comfortably
        public const float FLAT_ENOUGH =(float) Math.PI / 6f; // in radians, the the steepest slope we'll call flat (10 degrees)
    }
}
