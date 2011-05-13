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
    /// <summary>
    /// Defines how user can manipulate menu
    /// </summary>
    public enum MenuInput
    {
        NONE,
        UP,
        DOWN,
        SELECT,
    }

    public class MenuController
    {
        // Instantiates keyboard states
        public KeyboardState keyState, prevState = Keyboard.GetState();

        private MenuInput controlCode;
        public MenuInput ControlCode
        {
            get { return controlCode; }
        }

        public void UpdateInput()
        {
            keyState = Keyboard.GetState();
            MouseState ms = Mouse.GetState();

            controlCode = MenuInput.NONE;

            if ( (keyState.IsKeyDown(Keys.S) || keyState.IsKeyDown(Keys.Down)) && !(prevState.IsKeyDown(Keys.Down) || prevState.IsKeyDown(Keys.S)) )
                controlCode = MenuInput.DOWN;
            else if ( (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W)) && !(prevState.IsKeyDown(Keys.Up) || prevState.IsKeyDown(Keys.W)) )
                controlCode = MenuInput.UP;
            else if ( ms.LeftButton == ButtonState.Pressed ||  keyState.IsKeyDown(Keys.Enter) && !prevState.IsKeyDown(Keys.Enter))
                controlCode = MenuInput.SELECT;
            prevState = keyState;
        }
    }
}
