using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StellarRampage.GameObjects;
using StellarRampage.Miscellaneous;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.Particles
{
    public sealed class ShockwaveManager
    {
        //Creates a new static instance of this manager, there will only be one
        private static ShockwaveManager instance = null;

        //The working instance of the class
        public static ShockwaveManager Instance
        {
            //Returns the instance if it exists, creates it if not
            get
            {
                if (instance == null)
                {
                    instance = new ShockwaveManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// Initialize replaces constructor. Should only be called once on creation.
        /// </summary>
        public void Initialize(Texture2D ring)
        {
            this.ring = ring;
        }

        List<Shockwave> shockwaves = new List<Shockwave>();
        Texture2D ring;

        /// <summary>
        /// Update the shockwaves
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            //Iterate backwards to prevent error while removing
            for (int i = shockwaves.Count - 1; i >= 0; i--)
            {
                //Update the radius
                shockwaves[i].Update(gameTime);

                //Check if the shockwave has finished 
                if (shockwaves[i].IsComplete)
                {
                    //Remove from list
                    shockwaves.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Draws all the shockwaves
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Shockwave s in shockwaves)
            {
                s.Draw(spriteBatch);
            }
        }


        /// <summary>
        /// Adds a shockwave to the screen
        /// </summary>
        public void AddWave(Vector2 center)
        {
            shockwaves.Add(new Shockwave(center, 900, 1000, ring));
        }

    }
}
