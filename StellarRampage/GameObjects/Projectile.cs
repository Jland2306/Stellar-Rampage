using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellarRampage.Managers;
using StellarRampage.HelperClasses;
using StellarRampage.Miscellaneous;
using StellarRampage.Particles;
using System.IO;
using StellarRampage.GameObjects.Enemies;

namespace StellarRampage.GameObjects
{
    public class Projectile : GameObject
    {
        protected double maxTime;
        protected double timeCounter;
        protected float angle;
        protected Circle circleHitbox;
        protected List<Enemy> enemiesHit;


        //If a boss is hit, toggle it.
        //This will stop the projectile from firing more than once
        public bool HitBoss;

        /// <summary>
        /// Returns the circle hitbox
        /// </summary>
        public Circle Circle
        {
            get { return circleHitbox; }
        }

        public Projectile(Texture2D asset, float health, Vector2 pos, Vector2 dirVector, float angle, Texture2D pixel)
            : base(asset, health, pos)
        {
            movement = dirVector;
            speed = 8f;
            maxTime = 2f;
            this.angle = angle;
            circleHitbox = new Circle(Center, 7);
            enemiesHit = new List<Enemy>();
        }

        public override void Draw(SpriteBatch _spriteBatch, bool debugOn)
        {
            //Draws the projectile based on its angle.
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
        public virtual void Update(GameTime gameTime)
        {
            timeCounter += gameTime.ElapsedGameTime.TotalSeconds;

            if (timeCounter > maxTime)
            {
                ProjectileManager.Instance.RemoveBullet(this);
            }
            else
            {
                MoveBullet();
                CheckCollision();
            }
        }

        /// <summary>4
        /// Fire this anytime a boss is hit, making sure the boss is not hit twice
        /// </summary>
        public void BossCollided()
        {
            health--;
            HitBoss = true;
        }

        /// <summary>
        /// Check if the bullet is still alive
        /// </summary>
        private void CheckHealth()
        {
            if (health <= 0)
            {
                ProjectileManager.Instance.RemoveBullet(this);
            }
        }

        //Keeps the bullet moving in a straight line
        private void MoveBullet()
        {
            //Update position of asset
            position += movement * speed;
            //Update position of hitbox
            circleHitbox.Center = Position;
        }

        /// <summary>
        /// Checks if if a projectile hits an enemy
        /// </summary>
        /// <returns></returns>
        public Enemy CheckCollision()
        {
            //Gets the 9 cells near the bullet
            List<Enemy> enemiesNear = Grid.Instance.GetEnemies(position);

            //Iterates over all nearby enemies
            foreach (Enemy e in enemiesNear)
            {
                bool collision = TriangleVsCircle(e.Hitbox);

                //Only hit an enemy once
                if (collision && !enemiesHit.Contains(e))
                {
                    //make the enemy take damage
                    e.TakeDamage(1);
                    //reduce the health of this bullet
                    health--; 
                    //Check if the bullet should be destroyed
                    CheckHealth();

                    //Add the enemy to prevent double hit
                    enemiesHit.Add(e);
                }
            }
            return null;
        }

        /// <summary>
        /// Checks collisions of a circle vs all lines of a triangle
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        private bool TriangleVsCircle(Vector2[] vertices)
        {
            //Starting vertice
            
            int next = 0;

            for (int i = 0; i < vertices.Length; i++)
            {
                //Get the next vertex in array
                next = i + 1;
                //Hit end of list, restart
                if (next == vertices.Length) next = 0;

                Vector2 currentVertex = vertices[i];
                Vector2 nextVertex = vertices[next];

              //Check if the circle collided with that line
              if (LineVsCircle(currentVertex, nextVertex))
              {
                    return true;
              }
            }
            
            if (TriangleVsPoint(vertices))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check collision for inside triangle vs 1 point
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        private bool TriangleVsPoint(Vector2[] vertices)
        { 

            bool collision = false;

            float x = circleHitbox.Center.X;
            float y = circleHitbox.Center.Y;
            //Starting vertice
            int next = 0;

            for (int i = 0; i < vertices.Length; i++)
            {
                //Get the next vertex in array
                next = i + 1;
                //Hit end of list, restart
                if (next == vertices.Length) next = 0;

                Vector2 currentVertex = vertices[i];
                Vector2 nextVertex = vertices[next];

                if (((currentVertex.Y > y && nextVertex.Y < y) || (currentVertex.Y < y && nextVertex.Y > y)) &&
                    (x < (nextVertex.X-currentVertex.X) * (y-currentVertex.Y)/(nextVertex.Y-currentVertex.Y) + currentVertex.X))
                {
                        collision = !collision;
                }

            }
            return collision;
        }
        /// <summary>
        /// Checks collision of a circle vs 1 line
        /// </summary>
        /// <param name="startVertex"></param>
        /// <param name="endVertex"></param>
        /// <returns></returns>
        private bool LineVsCircle(Vector2 startVertex, Vector2 endVertex)
        {
            //Gets length of line
            float distX = startVertex.X - endVertex.X;
            float distY = startVertex.Y - endVertex.Y;

            //Distance formula
            float len = MathF.Sqrt((distX * distX) + (distY * distY));

            //Dot product from line to circle
            float x = circleHitbox.Center.X;
            float y = circleHitbox.Center.Y;
            float r = circleHitbox.Radius;
            float dot = ((x-startVertex.X)*(endVertex.X-startVertex.X) +
                (y - startVertex.Y) * (endVertex.Y - startVertex.Y)) / MathF.Pow(len,2);

            dot = Math.Clamp(dot, 0, 1);

            //Find closest point
            float closestX = startVertex.X + (dot * (endVertex.X - startVertex.X));
            float closestY = startVertex.Y + (dot * (endVertex.Y - startVertex.Y));

            distX = closestX - x;
            distY = closestY - y;

            float distance = MathF.Sqrt((distX * distX) + (distY * distY));
            if(distance <= r)
            {
                return true;
            }
            return false;

        }

        private bool LineVsPoint(Vector2 start, Vector2 end, float closestX, float closestY)
        {
            return false;
        }
        public void DebugDraw(SpriteBatch _spriteBatch, int numberOfPoints)
        {
            DebugLib.DrawCircleOutline(_spriteBatch, circleHitbox.Center, circleHitbox.Radius, numberOfPoints, 2, Color.Red);
        }
    }

}
