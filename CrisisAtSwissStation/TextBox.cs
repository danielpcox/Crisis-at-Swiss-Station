using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace CrisisAtSwissStation
{
    //note: some of this code was modified from a preexisting Textbox class and edited to work with our game.
    class TextBox
    {
        
        Color textColor = Color.Black;//color of text in the box
        Color bgColor = Color.White;//bg color of the box
        SpriteFont fontText;
        Vector2 loc;
        Vector2 size;//width and height of the box
        Texture2D empty;//filler texture for text box
        KeyboardState ks, lastKeyboardState;
        bool hasFocus = true;//bool for writing in the box (if true, cursor clicked there and ready to type)
        bool cursorFlashing = true;//bool for creating the cursor flashing to show the user that the box may be typed in now
        float flashTimer = 0f;//timer for coordinating flashes
        char[] Text = "".ToCharArray();//char array used in storing key strokes
        int cursorLoc = 0;//location of cursor in box
       

        public enum KeyType { Number,  Enter };

       
        public TextBox(Vector2 location, int Width, ContentManager content)
        {
            this.loc = location;
            size = new Vector2(Width, 20); //the 20 is the height
            fontText = content.Load<SpriteFont>("PhysicsFont"); 
            empty = content.Load<Texture2D>("pixel"); // 1x1 image of a white pixel. Texture used in drawing the text box bg

            ks = Keyboard.GetState();
            lastKeyboardState = Keyboard.GetState();
        }

        public void SetTextColor(Color color) { textColor = color; } // Sets the color of the TextBox's text
        public void SetBgColor(Color color) { bgColor = color; } // Sets the background color of the TextBox

        public void GiveFocus() // Gives the TextBox the focus (allows user to type into it)
        {
            hasFocus = true;
            cursorLoc = Text.Length;
        }
        public void TakeFocus() { hasFocus = false; } // Takes focus away from this TextBox
    
        public string GetText() // Returns the text of the TextBox
        {
            string text = "";
            foreach (char c in Text)
                text += c;
            return text;
        }

        //updates the keytype, focus for the cursor, converts keystroke to string, handles number deletion and insertion
        public KeyType Update(GameTime gameTime)
        {
            lastKeyboardState = ks;
            ks = Keyboard.GetState();

            if (Mouse.GetState().RightButton == ButtonState.Pressed &&
                Mouse.GetState().X >= loc.X && Mouse.GetState().X <= loc.X + size.X &&
                Mouse.GetState().Y >= loc.Y && Mouse.GetState().Y <= loc.Y + size.Y)
                GiveFocus();//give focus if clicked in the box
            else TakeFocus();

            if (hasFocus)//cursor flash if box has focus
            {
                flashTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (flashTimer >= 0.5f)
                {
                    flashTimer -= 0.5f;
                    cursorFlashing = !cursorFlashing;
                }

                foreach (Keys key in ks.GetPressedKeys())
                {
                    if (ks.IsKeyDown(key) && lastKeyboardState.IsKeyUp(key))
                    {
                        bool shift = ks.IsKeyDown(Keys.LeftShift) || ks.IsKeyDown(Keys.RightShift);//needed to get numbers working for some reason

                        string keyString = ""; // key.ToString().ToLower();
                        if ((key.ToString().Length == 2 && key.ToString().StartsWith("D")) || (key.ToString().Length == 7 && key.ToString().StartsWith("NumPad")))
                         keyString = ("" + key.ToString().ToCharArray()[key.ToString().Length - 1]);//converting key code to a string, yeah its weird
                        if (key == Keys.Space)
                            keyString = " ";
                        if (key == Keys.D1 && shift)
                            keyString = "!";
                        if (key == Keys.D2 && shift)
                            keyString = "\"";
                        if (key == Keys.D3 && shift)
                            keyString = "£";
                        if (key == Keys.D4 && shift)
                            keyString = "$";
                        if (key == Keys.D4 && shift )
                            keyString = "€";
                        if (key == Keys.D5 && shift)
                            keyString = "%";
                        if (key == Keys.D6 && shift)
                            keyString = "^";
                        if (key == Keys.D7 && shift)
                            keyString = "&";
                        if (key == Keys.D8 && shift)
                            keyString = "*";
                        if (key == Keys.D9 && shift)
                            keyString = "(";
                        if (key == Keys.D0 && shift)
                            keyString = ")";

                        if (key == Keys.OemPeriod || key == Keys.Decimal)
                            keyString = ".";

                       

                        if (keyString.Length == 1 && fontText.MeasureString(GetText()).X < size.X - 20)//makes sure you don't exceed box capacity
                        {
                    
                            string preCursor = "", postCursor = "";
                            for (int i = 0; i < Text.Length; i++)
                                if (i < cursorLoc)
                                    preCursor += Text[i];
                                else
                                    postCursor += Text[i];
                            Text = string.Concat(preCursor, keyString, postCursor).ToCharArray();//adds new keystroke to old
                            cursorLoc++;//counter for cursor spot
                        }
                        else
                        {
                            // Enter Check
                            if (key == Keys.Enter)
                                return KeyType.Enter;
                        

                            else if (key == Keys.Back) // Backspace
                            {
                                string preCursor = "", postCursor = "";
                                for (int i = 0; i < Text.Length; i++)
                                    if (i < cursorLoc - 1)
                                        preCursor += Text[i];
                                    else if (i == cursorLoc - 1) { } // Skip deleted character
                                    else
                                        postCursor += Text[i];
                                Text = string.Concat(preCursor, postCursor).ToCharArray();
                                if (cursorLoc > 0)
                                    cursorLoc--;
                            }
                         
                        }
                    }
                }
            }
            return KeyType.Number;
        }

       
        private void DrawTextBox(SpriteBatch spriteBatch, byte alpha)//draws the textbox background, text, and cursor
        {
            spriteBatch.Draw(empty, new Rectangle((int)loc.X, (int)loc.Y, (int)size.X, (int)size.Y), new Color(bgColor.R, bgColor.G, bgColor.B, alpha));//draw background
            string text = "";
            foreach (char c in Text)
            
                    text += c;
          
            Vector2 stringSize = fontText.MeasureString(text);
            spriteBatch.DrawString(fontText, text, loc + new Vector2(10, size.Y / 2), new Color(textColor.R, textColor.G, textColor.B, alpha), 0, new Vector2(0, stringSize.Y / 2), 1, SpriteEffects.None, 0);//draw numbers
            if (cursorFlashing && hasFocus)
            {
                text = "";
                for (int i = 0; i < cursorLoc; i++)
                
                        text += Text[i];
                 
                stringSize = fontText.MeasureString(text);
                spriteBatch.Draw(empty, new Rectangle((int)(loc.X + 10 + stringSize.X), (int)loc.Y + 3, 1, 14), null, new Color(textColor.R, textColor.G, textColor.B, alpha));//draw cursor
            }
        }

        public void Draw(SpriteBatch spriteBatch, byte alpha)
        {
            try
            {
                spriteBatch.Begin();
                DrawTextBox(spriteBatch, alpha);
                spriteBatch.End();
            }
            catch
            {
                DrawTextBox(spriteBatch, alpha);
            }
        }

    }
}