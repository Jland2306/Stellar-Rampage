using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StellarRampage.GameObjects;
using StellarRampage.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;

namespace StellarRampage.Managers
{
    //Class cannot be inherited from
    //Class cannot be inherited from
    public sealed class ProjectileManager
    {

        //Creates a new static instance of this manager, there will only be one
        private static ProjectileManager instance = null;
        public static ProjectileManager Instance
        {
            //Returns the instance if it exists, creates it if not
            get
            {
                if (instance == null)
                {
                    instance = new ProjectileManager();
                }
                return instance;
            }
        }

        public void Initialize(Texture2D pixel, Texture2D asset, Texture2D wave)
        {
            bullets = new List<Projectile>();
            removalList = new List<Projectile>();
            bulletTexture = asset;
            this.pixel = pixel;
            particleSystem = new ParticleSystem(pixel);
            this.wave = wave;
        }

        private List<Projectile> bullets;
        private Texture2D bulletTexture;
        private List<Projectile> removalList;
        private Texture2D pixel;
        private Texture2D wave;

        //Gunpowder smoke
        ParticleSystem particleSystem;

        //UPGRADE   
        public int extraBullets = 0;
        public int bulletHealth = 1;

        //Collision
        public List<Projectile> Bullets
        {
            get { return bullets; }
        }

        //Player has fired, add a new bullet
        public void AddBullet(Point playerCenter, Vector2 dirVector)
        {
            float angle;

            for (int i = 0; i < 30; i++)
            {
                particleSystem.CreateParticle(playerCenter.ToVector2(), dirVector * 100, Color.White, 5, 0.5f);
            }

            //Iterates at least once, plus each extra bullet the player has
            for (int i = 0; i < extraBullets + 1; i++) 
            {
                // 2pi represents 360 degrees in rads.
                // Divide that amongst the number of bullets, and multiply by the current
                // bullet to find how far the angle must rotate
                float spreadAngle = ((MathF.PI / 4)/ (extraBullets + 1)) * (i - (extraBullets * 0.5f));

                // Apply the rotation by using the rotation matrix 
                Vector2 rotatedDir = new Vector2(
                    (float)Math.Cos(spreadAngle) * dirVector.X - (float)Math.Sin(spreadAngle) * dirVector.Y,
                    (float)Math.Sin(spreadAngle) * dirVector.X + (float)Math.Cos(spreadAngle) * dirVector.Y);

                //Find the new angle of that bullet
                angle = (float)Math.Atan2(rotatedDir.Y, rotatedDir.X);

                //Create a new bullet using the dir vector and angle
                bullets.Add(new Projectile(
                    bulletTexture,
                    bulletHealth,
                    new Vector2(playerCenter.X, playerCenter.Y),
                    rotatedDir,
                    angle,
                    pixel));
            }

        }

        /// <summary>
        /// Player has swung sword, show the wave
        /// </summary>
        /// <param name="playerCenter">player location</param>
        /// <param name="dirVector">the direction the projectile is moving in</param>
        public void AddWave(Point playerCenter, Vector2 dirVector, float scale)
        {
            float angle;

            //Iterates at least once, plus each extra bullet the player has
            for (int i = 0; i < extraBullets + 1; i++)
            {
                // 2pi represents 360 degrees in rads.
                // Divide that amongst the number of bullets, and multiply by the current
                // bullet to find how far the angle must rotate
                float spreadAngle = ((MathF.PI / 4) / (extraBullets + 1)) * (i - (extraBullets * 0.5f));

                // Apply the rotation by using the rotation matrix 
                Vector2 rotatedDir = new Vector2(
                (float)Math.Cos(spreadAngle) * dirVector.X - (float)Math.Sin(spreadAngle) * dirVector.Y,
                    (float)Math.Sin(spreadAngle) * dirVector.X + (float)Math.Cos(spreadAngle) * dirVector.Y);

                //Find the new angle of that bullet
                angle = (float)Math.Atan2(rotatedDir.Y, rotatedDir.X);

                //Create a new bullet using the dir vector and angle
                bullets.Add(new WaveProjectile(
                    wave,
                    bulletHealth,
                    new Vector2(playerCenter.X, playerCenter.Y),
                    rotatedDir,
                    angle,
                    pixel,
                    scale));
            }
        }

        public void OrbShot(Vector2 orbCenter, float angle)
        {
            //Get the direction the shot should travel
            Vector2 orbDir = new Vector2(MathF.Cos(angle), MathF.Sin(angle));
            orbDir.Normalize();

            //Create a new bullet using the dir vector and angle
            bullets.Add(new Projectile(
                bulletTexture,
                bulletHealth,
                new Vector2(orbCenter.X, orbCenter.Y),
                orbDir,
                angle,
                pixel));
        }
        /// <summary>
        /// Create death explosion
        /// </summary>
        public void Explosion(Vector2 position)
        {
            for (int i = 0; i < 100; i++)
            {
                particleSystem.CreateParticle(position, Vector2.Zero, Color.DarkGray, 5, 0.5f);
            }
        }

        //Draw all bullets to screen
        public void DrawBullets(SpriteBatch sb, bool debugOn)
        {
            foreach (Projectile p in bullets)
            {
                p.Draw(sb, debugOn);

                if (debugOn)
                {
                    p.DebugDraw(sb, 10);
                }
            }
            particleSystem.Draw(sb);
        }
        //Update each active bullet
        public void UpdateAll(GameTime gameTime)
        {
            foreach (Projectile p in bullets)
            {
                p.Update(gameTime);
            }
            foreach (Projectile p in removalList)
            {
                bullets.Remove(p);
            }
            removalList.Clear();

            //Update particles
            particleSystem.Update(gameTime);
        }

        /// <summary>
        /// Bullet has expired, remove it
        /// </summary>
        /// <param name="bullet"></param>
        public void RemoveBullet(Projectile bullet)
        {
            if (bullets.Contains(bullet))
            {
                removalList.Add(bullet);
            }
        }
    }
}
