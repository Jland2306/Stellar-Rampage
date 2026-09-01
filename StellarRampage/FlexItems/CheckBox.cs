using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.FlexItems
{

    /// <summary>
    /// Checkbox is a toggle, on or off. Works well in menus
    /// </summary>
    public class CheckBox : FlexItem
    {
        //Textures
        private Texture2D onTexture;
        private Texture2D offTexture;

        // is it on?
        private bool isChecked;

        //Check if mouse is hovering, allows user to check for switch
        private bool isHovering;

        private MouseState prevMouse;

        private Rectangle drawRect;

        public bool IsChecked
        {
            get { return isChecked; }
        }

        //Action to toggle
        public Action<bool> OnToggle;


        /// <summary>
        /// Create a new checkbox that can toggle methods on/off
        /// </summary>
        /// <param name="onTexture">Checkbox texture</param>
        /// <param name="offTexture">Empty check texture</param>
        /// <param name="initialValue">Start on or off?</param>
        public CheckBox(Texture2D onTexture, Texture2D offTexture, bool initialValue)
        {
            //Assign the fields
            this.onTexture = onTexture;
            this.offTexture = offTexture;
            this.isChecked = initialValue;

            // Preserve aspect ratio, scale to max of width or height
            int textureSize = Math.Min(rectangle.Width, rectangle.Height);
            int offsetX = rectangle.X + (rectangle.Width - textureSize) / 2;
            int offsetY = rectangle.Y + (rectangle.Height - textureSize) / 2;

            //Add the new rectangle to the checkbox
            rectangle = new Rectangle(offsetX, offsetY, textureSize, textureSize);
        }

        /// <summary>
        /// Check if player hits the toggle
        /// </summary>
        public override void Update()
        {
            MouseState mouseState = Mouse.GetState();

            //Check if mouse is over the button
            isHovering = drawRect.Contains(mouseState.Position);

            //User is hovering, and pressing down
            if (isHovering &&
                prevMouse.LeftButton == ButtonState.Pressed &&
                mouseState.LeftButton == ButtonState.Released)
            {
                //Switch button
                isChecked = !isChecked;
                 
                //Invoke method
                OnToggle?.Invoke(isChecked);
            }

            //Assign previous for next frame
            prevMouse = mouseState;
        }

        /// <summary>
        /// Draw the checkbox
        /// </summary>
        /// <param name="sb">spritebatch</param>
        public override void Draw(SpriteBatch sb)
        {
            //Draw the checkbox with either on/off
            if(onTexture != null && offTexture != null)
            {
                if (isChecked)
                {
                    sb.Draw(
                        onTexture,          //Texture
                        drawRect,           //Rectangle
                        Color.White);       //Tint
                }
                else
                {
                    sb.Draw(
                        offTexture,         //Texture
                        drawRect,           //Rectangle
                        Color.White);       //Tint
                }
            }

        }

        /// <summary>
        /// Size of the checkbox
        /// </summary>
        public override Rectangle Rectangle
        {
            set
            {
                //Assign the new Rectangle
                rectangle = value;

                // Preserve aspect ratio, scale to max of width or height
                int textureSize = Math.Min(rectangle.Width, rectangle.Height);
                int offsetX = rectangle.X + (rectangle.Width - textureSize) / 2;
                int offsetY = rectangle.Y + (rectangle.Height - textureSize) / 2;

                //add the new size the draw rectangle
                drawRect = new Rectangle(offsetX, offsetY, textureSize, textureSize);
            }
        }
    }
}
