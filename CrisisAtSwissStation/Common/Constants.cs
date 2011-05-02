﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CrisisAtSwissStation.Common
{
    class Constants
    {
        public const string NEW_GAME_NAME = "new_game.world";

        public const string SAVED_GAME_FILENAME = "SAVEDGAME.sav";

        public static List<MenuCommand> floors = new List<MenuCommand> { MenuCommand.LoadGenesis, MenuCommand.LoadExodus, MenuCommand.LoadLeviticus, MenuCommand.LoadNumbers};

        public const float HALF_GUN = 30f;
    }
}
