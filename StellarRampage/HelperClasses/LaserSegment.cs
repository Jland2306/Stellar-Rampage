using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.HelperClasses
{
    public class LaserSegment 
    {
        /// <summary>
        /// The animated laser sprite
        /// </summary>
        private AnimatedSprite laserSprite;

        /// <summary>
        /// Returns the location of the segment
        /// </summary>
        public Vector2 Position
        {
            get { return new Vector2(
                laserSprite.Position.X,
                laserSprite.Position.Y); 
            }
 
        }
        private float rotation;

        public LaserSegment(Texture2D spriteSheet, int x, int y, float scale, float rotation)
        {
            laserSprite = new AnimatedSprite(spriteSheet, 18, 38, 4, 0.1f, new Rectangle(x, y, 0, 0), scale: scale);
            this.rotation = rotation;
            laserSprite.Rotation = rotation;
        }

        public void Draw(SpriteBatch _spriteBatch, bool debugOn, Color color)
        {
            laserSprite.Color = color;
            laserSprite.DrawRotated(_spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            laserSprite.Update(gameTime);
        }
    }
}
