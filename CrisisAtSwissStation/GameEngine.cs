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

        //textbox for setting insta-steel
        TextBox instaSteelTextBox;
        //textbox for setting Jump
        TextBox jumpTextBox;
       
    
        // TODO : pretty much nothing about drawing should eventually be in this class - it should be moved into ScrollingWorld.cs
        // current and previous mouse state (for drawing)
        MouseState ms, prevms;
        List<Vector2> dotPositions = new List<Vector2>();
        Texture2D paintTexture;
        Vector2 halfdotsize;
        float PAINTING_GRANULARITY = 1f; // how far apart points in a painting need to be for us to store them both

        public const int SCREEN_WIDTH = 1024; // the width of the screen
        public const int SCREEN_HEIGHT = 768; // the height of the scren

        // How many frames after winning/losing do we continue?

        int COUNTDOWN = 200;
        
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
        CASSWorld currentWorld;

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
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.ApplyChanges();

            //initializes our InstaSteel text box
            instaSteelTextBox = new TextBox(new Vector2(50, 160), 80, Content);//instantiates with vector for location, 80 is the width, Content is a content manager
            instaSteelTextBox.SetBgColor(Color.White);
            instaSteelTextBox.SetTextColor(Color.Black);

            //initializes our jumpTextBox
            jumpTextBox = new TextBox(new Vector2(50, 220), 80, Content);
            jumpTextBox.SetBgColor(Color.White);
            jumpTextBox.SetTextColor(Color.Black);

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

            // toggle mute if they press 'm' 
            if (ks.IsKeyDown(Keys.M))
                audioManager.Mute();

            // Move to next track if they press 'x'
            if (ks.IsKeyDown(Keys.X))
                audioManager.PlayNext();

            if (currentWorld != null && (currentWorld.Succeeded || currentWorld.Failed))
            {
                countdown--;
                audioManager.DecreaseMusicVolume(.005f);
                if (currentWorld.Failed)
                {
                    countdown--;
                }

                //Play the level complete SFX
                if (countdown == 180 && currentWorld.Succeeded)
                {
                    audioManager.Stop();
                    audioManager.Play(CrisisAtSwissStation.AudioManager.SFXSelection.LevelComplete);
                }

                if (countdown == 0)
                {
                   
                    reset = currentWorld.Failed;
                    next = !currentWorld.Failed && currentWorld.Succeeded;
                    audioManager.IncreaseMusicVolume(0.5f);
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

           

            //Call to update the Text Box
            instaSteelTextBox.Update(gameTime);
            jumpTextBox.Update(gameTime);

            //Need the text from the text box to interpret it later
            string temp = instaSteelTextBox.GetText();
           
            //check if enter pushed and a valid number entered. Update insta-steel accordingly 
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
            {
                currentWorld.Draw(graphics.GraphicsDevice, Matrix.Identity);
            }


            spriteBatch.Begin();


            //Draw IS label 
            spriteBatch.DrawString(spriteFont, "Insta-Steel :", new Vector2(50, 140), Color.Red);
            //Draw IS amount
            spriteBatch.DrawString(spriteFont, ((int)ScrollingWorld.numDrawLeft).ToString(), new Vector2(155, 140), Color.Yellow);

            //Draw Jump label 
            spriteBatch.DrawString(spriteFont, "Jump :", new Vector2(50, 200), Color.Red);
            //Draw Jump amount
            spriteBatch.DrawString(spriteFont, DudeObject.jumpImpulse.ToString(), new Vector2(115, 200), Color.Yellow);


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
            instaSteelTextBox.Draw(spriteBatch, 255); //draws the textbox here, no transparency 
            jumpTextBox.Draw(spriteBatch, 255);
            base.Draw(gameTime);
        }
    }
}
