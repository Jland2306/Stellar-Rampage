using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StellarRampage.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StellarRampage.FlexItems
{
    internal class SliderPanel : Panel
    {
        //Visual
        private Slider slider;
        private Rectangle rect;
        private Texture2D fillTexture;
        private Texture2D backTexture;

        //Backend
        private Action<float> onChange;
        private bool isDragging;
        private float value;

        private MouseState prevMState;

        //Set the starting values
        public float Value
        {
            set { this.value = value; }
        }

        public SliderPanel(string name, SpriteFont font, Texture2D backgroundTex, Texture2D backTexture, Texture2D fillTexture,
                            Rectangle rect, float initialValue, Action<float> onChange)
            : base(backgroundTex, rect,
                new TextBox(font, Rectangle.Empty, Color.White, centerText: false),
                isVerticle: false)
        {
            //Assign the name
            TextBox.Text = name;
            this.onChange = onChange;
            this.fillTexture = fillTexture;
            this.backTexture = backTexture;

            //Align text
            AlignTextCenter();

            //the value to start at. Shold not go above 1
            value = Math.Clamp(initialValue, 0 , 1);

            //create the slider
            slider = new Slider(fillTexture, rect.Location.ToVector2(), rect.Width, rect.Height, 1);
        }

        public override void Update()
        {
            base.Update();

            MouseState mState = Mouse.GetState();

            //Check if user is dragging. Should only start if not already dragging, means use can only change one at a time
            if (mState.LeftButton == ButtonState.Pressed && prevMState.LeftButton == ButtonState.Released && rect.Contains(mState.Position))
            {
                isDragging = true;
            }
            //Stopped dragging
            else if(mState.LeftButton == ButtonState.Released)
            {
                isDragging = false;
            }

            //If player is dragging, change the value
            if (isDragging)
            {
                //get the percent on the track, clamp to max of 100%
                value = Math.Clamp(((mState.X - rect.X) / (float)rect.Width), 0, 1);
                slider.SetPercent(value);
                onChange?.Invoke(value);
            }

            prevMState = mState;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            spriteBatch.Draw(backTexture, rect, new Color(Color.Black, 50));
            slider.Draw(spriteBatch);
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

                //Left and right padding
                int padding = 300;
                
                //Update the rectangle to have left padding
                rect = new Rectangle(
                    value.X + 500,
                    value.Y + value.Height / 2,
                    value.Width - 711,
                    23
                );

                //Recreate the slider at new position
                slider = new Slider(fillTexture, rect.Location.ToVector2(), rect.Width, rect.Height, 1);
                slider.SetPercent(this.value);
            }
        }
    }
}
