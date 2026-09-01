using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StellarRampage.FlexItems
{
    /// <summary>
    /// A flex item refers to an item that can be dynamically changed when put inside
    /// of a flexbox. The flexbox keeps items sized appropriately while keeping visible on 
    /// screen
    /// </summary>
    public class FlexItem
    {
        //The size the item takes up
        protected Rectangle rectangle;

        //Flexbox needs to change the rectangle. Must be public
        public virtual Rectangle Rectangle
        {
            get { return rectangle; }
            set { rectangle = value; }
        }

        //Allows iteration of draw methods
        public virtual void Draw(SpriteBatch _spriteBatch)
        {

        }

        //Allows iteration of update methods
        public virtual void Update()
        {

        }
    }

}
