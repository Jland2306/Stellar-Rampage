using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellarRampage.Managers;

namespace StellarRampage.GameObjects
{
    /// <summary>
    /// Base orbitals shoot after a given amount of time. They take no damage
    /// </summary>
    internal class Orbital : GameObject
    {
        //Changes how fast the orb floats
        protected float baseRadius = 50;
        protected float angle = 0;

        //Will allow for orbital to knock back a little 
        //bit everytime it shoots
        protected float bobAmp = 1;
        protected float bobSpeed = 25;

        //Makes it match the color of player
        protected Texture2D topAsset;
        protected Color topColor;

        //Dictates when orb shoots
        protected double timeTillShoot;
        protected double timeMax = 1;

        //How far off the orbital is offset.
        //Changes between different orbital types
        protected float radiusOffset = 0;

        //Update for each additional orbital. Each one should be 
        // evenly spaced. This will represent angle from the next orbital
        protected float AngleOffset = 0;

        /// <summary>
        /// The actual angle of the orbital
        /// </summary>
        public virtual float TotalAngle
        {
            get { return angle + AngleOffset; }
        }

        /// <summary>
        /// The color of top should match the player
        /// </summary>
        public Color TopColor
        {
            set { topColor = value; }
        }

        /// <summary>
        /// Create a new orbital
        /// </summary>
        /// <param name="asset">Texture</param>
        /// <param name="health">Amount of health</param>
        /// <param name="position">Player position</param>
        /// <param name="top">The top asset</param>
        /// <param name="color">Color of player</param>
        public Orbital(Texture2D asset, float health, Vector2 position, Texture2D top, Color color) 
            : base(asset, health, position)
        {
            //How fast to orbit
            speed = 1;

            //Change top to match player
            topColor = color;
            topAsset = top;
        }

        /// <summary>
        /// Draw the orbital
        /// </summary>
        /// <param name="_spriteBatch"></param>
        /// <param name="debugOn"></param>
        public override void Draw(SpriteBatch _spriteBatch, bool debugOn)
        {
            //Draw the asset
            _spriteBatch.Draw(
                asset,                  //Asset
                new Rectangle(          //Rect
                    (int)position.X,    //X
                    (int)position.Y,    //Y
                    asset.Width,        //Width
                    asset.Height),      //Height
                null,                   //Source
                Color.White,            //Color
                TotalAngle,             //Angle
                LocalCenter,            //Origin
                SpriteEffects.None,     //SpriteEffect
                0f);                    //Layer

            //Draw the matching top
            //Shields do not inherit color
            if(this is not Shield)
            {
                _spriteBatch.Draw(
                topAsset,               //Asset
                new Rectangle(          //Rect
                    (int)position.X,    //X
                    (int)position.Y,    //Y
                    asset.Width,        //Width
                    asset.Height),      //Height
                null,                   //Source
                topColor,               //Color
                TotalAngle,             //Angle
                LocalCenter,            //Origin
                SpriteEffects.None,     //SpriteEffect
                0f);                    //Layer
            }

        }

        /// <summary>
        /// Update the angle of the orbital
        /// </summary>
        /// <param name="gameTime">time thats elapsed</param>
        /// <param name="playerPos">the position the player is at</param>
        /// <param name="angle">angle rotated</param>
        public virtual void Update(GameTime gameTime, Vector2 playerPos, float angle)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Allows the orb to bob around player
            float radius = ((baseRadius + MathF.Sin(time * bobSpeed)) * bobAmp) + radiusOffset;

            //Polar offset, follow circle
            Vector2 offset = new Vector2(
                (float)Math.Cos(TotalAngle),
                (float)Math.Sin(TotalAngle)) * radius;

            this.angle = angle;

            //the orb will always follow player position
            position = playerPos + offset;

            Shoot(time);
        }

        /// <summary>
        /// Orbs should auto fire at the angle they are facing
        /// </summary>
        public virtual void Shoot(float time)
        {
            //Check if orb should shoot
            timeTillShoot += time;
            if (timeTillShoot >= timeMax)
            {
                //reset counter
                timeTillShoot = 0;
                //Shoot a bullet
                ProjectileManager.Instance.OrbShot(position, TotalAngle);
            }
        }

        /// <summary>
        /// Add the offset that having multiple orbs causes
        /// </summary>
        /// <param name="offsetAngle">the amount to offset</param>
        public void IncreaseAngle(float offsetAngle)
        {
            AngleOffset = offsetAngle;
        }
    }
}
