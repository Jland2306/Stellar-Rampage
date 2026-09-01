
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StellarRampage.HelperClasses;
using StellarRampage.Managers;

namespace StellarRampage.FlexItems
{
    /// <summary>
    /// Creates a button that highlights and/or scales up on hover
    /// </summary>
    public class Button : FlexItem
    {
        //The normal button texture
        private Texture2D buttonTexture;

        //Texture when mouse is hovering
        private Texture2D hoveringTexture;

        //The text on the button, not needed
        private TextBox textBox;

        //size the textbox is normally
        private Rectangle textBoxStart;

        //size when user is hovering
        private Rectangle textBoxEnd;

        //How much to scale up on hover
        protected float scale = 1.08f;

        //tint of the button
        private Color buttonColor = Color.White;

        //Certain button textures do not support text to be dead center.
        //For our current texture, smallTextBox will align the text to be slightly higher
        //and smaller, that way it fits right
        private bool smallTextBox;

        //font to use
        private SpriteFont font;

        //This bool allows the mouse bounds to be checked in the update,
        //and then use the draw method based on this state
        protected bool isHovering;

        //Check if this button was also hovering last frame.
        //this allows game1 to determine if a button is being hovered which
        //will allow certain actions to be disable
        protected bool wasHovering;

        public bool IsHovering
        {
            get { return isHovering; }
        }
        public int Width
        {
            get { return rectangle.Width; }
            set { rectangle.Width = value; }
        }

        public int Height
        {
            get { return rectangle.Height; }
            set { rectangle.Height = value; }
        }
        public int X
        {
            get { return rectangle.X; }
            set { rectangle.X = value; }
        }

        public int Y
        {
            get { return rectangle.Y; }
            set { rectangle.Y = value; }
        }

        public float ScaleMult
        {
            get { return ScaleMult; }
            set { ScaleMult = value; }
        }
        /// <summary>
        /// Returns the textbox in the button
        /// </summary>
        public TextBox TextBox
        {
            get { return textBox; }
        }

        /// <summary>
        /// Allows the change of button color
        /// </summary>
        public Color ButtonColor
        {
            get { return buttonColor; }
            set { buttonColor = value; }
        }

        /// <summary>
        /// Creates a highlightable button
        /// </summary>
        /// <param name="buttonTexture">The default texture</param>
        /// <param name="hoveringTexture">Texture when button is being hovered over</param>
        /// <param name="rectangle">The size and position of the button</param>
        public Button(Texture2D buttonTexture, Texture2D hoveringTexture, Rectangle rectangle, SpriteFont font, bool smallTextBox = false)
        {
            this.buttonTexture = buttonTexture;
            this.hoveringTexture = hoveringTexture;
            this.rectangle = rectangle;
            this.smallTextBox = smallTextBox;
            this.font = font;
            //How large the text is in the box
            if (smallTextBox)
            {
                textBoxStart = new Rectangle(
                   X + 50,
                   Y + 1,
                   Width - 100,
                   Height - 25);
            }
            else
            {
                textBoxStart = new Rectangle(
                     X + 5,
                     Y + 5,
                     Width - 10,
                     Height - 5);
            }

            //Create a textbox that will position it firmly within the button
            textBox = new TextBox(
                font,
                textBoxStart,
                Color.White);

            //Text box end is a scaled up version of the text rectangle.
            //Text will always be centered, and will scale to meet the button size.
            textBoxEnd = new Rectangle(
                        textBox.X - (int)(textBox.Width * scale - textBox.Width) / 2,
                        textBox.Y - (int)(textBox.Height * scale - textBox.Height) / 2,
                        (int)(textBox.Width * scale),
                        (int)(textBox.Height * scale));
        }

        public void UpdateTextRect(Rectangle rect)
        {
            if (smallTextBox)
            {
                textBoxStart = new Rectangle(
                   X + 50,
                   Y + 1,
                   Width - 100,
                   Height - 25);
            }
            else
            {
                textBoxStart = new Rectangle(
                     X + 5,
                     Y + 5,
                     Width - 10,
                     Height - 5);
            }

            //Create a textbox that will position it firmly within the button
            textBox.Rectangle = textBoxStart;

            textBoxEnd = new Rectangle(
                        textBox.X - (int)(textBox.Width * scale - textBox.Width) / 2,
                        textBox.Y - (int)(textBox.Height * scale - textBox.Height) / 2,
                        (int)(textBox.Width * scale),
                        (int)(textBox.Height * scale));

            textBox.Rectangle = textBoxStart;
        }

        /// <summary>
        /// Check if button is being hovered over
        /// </summary>
        public override void Update()
        {
            MouseState mouseState = Mouse.GetState();

            //Checks if the mouse is within bounds of the button.
            //Changes the hovering button to true if so
            if (mouseState.X > X &&
                mouseState.Y > Y &&
                mouseState.X < X + Width &&
                mouseState.Y < Y + Height)
            {
                isHovering = true;
                //Scale up text
                textBox.Rectangle = textBoxEnd;

                Game1.HoveringButton = true;
            }
            else
            {
                isHovering = false;
                //Scale text back down
                textBox.Rectangle = textBoxStart;
                //If a button was being hovered, turn the bool to false.
                //If the wasHovering did not exist, then each button would 
                //toggle this off each frame
                if (wasHovering)
                {
                    Game1.HoveringButton = false;
                }
            }

            //Just started hovering
            if(!wasHovering && isHovering)
            {
                SoundManager.PlaySound("Crystal7", volume: 1f);
            }

            wasHovering = isHovering;
        }


        /// <summary>
        /// Draw the button and any optional text
        /// </summary>
        /// <param name="sb">working spritebatch</param>
        public override void Draw(SpriteBatch sb)
        {
            //Change text color based on if its hovering
            Color textColor;

            //Just draw text
            if (buttonTexture == null || hoveringTexture == null)
            {
                //Draw the text if it exists
                if (textBox.HasText)
                {
                    textBox.Draw(sb);
                }
            }
            else
            {
                //users mouse is on button
                if (isHovering)
                {
                    //Scales the button up when being hovered
                    sb.Draw
                    (hoveringTexture, //Texture
                    new Rectangle(
                        X - (int)(Width * scale - Width) / 2,
                        Y - (int)(Height * scale - Height) / 2,
                        (int)(Width * scale),
                        (int)(scale * Height)),
                    buttonColor);
                    textColor = Color.Cyan;
                }

                else
                {
                    //Draws a non-highlighted button
                    sb.Draw
                    (buttonTexture,   //Texture
                    rectangle,        //Size
                    buttonColor);     //Tint

                    textColor = Color.White;
                }

                //Draw the text if it exists
                if (textBox.HasText)
                {
                    textBox.Draw(sb);
                }
            }


        }

    }

}
