
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using StellarRampage.Managers;
namespace StellarRampage.GameObjects
{
    /// <summary>
    /// Gameobject is the barebones of any object. All objects in a scene
    /// will be derived and controlled using this base class
    /// </summary>
    public abstract class GameObject
    {
        //Gameobject image
        protected Texture2D asset;

        //The amount of health a player/enemy has, or the number of 
        //times a bullet can pierce
        protected float health;

        //Location of the object
        protected Vector2 position;

        // Unit movement vector
        protected Vector2 movement;

        // Speed multiplier of the object
        protected float speed;

        /// <summary>
        /// Returns the object position
        /// </summary>
        public Vector2 Position
        {
            get { return position; }

        }

        /// <summary>
        /// Returns the game object sprite
        /// </summary>
        public Texture2D Asset
        {
            get { return asset; }
        }

        /// <summary>
        /// Returns the center of the object
        /// </summary>
        public Vector2 Center
        {
            //Takes the width/height and splits it
            //in half to find the center point
            get
            {
                return new Vector2(
                position.X + asset.Width / 2,
                position.Y + asset.Height / 2);
            }
        }

        /// <summary>
        /// Returns the center of the asset in local space
        /// </summary>
        public Vector2 LocalCenter
        {
            get
            {
                //Gets the asset center
                return new Vector2(
                asset.Width / 2,
                asset.Height / 2);
            }
        }

        /// <summary>
        /// Returns the location right above the gameObject
        /// </summary>
        public Vector2 DebugPosition
        {
            get
            {
                //Gets the position above asset
                return new Vector2(
                position.X - 60,
                position.Y - 20);
            }
        }


        /// <summary>
        /// Returns the string position rounded to 0 decimal places
        /// </summary>
        public string RoundedPos
        {
            get
            {
                //Rounds the position
                return 
                    $"X:{MathF.Round(position.X)}" +
                    $", Y:{MathF.Round(position.Y)}";
            }
        }
        
        /// <summary>
        /// Get-only property for object health
        /// </summary>
        public float Health
        {
            get { return health; }
            set { health = value; }
        }
        /// <summary>
        /// Creates a new game object given a texture2D,
        /// amount of health,
        /// and starting location
        /// </summary>
        /// <param name="asset">Image of asset</param>
        /// <param name="health">Durability</param>
        /// <param name="position">Location of object</param>
        public GameObject(Texture2D asset, float health, Vector2 position)
        {
            this.asset = asset;
            this.health = health;
            this.position = position;
        }

        /// <summary>
        /// Draw the gameobject to screen
        /// </summary>
        /// <param name="_spriteBatch">The working spritebatch</param>
        public abstract void Draw(SpriteBatch _spriteBatch, bool debugOn);

        /// <summary>
        /// Updates the object position by adding the movement vector to current position
        /// </summary>
        public virtual void UpdatePosition()
        {
            position += movement * speed;
        }

        /// <summary>
        /// Damages object by a certain amount
        /// </summary>
        /// <param name="damage">amount of damage the object takes</param>
        public virtual void TakeDamage(int damage)
        {
            SoundManager.PlayEnvironmentalSound("Hit", Position,1f, 0);
            health -= damage;
        }


    }
}
