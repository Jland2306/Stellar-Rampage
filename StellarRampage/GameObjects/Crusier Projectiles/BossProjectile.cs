using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StellarRampage.HelperClasses;
using StellarRampage.Managers;

namespace StellarRampage.GameObjects.Crusier_Projectiles
{

    enum ProjectileState
    {
        /// <summary>
        /// Not locked on to the player
        /// </summary>
        Dumb,
        /// <summary>
        /// Locked onto the player
        /// </summary>
        Smart,
        /// <summary>
        /// end of life / destroyed
        /// </summary>
        Destroyed
    }

    internal class BossProjectile : GameObject
    {

        private double maxTime;
        private double timeCounter;
        private float angle;
        private Circle circleHitbox;
        private ProjectileState state;

        public BossProjectile(float health, Vector2 pos, Vector2 dirVector, float angle, Texture2D Explosion, float speed, float maxTime)
            : base(null, health, pos)
        {
            movement = dirVector;
            this.speed = speed;
            this.maxTime = maxTime;
            this.angle = angle;
            this.circleHitbox = new Circle(Center, 7);
        }


        public override void Draw(SpriteBatch _spriteBatch, bool debugOn)
        {
            _spriteBatch.Draw(asset,
                position,
                new Rectangle(0, 0, asset.Width, asset.Height),
                Color.White,
                angle,
                LocalCenter,
                1f,
                SpriteEffects.None,
                0
            );
        }

        /// <summary>
        /// Update projectile position
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            if (timeCounter > maxTime)
            {
                Destroy();
            }
            else
            {
                MoveProjectile();
            }
        }

        public virtual void MoveProjectile()
        {
            position += movement * speed; 
        }

        public virtual void Destroy()
        {

        }

        protected void AddVelocity(Vector2 initialVelocity, Vector2 addedVelocity,
                                out Vector2 combinedVelocity, out float magnitude)
        {
            combinedVelocity = initialVelocity + addedVelocity;

            magnitude = combinedVelocity.Length();

            combinedVelocity.Normalize();
        }
    }
}
