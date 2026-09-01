using System;
using System.Collections.Generic;

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StellarRampage.FlexItems
{

    /// <summary>
    /// Flexbox represents an item container. Each item will dynamically change to 
    /// fit firmly in the container. Flexbox should only be used for singular rows/columns
    /// </summary>
    public class FlexBox
    {
        private Rectangle boxRect;
        private List<FlexItem> items;
        private bool isVertical;
        private int gap;


        //Returns the item given an index
        public FlexItem this[int index]
        {
            get { return items[index]; }
        }

        /// <summary>
        /// Creates a flexbox that will dynamically change the size of every element
        /// </summary>
        /// <param name="boxRect">Size of the container</param>
        /// <param name="gap">gap between items</param>
        /// <param name="isVertical">which direction is it going</param>
        public FlexBox(Rectangle boxRect, int gap, bool isVertical = false)
        {
            this.boxRect = boxRect;
            this.isVertical = isVertical;
            this.gap = gap;
            items = new List<FlexItem>();
        }

        /// <summary>
        /// Adds a new item to the container
        /// </summary>
        /// <param name="item">the item to add</param>
        public void Add(FlexItem item)
        {
            items.Add(item);
            Resize();
        }

        /// <summary>
        /// Resizes all items
        /// </summary>
        public void Resize()
        {
            //Only runs if there are items in the flexbox
            if (items.Count != 0)
            {
                int width;
                int height;
                int x;
                int y;

                //If vertical, the width will be the entire size of the box and the height is divided up
                //Vise versa for horizontal;
                if (isVertical)
                {
                    width = boxRect.Width;

                    //Allocates the whole height removing any space dedicated for gaps.
                    //The rest is divided by the number of items to find how tall each item will be
                    height = (boxRect.Height - (gap * items.Count - 1))
                            / items.Count;
                }
                else
                {
                    //Allocates the whole Width removing any space dedicated for gaps.
                    //The rest is divided by the number of items to find how wide each item will be
                    width = (boxRect.Width - (gap * items.Count - 1)) / items.Count;
                    height = boxRect.Height;
                }

                //Change each item in the list
                for (int i = 0; i < items.Count; i++)
                {
                    if (isVertical)
                    {
                        //X will be the same for each
                        x = boxRect.X;
                        //Y moves down the number of items before it. 
                        //Each time adding a gap between
                        y = boxRect.Y + height * i + gap * i;
                    }
                    else
                    {
                        //X moves over the number of items before it. 
                        //Each time adding a gap between
                        x = boxRect.X + width * i + gap * i;
                        //Y will be the same for each
                        y = boxRect.Y;
                    }

                    //Update the rectangle in the box
                    Rectangle itemRect = new Rectangle(x, y, width, height);
                    items[i].Rectangle = itemRect;
                    if (items[i] is Button b)
                    {
                        b.TextBox.Rectangle = itemRect;
                        b.UpdateTextRect(itemRect);
                    }
                    else if (items[i] is Panel p)
                    {
                        p.TextBox.Rectangle = itemRect;
                        p.AlignTextCenter(); // aligns text to panel size
                    }
                }

            }
        }

        /// <summary>
        /// Update all flexitems, some may need separate functions,
        /// so they are currently split
        /// </summary>
        public void Update()
        {
            foreach (FlexItem i in items)
            {
                if (i is Button)
                {
                    i.Update();
                }
                else if(i is PanelButton)
                {
                    i.Update();
                }
                else if (i is SliderPanel)
                {
                    i.Update();
                }
            }
        }

        /// <summary>
        /// Draw all flexitems
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void Draw(SpriteBatch _spriteBatch)
        {
            //Iterate through all flex items
            foreach (FlexItem i in items)
            {
                //Extra checks on the item, to allow for changes in future
                if (i is Button || i is Panel || i is CheckBox || i is PanelButton)
                {
                    i.Draw(_spriteBatch);
                }


            }
        }


    }
}
