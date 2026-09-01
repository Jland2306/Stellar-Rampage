using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.FlexItems
{
    internal class TexturedButton : Button
    {
        private Texture2D extraTexture;
        public TexturedButton(Texture2D buttonTexture, Texture2D hoveringTexture, Rectangle rectangle, SpriteFont font, Texture2D extraTexture, bool smallTextBox = false)
            : base(buttonTexture, hoveringTexture, rectangle, font, smallTextBox)
        {
            this.extraTexture = extraTexture;
        }

        public override void Update()
        {
            base.Update();
        }
        public override void Draw(SpriteBatch _spriteBatch)
        {
            base.Draw(_spriteBatch);

            //users mouse is on button
            if (isHovering)
            {
                //Scales the button up when being hovered
                _spriteBatch.Draw
                (extraTexture, //Texture
                new Rectangle(
                    X - (int)(Width * scale - Width) / 2 + 10,
                    (Y - (int)(Height * scale - Height) / 2) - 20,
                    (int)(Width * scale),
                    (int)(scale * Height)),
                Color.White);

            }

            else
            {
                //Draws a non-highlighted button
                _spriteBatch.Draw
                (extraTexture,   //Texture
                new Rectangle(
                    X + 10,
                    Y - 20,
                    Width,Height),  
                Color.White);     //Tint
            }



        }

    }
}
