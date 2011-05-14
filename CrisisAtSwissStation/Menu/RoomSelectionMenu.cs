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
            base.Draw(spriteBatch);

            // draw cursor
            Texture2D crosshair = GameEngine.TextureList["Crosshair"];
            spriteBatch.Draw(crosshair, new Vector2(ms.X - (crosshair.Width / 2), ms.Y - (crosshair.Height / 2)), Color.White);
        }

        public void Update()
        {
            base.Update();
        }
    }
}
