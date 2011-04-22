using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using CrisisAtSwissStation.Common;
using System.Threading;

using Forms = System.Windows.Forms; // alias as Forms so that we don't get collisions with keyboard stuff

namespace CrisisAtSwissStation
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameEngine : Microsoft.Xna.Framework.Game
    {
        private const bool LOAD_FROM_FILE = false;

        static MenuEngine currentMenu;
        static MenuEngine startMenu = new MenuEngine(), optionMenu = new MenuEngine(), pauseMenu = new MenuEngine();

        KeyboardState keyState, prevKeyState;

        public static Texture2D onepixel; // used for darkening the string
        public static Texture2D menuBack;
        public static Texture2D menuPanel;
        
        enum ProgramState
        {
            Menu,
            Playing,
            EditorOpen
        }
        

        static ProgramState progstate;

        /// <summary>
        /// Sets the current menu
        /// </summary>
        /// <param name="menu"></param>
        private static void SetCurrentMenu(MenuEngine menu)
        {
            currentMenu = menu;
            currentMenu.Reset();
        }

        //textbox for setting insta-steel
        //TextBox instaSteelTextBox;
        //textbox for setting Jump
        //TextBox jumpTextBox;

        public static bool level_editor_open = false;

        Forms.Form editor;
       
        private static Dictionary<string, Texture2D> textureList = new Dictionary<string, Texture2D>();
        public static Dictionary<string, Texture2D> TextureList
        {
            get { return textureList; }
        }
    
        public const int SCREEN_WIDTH = 1024; // the width of the screen
        public const int SCREEN_HEIGHT = 768; // the height of the scren

        // How many frames after winning/losing do we continue?

        const int COUNTDOWN = 200;
        
        // Lets us draw things on the screen
        GraphicsDeviceManager graphics;

        // Manages sprite drawing
        SpriteBatch spriteBatch;

        // Manages polygon drawing
        PolygonDrawer polygonDrawer;

        // Font
        SpriteFont spriteFont;

        // Manages audio
        static AudioManager audioManager;

        // Victory and Failure Textures
        Texture2D victory;
        Texture2D failure;

        // Win/Lose countdown
        int countdown = COUNTDOWN;

        // Current world instance
        CASSWorld currentWorld;
        string cwname;

        // Keyboard state
        KeyboardState lastKeyboardState;

        /**
         * Gets the unique instance of GameEngine
         */
        private static GameEngine instance;
        public static GameEngine Instance
        {
            get
            {
                if (instance == null)
                    instance = new GameEngine();
                return instance;
            }
        }

        /**
         * Constructor is private - can only be called via
         * the Instance property
         */
        private GameEngine()
        {
            graphics = new GraphicsDeviceManager(this);
            
            //Adds an audio manager when the constructor is called
            audioManager = new AudioManager();
            audioManager.Initialize();

            InitializeMenus();
            progstate = ProgramState.Menu;

            Content.RootDirectory = "Content";
        }

        /**
         * Gets a sprite batch for drawing
         */
        public SpriteBatch SpriteBatch
        { get { return spriteBatch; } }

        /**
         * Gets an audio manager to play music
         */
        public static AudioManager AudioManager
        {
            get { return audioManager; }
        }


        /**
         * Gets a polygon drawer for drawing
         */
        public PolygonDrawer PolygonDrawer
        { get { return polygonDrawer; } }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            this.IsMouseVisible = false;
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.ApplyChanges();

            //initializes our InstaSteel text box
            /*
            instaSteelTextBox = new TextBox(new Vector2(50, 160), 80, Content);//instantiates with vector for location, 80 is the width, Content is a content manager
            instaSteelTextBox.SetBgColor(Color.White);
            instaSteelTextBox.SetTextColor(Color.Black);

            //initializes our jumpTextBox
            jumpTextBox = new TextBox(new Vector2(50, 220), 80, Content);
            jumpTextBox.SetBgColor(Color.White);
            jumpTextBox.SetTextColor(Color.Black);
            */

     //       audioManager.Play(AudioManager.MusicSelection.EarlyLevelv2);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            polygonDrawer = new PolygonDrawer(GraphicsDevice, Window.ClientBounds.Width, Window.ClientBounds.Height);
            spriteFont = Content.Load<SpriteFont>("PhysicsFont");
            victory = Content.Load<Texture2D>("Victory");
            failure = Content.Load<Texture2D>("failure");
            audioManager.LoadContent(Content);

            startMenu.LoadContent(Content);
            optionMenu.LoadContent(Content);
            pauseMenu.LoadContent(Content);

            onepixel = Content.Load<Texture2D>("Art\\Misc\\onepixel");
            menuBack= Content.Load<Texture2D>("Art\\Menus\\menu_back");
            menuPanel = Content.Load<Texture2D>("Art\\Menus\\menu_panel");

            DirectoryInfo di = new DirectoryInfo(Directory.GetCurrentDirectory() + "\\Content"); //
            FileInfo[] fileList = di.GetFiles("*.xnb", SearchOption.AllDirectories);                  //
            string test  = fileList[1].DirectoryName;
            string test2 = fileList[1].Name;
            string test3 = fileList[1].FullName;
            //DirectoryInfo di = new DirectoryInfo(Editor.CurrDirHack());
            //FileInfo[] fileList = di.GetFiles("*.png", SearchOption.AllDirectories);

            /*
             * Load each texture and take its accompanying string
             * relative to the Content directory.  The string is what
             * each SpaceObject stores from the level editor.
             */
            foreach (FileInfo file in fileList)
            {
                string dirname = file.DirectoryName + "\\";
                int dirIndex = dirname.LastIndexOf("\\Content") + 9; // Index to remove head of directory
                string fullName = dirname.Substring(dirIndex) + file.Name.Replace(".xnb", ""); // Art\...\..., without .png
                //string fullName = file.DirectoryName.Substring(dirIndex) + "\\" + file.Name.Replace(".xnb", ""); // Art\...\..., without .png
                try
                {
                    Texture2D tex = Content.Load<Texture2D>(fullName);
                    //Console.WriteLine("***" + fullName + "***"); // DEBUG
                    textureList.Add(fullName, tex);
                }
                catch(ContentLoadException)
                {
                }
            }
         
            // TODO : change this to something more appropriate
            //painting texture
            //paintTexture = Content.Load<Texture2D>("paint");
            //halfdotsize = new Vector2(paintTexture.Width / 2, paintTexture.Height / 2);


            // Load world assets
            //ScrollingWorld.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            textureList.Clear();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();


            ////////////// FROM BOUNCE

            bool skipdialog = false;

            keyState = Keyboard.GetState();

            // Allows the game to exit
            if (keyState.IsKeyDown(Keys.F4))
            {
                ExitGame();
            }

            if (keyState.IsKeyDown(Keys.Enter) && !prevKeyState.IsKeyDown(Keys.Enter))
            {
                skipdialog = true;
            }

            switch (progstate)
            {
                case ProgramState.Menu:
                    currentMenu.Update();

                    switch (currentMenu.ReturnAndResetCommand())
                    {
                        case MenuCommand.LinkToMainMenu:
                            LinkToMain();
                            currentWorld = null;
                            audioManager.Stop();
                            break;

                        case MenuCommand.ExitProgram:
                            ExitGame();
                            break;

                        case MenuCommand.Load:
                            if (LoadWorld())
                            {
                                progstate = ProgramState.Playing;
                            }
                            break;

                        case MenuCommand.LaunchEditor:
                            {
                                progstate = ProgramState.EditorOpen;
                                editor = new CrisisAtSwissStation.LevelEditor.Editor();
                                editor.Show();
                            }
                            break;

                        case MenuCommand.Resume:
                            progstate = ProgramState.Playing;
                            break;

                        case MenuCommand.New:
                            if (NewWorld())
                            {
                                progstate = ProgramState.Playing;
                            }
                            break;
                    }

                    break;

                case ProgramState.Playing:

                    //Updates the room.
                    KeyboardState ks = Keyboard.GetState();
                    if (ks.IsKeyDown(Keys.Escape))
                    {
                        EnterMenu();
                    }
                    else if (ks.IsKeyDown(Keys.R))
                    {
                        LoadWorld(cwname);
                    }
                    //world.Update();
                    currentWorld.Simulate((float)gameTime.ElapsedGameTime.TotalSeconds);
                    //SetDebugInfo("Level = " + world.player.Level);
                    break;

                case ProgramState.EditorOpen:

                    if (editor.IsDisposed)
                        progstate = ProgramState.Menu;
                    break;
            }

            //base.Update(gameTime);
            ///////////// END FROM BOUNCE

            //KeyboardState ks = Keyboard.GetState();
            //bool next = ks.IsKeyDown(Keys.N) && lastKeyboardState.IsKeyUp(Keys.N);
            //bool prev = ks.IsKeyDown(Keys.P) && lastKeyboardState.IsKeyUp(Keys.P);
            //bool reset = keyState.IsKeyDown(Keys.R) && prevKeyState.IsKeyUp(Keys.R);
           
            /*
            // exit when they press escape
            if (keyState.IsKeyDown(Keys.Escape))
                this.Exit();

            if (keyState.IsKeyDown(Keys.L) && !GameEngine.level_editor_open)
            {
                editor = new CrisisAtSwissStation.LevelEditor.Editor();
                editor.Show();
                GameEngine.level_editor_open = true;
            }

            // toggle mute if they press 'm' 
            if (keyState.IsKeyDown(Keys.M) && prevKeyState.IsKeyUp(Keys.M))
                audioManager.Mute();

            // Move to next track if they press 'x'
            if (keyState.IsKeyDown(Keys.X) && prevKeyState.IsKeyUp(Keys.X))
                audioManager.PlayNext();
            */

            if (currentWorld != null && (currentWorld.Succeeded || currentWorld.Failed))
            {
                countdown--;
                audioManager.DecreaseMusicVolume(.005f);
                //if (currentWorld.Failed)
                //{
                //    countdown--;
                //}

                //Play the level complete SFX
                if (countdown == 180 && currentWorld.Succeeded)
                {
                    audioManager.Stop();
                    audioManager.Play(CrisisAtSwissStation.AudioManager.SFXSelection.LevelComplete);
                }

                if (countdown == 0)
                {
                    //reset = currentWorld.Failed;
                    LinkToMain();
                    progstate = ProgramState.Menu;
                    currentWorld = null;
                    audioManager.IncreaseMusicVolume(0.5f);
                }
            }
            
            // Current world is invalid for some reason - construct a new one!
            /*
            if (next || reset || currentWorld == null)
            {
                // Uses C# reflection to construct a new world with minimal code
                //currentWorld = worldTypes[currentType].GetConstructor(Type.EmptyTypes).Invoke(null) as DemoWorld;

                string currdir =  (Directory.GetCurrentDirectory()).Replace("bin\\x86\\Debug", "Content").Replace("bin\\x86\\Release", "Content").Replace("\\Worlds", "");
                Console.WriteLine("Current Directory: " + currdir);

                if (LOAD_FROM_FILE)
                {
                    currentWorld = Serializer.DeSerialize(currdir + "\\Worlds\\asdf.world");
                    currentWorld.reloadNonSerializedAssets();
                }
                else
                {
                    currentWorld = new ScrollingWorld();
                }
                
                countdown = 0;
            }
            */

                

            // Just won or lost - initiate countdown
            if (currentWorld!=null && (currentWorld.Failed || currentWorld.Succeeded) && countdown == 0)
                countdown = COUNTDOWN;

           

            //Call to update the Text Box
            /*
            instaSteelTextBox.Update(gameTime);
            jumpTextBox.Update(gameTime);
            */

            //Need the text from the text box to interpret it later
            //string temp = instaSteelTextBox.GetText();
           
            //check if enter pushed and a valid number entered. Update insta-steel accordingly 
            /*
            if (ks.IsKeyDown(Keys.Enter))
            {
                int num;
                bool parseWin = Int32.TryParse(temp, out num);//try to parse the string to int

                if (parseWin)
                {
                     ScrollingWorld.numDrawLeft = (float) Int32.Parse(temp);
                }
            }

            string temp2 = jumpTextBox.GetText();

            //check if enter pushed and a valid number entered. Update Jump accordingly 
            if (ks.IsKeyDown(Keys.Enter))
            {
                float num;
                bool parseWin = float.TryParse(temp2, out num);//try to parse the string to int

                if (parseWin)
                {
                    DudeObject.jumpImpulse = -float.Parse(temp2);
                }
            }
            */


            base.Update(gameTime);
            prevKeyState = keyState;
        }

        public static void EnterMenu()
        {
            SetCurrentMenu(pauseMenu);
            progstate = ProgramState.Menu;
        }

        public static void LinkToMain()
        {
            SetCurrentMenu(startMenu);
        }

        public void ExitGame()
        {
            UnloadContent();
            this.Exit();
        }

        public bool NewWorld()
        {
            string currdir = (Directory.GetCurrentDirectory()).Replace("bin\\x86\\Debug", "Content").Replace("bin\\x86\\Release", "Content").Replace("\\Worlds", "");
            cwname = currdir + "\\Worlds\\" + Constants.NEW_GAME_NAME;
            ScrollingWorld world = Serializer.DeSerialize(cwname);

            if (world != null)
            {
                //this.world = world;
                currentWorld = world;
                //world.LoadContent(Content);
                currentWorld.reloadNonSerializedAssets();
                return true;
            }
            return false;
        }

        public bool LoadWorld()
        {
            ScrollingWorld world;

            //First, choose the file we want to load. This is slightly magic.
            Forms.OpenFileDialog dialog = new Forms.OpenFileDialog();
            dialog.Filter =
               "World Files | *.world";
            dialog.InitialDirectory = ".";
            dialog.Title = "Select a world file.";

            Forms.DialogResult result = dialog.ShowDialog();

            //If the result was ok, load the resultant file into world and return it. Otherwise,
            //return null.
            if (result == Forms.DialogResult.OK)
            {
                world = Serializer.DeSerialize(dialog.FileName);
                cwname = dialog.FileName;
            }
            else
            {
                return false;
            }

            if (world != null)
            {
                //this.world = world;
                currentWorld = world;
                //world.LoadContent(Content);
                currentWorld.reloadNonSerializedAssets();

                //Bug: When reloading a game, player starts at last known rotation, but kicking
                //box is always redrawn from initial rotaion. (So kicking box is offset)
                //Solution: Always create a new player, only transferring state. (No one will notice/care)
                //****// 
                //world.player.Rotation = world.player.OriginalRotation;
                return true;
            }

            return false;
        }

        public bool LoadWorld(string worldpath)
        {
            ScrollingWorld world;

            if (worldpath!=null)
            {
                world = Serializer.DeSerialize(worldpath);
                cwname = worldpath;
            }
            else
            {
                return false;
            }

            if (world != null)
            {
                //this.world = world;
                currentWorld = world;
                //world.LoadContent(Content);
                currentWorld.reloadNonSerializedAssets();

                //Bug: When reloading a game, player starts at last known rotation, but kicking
                //box is always redrawn from initial rotaion. (So kicking box is offset)
                //Solution: Always create a new player, only transferring state. (No one will notice/care)
                //****// 
                //world.player.Rotation = world.player.OriginalRotation;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Initialize the game menus
        /// </summary>
        public void InitializeMenus()
        {
            // Set up main screen
            MenuScreen mainScreen = new MenuScreen(520.0f, 180.0f, 50.0f);
            MenuScreen optionScreen = new MenuScreen(520.0f, 180.0f, 50.0f);

            mainScreen.Options.Add(new MenuOption(MenuOptionType.Command, "New Game", MenuCommand.New));
            mainScreen.Options.Add(new MenuOption(MenuOptionType.Command, "Load Game", MenuCommand.Load));
            mainScreen.Options.Add(new MenuOption(MenuOptionType.Command, "Launch Editor", MenuCommand.LaunchEditor));
            //mainScreen.Options.Add(new MenuOption(MenuOptionType.Link, "Options", optionScreen));
            mainScreen.Options.Add(new MenuOption(MenuOptionType.Command, "Exit", MenuCommand.ExitProgram));

            optionScreen.Options.Add(new MenuOption(MenuOptionType.Setting, "Options here?", MenuCommand.NONE));
            optionScreen.Options.Add(new MenuOption(MenuOptionType.Link, "Main Menu", mainScreen));

            startMenu.Screens.Add(mainScreen);
            startMenu.Screens.Add(optionScreen);

            // Pause screen

            MenuScreen pauseScreen = new MenuScreen(520.0f, 180.0f, 50.0f);

            pauseScreen.Options.Add(new MenuOption(MenuOptionType.Command, "Resume", MenuCommand.Resume));
            pauseScreen.Options.Add(new MenuOption(MenuOptionType.Command, "Load", MenuCommand.Load));
            pauseScreen.Options.Add(new MenuOption(MenuOptionType.Command, "Main Menu", MenuCommand.LinkToMainMenu)); // Can link between engines...

            pauseMenu.Screens.Add(pauseScreen);

            SetCurrentMenu(startMenu);

        }
        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

           
            /*
            if (currentWorld != null)
            {
                currentWorld.Draw(graphics.GraphicsDevice, Matrix.Identity);
            }
            */




            //Draw IS label 
            //spriteBatch.DrawString(spriteFont, "Insta-Steel :", new Vector2(50, 140), Color.Red);
            //Draw IS amount
            //spriteBatch.DrawString(spriteFont, ((int)ScrollingWorld.numDrawLeft).ToString(), new Vector2(155, 140), Color.Yellow);

            //Draw Jump label 
           // spriteBatch.DrawString(spriteFont, "Jump :", new Vector2(50, 200), Color.Red);
            //Draw Jump amount
           // spriteBatch.DrawString(spriteFont, DudeObject.jumpImpulse.ToString(), new Vector2(115, 200), Color.Yellow);

            switch (progstate)
            {
                case ProgramState.Menu:

                    if (currentMenu == pauseMenu)
                    {
                        //currentWorld.Draw(spriteBatch, (float)this.Window.ClientBounds.Width, (float)this.Window.ClientBounds.Height);
                        currentWorld.Draw(graphics.GraphicsDevice, Matrix.Identity);

                        // Draw a "dimming" layer over the world
                        spriteBatch.Begin();
                        spriteBatch.Draw(onepixel,
                            new Rectangle(0, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height),
                            new Color(0,0,0,100));
                        spriteBatch.End();
                    }
                    else
                    {
                        spriteBatch.Begin();
                        spriteBatch.Draw(menuBack,
                            new Rectangle(0, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height), Color.White);
                        currentMenu.Draw(spriteBatch);
                        spriteBatch.End();
                    }
                        
                    spriteBatch.Begin();
                    spriteBatch.Draw(menuPanel,
                                new Rectangle(0, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height), Color.White);
                    currentMenu.Draw(spriteBatch);
                    spriteBatch.End();

                    break;

                case ProgramState.Playing:
                    //currentWorld.Draw(spriteBatch, (float)this.Window.ClientBounds.Width, (float)this.Window.ClientBounds.Height);
                    currentWorld.Draw(graphics.GraphicsDevice, Matrix.Identity);
                    break;
            }

            spriteBatch.Begin();
            // Draw success or failure image
            if (currentWorld!=null && currentWorld.Succeeded && !currentWorld.Failed)
            {
                spriteBatch.Draw(victory, new Vector2((graphics.PreferredBackBufferWidth - victory.Width) / 2,
                    (graphics.PreferredBackBufferHeight - victory.Height) / 2), Color.White);
            }
            else if (currentWorld!=null && currentWorld.Failed)
            {
                spriteBatch.Draw(failure, new Vector2((graphics.PreferredBackBufferWidth - failure.Width) / 2,
                    (graphics.PreferredBackBufferHeight - failure.Height) / 2), Color.White); ;
            }
            spriteBatch.End();
            
            //instaSteelTextBox.Draw(spriteBatch, 255); //draws the textbox here, no transparency 
           // jumpTextBox.Draw(spriteBatch, 255);
            base.Draw(gameTime);
        }
    }
}
