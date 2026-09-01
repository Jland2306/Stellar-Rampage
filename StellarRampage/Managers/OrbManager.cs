using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StellarRampage.GameObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.Managers
{
    /// <summary>
    /// Orbitals are any objects that float around the player. This can be a shield
    /// turret, or even an enemy.
    /// </summary>
    public class OrbManager
    {
        //All current orbitals possible
        private List<Orbital> orbitals;
        private List<Shield> shields;
        private List<Satellite> satellites;

        //Orbital assets
        private Texture2D orbitalAsset;
        private Texture2D orbitalTop;
        private Texture2D shieldText;
        private Texture2D satellite;

        //Angles for each orbital
        private float angle;
        private float slowAngle;
        private float satelliteAngle;

        //How fast a slow orb should rotate, shields
        private float baseSpeed = 1f;

        private float shieldSpacing = 0.45f;

        /// <summary>
        /// Create a new orbmanager
        /// </summary>
        /// <param name="orbitalAsset"></param>
        /// <param name="orbitalTop"></param>
        /// <param name="shield"></param>
        /// <param name="satellite"></param>
        public OrbManager(Texture2D orbitalAsset, Texture2D orbitalTop, Texture2D shieldText, Texture2D satellite)
        {
            //Assign the textures
            this.orbitalAsset = orbitalAsset;
            this.orbitalTop = orbitalTop;
            this.shieldText = shieldText;
            this.satellite = satellite;

            //Create the lists
            orbitals = new List<Orbital>();
            shields = new List<Shield>();
            satellites = new List<Satellite>();

            //Test satellite
            satellites.Add(new Satellite(satellite, 1, Vector2.Zero, satellite, Color.White));
        }

        /// <summary>
        /// Draws all orbitals
        /// </summary>
        /// <param name="_spriteBatch"></param>
        /// <param name="debugOn">draw hitbox?</param>
        public void Draw(SpriteBatch _spriteBatch, bool debugOn)
        {
            //Draw any orbitals that exist
            foreach (Orbital o in orbitals)
            {
                o.Draw(_spriteBatch, false);
            }
            foreach (Shield s in shields)
            {
                s.Draw(_spriteBatch, debugOn);
            }
            foreach (Satellite s in satellites)
            {
                s.Draw(_spriteBatch, debugOn);
            }
        }


        /// <summary>
        /// Updates the angle each orb should be at
        /// </summary>
        /// <param name="gameTime">time elapsed</param>
        /// <param name="speed">how fast the player is moving</param>
        /// <param name="position">player position</param>
        public void Update(GameTime gameTime, float speed, Vector2 position)
        {
            //Get the time passed
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Update orbs
            //Get the angle the orb should be at
            //Orbs should move faster when player moves
            angle += (speed + baseSpeed) * time;

            //Cap angle to 360 degrees
            angle %= MathF.PI * 2;

            //Slow angles do not inherit speed of player, they rotate
            //constantly
            slowAngle += baseSpeed * time;
            slowAngle %= MathF.PI * 2;
            
            //Satellites should move really slow
            satelliteAngle += baseSpeed * time / 15;
            satelliteAngle %= MathF.PI * 2;

            //Update each orbital position
            foreach (Orbital o in orbitals)
            {
                //Pass the center of the player
                o.Update(gameTime, position, angle);
            }
            foreach(Shield s in shields)
            {
                //Pass the center of the player
                s.Update(gameTime, position, slowAngle);
            }
            foreach (Satellite s in satellites)
            {
                //Pass the center of the camera
                s.Update(gameTime,Game1.Cam.CameraPosition, satelliteAngle);
            }
        }

        /// <summary>
        /// Add a turret orbital around the player 
        /// </summary>
        /// <param name="position">player position</param>
        /// <param name="color">color should match player outfit</param>
        public void SpawnOrbital(Vector2 position, Color color)
        {
            //Create and add the orbital to the list
            Orbital orbital = new Orbital(orbitalAsset, 1, position, orbitalTop, color);
            orbitals.Add(orbital);

            //Iterate through all orbs
            for (int i = 0; i < orbitals.Count; i++)
            {
                //get the new offset for each. divide 360 degrees by each orb
                float angleOffset = (MathF.PI * 2 / orbitals.Count) * i;

                //Add the angle offset to that shield 
                orbitals[i].IncreaseAngle(angleOffset);
            }
        }

        /// <summary>
        /// Adds a shield around the player
        /// </summary>
        /// <param name="position">the players position</param>
        /// <param name="color">what color is the player?</param>
        public void SpawnShield(Vector2 position, Color color)
        {
            //Create a new shield
            Shield shield = new Shield(shieldText, 1, position, shieldText, color);

            //Add it to the list that gets updated
            shields.Add(shield);

            //Start halfway back to center the shields.
            float start = -((shields.Count - 1) * shieldSpacing) / 2f;

            //Iterate through all shields
            for (int i = 0; i < shields.Count; i++)
            {
                //get the new offset for each. divide 360 degrees by each orb
                float angleOffset = start + (shieldSpacing * i);

                //Add the angle offset to that shield
                shields[i].IncreaseAngle(angleOffset);
            }
        }
        /// <summary>
        /// Remove all orbitals from player
        /// </summary>
        public void Clear()
        {
            shields.Clear();
            orbitals.Clear(); 
        }
    }
}
