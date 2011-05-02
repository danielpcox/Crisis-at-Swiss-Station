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
    class MenuScreen
    {
        public static readonly Color Selected = new Color(255, 247, 153);
        public static readonly Color Unselected = new Color(182,0,0);

        private float initialX;
        private float initialY;
        /// <summary>
        /// Y distance between menu options
        /// </summary>
        private float distY;

        /// <summary>
        /// Options player has to choose
        /// </summary>
        private List<MenuOption> options;
        public List<MenuOption> Options
        {
            get { return options; }
        }

        private int selected;
        private bool returnSelected; // Return command to menu

        /// <summary>
        /// Title of menu
        /// </summary>
        private string title;

        public MenuController controller;

        private SpriteFont font;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">X coordinate of first option</param>
        /// <param name="y">Y coordinate of first option</param>
        /// <param name="vertDist">Y distance to next option</param>
        public MenuScreen(float x, float y, float vertDist)
        {
            controller = new MenuController();
            options = new List<MenuOption>();

            selected = 0; // Set default selected item to first

            initialX = x;
            initialY = y;
            distY = vertDist;
        }

        public void LoadContent(ContentManager Content)
        {
            font = Content.Load<SpriteFont>("MenuFont");
        }

        /// <summary>
        /// Draw current menu, including state
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 rollingPos = new Vector2(initialX, initialY);

            for (int i = 0; i < options.Count; i++)
            {
                spriteBatch.DrawString(font,
                    options[i].Text,
                    rollingPos,
                    (i == selected ? Selected : Unselected));

                rollingPos.Y += distY;
            }
        }

        public void Update()
        {
            controller.UpdateInput();
            returnSelected = false;

            switch (controller.ControlCode)
            {
                case MenuInput.DOWN:
                    selected += 1;
                    break;

                case MenuInput.UP:
                    selected -= 1;
                    break;

                case MenuInput.SELECT:
                    returnSelected = true;
                    break;
            }

            selected = (selected + options.Count) % options.Count;
        }

        /// <summary>
        /// Returns selected command.  Null if not returning command.
        /// </summary>
        /// <returns></returns>
        public MenuOption ReturnCommand()
        {
            if (!returnSelected)
                return null;

            return options[selected];
        }
    }
}
