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

namespace CrisisAtSwissStation
{
    

    class MenuEngine
    {
        /// <summary>
        /// Current screen viewed by user
        /// </summary>
        MenuScreen currentScreen;

        private List<MenuScreen> screens;
        public List<MenuScreen> Screens
        {
            get { return screens; }
        }

        private MenuCommand command;

        /// <summary>
        /// 
        /// </summary>
        public MenuEngine()
        {
            screens = new List<MenuScreen>();
            command = MenuCommand.NONE;
        }

        /// <summary>
        /// Load content for menu screens
        /// </summary>
        public void LoadContent(ContentManager Content)
        {
            foreach (MenuScreen screen in screens)
            {
                screen.LoadContent(Content);
            }
        }

        public void Reset()
        {
            currentScreen = screens[0];
        }

        public void NextMenu()
        {
            try
            {
                currentScreen = screens[screens.IndexOf(currentScreen) + 1];
            }
            finally
            {
            }
        }

        public void Update()
        {
            currentScreen.Update();

            MenuOption selected = currentScreen.ReturnCommand(); // command returned by user

            if (selected != null)
            {
                switch (selected.Type)
                {
                    case MenuOptionType.Link:
                        KeyboardState prevks = currentScreen.controller.keyState;
                        currentScreen = selected.Link;
                        // pass along the previous keystate the first time so we don't duplicate selection
                        currentScreen.controller.prevState = prevks;
                        break;

                    case MenuOptionType.Command:
                        command = selected.Command;
                        break;

                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            currentScreen.Draw(spriteBatch);
        }

        /// <summary>
        /// Returns command to main game loop and resets this menu's command
        /// to prevent getting stuck in an infinite loop.
        /// </summary>
        /// <returns></returns>
        public MenuCommand ReturnAndResetCommand(MenuCommand forcedCommand = MenuCommand.NONE)
        {
            if (!(forcedCommand == MenuCommand.NONE))
                return forcedCommand;
            MenuCommand temp = command;
            command = MenuCommand.NONE;
            return temp;
        }
    }
}
