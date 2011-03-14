using System;
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
    /// This is the main type for your game
    /// </summary>
    public class GameEngine : Microsoft.Xna.Framework.Game
    {

        // TODO : pretty much nothing about drawing should eventually be in this class - it should be moved into ScrollingWorld.cs
        // current and previous mouse state (for drawing)
        MouseState ms, prevms;
        List<Vector2> dotPositions = new List<Vector2>();
        Texture2D paintTexture;
        Vector2 halfdotsize;
        float PAINTING_GRANULARITY = 1f; // how far apart points in a painting need to be for us to store them both

        public const int GAME_WINDOW_WIDTH = 800; // how much of the game you can see at one time
        public const int GAME_WINDOW_HEIGHT = 600; // how much of the game you can see at one time

        // How many frames after winning/losing do we continue?
        int COUNTDOWN = 60;

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
        int countdown;

        // Current world instance
        DemoWorld currentWorld;

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
            graphics.PreferredBackBufferWidth = GAME_WINDOW_WIDTH;
            graphics.PreferredBackBufferHeight = GAME_WINDOW_HEIGHT;
            graphics.ApplyChanges();
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

            // TODO : change this to something more appropriate
            //painting texture
            paintTexture = Content.Load<Texture2D>("paint");
            halfdotsize = new Vector2(paintTexture.Width / 2, paintTexture.Height / 2);


            // Load world assets
            ScrollingWorld.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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

            // TODO: XBox controls
            KeyboardState ks = Keyboard.GetState();
            bool next = ks.IsKeyDown(Keys.N) && lastKeyboardState.IsKeyUp(Keys.N);
            bool prev = ks.IsKeyDown(Keys.P) && lastKeyboardState.IsKeyUp(Keys.P);
            bool reset = ks.IsKeyDown(Keys.R) && lastKeyboardState.IsKeyUp(Keys.R);
            lastKeyboardState = ks;
            
            // exit when they press escape
            if (ks.IsKeyDown(Keys.Escape))
                this.Exit();

            if (currentWorld != null && (currentWorld.Succeeded || currentWorld.Failed))
            {
                countdown--;
                if (countdown == 0)
                {
                    reset = currentWorld.Failed;
                    next = !currentWorld.Failed && currentWorld.Succeeded;
                }
            }
            
            // Current world is invalid for some reason - construct a new one!
            if (next || prev || reset || currentWorld == null)
            {
                // Uses C# reflection to construct a new world with minimal code
                //currentWorld = worldTypes[currentType].GetConstructor(Type.EmptyTypes).Invoke(null) as DemoWorld;
                currentWorld = new ScrollingWorld();
                countdown = 0;
            }

            currentWorld.Simulate((float)gameTime.ElapsedGameTime.TotalSeconds);

            // Just won or lost - initiate countdown
            if ((currentWorld.Failed || currentWorld.Succeeded) && countdown == 0)
                countdown = COUNTDOWN;

            
            base.Update(gameTime);

            prevms = ms;
        }
        
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (currentWorld != null)
                currentWorld.Draw(new Vector2(0,0));

            spriteBatch.Begin();

            if (currentWorld.Succeeded && !currentWorld.Failed)
            {
                //Currently plays the victory trumpets... a lot.
                //audioManager.Play(AudioManager.SFXSelection.VictoryTrumpets);
                spriteBatch.Draw(victory, new Vector2((graphics.PreferredBackBufferWidth - victory.Width) / 2,
                    (graphics.PreferredBackBufferHeight - victory.Height) / 2), Color.White);
            }
            else if (currentWorld.Failed)
            {
                spriteBatch.Draw(failure, new Vector2((graphics.PreferredBackBufferWidth - failure.Width) / 2,
                    (graphics.PreferredBackBufferHeight - failure.Height) / 2), Color.White); ;
            }

            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
