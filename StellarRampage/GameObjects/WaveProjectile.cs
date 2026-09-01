using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StellarRampage.GameObjects.Enemies;
using StellarRampage.HelperClasses;
using StellarRampage.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Metadata;

namespace StellarRampage.GameObjects
{
    /// <summary>
    /// A wave is a projectile that gets fired from a sword
    /// </summary>
    internal class WaveProjectile : Projectile
    {
        // Hitbox points
        Vector2 tip;
        Vector2 topLeft;
        Vector2 topRight;

        //Triangle accounting for rotation and position
        Vector2 newTip;
        Vector2 newTopLeft;
        Vector2 newTopRight;

        /// <summary>
        /// Collider
        /// </summary>
        public Vector2[] Vertices
        {
            get
            {
                //the hitbox to return
                Vector2[] hitbox = new Vector2[6];

                //Points needed to make a wave
                hitbox[0] = newTip;
                hitbox[1] = newTopLeft;
                hitbox[2] = newTopRight;

                return hitbox;
            }
        }

        //The wave sprite
        private AnimatedSprite sprite;

        public WaveProjectile(Texture2D asset, float health, Vector2 pos, Vector2 dirVector, float angle, Texture2D pixel, float scale) 
            : base(asset, health, pos, dirVector, angle, pixel)
        {
            //Create a new animated sprite
            sprite = new AnimatedSprite(asset, asset.Height, asset.Height, 6, 0.15f, new Rectangle(0, 0, asset.Width, asset.Height), scale);
            //sprite.UpdateFrameCount();
        }

        /// <summary>
        /// Draw the sword wave
        /// </summary>
        /// <param name="_spriteBatch"></param>
        /// <param name="debugOn"></param>
        public override void Draw(SpriteBatch _spriteBatch, bool debugOn)
        {
            sprite.DrawRotated(_spriteBatch);
        }

        /// <summary>
        /// Update projectile position
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            sprite.Position = new Rectangle((int)position.X,(int)position.Y, 64,64);

            //Increase by 90 degrees
            sprite.Rotation = angle + MathF.PI /2;
        }

    }
}
