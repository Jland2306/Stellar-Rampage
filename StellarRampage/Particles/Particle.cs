using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StellarRampage.Particles
{

    /// <summary>
    /// Particles can be modified to create smoke, dust, and death vfx.
    /// Since the camera is on the player at all times, particles will help
    /// bring a sense of movement to the world.
    /// </summary>
    internal class Particle
    {
        //Changes the speed and location
        private Vector2 position;
        private Vector2 velocity;

        //Fields to change appearance
        private Color color;
        private int size;

        //How long the particle will last
        private float maxLifetime;

        //The current life left
        private float lifetime;

        //default should be single pixel, will match
        //pixel art style
        private Texture2D asset;

        /// <summary>
        /// Does the particle still have time left
        /// </summary>
        public double Lifetime
        {
            get { return lifetime; }
        }

        /// <summary>
        /// Particles can be modified to create smoke, dust, and death vfx.
        /// Since the camera is on the player at all times, particles will help
        /// bring a sense of movement to the world.
        /// </summary>
        /// <param name="position">position, probably near player</param>
        /// <param name="velocity"> speed</param>
        /// <param name="color"> color</param>
        /// <param name="size"> width x height</param>
        /// <param name="lifetime"> how long for it to be alive</param>
        public Particle(Vector2 position, Vector2 velocity, Color color, int size, float lifetime, Texture2D asset)
        {
            this.position = position;
            this.velocity = velocity;
            this.color = color;
            this.size = size;
            this.lifetime = lifetime;
            this.asset = asset;

            //Particle just got created, start at max, then count down
            maxLifetime = lifetime;
        }

        /// <summary>
        /// Update the position and time of particle
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            //reduce total time left
            lifetime -= (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Is the particle still alive
            if (lifetime > 0)
            {
                //Add velocity to particle position
                position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

                //check the progress left on the particle
                float fade = lifetime / maxLifetime;

                //Convert to byte values
                fade *= 255;

                //Create a new color that is slightly more transparent.
                //Will fade out entirely whenever life = 0
                //Byte cast is needed to use the right color properties
                color = new Color(color, (byte)fade);
            }
        }

        /// <summary>
        /// Draws a single pixel
        /// </summary>
        /// <param name="_spriteBatch">working spriteBatch</param>
        public void Draw(SpriteBatch _spriteBatch)
        {
            //Only draw if still alive
            if (lifetime > 0)
            {
                //Draw the pixel at the position, with the amount of opaqueness left
                _spriteBatch.Draw(asset, new Rectangle((int)position.X, (int)position.Y, size, size), color);
            }
        }
    }
}
