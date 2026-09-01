using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellarRampage.Miscellaneous;
using StellarRampage.GameObjects.Enemies;
using StellarRampage.Managers;

namespace StellarRampage.HelperClasses
{
    public class Sword
    {
        private Texture2D texture;
        //Flipping
        SpriteEffects effect;

        private float angle;
        private Vector2 position;
        private float swingAngle;
        private bool swinging;
        private float offset = -0.5f;
        private float scale = 1;

        //Swing fields
        private float swingMax = 0.15f;
        private float swingCurr = 0;
        private float swingReturn = 0.13f;
        private float swingAmount = 3f;
        private Tween.ObjectTween swingTween;

        //Collision
        private Vector2 origin;
        private Vector2 end;
        private bool flip;
        protected List<Enemy> enemiesHit;


        //Change the size of the sword
        public float Scale
        {
            get { return scale; }
            set {  scale = value; }
        }
        public Sword(Texture2D sword) 
        { 
            this.texture = sword;
            enemiesHit = new List<Enemy>();
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            //DebugLib.DrawLine(_spriteBatch, origin, end, 2f, Color.Red);
            if (flip)
            {
                origin = new Vector2((int)position.X + 10, (int)position.Y + 25);
                effect = SpriteEffects.FlipVertically;
                //Flip the sword to match
                _spriteBatch.Draw(
                    texture,
                    origin,
                    new Rectangle(0, 0, texture.Width, texture.Height),
                    Color.White,
                    angle - swingAngle - offset,
                    new Vector2(0, 0),
                    scale,
                    effect,
                    0);

            }
            else
            {
                origin = new Vector2((int)position.X + 20, (int)position.Y + 25);
                effect = SpriteEffects.None;
                //Flip the sword to match
                _spriteBatch.Draw(
                    texture,
                    origin,
                    new Rectangle(0, 0, texture.Width, texture.Height),
                    Color.White,
                    angle + swingAngle + offset,
                    new Vector2(0, texture.Height),
                    scale,
                    effect,
                    0);
            }


        }

        /// <summary>
        /// Updates the sword angle and position
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="pos"></param>
        public void Update(float angle, Vector2 pos, GameTime gameTime, bool flip)
        {
            //Update sword
            this.angle = angle;
            this.position = pos;
            this.flip = flip;

            //Increase sword angle
            if (swinging)
            {
                swingAngle = swingTween.currValue;
            }

            //size of sword
            float length = 65 * scale;
            float degrees = MathF.PI / 4;
            if (flip)
            {
                end = origin + new Vector2(
                (float)Math.Cos(angle - offset - swingAngle + degrees) * length,
                (float)Math.Sin(angle - offset - swingAngle + degrees) * length);
            }
            else
            {
                end = origin + new Vector2(
                (float)Math.Cos(angle + offset + swingAngle - degrees) * length,
                (float)Math.Sin(angle + offset + swingAngle - degrees) * length);
            }


            List<Enemy> enemies = Grid.Instance.GetEnemies(end);


            foreach(Enemy e in enemies)
            {
                if (CollisionDetection.Instance.LineVsPoly(origin, end, e.Hitbox))
                {
                    e.TakeDamage(10);
                    SoundManager.PlaySound("Hit", 1f);
                }
            }

            //angle failed to reset. set it back
            if(!swinging && swingTween != null && swingTween.IsCompleted && swingAngle != 0)
            {
                swingAngle = 0;
            }
        }

        /// <summary>
        /// Swings the sword
        /// </summary>
        public void Swing()
        {
            swinging = true;
            swingCurr = 0;

            //Create a new swing
            swingTween = Tween.CreateTween(0, swingAmount, swingMax, EaseType.EaseOut);
            swingTween.OnComplete = () =>
            {
                //return the swing
                swingTween = Tween.CreateTween(swingAmount, 0, swingReturn, EaseType.EaseOut);

                //Finish the swing
                swingTween.OnComplete = () =>
                {
                    swinging = false;
                };
            };
        }
    }
}
