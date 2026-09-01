using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StellarRampage.FlexItems;
using static System.Formats.Asn1.AsnWriter;

namespace StellarRampage.HelperClasses
{
    /// <summary>
    /// A textbox dynamically resizes text to fit with a rectangle
    /// </summary>
    public class TextBox : FlexItem
    {
        //Text to draw
        private string text = "";

        //What font should the text be
        private SpriteFont font;

        //Color of the text
        private Color color;

        //Text is either in the center, or left aligned. Right not implemented
        private bool centerText;

        /// <summary>
        /// Checks if there is any text on the button
        /// </summary>
        public bool HasText
        {
            get { return text != ""; }
        }

        public SpriteFont Font
        {
            set { font = value; }
        }

        /// <summary>
        /// Width of the textbox
        /// </summary>
        public int Width
        {
            get { return rectangle.Width; }
            set { rectangle.Width = value; }
        }

        /// <summary>
        /// Height of the textbox
        /// </summary>
        public int Height
        {
            get { return rectangle.Height; }
            set { rectangle.Height = value; }
        }

        /// <summary>
        /// Top left x cord
        /// </summary>
        public int X
        {
            get { return rectangle.X; }
            set { rectangle.X = value; }
        }

        /// <summary>
        /// Top left y cord
        /// </summary>
        public int Y
        {
            get { return rectangle.Y; }
            set { rectangle.Y = value; }
        }

        /// <summary>
        /// Color of the text
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>
        /// Finds out where the text needs to be placed to center it in a box
        /// </summary>
        public Vector2 TextCenter
        {
            get
            {
                //Center text
                if (centerText)
                {
                    //Takes the size of the box and text and gets the offset
                    //to find the x/y center
                    return new Vector2(
                           X + Width / 2 - TextWidth / 2,
                           Y + Height / 2 - TextHeight / 2);
                }
                //Align left
                else
                {
                    //Start at the farthest left, but middle aligned
                    return new Vector2(
                        X,
                        Y + Height / 2 - TextHeight / 2);
                }
            }
        }


        /// <summary>
        /// Gets the lowest scale needed to fit within a text box
        /// </summary>
        public float TextScale
        {
            get
            {
                //Checks whether scaling on the x, or y is smaller.
                //Return the smallest one, that way both dimensions fit in the box
                return MathF.Min(
                rectangle.Width / TextSize.X,
                rectangle.Height / TextSize.Y) * 0.8f;
            }
        }

        /// <summary>
        /// Lowest scale for text that is being wrapped
        /// </summary>
        private float WrappedTextScale
        {
            get
            {
                //Wrap the text
                List<string> lines = WrapText(1);

                //The total height of the box
                float lineHeight = lines.Count * font.LineSpacing;

                //How much to scale
                float verticalScale = rectangle.Height / lineHeight;

                //Dont overflow
                return MathF.Min(verticalScale, 1f) * 0.9f; 
            }
        }

        /// <summary>
        /// Gets the size of the current text
        /// </summary>
        public Vector2 TextSize
        {
            get { return font.MeasureString(text); }
        }

        /// <summary>
        /// How wide is the text after scaling
        /// </summary>
        public float TextWidth
        {
            get { return TextSize.X * TextScale; }
        }

        /// <summary>
        /// How tall is the text after scaling
        /// </summary>
        public float TextHeight
        {
            get { return TextSize.Y * TextScale; }
        }

        /// <summary>
        /// Change the text for the button
        /// </summary>
        public string Text
        {
            //Set new text
            set { text = value; }
        }

        /// <summary>
        /// Create a new text box
        /// </summary>
        /// <param name="font">font to draw</param>
        /// <param name="textBox">size of the box</param>
        /// <param name="color">color of text</param>
        /// <param name="centerText">should text be centered?</param>
        public TextBox(SpriteFont font, Rectangle textBox, Color color, bool centerText = true)
        {
            //Assign fields
            this.font = font;
            rectangle = textBox;
            this.color = color;
            this.centerText = centerText;
        }

        /// <summary>
        /// Draw the text
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void Draw(SpriteBatch _spriteBatch)
        {
            //Only try and draw if text exists
            if (HasText)
            {
                //Draw text, position is moved based on if its centered or not
                _spriteBatch.DrawString(
                    font,                       //Font
                    text,                       //Text
                    TextCenter,                 //Position
                    color,                      //Color
                    0,                          //Rotation
                    Vector2.Zero,               //Origin
                    TextScale,                  //Scale
                    SpriteEffects.None,         //Effect
                    0);                         //Depth


            }
        }

        /// <summary>
        /// Draws the text using a wrapping method
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public List<string> DrawWrapped(SpriteBatch _spriteBatch) 
        {
            //dont draw if there is not text
            if (!HasText) { return null; }

            float scale = WrappedTextScale;

            //Get the list of words in proper format
            List<string> lines = WrapText(scale);

            //The amount of space between each line
            float lineHeight = font.LineSpacing * scale;

            //how tall it is
            float totalHeight = lines.Count * lineHeight;

            float startY = rectangle.Y;

            //Draw line by line
            foreach (string line in lines)
            {
                _spriteBatch.DrawString(
                    font,
                    line,
                    new Vector2(rectangle.X, startY),
                    color,
                    0,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0);

                startY += lineHeight;
            }

            return lines;
        }

        /// <summary>
        /// Prevents wrap text from adjusting on a button hover
        /// </summary>
        /// <param name="_spriteBatch"></param>
        /// <param name="lines"></param>
        public void DrawWrapLarger(SpriteBatch _spriteBatch, List<string> lines)
        {
            //dont draw if there is not text
            if (lines == null) { return; }

            float startY = rectangle.Y;
            float scale = WrappedTextScale;
            //The amount of space between each line
            float lineHeight = font.LineSpacing * scale;

            //Draw line by line
            foreach (string line in lines)
            {
                _spriteBatch.DrawString(
                    font,
                    line,
                    new Vector2(rectangle.X, startY),
                    color,
                    0,
                    Vector2.Zero,
                    scale,
                    SpriteEffects.None,
                    0);

                startY += lineHeight;
            }
        }
        /// <summary>
        /// Draws the text in the text box, rotated based on input
        /// </summary>
        /// <param name="_spriteBatch"></param>
        /// <param name="rotation">Amount to rotate</param>
        public void DrawRotated(SpriteBatch _spriteBatch, float rotation) {

            //Only try and draw if text exists
            if (HasText)
            {
                //Draw button text
                _spriteBatch.DrawString(
                    font,                       //Font
                    text,                       //Text
                    TextCenter,                 //Position
                    color,                      //Color
                    rotation,                   //Rotation
                    Vector2.Zero,               //Origin
                    TextScale,                  //Scale
                    SpriteEffects.None,         //Effect
                    0);                         //Depth
            }
        }


        /// <summary>
        /// Makes text take multiple lines
        /// </summary>
        /// <returns>An array with the proper sentence format</returns>
        private List<string> WrapText(float scale)
        {

            List<string> lines = new List<string>();

            //Split all the words into an array
            string[] words = text.Split(' ');

            //Go through each line
            string currentLine = "";

            //scales down before text is scaled up
            float maxLineWidth = rectangle.Width / scale;

            //Check each word to see if it fits
            foreach (string word in words)
            {
                //Is there a word, if so, add it
                string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;

                //check if the word fits into the line
                if (font.MeasureString(testLine).X <= maxLineWidth)
                {
                    //Fits
                    currentLine = testLine;
                }
                else
                {
                    //Does not fit, new line
                    lines.Add(currentLine);
                    currentLine = word;
                }
            }
            //add anything left at the end
            if (!string.IsNullOrEmpty(currentLine))
            {
                lines.Add(currentLine);
            }


            return lines;
        }
    }
}
