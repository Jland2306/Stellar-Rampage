using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StellarRampage.HelperClasses;
using StellarRampage.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.Particles
{

    /// <summary>
    /// Create a shockwave to show enemies exploding
    /// </summary>
    internal class Shockwave
    {
        //The circle 
        private Vector2 center;
        private float maxRadius;
        private float currRadius;

        //Visuals
        private float speed;
        private Texture2D ring;


        /// <summary>
        /// Should the ring be removed
        /// </summary>
        public bool IsComplete
        {
            get { return currRadius >= maxRadius; }
        }

        /// <summary>
        /// Create a shockwave with circle properties
        /// </summary>
        /// <param name="center">Where to start</param>
        /// <param name="maxRadius">the max size</param>
        /// <param name="speed">How fast to grow</param>
        /// <param name="ring">The shockwave texture</param>
        public Shockwave(Vector2 center, float maxRadius, float speed, Texture2D ring)
        {
            //Assign the fields
            this.center = center;
            this.maxRadius = maxRadius;
            this.speed = speed;
            this.ring = ring;

            //Start at nothing
            currRadius = 0f;
        }

        /// <summary>
        /// Update percent complete
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            //Increase the radius by the speed. Lock it to frame rate
            currRadius += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Kill any enemies
            EnemyManager.Instance.ShockwaveCollisionCheck(center, currRadius);
        }


        /// <summary>
        /// Draw the shockwave
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            //Create a new scaled up rectangle
            Rectangle rect = new Rectangle(
                (int)(center.X - currRadius),       //Offset to center
                (int)(center.Y - currRadius),       //Offset to center
                (int)currRadius * 2,                //Diameter
                (int)currRadius * 2);               //Diameter

            //Fade out the shock. At the very end, it should be 0. or 0 opacity
            float percentDone = (1 - (currRadius / maxRadius));
            //Draw the shock
            spriteBatch.Draw(
                ring,                       //Sprite
                rect,                       //Rectangle
                null,                       //Source
                Color.White * percentDone,  //Color
                0f,                         //Rotation
                Vector2.Zero,               //Origin
                SpriteEffects.None,         //SpriteEffects
                0f                          //Layer
            );
        }
    }
}
