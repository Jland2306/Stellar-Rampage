using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StellarRampage.HelperClasses;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StellarRampage.Managers;

namespace StellarRampage.Miscellaneous
{
    internal class XpOrb
    {
        private Vector2 pos;
        private AnimatedSprite sprite;
        private double maxTime = 15f;
        private double currTime;

        //Make the hitbox really large, so if player gets near, it flys toward player
        private Circle hitbox;


        //Drifting
        private Vector2 anchor;
        private float time;
        private float amplitudeX = 10f;
        private float amplitudeY = 10f;
        private float frequencyX = 1.5f;
        private float frequencyY = 1.5f;

        //orb should magnetize toward the player when they get too close
        private bool attracted;
        //time since magnetized
        private float attractTimer = 0f;
        //max speed wind up
        private float attractDuration = 0.5f;
        private float maxSpeed = 800;

        //curr velocity
        private Vector2 velocity = Vector2.Zero;

        private bool pickedUp;

        /// <summary>
        /// Should the orb be removed
        /// </summary>
        public bool IsComplete
        {
            get { return currTime >= maxTime && !attracted || pickedUp; }
        }

        /// <summary>
        /// Play the picked up sound
        /// </summary>
        public bool PickedUp
        {
            get { return pickedUp; }
        }

        public float Radius
        {
            get { return hitbox.Radius; }
            set { hitbox.Radius = value; }
        }

        public XpOrb(Vector2 pos, Texture2D asset, float radius)
        {
            this.pos = pos;
            anchor = pos;
            sprite = new AnimatedSprite(asset, 8, 8, 6, 0.15f, new Rectangle((int)pos.X,(int)pos.Y,8, 8), 3);
            hitbox = new Circle(pos, radius);
        }

        /// <summary>
        /// Draw the orb, and hitbox
        /// </summary>
        public void Draw(SpriteBatch _spriteBatch, bool debugOn)
        {
            sprite.DrawRotated(_spriteBatch);

            if (debugOn)
            {
                DebugLib.DrawCircleOutline(_spriteBatch, hitbox.Center, hitbox.Radius, 10, 2, Color.Green);
            }
        }

        /// <summary>
        /// Check collision and update animation
        /// </summary>
        public void Update(GameTime gameTime, Vector2 playerPos)
        {
            if (!attracted)
            {
                Float(gameTime);
            }

            currTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            sprite.Position = new Rectangle((int)pos.X,(int)pos.Y, 8,8);
            sprite.Update(gameTime);

            //Check if player got the orb
            if (CheckPlayerAttract(playerPos, (float)gameTime.ElapsedGameTime.TotalSeconds))
            {
                pickedUp = true; 
                UpgradeManager.Instance.CalculateXP(5);
            }

        }

        /// <summary>
        /// Check if orb should fly to player
        /// </summary>
        /// <param name="playerPos"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public bool CheckPlayerAttract(Vector2 playerPos, float time)
        {
            // Check if player is inside the hitbox
            if (!attracted && CollisionDetection.Instance.CircleVsPlayer(hitbox))
            {
                //Orb should fly to player
                attracted = true;
                attractTimer = 0f;
            }

            //If flying, update velocity
            if (attracted)
            {
                attractTimer += time;

                //Get the vector toward the player
                Vector2 dir = playerPos - pos;

                //How far is the orb from player
                float distance = dir.Length();

                //If orb is within a certain distance, pick it up
                if (distance < 10)
                {
                    return true; 
                }

                //Only normalize after, that way length can be checked
                dir.Normalize();

                //increase pull strength over time, get percent toward max speed
                float t = MathF.Min(attractTimer / attractDuration, 1f);

                //ramp up based on max speed
                float speed = t * t * maxSpeed;

                //Ramp up speed over time
                velocity = Vector2.Lerp(velocity, dir * speed, 0.2f);

                //Move the orb
                pos += velocity * time;
            }

            return false;
        }

        /// <summary>
        /// Moves the orb around so it feels like its drifting
        /// </summary>
        private void Float(GameTime gameTime)
        {
            //increase time
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            float x = MathF.Sin(time * frequencyX) * amplitudeX;
            float y = MathF.Cos(time * frequencyY) * amplitudeY;

            //Sets the position to the floating value
            pos = anchor + new Vector2(x, y);
        }
    }
}
