using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.Miscellaneous
{
    public class AsteroidSpawner
    {
        //Asteroids
        private List<Asteroid> asteroids;
        private List<Texture2D> asteroidTextures;

        //Position
        private Random randy;
        private Vector2 bottomMid;

        //Spawn
        private float currSpawnTime = 0;
        private float spawnRate = 1.25f;

        public AsteroidSpawner(Texture2D asteroid1, Texture2D asteroid2)
        {

            //Create the lists
            asteroids = new List<Asteroid>();
            asteroidTextures = new List<Texture2D>();

            asteroidTextures.Add(asteroid1);
            asteroidTextures.Add((asteroid2));

            randy = new Random();

            ScreenStart();
        }

        public void ScreenStart()
        {
            //Create 8 asteroids to start the game
            for (int i = 0; i < 4; i++)
            {
                //Pick a random texture from the asset list
                Texture2D randomTexture = asteroidTextures[randy.Next(asteroidTextures.Count)];

                Vector2 velocity = new Vector2(-1f, -1f);

                //Random number from -1 to 1
                float rotationSpeed = (float)randy.NextDouble() * 2 - 1;

                //Random scale, must be positive
                float scale = (float)randy.NextDouble() / 3;

                //Fill the screen randomly
                int x = randy.Next(2400);
                int y = randy.Next(1500);

                asteroids.Add(new Asteroid(randomTexture, new Vector2(x,y), velocity, rotationSpeed / 3, 5, randy));
            }
        }

        public void Update(GameTime gameTime)
        {
            currSpawnTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            bottomMid = new Vector2(Game1.Cam.CameraPosition.X, Game1.Cam.CameraPosition.Y + 800);

            //Enough time has passed to spawn another asteroid
            if (currSpawnTime >= spawnRate)
            {
                //Spawn and reset time
                Spawn();
                currSpawnTime = 0;
            }

            //Iterate backwards to remove in list
            for(int i = asteroids.Count - 1; i>= 0; i--)
            {
                asteroids[i].Update(gameTime);

                //the asteroid has left the screen
                if (asteroids[i].Position.X < Game1.Cam.CameraPosition.X - 1500 && asteroids[i].Position.Y < Game1.Cam.CameraPosition.Y - 900)
                {
                    asteroids.RemoveAt(i);
                }
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            //Draw each asteroid
            foreach(Asteroid a in asteroids)
            {
                a.Draw(_spriteBatch);
            }
        }
        /// <summary>
        /// Spawn asteroids
        /// </summary>
        private void Spawn()
        {
            //Pick a random texture from the asset list
            Texture2D randomTexture = asteroidTextures[randy.Next(asteroidTextures.Count)];

            float newX = (float)randy.NextDouble() * 4000 - 1000;

            Vector2 velocity = new Vector2(-1f, -1f);

            //Random number from -1 to 1
            float rotationSpeed = (float)randy.NextDouble() * 2 - 1;

            //Random scale, must be positive
            float scale = (float)randy.NextDouble() /3;

            asteroids.Add(new Asteroid(randomTexture, new Vector2(bottomMid.X + newX, bottomMid.Y), velocity, rotationSpeed / 3, 5, randy));
        }
    }
}
