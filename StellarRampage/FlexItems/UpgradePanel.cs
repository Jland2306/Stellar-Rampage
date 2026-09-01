using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellarRampage.Miscellaneous;
using Microsoft.Xna.Framework.Input;
using static System.Formats.Asn1.AsnWriter;
using StellarRampage.HelperClasses;

namespace StellarRampage.FlexItems
{
    /// <summary>
    /// Upgrade panels hold all data needed for the visual aspect of an upgrade,
    /// Inherits from button so user can select
    /// </summary>
    public class UpgradePanel : Button
    {
        //Name of upgrade
        private TextBox upgradeName;
        //The level the upgrade is at
        private TextBox description;
        private TextBox text;
        private TextBox type;

        //Upgrade to give
        private Upgrade upgrade;

        private SpriteFont m6;

        //Start and large represent the sizes the items grow after hovering,
        //The scale factor allows for the panel to feel responsive on hover.
        private Rectangle nameRectStart;
        private Rectangle nameRectLarge;
        private Rectangle assetRect;
        private Rectangle assetRectLarge;
        private Rectangle desRect;
        private Rectangle desLarge;

        private Rectangle textRect;
        private Rectangle textLarge;


        private Rectangle typeRect;
        private Rectangle typeLarge;


        //Upgrade spritesheet
        private Texture2D spriteSheet;
        //Amount to upscale the pixels
        private float spriteScale = 4;
        //Base width of 1 upgrade icon
        private int spriteWidth = 32;


        //Extra assets
        private Texture2D outline;
        private Texture2D square;
        private Texture2D filledSquare;

        private List<string> textLayout;

        //Allows spriteScale to be modified for the debug panel
        public float SpriteScale
        {
            set { spriteScale = value; }
        }
        /// <summary>
        /// Check if the panel has been assigned an upgrade
        /// </summary>
        public bool HasUpgrade
        {
            get { return upgrade != null; }
        }

        /// <summary>
        /// Sets a new upgrade for the card
        /// </summary>
        public Upgrade Upgrade
        {
            //Add an upgrade to the panel
            set
            {
                upgrade = value;
                if (upgrade != null)
                {
                    //Assign the name/level
                    ChangeText();
                }
            }
            //get the upgrade to give to player
            get { return upgrade; }
        }

        /// <summary>
        /// Create a new upgrade panel
        /// </summary>
        /// <param name="buttonTexture"></param>
        /// <param name="hoveringTexture"></param>
        /// <param name="rectangle"></param>
        /// <param name="font"></param>
        /// <param name="upgrade"></param>
        /// <param name="spriteSheet"></param>
        public UpgradePanel(Texture2D buttonTexture, Texture2D hoveringTexture, Rectangle rectangle, SpriteFont font, SpriteFont m6,
            Upgrade upgrade, Texture2D spriteSheet, Texture2D outline, Texture2D square, Texture2D filledSquare, SpriteFont nameFont)
            : base(buttonTexture, hoveringTexture, rectangle, font)
        {
            //Create a text box at the very top for the upgrade name
            upgradeName = new TextBox(
                font,                       //Font
                Rectangle.Empty,            //Rectangle
                Color.White,                //Color
                false);

            //Assign fields
            this.spriteSheet = spriteSheet;

            description = new TextBox(
                m6,
                Rectangle.Empty,
                Color.White,
                false);

            text = new TextBox(
                m6,
                Rectangle.Empty,
                Color.White,
                false);

             type = new TextBox(
                m6,
                Rectangle.Empty,
                Color.White,
                false);

            upgradeName.Font = nameFont;

            this.outline = outline;
            this.square = square;  
            this.filledSquare = filledSquare;
        }

        /// <summary>
        /// Update the panel, check for clicks, enlarge size
        /// </summary>
        public override void Update()
        {
            base.Update();
            //If hovering, increase size
            if (isHovering)
            {
                upgradeName.Rectangle = nameRectLarge;
                description.Rectangle = desLarge;
                text.Rectangle = textLarge;
                type.Rectangle = typeLarge;
            }
            //Stay normal
            else
            {
                upgradeName.Rectangle = nameRectStart;
                description.Rectangle = desRect;
                text.Rectangle = textRect;
                type.Rectangle = typeRect;
            }
        }

        /// <summary>
        /// Resize the textbox to fit as large as possible
        /// </summary>
        public void UpdateTextBox()
        {
            //This represents how large the pixels move when the panel is scaled
            int widthOffset = (int)(Width * scale - Width) / 2;
            int heightOffset = (int)(Height * scale - Height) / 2;
            int padding = 35;

            //how far text starts from left, includes padding
            int textOffset = (int)(spriteWidth * spriteScale + padding);

            //Add 10 pixel padding for right edge
            int textWidth = rectangle.Width - textOffset + 10;

            upgradeName.Rectangle = new Rectangle(
                    rectangle.X + textOffset,        //X
                    rectangle.Y + 50,       //Y
                    textWidth,    //Width
                    50);  //Height

            nameRectStart = upgradeName.Rectangle;

            Rectangle r = upgradeName.Rectangle;
            nameRectLarge = new Rectangle(
                        (int)(rectangle.X + (textOffset * scale) - widthOffset),
                        r.Y - heightOffset,
                        (int)(textWidth * scale),
                        (int)(50 * scale));


            //Creates the icon at the top left of the panel
            assetRect = new Rectangle(
                (int)(X + padding),
                (int)(Y + padding),
                (int)(spriteWidth * spriteScale),
                (int)(spriteWidth * spriteScale)
            );

            r = assetRect;


            assetRectLarge = new Rectangle(
                r.X - widthOffset,
                r.Y - heightOffset,
                (int)(r.Width * scale),
                (int)(r.Height * scale)
            );

            int yOffset = 160;
            int xOffset = 50;
            //Create a new rectangle that fills the bottom middle
            description.Rectangle = new Rectangle(
                (X + xOffset),
                Y + yOffset,
                Width - 20,
                50);

            desRect = description.Rectangle;

            //Shorten the rectangle name
            r = desRect;
            desLarge = new Rectangle(
                (int)(X + (xOffset * scale) - widthOffset),
                (int)(Y + (yOffset * scale) - heightOffset),
                (int)(r.Width * scale),
                (int)(r.Height * scale));



            //Create a new rectangle that fills the bottom middle
            text.Rectangle = new Rectangle(
                (X + xOffset),
                Y + yOffset + 70,
                Width - 100,
                220);

            textRect = text.Rectangle;

            //Shorten the rectangle name
            r = textRect;
            textLarge = new Rectangle(
                (int)(X + (xOffset * scale) - widthOffset),
                (int)(Y + ((yOffset + 70) * scale) - heightOffset),
                (int)((r.Width - 40) * scale),
                (int)(r.Height * scale));

            //Create a new rectangle that fills the bottom middle
            type.Rectangle = new Rectangle(
                (X + 165),
                Y + 100,
                Width - 20,
                40);

            typeRect = type.Rectangle;

            //Shorten the rectangle name
            r = typeRect;
            typeLarge = new Rectangle(
                (int)(X + (165 * scale) - widthOffset),
                (int)((Y + 100 * scale) - heightOffset),
                (int)(r.Width * scale),
                (int)(r.Height * scale));
        }

        /// <summary>
        /// Update text
        /// </summary>
        private void ChangeText()
        {
            upgradeName.Text = $"{upgrade.Name}";

            description.Text = upgrade.Description;

            text.Text = upgrade.Text;

            type.Text = upgrade.Type;

            if(upgrade.Type == "Attack")
            {
                type.Color = Color.PaleVioletRed;
                description.Color = Color.PaleVioletRed;
            }
            else if (upgrade.Type == "Movement")
            {
                type.Color = Color.Aqua;
                description.Color = Color.Aqua;
            }
            else if (upgrade.Type == "Utility")
            {
                type.Color = Color.LightGoldenrodYellow;
                description.Color = Color.LightGoldenrodYellow;
            }
        }

        /// <summary>
        /// Draws the panel
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void DrawUpgradeText(SpriteBatch _spriteBatch)
        {
            //This represents how large the pixels move when the panel is scaled
            int widthOffset = (int)(Width * scale - Width) / 2;
            int heightOffset = (int)(Height * scale - Height) / 2;

            //Draw the upgrade name
            if (upgradeName.HasText)
            {
                upgradeName.Draw(_spriteBatch);
            }
            //Only draw if there is a description
            if (description.HasText)
            {
                description.Draw(_spriteBatch);
            }
            //Only draw if there is a description
            if (text.HasText)
            {
                if (IsHovering)
                {
                    text.DrawWrapLarger(_spriteBatch, textLayout);
                }
                else
                {
                    textLayout = text.DrawWrapped(_spriteBatch);

                }

            }
            if (type.HasText)
            {
                type.Draw(_spriteBatch);
            }
            _spriteBatch.End();
            //Draw pixelated
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            int yOffset = 330;

            if (isHovering)
            {
                //Draw the icon
                _spriteBatch.Draw(
                    spriteSheet,
                    assetRectLarge,
                    upgrade.SourceRect,
                    Color.White
                    );


                _spriteBatch.End();
                _spriteBatch.Begin();

                _spriteBatch.Draw(
                    outline,
                    new Rectangle(
                        (int)(rectangle.X + (rectangle.Width - (outline.Width * scale)) /2),
                        (int)(Y + (yOffset * scale) - heightOffset),
                        (int)(outline.Width * scale),
                        (int)( outline.Height * scale)),
                    Color.White);



                //Draw the upgrade boxes to indidcate the level
                for (int i = 0; i < upgrade.CurrLevel; i++)
                {
                    _spriteBatch.Draw(
                        filledSquare,
                        new Rectangle(
                            (int)(X + 63 * scale - widthOffset + (i * ((square.Width / 2 + 4.1) * scale))),
                            (int)(Y + (yOffset + 36) * scale - heightOffset),
                            (int)(filledSquare.Width * scale),
                           (int)(filledSquare.Height * scale)),
                           Color.White * 75);
                }

                //Draw the upgrade boxes to indidcate the level
                for (int i = 0; i < upgrade.MaxLevel; i++)
                {
                    _spriteBatch.Draw(
                        square,
                        new Rectangle(
                            (int)(X + 45 * scale - widthOffset + (i * (square.Width * scale / 2 + 4 * scale))),
                            (int)(Y + (yOffset + 23) * scale - heightOffset),
                            (int)(square.Width * scale),
                           (int)(square.Height * scale)),
                           Color.White * 75);
                }


            }
            else
            {
                //Draw the icon
                _spriteBatch.Draw(
                    spriteSheet,
                    assetRect,
                    upgrade.SourceRect,
                    Color.White
                    );


                _spriteBatch.End();
                _spriteBatch.Begin();

                _spriteBatch.Draw(
                    outline,
                    new Rectangle(
                        rectangle.X + (rectangle.Width - outline.Width )/2,
                        rectangle.Y + yOffset,
                        outline.Width,
                        outline.Height),
                    Color.White);


                //Draw the upgrade boxes to indidcate the level
                for (int i = 0; i < upgrade.CurrLevel; i++)
                {
                    _spriteBatch.Draw(
                        filledSquare,
                        new Rectangle(
                            (int)(X + 64 + (i * (square.Width / 2 + 4))),
                            (int)(Y + yOffset + 36),
                            (int)(filledSquare.Width),
                           (int)(filledSquare.Height)),
                           Color.White * 75);
                }

                //Draw the upgrade boxes to indidcate the level
                for (int i = 0; i < upgrade.MaxLevel; i++)
                {
                    _spriteBatch.Draw(
                        square,
                        new Rectangle(
                            (int)(X + 45 + (i * (square.Width / 2 + 4))),
                            (int)(Y + yOffset + 23),
                            (int)(square.Width),
                           (int)(square.Height)),
                           Color.White * 75);
                }



            }
        }

        public void DrawUpgradeIcon(SpriteBatch _spriteBatch)
        {
            _spriteBatch.End();
            //Draw pixelated
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            if (isHovering)
            {
                //Draw the icon
                _spriteBatch.Draw(
                    spriteSheet,
                    assetRectLarge,
                    upgrade.SourceRect,
                    Color.White
                    );
            }
            else
            {
                //Draw the icon
                _spriteBatch.Draw(
                    spriteSheet,
                    assetRect,
                    upgrade.SourceRect,
                    Color.White
                    );

            }
            _spriteBatch.End();
            _spriteBatch.Begin();
        }

    }
}
