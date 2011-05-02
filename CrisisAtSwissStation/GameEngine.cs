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
        static MenuEngine startMenu = new MenuEngine(), pauseMenu = new MenuEngine();
        static MenuScreen floorsScreen;

        public static SavedGame savedgame = new SavedGame();

        KeyboardState keyState, prevKeyState;

        public static Texture2D onepixel; // used for darkening the screen
        public static Texture2D menuBack;
        public static Texture2D menuPanel;
        public static Texture2D menuPanelAnimation;
        
        //victory animation stuff
        private static bool animate;
        private static Rectangle sourceRect;
        private Vector2 origin;
        private static int xFrame;
        private static int yFrame;
        private static int spriteWidth;
        private static int spriteHeight;           
        private int myGameTime, animateTimer, animateInterval;

        //menu animation stuff
        private static bool menuAnimate;
        private static Rectangle menuSourceRect;
        private Vector2 menuOrigin;
        private static int menuXFrame;
        private static int menuYFrame;
        private static int menuSpriteWidth;
        private static int menuSpriteHeight;           
        private int menuMyGameTime, menuAnimateTimer, menuAnimateInterval;


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

            //victory animation stuff
            animate = false;
            myGameTime = 0;
            animateTimer = 0;
            animateInterval = 20;
            xFrame = 0;
            yFrame = 0;           
            spriteWidth = 700;
            spriteHeight = 350;
            sourceRect = new Rectangle(xFrame * spriteWidth, yFrame * spriteHeight, spriteWidth, spriteHeight);
            origin = new Vector2(sourceRect.Width / 2, sourceRect.Height / 2);

            //menuanimation stuff
            menuAnimate = false;


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
            victory = Content.Load<Texture2D>("Art\\victory_strip");
            failure = Content.Load<Texture2D>("failure");
            audioManager.LoadContent(Content);

            startMenu.LoadContent(Content);
            //floorsMenu.LoadContent(Content);
            pauseMenu.LoadContent(Content);

            onepixel = Content.Load<Texture2D>("Art\\Misc\\onepixel");
            menuBack= Content.Load<Texture2D>("Art\\Menus\\menu_back");
            menuPanel = Content.Load<Texture2D>("Art\\Menus\\menu_panel");
            menuPanelAnimation = Content.Load<Texture2D>("Art\\Menus\\main_strip");

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


        public static void resetVictoryAnimation()
        {
            animate = false;
            xFrame = 0;
            yFrame = 0;
            sourceRect = new Rectangle(xFrame * spriteWidth, yFrame * spriteHeight, spriteWidth, spriteHeight);

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            
            //ronnie added for animation
            //Console.WriteLine("{0} {1} {2} {3}", myGameTime, xFrame,yFrame,animateTimer);
            if (animate == true)
            {
                myGameTime++;
                sourceRect = new Rectangle(xFrame * spriteWidth, yFrame * spriteHeight, spriteWidth, spriteHeight);
                if (!((xFrame == 3) && (yFrame == 1)))
                {
                    animateTimer += myGameTime;

                    if (animateTimer > animateInterval)
                    {
                        xFrame++;

                        if (xFrame > 4 && yFrame == 0)
                        {
                            xFrame = 0;
                            yFrame = 1;
                        }
                        else if (xFrame > 3 && yFrame == 1)
                        {
                            xFrame = 3;
                            yFrame = 1;
                        }                     

                        // -= (int)walkInterval;
                        myGameTime = 0;
                        animateTimer = 0;
                    }
                }
            }
            


            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

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
                                countdown = COUNTDOWN;
                                progstate = ProgramState.Playing;
                            }
                            break;

                        case MenuCommand.LoadGenesis:
                            if (LoadRelWorld("genesis"))
                            {
                                countdown = COUNTDOWN;
                                progstate = ProgramState.Playing;
                            }
                            break;

                        case MenuCommand.LoadExodus:
                            if (LoadRelWorld("exodus"))
                            {
                                countdown = COUNTDOWN;
                                progstate = ProgramState.Playing;
                            }
                            break;

                        case MenuCommand.LoadLeviticus:
                            if (LoadRelWorld("leviticus"))
                            {
                                countdown = COUNTDOWN;
                                progstate = ProgramState.Playing;
                            }
                            break;

                        case MenuCommand.LoadNumbers:
                            if (LoadRelWorld("numbers"))
                            {
                                countdown = COUNTDOWN;
                                progstate = ProgramState.Playing;
                            }
                            break;

                        case MenuCommand.LoadDeuteronomy:
                            if (LoadRelWorld("deuteronomy"))
                            {
                                countdown = COUNTDOWN;
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
                           // menuAnimate = true;
                            //animateMyMenu();
                            if (NewWorld())
                            {
                                countdown = COUNTDOWN;
                                progstate = ProgramState.Playing;
                            }
                            break;
                    }

                    break;

                case ProgramState.Playing:
                    //menuAnimate = false;
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
                    else if (ks.IsKeyDown(Keys.N))
                    {
                        LoadNextWorld();
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

            //toggle mute if they press 'm' 
            if (keyState.IsKeyDown(Keys.M) && prevKeyState.IsKeyUp(Keys.M))
                audioManager.Mute();

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

                if (countdown <= 0 && currentWorld.Succeeded && !currentWorld.Failed)
                {
                    //reset = currentWorld.Failed;
                    if (!audioManager.isMuted())
                    {
                        audioManager.IncreaseMusicVolume(0.5f);
                    }
                    LoadNextWorld();
                }
                else if (countdown <= 0) // failed
                {
                        LoadWorld(cwname);
                        progstate = ProgramState.Playing;
                        countdown = COUNTDOWN;
                        if (!audioManager.isMuted())
                        {
                            audioManager.IncreaseMusicVolume(0.5f);
                        }
                }
            }
                
            // Just won or lost - initiate countdown
            if (currentWorld!=null && (currentWorld.Failed || currentWorld.Succeeded) && countdown <= 0)
                countdown = COUNTDOWN;

            base.Update(gameTime);
            prevKeyState = keyState;
        }

        private void LoadNextWorld()
        {
            int levelnum;
            string[] pieces = cwname.Split('.');
            bool parsed = int.TryParse(pieces[0].Substring(pieces[0].Length - 1), out levelnum);
            string origname;
            if (parsed)
            {
                origname = pieces[0].Substring(0, pieces[0].Length - 1);
            }
            else
            {
                origname = pieces[0];
            }

            string newfilename = origname + (levelnum + 1) + ".world";

            if (File.Exists(newfilename))
            {
                LoadWorld(newfilename);
                savedgame.currentRoom++; savedgame.SaveGame();
                progstate = ProgramState.Playing;
            }
            else
            {
                EnableNextFloor();
                LinkToFloors();
                progstate = ProgramState.Menu;
                currentWorld = null;
            }

            countdown = COUNTDOWN;
        }

        public void EnableNextFloor()
        {
            int low_water_mark = 200; // starts as "infinity"
            foreach (int floor in savedgame.disabledOptions)
            {
                if (floor < low_water_mark)
                {
                    low_water_mark = floor;
                }
            }
            savedgame.currentRoom = 0;
            savedgame.disabledOptions.Remove(low_water_mark);
            savedgame.SaveGame();
            //floorsScreen.disabledOptions.Remove(low_water_mark);
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

        public static void LinkToFloors()
        {
            SetCurrentMenu(startMenu);
            startMenu.NextMenu();
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

        // load a world with a pathname relative to the worlds directory
        public bool LoadRelWorld(string worldname)
        {
            string currdir = (Directory.GetCurrentDirectory()).Replace("bin\\x86\\Debug", "Content").Replace("bin\\x86\\Release", "Content").Replace("\\Worlds", "");
            cwname = currdir + "\\Worlds\\" + worldname + ".world";
            return LoadWorld(cwname);
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
                //world = new ScrollingWorld("background"); // DEBUG
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
            floorsScreen = new MenuScreen(520.0f, 150.0f, 50.0f, true);

            mainScreen.Options.Add(new MenuOption(MenuOptionType.Command, "New Game", MenuCommand.New));
            //mainScreen.Options.Add(new MenuOption(MenuOptionType.Command, "Load Game", MenuCommand.Load));
            mainScreen.Options.Add(new MenuOption(MenuOptionType.Link, "Select Floor", floorsScreen));
            mainScreen.Options.Add(new MenuOption(MenuOptionType.Command, "Launch Editor", MenuCommand.LaunchEditor));
            mainScreen.Options.Add(new MenuOption(MenuOptionType.Command, "Exit", MenuCommand.ExitProgram));

            floorsScreen.Options.Add(new MenuOption(MenuOptionType.Command, "Genesis", MenuCommand.LoadGenesis));
            floorsScreen.Options.Add(new MenuOption(MenuOptionType.Command, "Exodus", MenuCommand.LoadExodus));
            floorsScreen.Options.Add(new MenuOption(MenuOptionType.Command, "Leviticus", MenuCommand.LoadLeviticus));
            floorsScreen.Options.Add(new MenuOption(MenuOptionType.Command, "Numbers", MenuCommand.LoadNumbers));
            //floorsScreen.Options.Add(new MenuOption(MenuOptionType.Setting, "Deuteronomy", MenuCommand.LoadDeuteronomy));
            floorsScreen.Options.Add(new MenuOption(MenuOptionType.Link, "Main Menu", mainScreen));
            savedgame.disabledOptions.AddRange(new List<int> { 1, 2, 3 });

            startMenu.Screens.Add(mainScreen);
            startMenu.Screens.Add(floorsScreen);



            // Pause screen

            MenuScreen pauseScreen = new MenuScreen(520.0f, 180.0f, 50.0f);

            pauseScreen.Options.Add(new MenuOption(MenuOptionType.Command, "Resume", MenuCommand.Resume));
            pauseScreen.Options.Add(new MenuOption(MenuOptionType.Command, "Load", MenuCommand.Load));
            pauseScreen.Options.Add(new MenuOption(MenuOptionType.Command, "Main Menu", MenuCommand.LinkToMainMenu)); // Can link between engines...

            pauseMenu.Screens.Add(pauseScreen);

            SetCurrentMenu(startMenu);

        }


        private void animateMyMenu()
        {
            menuXFrame = 0;
            menuYFrame = 0;
            menuSpriteWidth = 500;
            menuSpriteHeight = 500;
            menuAnimateInterval = 20;
            menuMyGameTime = 0;
            menuAnimateTimer = 0;
            menuSourceRect = new Rectangle(menuXFrame * menuSpriteWidth, menuYFrame * menuSpriteHeight, menuSpriteWidth, menuSpriteHeight);
            menuOrigin = new Vector2(menuSourceRect.Width / 2, menuSourceRect.Height / 2);
            spriteBatch.Begin();
            for (int i = 0; i < 150; i++)
            {
                menuMyGameTime++;
                menuSourceRect = new Rectangle(menuXFrame * menuSpriteWidth, menuYFrame * menuSpriteHeight, menuSpriteWidth, menuSpriteHeight);
                if (!((menuXFrame == 5) && (menuYFrame == 1)))
                {
                    menuAnimateTimer += menuMyGameTime;

                    if (menuAnimateTimer > menuAnimateInterval)
                    {
                        menuXFrame++;

                        if (menuXFrame > 5 && menuYFrame == 0)
                        {
                            menuXFrame = 0;
                            menuYFrame = 1;
                        }
                        else if (menuXFrame > 5 && menuYFrame == 1)
                        {
                            menuXFrame = 5;
                            menuYFrame = 1;
                        }
                        myGameTime = 0;
                        animateTimer = 0;
                    }
                }

                spriteBatch.Draw(menuPanelAnimation,
                        new Rectangle(0, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height), menuSourceRect, Color.White);


            }
            spriteBatch.End();

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

                    /*
                    //oh god hacking for menu screen
                    if (menuAnimate)
                    {
                        menuXFrame = 0;
                        menuYFrame = 0;
                        menuSpriteWidth = 500;
                        menuSpriteHeight = 500;
                        menuAnimateInterval = 20;
                        menuMyGameTime = 0;
                        menuAnimateTimer = 0;
                        menuSourceRect = new Rectangle(menuXFrame * menuSpriteWidth, menuYFrame * menuSpriteHeight, menuSpriteWidth, menuSpriteHeight);
                        menuOrigin = new Vector2(menuSourceRect.Width / 2, menuSourceRect.Height / 2);
                        spriteBatch.Begin();
                        for (int i = 0; i < 150; i++)
                        {
                            Console.WriteLine("i got here");
                            menuMyGameTime++;
                            menuSourceRect = new Rectangle(menuXFrame * menuSpriteWidth, menuYFrame * menuSpriteHeight, menuSpriteWidth, menuSpriteHeight);
                            if (!((menuXFrame == 5) && (menuYFrame == 1)))
                            {
                                menuAnimateTimer += menuMyGameTime;

                                if (menuAnimateTimer > menuAnimateInterval)
                                {
                                    menuXFrame++;

                                    if (menuXFrame > 5 && menuYFrame == 0)
                                    {
                                        menuXFrame = 0;
                                        menuYFrame = 1;
                                    }
                                    else if (menuXFrame > 5 && menuYFrame == 1)
                                    {
                                        menuXFrame = 5;
                                        menuYFrame = 1;
                                    }                                   
                                    myGameTime = 0;
                                    animateTimer = 0;
                                }
                            }

                            spriteBatch.Draw(menuPanelAnimation,
                                    new Rectangle(0, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height), menuSourceRect, Color.White);
                           

                        }
                        spriteBatch.End();

                        menuAnimate = false;

                    }
                    
                    */
                    DrawSuccessOrFail();

                    spriteBatch.Begin();
                    spriteBatch.Draw(menuPanel,
                                new Rectangle(0, 0, this.Window.ClientBounds.Width, this.Window.ClientBounds.Height), Color.White);
                    currentMenu.Draw(spriteBatch);
                    spriteBatch.End();
                

                    break;

                case ProgramState.Playing:        

                    //currentWorld.Draw(spriteBatch, (float)this.Window.ClientBounds.Width, (float)this.Window.ClientBounds.Height);
                    currentWorld.Draw(graphics.GraphicsDevice, Matrix.Identity);
                    DrawSuccessOrFail();
                    break;
            }

            //instaSteelTextBox.Draw(spriteBatch, 255); //draws the textbox here, no transparency 
           // jumpTextBox.Draw(spriteBatch, 255);
            base.Draw(gameTime);
        }

        private void DrawSuccessOrFail()
        {

            spriteBatch.Begin();
            // Draw success or failure image
            if (currentWorld != null && currentWorld.Succeeded && !currentWorld.Failed)
            {
                animate = true;
                spriteBatch.Draw(victory, new Vector2((graphics.PreferredBackBufferWidth - sourceRect.Width) / 2,
                    (graphics.PreferredBackBufferHeight - sourceRect.Height) / 2), sourceRect, Color.White);
            }
            else if (currentWorld != null && currentWorld.Failed)
            {
                spriteBatch.Draw(failure, new Vector2((graphics.PreferredBackBufferWidth - failure.Width) / 2,
                    (graphics.PreferredBackBufferHeight - failure.Height) / 2), Color.White); ;
            }
            spriteBatch.End();
        }
    }
}
