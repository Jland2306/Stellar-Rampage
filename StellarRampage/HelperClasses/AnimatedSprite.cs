using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using System.Threading;

namespace StellarRampage.HelperClasses
{
    public  class AnimatedSprite
    {
        //---------------------------------------------------------------------
        //                        FIELDS & PROPERTIES
        //---------------------------------------------------------------------

        // Sprite sheet field
        private List<Texture2D> sheets;
        private int index = 0;

        // Frame fields
        private int frameWidth;
        private int frameHeight;
        private int frameCount;
        private int currentFrame;
        private float frameSpeed;
        private double timer;
        private Rectangle position;
        private float rotation;

        //Size
        private float scale;

        //Color
        private Color color;

        //Special Boss properties
        private bool isBoss;

        public bool IsBoss
        {
            set { isBoss = value; }
        }

        /// <summary>
        /// Get and set property for sprites position
        /// </summary>
        public Rectangle Position
        {
            get { return position; }
            set { position = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Color Color
        {
            get { return color; }
            set {  color = value; }
        }

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        //---------------------------------------------------------------------
        //                            CONSTRUCTOR
        //---------------------------------------------------------------------

        /// <summary>
        /// Initialize the frame fields
        /// </summary>
        /// <param name="texture">spritesheet</param>
        /// <param name="_frameWidth">width of each frame</param>
        /// <param name="_frameHeight">height of each frame</param>
        /// <param name="_frameCount">frame count</param>
        /// <param name="_frameSpeed">frame speed</param>
        public AnimatedSprite(
            Texture2D texture, int _frameWidth, int _frameHeight,
            int _frameCount, float _frameSpeed, Rectangle _position, float scale = 1)
        {
            //Create a new texture sheet
            sheets = new List<Texture2D>();
            sheets.Add(texture);
            frameWidth = _frameWidth;
            frameHeight = _frameHeight;
            frameCount = _frameCount;
            frameSpeed = _frameSpeed;
            position = _position;
            currentFrame = 0;
            timer = 0;
            this.scale = scale;
            color = Color.White;
        }

        //---------------------------------------------------------------------
        //                              METHODS   
        //---------------------------------------------------------------------

        /// <summary>
        /// Adds a new list of sheets to the animated sprite
        /// </summary>
        /// <param name="sheets">the list of sheets to add</param>
        public void AddSheets(List<Texture2D> newSheets)
        {
            //remove any assets
            sheets.Clear();
            //Add all the new assets at once
            sheets.AddRange(newSheets);
        }

        /// <summary>
        /// Calculates the frame count by using the width and spriteSheet width
        /// </summary>
        public void UpdateFrameCount()
        {
            //Divide the sprite width by the width of one sprite
            frameCount = sheets[index].Width / frameWidth;
        }

        /// <summary>
        /// Changes the sprite sheet to a selected index
        /// </summary>
        public void SetIndex(int index)
        {
            this.index = index;
            //get the new amount of frames
            UpdateFrameCount();
        }

        /// <summary>
        /// Update method for animation
        /// </summary>
        /// <param name="gameTime">gameTime</param>
        public bool Update(GameTime gameTime)
        {
            // Update timer
            timer += gameTime.ElapsedGameTime.TotalSeconds;

            // If statemnet to handle frame looping
            if (timer >= frameSpeed)
            {
                // Update frame
                currentFrame++;

                // If animation gets to last frame loop it
                if (currentFrame >= frameCount)
                {
                    currentFrame = 0;
                    timer = 0;
                    return true;
                }
                // reset timer
                timer = 0;
            }
            return false;
        }

        /// <summary>
        /// Draw method
        /// </summary>
        /// <param name="spriteBatch">spriteBatch</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Create source rectangle
            Rectangle sourceRec = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);

            // Draw animation
            spriteBatch.Draw(
                sheets[index],
                position,
                sourceRec,
                Color.White);

        }

        public void DrawRotated(SpriteBatch spriteBatch)
        {
            // Create source rectangle
            Rectangle sourceRec = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);

            // Draw animation
            spriteBatch.Draw(
                sheets[index],
                new Vector2(position.X, position.Y),
                sourceRec,
                color,
                rotation,
                new Vector2(frameWidth / 2 , frameHeight / 2),
                scale,
                SpriteEffects.None,
                0);

            //Draw on top of the extra sprite. this includes trails and shields
            if (index == 1 && !isBoss)
            {
                //Draw the single sprite
                spriteBatch.Draw(
                    sheets[0],
                    new Vector2(position.X, position.Y),
                    new Rectangle(0, 0, frameWidth, frameHeight),
                    Color.White,
                    rotation,
                    new Vector2(frameWidth / 2, frameHeight / 2),
                    scale,
                    SpriteEffects.None,
                    0);
            }
        }
    }
}
