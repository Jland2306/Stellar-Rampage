
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StellarRampage.HelperClasses;

namespace StellarRampage.GameObjects.Enemies
{
    public class Enemy : GameObject
    {
        //---------------------------------------------------------------------
        //                          FIELDS
        //---------------------------------------------------------------------
        // Hitbox
        private Vector2 nosePoint;
        private Vector2 tailPoint1;
        private Vector2 tailPoint2;

        // Movement
        private Vector2 movementDirection;
        private float angle;

        // Switches the animations
        protected AnimatedSprite animHandler;

        //Used to show the enemy takes damage
        private Tween.ColorTween tintTween;

        //Debug
        private SpriteFont font;

        //Extra rotation
        protected float baseRotation;

        //Explode
        public bool isExploding;

        protected enum AnimState
        {
            Single,
            Shield,
            Death
        }

        //---------------------------------------------------------------------
        //                          PROPERTIES
        //---------------------------------------------------------------------
        public Vector2[] Hitbox
        {
            get
            {
                Vector2[] hitbox = new Vector2[3];

                hitbox[0] = nosePoint;
                hitbox[1] = tailPoint1;
                hitbox[2] = tailPoint2;

                return hitbox;
            }
        }

        public float BaseRotation
        {
            get { return baseRotation; }
            set { baseRotation = value; }
        }

        //---------------------------------------------------------------------
        //                          CONSTRUCTOR
        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes the enemy object
        /// </summary>
        public Enemy(List<Texture2D> assets, float health, Vector2 position, float speed, SpriteFont font)
            : base(assets[0], health, position)
        {
            this.speed = speed;

            //Create the animation
            animHandler = new AnimatedSprite(asset, 64, 64, 2, 0.1f, new Rectangle((int)position.X, (int)position.Y, 32, 32), scale: 3f);

            //This ensures frame count matches up for every spriteSheet
            animHandler.UpdateFrameCount(); 

            // Allows position to be printed above enemy
            this.font = font;

            nosePoint = position + movementDirection * 50;
            tailPoint1 = position - movementDirection;
            tailPoint2 = position - new Vector2(movementDirection.Y, movementDirection.X) * 50;
        }

        //---------------------------------------------------------------------
        //                          METHODS
        //---------------------------------------------------------------------

        /// <summary>
        /// Updates the enemy's position & state
        /// </summary>
        /// <param name="gameTime"></param>
        public virtual void Update(GameTime gameTime, Vector2 playerPosition)
        {
            // Update position with Move method
            UpdatePosition(playerPosition);

            // Update the Animation
            animHandler.Update(gameTime);

            //Update any existing tint
            if (tintTween != null && !tintTween.IsCompleted)
            {
                //Change the current opacity of the color
                animHandler.Color = tintTween.currColor;
            }
        }

        /// <summary>
        /// Draws the enemy to the game window
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public override void Draw(SpriteBatch _spriteBatch, bool debugOn)
        {
            //Draw the animation
            animHandler.DrawRotated(_spriteBatch);
            if (debugOn)
            {
                Debug(_spriteBatch);
            }
        }

        /// <summary>
        /// Moves the Enemy instance towards the player
        /// </summary>
        /// <param name="playerPosition">the player's position to move towards</param>
        public void UpdatePosition(Vector2 playerPosition)
        {
            // Draw the direction vector
            movementDirection = playerPosition - position;

            movementDirection.Normalize();

            position += movementDirection * speed;

            // calculate the angle
            angle = (float)Math.Atan2(movementDirection.Y, movementDirection.X);


            // Update Hitbox
            nosePoint = position + movementDirection * 45;
            tailPoint1 = position -
                        new Vector2(-movementDirection.Y, movementDirection.X) * -35 - movementDirection * 35;
            tailPoint2 = position -
                        new Vector2(-movementDirection.Y, movementDirection.X) * 35 - movementDirection * 35;

            //Update animation position and angle
            animHandler.Position = new Rectangle((int)position.X, (int)position.Y, asset.Width,asset.Height);
            animHandler.Rotation = angle + baseRotation;
        }

        /// <summary>
        /// Adds a new list of sheets to the animated sprite
        /// </summary>
        /// <param name="sheets">the list of sheets to add</param>
        public void AddSheets(List<Texture2D> newSheets)
        {
            //Add the sheets to the anim handler
            animHandler.AddSheets(newSheets);
        }

        /// <summary>
        /// Keep the explosion animation running 
        /// </summary>
        public bool Explode(GameTime gameTime)
        {
            //If update returns true, then the animation finished
            if (animHandler.Update(gameTime))
            {
                //Remove the exploding animation
                isExploding = false;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Draws the debug information of the enemy
        /// </summary>
        public void Debug(SpriteBatch sb)
        {
            // Draw the movement vector
            DebugLib.DrawLine(sb, position + movementDirection * 50, position + movementDirection * 100, 2, Color.White);

            //Draw the position above enemy
            sb.DrawString(
                font,
                RoundedPos,
                DebugPosition,
                Color.White);

            // Draw the hitbox
            DebugLib.DrawTriangleOutline(sb, nosePoint, tailPoint1, tailPoint2, 2, Color.Blue);
        }

        public virtual void EnemyStartUp()
        {
            BaseRotation = MathF.PI / 2;
        }


        /// <summary>
        /// Damages enemy by a certain amount
        /// </summary>
        /// <param name="damage">amount of damage the enemy takes</param>
        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);

            //start a tween to red
            tintTween = Tween.CreateColorTween(Color.White, Color.Red, 0.15f, EaseType.EaseOut);
            tintTween.OnComplete = () =>
            {
                //Start a tween back to white
                tintTween = Tween.CreateColorTween(Color.Red, Color.White, 0.5f, EaseType.EaseOutCubic);
            };

            //Cause the enemy to explode
            if(health <= 0)
            {
                //Death animation
                animHandler.SetIndex(2);
                isExploding = true;
            }
        }
    }
}
