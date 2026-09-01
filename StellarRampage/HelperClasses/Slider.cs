using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace StellarRampage.HelperClasses
{
    /// <summary>
    /// A slider is a bar that moves according to a percent. Can be used 
    /// for ui such as boost or health
    /// </summary>
    public class Slider
    {
        //What the inside texture looks like.
        //The Ui that holds it needs to be drawn separate
        private Texture2D fillTexture;
        //location
        private Vector2 position;
        //Height should be the same as UI holder
        private int height;
        //How far out it can go
        private int maxWidth;
        //the percent the bar is at
        private float percent;
        //scale to draw the bar
        private float scale;

        /// <summary>
        /// Create a new slider to be used for UI
        /// </summary>
        /// <param name="fillTexture">the sprite texture</param>
        /// <param name="position">location</param>
        /// <param name="maxWidth">how large it is at 100%</param>
        /// <param name="height">height of the bar</param>
        /// <param name="scale">size to scale at</param>
        public Slider(Texture2D fillTexture, Vector2 position, int maxWidth, int height, float scale)
        {
            //Assign the fields in the class
            this.fillTexture = fillTexture;
            this.position = position;
            this.maxWidth = maxWidth;
            this.height = height;
            this.scale = scale;
        }

        /// <summary>
        /// Update the percent of the slider bar
        /// </summary>
        /// <param name="value">percent complete</param>
        public void SetPercent(float value)
        {
            //Clamp percent to stay between 0 and 1, otherwise the draw call
            //will be messed up
            percent = MathHelper.Clamp(value, 0f, 1f);
        }

        /// <summary>
        /// Draw the slider based on percent complete
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //figures out the amount of the bar to draw.
            //This is the percent complete times the total size
            int drawWidth = (int)(maxWidth * percent);

            //Do not draw the bar if its non existent
            if (drawWidth > 0)
            {
                spriteBatch.Draw(
                    fillTexture,                    //Texture
                    new Rectangle(                  //Position Rect
                        (int)position.X,            //X
                        (int)position.Y,            //Y
                        (int)(drawWidth * scale),   //Width
                        (int)(height * scale)),     //Height
                    new Rectangle(                  //Source Rect
                        0,                          //Top left
                        0,                          //Top left
                        drawWidth                   //Width
                        , height),                  //Height
                    Color.White);                   //Tint
            }
        }
    }
}
