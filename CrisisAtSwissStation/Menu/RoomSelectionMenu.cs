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

namespace CrisisAtSwissStation.Menu
{
    class RoomSelectionMenu : MenuScreen
    {
        public RoomSelectionMenu()
            : base(0, 0, 80, true)
        {
            List<string>[] screenshots = new List<string>[4];
            screenshots[0] = new List<string>()
            {
                "Art\\Menus\\screenshots\\introduction\\introduction1",
                "Art\\Menus\\screenshots\\introduction\\introduction2",
                "Art\\Menus\\screenshots\\introduction\\introduction3",
                "Art\\Menus\\screenshots\\introduction\\introduction4",
                "Art\\Menus\\screenshots\\introduction\\introduction5"
            };
            screenshots[1] = new List<string>()
            {
                "Art\\Menus\\screenshots\\recreation\\recreation1",
                "Art\\Menus\\screenshots\\recreation\\recreation2",
                "Art\\Menus\\screenshots\\recreation\\recreation3",
                "Art\\Menus\\screenshots\\recreation\\recreation4",
                "Art\\Menus\\screenshots\\recreation\\recreation5"
            };
            screenshots[2] = new List<string>()
            {
                "Art\\Menus\\screenshots\\engineering\\engineering1",
                "Art\\Menus\\screenshots\\engineering\\engineering2",
                "Art\\Menus\\screenshots\\engineering\\engineering3",
                "Art\\Menus\\screenshots\\engineering\\engineering4",
                "Art\\Menus\\screenshots\\engineering\\engineering5"
            };
            screenshots[3] = new List<string>()
            {
                "Art\\Menus\\screenshots\\core\\core1",
                "Art\\Menus\\screenshots\\core\\core2",
                "Art\\Menus\\screenshots\\core\\core3",
                "Art\\Menus\\screenshots\\core\\core4",
                "Art\\Menus\\screenshots\\core\\core5"
            };
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // draw cursor
            Texture2D crosshair = GameEngine.TextureList["Crosshair"];
            spriteBatch.Draw(crosshair, new Vector2(ms.X - (crosshair.Width / 2), ms.Y - (crosshair.Height / 2)), Color.White);
        }

        public void Update()
        {
            ms = Mouse.GetState();

            float distance_lwm = 1280; // distance low-water-mark is sqrt(1024^2 + 768^2)
            Vector2 low_water_mark = new Vector2(1024, 768);

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

            // if the current mouse position is different from the previous one, update
            // the selected menu item with the one closest to the mouse
            /*
            if (ms.X != prevms.X || ms.Y != prevms.Y)
            {
                //Console.WriteLine(new Vector2(ms.X, ms.Y));
                foreach (Vector2 optionPos in optionPositions)
                {
                    float distance = Vector2.Distance(optionPos, new Vector2(ms.X, ms.Y));
                    if (distance < distance_lwm)
                    {
                        low_water_mark = optionPos;
                        distance_lwm = distance;
                    }
                }
                int potential_new_selected = optionPositions.IndexOf(low_water_mark);
                if (!GameEngine.savedgame.disabledOptions.Contains(potential_new_selected))
                    selected = potential_new_selected;
            }

            if (ms.LeftButton == ButtonState.Pressed && prevms.LeftButton != ButtonState.Pressed)
                returnSelected = true;
            prevms = ms;
            */
        }
    }
}
