using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace StellarRampage.Particles
{

    /// <summary>
    /// Keeps track of all particles. Adds and deletes particles as needed
    /// </summary>
    public class ParticleSystem
    {
        //Allow settings to change this
        public static bool DrawParticles = true;

        //All pixels to draw
        private List<Particle> particles;

        //Adds randomness to position, speed, color
        private Random randy;

        //The color of the particles, default = gray
        private Color gray = new Color(150, 150, 150, 255);

        //What should the average lifetime be
        const float Lifetime = 2.5f;

        //The average size
        const int Size = 4;

        //What should the particles look like
        private Texture2D asset;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="pixel"></param>
        public ParticleSystem(Texture2D asset)
        {
            particles = new List<Particle>();
            randy = new Random();
            this.asset = asset;
        }


        /// <summary>
        /// Create a new particle using constant variables.
        /// </summary>
        /// <param name="position">Where to spawn from</param>
        public void CreateParticle(Vector2 position)
        {
            //Dont create if particles is turned off
            if (DrawParticles)
            {
                //Create a new particle with the random variables
                CreateParticle(position, Vector2.Zero, gray, Size, Lifetime);
            }
        }


        /// <summary>
        /// Create a new particle using a set number of random variables.
        /// Can customize all aspects of the particle
        /// </summary>
        /// <param name="position">Where to spawn from</param>
        /// <param name="velocity">Where/How fast is it moving</param>
        /// <param name="color">Color/tint of particle</param>
        /// <param name="size">How large is it</param>
        /// <param name="lifetime">How long does it last</param>
        public void CreateParticle(Vector2 position, Vector2 velocity, Color color, int size, float lifetime)
        {
            //Take the size +- half the size
            int partSize = randy.Next(size - size / 2, size + size / 2);

            //Take the life +- 1 second. Use milliseconds to get fractions of a second
            float life = randy.Next(
                (int)(lifetime - 1) * 1000,
                (int)(lifetime + 1) * 1000) / 1000.00f;

            //Add variation to the average velocity
            Vector2 randVelocity = new Vector2
                (randy.Next(
                    (int)(velocity.X - 30),
                    (int)(velocity.X + 30)),
                randy.Next(
                    (int)(velocity.Y - 30),
                    (int)(velocity.Y + 30)));

            //Create a new particle 
            particles.Add(new Particle(position, randVelocity, color, size, lifetime, asset));
        }

        /// <summary>
        /// Update all particle positions
        /// </summary>
        /// <param name="gameTime">time passed</param>
        public void Update(GameTime gameTime)
        {
            //for loop needed over foreach as index will be removed once
            //time runs out
            for (int i = 0; i < particles.Count; i++)
            {
                //update the particle
                particles[i].Update(gameTime);

                //Check if particle is out of time
                if (particles[i].Lifetime <= 0)
                {
                    //remove particle
                    particles.RemoveAt(i);
                }
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            // Do not draw if turned off
            if (DrawParticles)
            {
                foreach (Particle p in particles)
                {
                    p.Draw(_spriteBatch);
                }
            }

        }
    }
}
