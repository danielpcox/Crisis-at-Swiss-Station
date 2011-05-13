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
    class MenuScreenGraphical : MenuScreen
    {
        Vector2 labelLocation = new Vector2(0, 0); // location of the label for which station you have currently selected

        List<string>[] menuState = new List<string>[4];

        List<Vector2> optionPositions = new List<Vector2>()
        {
            new Vector2(0, 0),
            new Vector2(1024, 0),
            new Vector2(0, 768 ),
            new Vector2(1024, 768)
        };

        MouseState ms, prevms;

        public MenuScreenGraphical()
            : base(0, 0, 80, true)
        {
            // only unlocked introduction, so can only see the introduction and intro selected
            menuState[0] = new List<string>() { 
                "Art\\Menus\\menu\\menu_introduction",
                "Art\\Menus\\menu\\menu_introduction_introselect"
            };

            // unlocked recreation, so can select introduction or recreation or nothing
            menuState[1] = new List<string>() { 
                "Art\\Menus\\menu\\menu_recreation",
                "Art\\Menus\\menu\\menu_recreation_introselect",
                "Art\\Menus\\menu\\menu_recreation_recselect"
            };

            // unlocked engineering, so can select introduction or recreation or engineering or nothing
            menuState[2] = new List<string>() { 
                "Art\\Menus\\menu\\menu_engineering",
                "Art\\Menus\\menu\\menu_engineering_introselect",
                "Art\\Menus\\menu\\menu_engineering_recselect",
                "Art\\Menus\\menu\\menu_engineering_engselect"
            };

            // unlocked core, so can select introduction or recreation or engineering or core or nothing
            menuState[3] = new List<string>() { 
                "Art\\Menus\\menu\\menu_core",
                "Art\\Menus\\menu\\menu_core_introselect",
                "Art\\Menus\\menu\\menu_core_recselect",
                "Art\\Menus\\menu\\menu_core_engselect",
                "Art\\Menus\\menu\\menu_core_coreselect"
            };
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 rollingPos = new Vector2(initialX, initialY);

            int currentFloor = GameEngine.savedgame.GetCurrentFloor(Options.Count);
            currentFloor = currentFloor % Options.Count - 1;

            //DEBUG
            //Console.WriteLine("GOT HERE");
            //Console.WriteLine(currentFloor);
            //Console.WriteLine(selected);

            spriteBatch.Draw(GameEngine.TextureList[menuState[currentFloor][selected+1]],
                        new Rectangle(0, 0, GameEngine.SCREEN_WIDTH, GameEngine.SCREEN_HEIGHT), Color.White);

            for (int i = 0; i < options.Count; i++)
            {
                spriteBatch.DrawString(font,
                    options[i].Text,
                    rollingPos, // labelLocation
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

            float distance_lwm = 1280; // distance low-water-mark is sqrt(1024^2 + 768^2)
            Vector2 low_water_mark = new Vector2(1024, 768);

            // if the current mouse position is different from the previous one, update
            // the selected menu item with the one closest to the mouse
            if (ms.X != prevms.X || ms.Y != prevms.Y)
            {
                Console.WriteLine(new Vector2(ms.X, ms.Y));
                foreach (Vector2 optionPos in optionPositions)
                {
                    float distance = Vector2.Distance(optionPos, new Vector2(ms.X, ms.Y));
                    if (distance < distance_lwm)
                    {
                        low_water_mark = optionPos;
                        distance_lwm = distance;
                    }
                }
                selected = optionPositions.IndexOf(low_water_mark);
            }

            base.Update();
            prevms = ms;
        }
    }
}
