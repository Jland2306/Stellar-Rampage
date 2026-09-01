using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StellarRampage.HelperClasses;

namespace StellarRampage.FlexItems
{
    //A panel represents a container that will be used to hold
    //multiple other items such as text, rects, buttons, etc.
    public class Panel : FlexItem
    {
        //Visual background
        private Texture2D panel;
        //Text
        private TextBox text;
        //Allows panel to contain other panels
        private FlexBox flexBox;
        //Display text?
        private bool hasTitle;

        /// <summary>
        /// Returns the textbox in the panel
        /// </summary>
        public TextBox TextBox
        {
            get { return text; }
        }

        /// <summary>
        /// A reference to the flexbox inside
        /// </summary>
        public FlexBox FlexBox
        {
            get { return flexBox; }
        }

        /// <summary>
        /// Create a panel to hold info
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="rect"></param>
        /// <param name="text"></param>
        /// <param name="isVerticle"></param>
        public Panel(Texture2D panel, Rectangle rect, TextBox text, bool isVerticle = false)
        {
            //Assign fields
            this.panel = panel;
            rectangle = rect;
            this.text = text;
            hasTitle = text != null;

            //Copy the size of the panel
            Rectangle contentRect = rect;

            //If theres a title, leave room for it, otherwise fill the whole rectangle
            if (hasTitle)
            {
                int titleHeight = rect.Height / 6;
                contentRect = new Rectangle(
                    rect.X,
                    rect.Y + titleHeight,
                    rect.Width,
                    rect.Height - titleHeight);
            }

            //Create a new flexbox to hold items, if its just one item, this will not be needed
            flexBox = new FlexBox(contentRect, 10, isVerticle);
        }

        /// <summary>
        /// Create a panel using a copy constructor
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="isVerticle"></param>
        public Panel(Panel panel, bool isVerticle = false)
        {
            this.panel = panel.panel;
            text = panel.text;
            rectangle = Rectangle;
            flexBox = new FlexBox(rectangle, 10, isVerticle);
        }
        /// <summary>
        /// Draw the panel to screen
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public override void Draw(SpriteBatch _spriteBatch)
        {
            //Draw background
            if (panel != null)
            {
                _spriteBatch.Draw(panel, rectangle, Color.White);
            }

            //Check if has text before drawing
            if (hasTitle && text != null)
                text.Draw(_spriteBatch);

            //Draw all other boxes
            flexBox.Draw(_spriteBatch);
        }

        public override void Update()
        {
            base.Update();
            flexBox.Update();
        }
        /// <summary>
        /// This keeps the text centered in the box
        /// </summary>
        public void AlignTextCenter()
        {
            //Take the whole space
            text.Rectangle = rectangle;
        }

        /// <summary>
        /// Each panel will have text stating what it is, this align the text at the top
        /// and keeps it centered
        /// </summary>
        public void AlignTextHeader()
        {
            //There is no text, dont carry on
            if (text == null) return;

            // Position title at the top of the panel with a gap
            int padding = 8;
            int titleHeight = rectangle.Height / 6; // Shrink title to 1/6 of panel height

            //Add the next rectangle to the textbox. It will now be positoned,
            //at the top, more like a header
            text.Rectangle = new Rectangle(
                rectangle.X + padding,
                rectangle.Y + padding,
                rectangle.Width - 2 * padding,
                titleHeight);
        }
    }
}
