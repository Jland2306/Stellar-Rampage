using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StellarRampage.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.FlexItems
{
    internal class PanelTexture : Panel
    {
        private Rectangle textureRect;
        private Texture2D texture;

        public PanelTexture(string name, SpriteFont font, Texture2D backgroundTex, Texture2D texture,
                            Rectangle rect, bool initialValue)
            : base(backgroundTex, rect,
                new TextBox(font, Rectangle.Empty, Color.White, centerText: false),
                isVerticle: false)
        {
            TextBox.Text = name;
            this.texture = texture;
            this.textureRect = rect;

            //Align text
            AlignTextCenter();
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.Draw(texture, textureRect, Color.White);
        }

        public override Rectangle Rectangle
        {
            get
            {
                return base.Rectangle;
            }
            set
            {
                base.Rectangle = value;

                // Forward layout rect to checkbox
                textureRect = new Rectangle(value.X + 380, value.Y + 25, 40, 40);
            }
        }
    }
}
