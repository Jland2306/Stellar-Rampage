using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StellarRampage.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellarRampage.HelperClasses;

namespace StellarRampage.Managers
{

    /// <summary>
    /// Controls all active subtitles
    /// </summary>
    public class SubtitleManager
    {
        private List<Subtitle> subtitles = new();
        private SpriteFont font;
        private Vector2 screenSize;

        // Show text?
        public bool Enabled = true;

        //Colors
        public Color textColor = Color.White;
        public Color backgroundColor = Color.Black * 0.5f;
        public Color shadowColor = Color.Black * 0.8f;

        //Background padding
        public int padding = 12;
        public int marginFromBottom = 40;

        //size of text
        public float fontScale = 1.5f;

        //Add customization
        public bool drawBackground = true;
        public bool drawShadow = true;

        //Used to form rectangle
        private Texture2D pixel;

        public SubtitleManager(SpriteFont font, Vector2 screenSize, GraphicsDevice graphicsDevice)
        {
            this.font = font;
            this.screenSize = screenSize;

            // Create 1x1 white pixel for backgrounds
            pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        /// <summary>
        /// Display subtitle
        /// </summary>
        /// <param name="text">text to show</param>
        /// <param name="duration">how long it stays</param>
        public void Show(string text, float duration = 1.5f)
        {
            //Do not continue
            if (!Enabled) return;
            //Create a new subtitle
            subtitles.Add(new Subtitle(text,0,duration));
        }

        /// <summary>
        /// Check if the subtitles time has expired
        /// </summary>
        /// <param name="gameTime">time since last frame</param>
        public void Update(GameTime gameTime)
        {
            //time elapsed
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //iterate backwards so item can be removed in list
            for (int i = subtitles.Count - 1; i >= 0; i--)
            {
                //Increase time on that title
                subtitles[i].timer += time;

                //check if its expiered
                if (subtitles[i].timer >= subtitles[i].duration)
                {
                    //remove
                    subtitles.RemoveAt(i);
                }
            }

        }

        /// <summary>
        /// Draw the subtitles
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void Draw(SpriteBatch _spriteBatch)
        {
            //There is nothing to draw
            if (!Enabled || subtitles.Count == 0) return;

            //Get the last item added
            Subtitle sub = subtitles[^1];

            //alpha is how visible the text is, calculated using time completed
            float alpha = 1f - (sub.timer / sub.duration);

            //Scale up text
            Vector2 textSize = font.MeasureString(sub.text) * fontScale;

            //Position in the bottom middle, adding in a margin
            Vector2 position = new Vector2(
                (screenSize.X - textSize.X) / 2f,
                screenSize.Y - textSize.Y - marginFromBottom
            );

            //Size of the background panel
            Rectangle backgroundRect = new Rectangle(
                (int)(position.X - padding),
                (int)(position.Y - padding),
                (int)(textSize.X + padding * 2),
                (int)(textSize.Y + padding * 2)
            );

            // if there is a background, draw it
            if (drawBackground)
            {
                _spriteBatch.Draw(
                    pixel,                      //texture
                    backgroundRect,             //Size/Position
                    backgroundColor * alpha);   //Color, fade with time
            }

            // if there is a shadow, draw it
            if (drawShadow)
            {
                //Position shadow slightly away from text
                Vector2 shadowOffset = new Vector2(2, 2);  

                _spriteBatch.DrawString(
                    font,                       //font
                    sub.text,                   //text
                    position + shadowOffset,    //Position
                    shadowColor * alpha,        //color/alpha
                    0f,                         //rotation
                    Vector2.Zero,               //origin
                    fontScale,                  //size
                    SpriteEffects.None,         //spriteEffect
                    0f);                        //Layer
            }

            // Draw main text
            _spriteBatch.DrawString(
                font,                           //font
                sub.text,                       //text
                position,                       //position
                textColor * alpha,              //color/alpha
                0f,                             //rotation
                Vector2.Zero,                   //origin
                fontScale,                      //size
                SpriteEffects.None,             //spriteEffects
                0f);                            //Layer
        }

    }
}

