using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.GameObjects
{

    /// <summary>
    /// Satellites have no current use; however, they can be used to allow for upgrades, enemies,
    /// or just visuals
    /// </summary>
    internal class Satellite : Orbital
    {
        public Satellite(Texture2D asset, float health, Vector2 position, Texture2D top, Color color)
         : base(asset, health, position, top, color)
        {
            //how far off the satellite orbits
            radiusOffset = 405;
        }

        /// <summary>
        /// No current shoot implemented
        /// </summary>
        public override void Shoot(float time)
        {
            return;
        }

        /// <summary>
        /// Draw the satellite
        /// </summary>
        /// <param name="_spriteBatch"></param>
        /// <param name="debugOn">Draw hitbox?</param>
        public override void Draw(SpriteBatch _spriteBatch, bool debugOn)
        {
            //Draw the satellite
            base.Draw(_spriteBatch, debugOn);
        }

        /// <summary>
        /// Update position and rotation of the satellite
        /// </summary>
        /// <param name="gameTime">time elapsed</param>
        /// <param name="screenCenter">location of camera</param>
        /// <param name="angle">angle the satellite is at</param>
        public override void Update(GameTime gameTime, Vector2 screenCenter, float angle)
        {
            //Update time
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //add the angle for the rotation
            this.angle = angle;

            //Get the direction vector of the angle
            Vector2 direction = new Vector2( MathF.Cos(TotalAngle), MathF.Sin(TotalAngle));

            // Calculate the distance to the edge. Take the screen size,
            // over the current direction the satellite is at. The 2 represents
            // 1 half, because the player is always centered. 
            //Take min to avoid division by zero
            float sideEdge = Game1.Width / (MathF.Max(MathF.Abs(direction.X), 0.01f) * 2f);
            float topEdge = Game1.Height / (MathF.Max(MathF.Abs(direction.Y), 0.01f) * 2f);

            //Figure out which edge is closer. The 0.9 represents a slight inset,
            //to make sure most of the texture is visible
            float radius = (Math.Min(topEdge, sideEdge)) * 0.9f;

            // Final position is the location the player is at, plus
            // the distance to the edge
            position = screenCenter + direction * radius;

            //If it has a shoot method, do it
            Shoot(time);
        }

    }
}
