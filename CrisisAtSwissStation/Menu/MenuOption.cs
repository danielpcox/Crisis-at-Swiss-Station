using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace CrisisAtSwissStation{
    /// <summary>
    /// High-level option types
    /// </summary>
    public enum MenuOptionType
    {
        Link,
        Setting,
        Command
    }

    public enum MenuSettingType
    {
        Switch,
        Variable,
        String
    }

    /// <summary>
    /// If menu directly affects game, this command is sent
    /// </summary>
    public enum MenuCommand
    {
        NONE,
        New,
        Continue,
        Resume,
        Load,
        LoadGenesis,
        LoadExodus,
        LoadLeviticus,
        LoadNumbers,
        LoadDeuteronomy,

        LoadIntroduction,
        LoadRecreation,
        LoadEngineering,
        LoadCore,

        Save,
        ExitProgram,
        LaunchEditor,
        LinkToMainMenu,
        LinkToRooms
    }

    public class MenuOption
    {
        private string text;
        public string Text
        {
            get { return text; }
        }

        private MenuOptionType type;
        public MenuOptionType Type
        {
            get { return type; }
        }

        private MenuScreen link;
        public MenuScreen Link
        {
            get { return link; }
        }

        private MenuCommand command;
        public MenuCommand Command
        {
            get { return command; }
        }

        private bool setting_switch;
        private int setting_variable;
        private string setting_string;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type">Type of menu option</param>
        /// <param name="text"></param>
        private MenuOption(MenuOptionType type, string text, MenuScreen link, MenuCommand command)
        {
            this.type = type;
            this.text = text;
            this.link = link;
            this.command = command;
        }

        public MenuOption(MenuOptionType type, string text, MenuScreen link)
            : this(type, text, link, MenuCommand.NONE) { }

        public MenuOption(MenuOptionType type, string text, MenuCommand command)
            : this(type, text, null, command) { }
    }
}
