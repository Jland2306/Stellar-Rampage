using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StellarRampage.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.GameObjects
{
    /// <summary>
    /// A boss represents a stronger enemy which varies from each boss
    /// </summary>
    public class Boss : GameObject
    {
        /// <summary>
        /// The state the boss should be animated with
        /// </summary>
        public enum AnimState
        {
            Base,
            Trail,
            Firing,
            Destroy,
            Shield,
        }

        protected Dictionary<AnimState, AnimatedSprite> animHandler;


        //How large to upscale boss
        private float scale = 3f;

        //The state the boss should be animated to
        protected AnimState state;

        //All the spriteSheets needed to animate a boss
        protected Texture2D baseSheet;
        protected Texture2D fireSheet;
        protected Texture2D destroySheet;
        protected Texture2D trailSheet;
        protected Texture2D shieldSheet;

        protected AnimatedSprite baseAnim;

        public Boss(Texture2D asset, float health, Vector2 position)
            : base(asset, health, position)
        {
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="boss"></param>
        public Boss(Boss boss)
            :base(boss.asset,boss.health,boss.position)
        {

        }

        public virtual void Update(Vector2 playerPos, GameTime gameTime) { }

        public override void Draw(SpriteBatch _spriteBatch, bool debugOn)
        {
            _spriteBatch.Draw(
                baseSheet,          //Texture
                position,           //Position
                null,               //Source
                Color.White,        //Tint
                MathF.PI,           //Rotation
                new Vector2(64,64), //Origin
                scale,              //Scale
                SpriteEffects.None, //Effects
                0);                 //Layer
        }

        /// <summary>
        /// Set the boss back to normal
        /// </summary>
        public virtual void Reset()
        {
        }
    }
}
