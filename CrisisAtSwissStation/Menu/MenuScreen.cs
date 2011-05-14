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
    public class MenuScreen
    {
        public static readonly Color Selected = new Color(255, 247, 153);
        public static readonly Color Unselected = new Color(182,0,0);
        public static readonly Color Disabled = new Color(25, 25, 25);

        public MouseState ms, prevms;

        protected bool sgconnect = false;

        protected float initialX;
        protected float initialY;
        /// <summary>
        /// Y distance between menu options
        /// </summary>
        protected float distY;

        /// <summary>
        /// Options player has to choose
        /// </summary>
        protected List<MenuOption> options;
        //public List<int> disabledOptions = new List<int>();
        public List<MenuOption> Options
        {
            get { return options; }
        }

        public int selected;
        protected bool returnSelected; // Return command to menu

        /// <summary>
        /// Title of menu
        /// </summary>
        protected string title;

        public MenuController controller;

        protected SpriteFont font;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">X coordinate of first option</param>
        /// <param name="y">Y coordinate of first option</param>
        /// <param name="vertDist">Y distance to next option</param>
        public MenuScreen(float x, float y, float vertDist, bool sgConnect = false)
        {
            controller = new MenuController();
            options = new List<MenuOption>();

            sgconnect = sgConnect;

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
                    (sgconnect && GameEngine.savedgame.disabledOptions.Contains(i) ? Disabled : (i == selected ? Selected : Unselected) ));

                rollingPos.Y += distY;
            }

            // draw cursor
            Texture2D crosshair = GameEngine.TextureList["Crosshair"];
            spriteBatch.Draw(crosshair, new Vector2(ms.X - (crosshair.Width/2), ms.Y - (crosshair.Height/2)), Color.White);
        }

        public void Update()
        {
            ms = Mouse.GetState();

            controller.UpdateInput();
            returnSelected = false;

            int viable = selected;
            switch (controller.ControlCode)
            {
                case MenuInput.DOWN:
                    viable += 1;
                    while (sgconnect && GameEngine.savedgame.disabledOptions.Contains(viable))
                        viable += 1;
                    //Console.WriteLine(viable); // DEBUG
                    selected = viable;
                    break;

                case MenuInput.UP:
                    viable -= 1;
                    if (viable < 0)
                        viable = Options.Count - Math.Abs(viable); // HACK
                    while (sgconnect && GameEngine.savedgame.disabledOptions.Contains(viable))
                        viable -= 1;
                    // Console.WriteLine(viable); // DEBUG
                    selected = viable;
                    break;

                case MenuInput.SELECT:
                    returnSelected = true;
                    break;
            }

            if (controller.ControlCode == MenuInput.NONE)
            {
                // code to click menu items
                if (ms.X != prevms.X || ms.Y != prevms.Y)
                {
                    Vector2 rollingPos = new Vector2(initialX, initialY);
                    int low_water_mark = 0;
                    float distance_lwm = 1280;
                    for (int i = 0; i < options.Count; i++)
                    {
                        float distance = Vector2.Distance(rollingPos + new Vector2(0, 35), new Vector2(ms.X, ms.Y));
                        //Console.WriteLine(distance); // DEBUG
                        if (distance < distance_lwm)
                        {
                            low_water_mark = i;
                            distance_lwm = distance;
                        }
                        rollingPos.Y += distY;
                    }
                    selected = low_water_mark;
                }

                //selected = (selected + options.Count) % options.Count;
                selected = (selected) % options.Count;

                // actually click the button
                if (ms.LeftButton == ButtonState.Pressed && prevms.LeftButton != ButtonState.Pressed && ms.X < GameEngine.SCREEN_WIDTH && ms.X > 0 && ms.Y > 0 && ms.Y < GameEngine.SCREEN_HEIGHT)
                    returnSelected = true;
            }

            prevms = ms;
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
