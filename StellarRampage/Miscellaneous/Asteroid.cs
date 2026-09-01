using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.Miscellaneous
{
    internal class Asteroid
    {
        //Fields
        private Texture2D asset;
        private Vector2 position;
        private Vector2 velocity;
        private float rot;
        private float rotSpeed;
        private float scale;

        private float speed = 100f;
        private Color color;

        //Properties
        public Vector2 Position
        {
            get { return position; }
        }

        public Asteroid(Texture2D asset, Vector2 position, Vector2 velocity, float rotSpeed, float scale, Random randy) 
        { 
            this.asset = asset;
            this.position = position;
            this.velocity = velocity;
            this.rotSpeed = rotSpeed;
            this.scale = scale;

            color = new Color(
                randy.Next(256),  
                randy.Next(256),  
                randy.Next(256));
        }

        public void Update(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Update position and rotation
            position += velocity * time * speed;
            rot += rotSpeed * time;
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(
                asset,                              //Texture
                position,                           //Position
                null,                               //Rect
                Color.White,                        //Color
                rot,                                //Rotation
                new Vector2(asset.Width / 2,        //X origin
                asset.Height / 2),                  //Y origin
                scale,                              //Scale
                SpriteEffects.None,                 //Sprite effect
                0f);                                //layer
        }
    }
}
