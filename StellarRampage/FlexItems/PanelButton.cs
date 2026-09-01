using Microsoft.Xna.Framework.Graphics;
using StellarRampage.HelperClasses;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StellarRampage.FlexItems
{
    public class PanelButton : Panel
    {
        //Box to check
        private CheckBox checkbox;

        public PanelButton(string name, SpriteFont font, Texture2D backgroundTex, Texture2D checkboxOn, Texture2D checkboxOff, 
                            Rectangle rect, bool initialValue, Action<bool> onToggle)
            : base(backgroundTex, rect,
                new TextBox(font, Rectangle.Empty, Color.White, centerText: false),
                isVerticle: false)
        {
            TextBox.Text = name;

            //Align text
            AlignTextCenter();


            // Create checkbox
            checkbox = new CheckBox(checkboxOn, checkboxOff, initialValue);
            checkbox.OnToggle += onToggle;

            // Add to inner flexbox
            FlexBox.Add(checkbox);
        }

        public override void Update()
        {
            base.Update();
            checkbox.Update();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
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
                checkbox.Rectangle = new Rectangle(value.X + 380, value.Y + 25, 40, 40);
            }
        }
    }
}
