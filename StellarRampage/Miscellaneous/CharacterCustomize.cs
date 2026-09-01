using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StellarRampage.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellarRampage.FlexItems;

namespace StellarRampage.Miscellaneous
{
    internal class CharacterCustomize
    {

        // Player customization
        private int numLayers = 5;

        //Allows draw to reference player layer
        enum Layer
        {
            Outline,
            Body,
            Backpack,
            Visor,
            Skin
        }

        //Random colors for character
        Color[] colors = new Color[]
        {
            Color.White,
            Color.Black,
            Color.Red,
            Color.Blue,
            Color.Green,
            Color.Yellow,
            Color.Purple,
            Color.Orange,
            Color.Cyan,
            Color.Magenta,
        };

        private int[] layerColorNum;
        private Rectangle playerRect;
        private Player player;
        private Texture2D asset;

        private List<Texture2D> UiAssets;
        private Rectangle holderRect;

        //Buttons
        private Texture2D button;
        private Button bodyButton;
        private Button packButton;
        private Button visorButton;
        private Button play;
        private SpriteFont font;
        private int buttonX;
        private int buttonY;
        private int buttonGap;

        public CharacterCustomize(Player player, Texture2D playerAsset, Texture2D button, SpriteFont font, List<Texture2D> UiAssets) 
        {
            asset = playerAsset;
            this.player = player;
            this.button = button;
            this.UiAssets = UiAssets;

            holderRect = new Rectangle(
                200,
                170,
                (int)(UiAssets[19].Width * 1.5),
                (int)(UiAssets[19].Height * 1.5));

            //Split the Asset into separate chunks based on number of layers
            playerRect = new Rectangle(
                0,                                  //X
                0,                                  //Y
                asset.Width / numLayers,            //Width
                asset.Height);                      //Height

            layerColorNum = new int[numLayers];
            layerColorNum[0] = 1;
            layerColorNum[1] = 5;
            layerColorNum[2] = 5;
            layerColorNum[3] = 0;
            layerColorNum[4] = 0;

            buttonX = (Game1.Width / 2);
            buttonY = 250;
            buttonGap = button.Height + 25;

            bodyButton = new Button(
                button,
                button,
                new Rectangle(
                    buttonX,
                    buttonY,
                    button.Width,
                    button.Height),
                font,
                smallTextBox: true);
            bodyButton.TextBox.Text = "BODY";

            packButton = new Button(
                button,
                button,
                new Rectangle(
                    buttonX,
                    buttonY + buttonGap,
                    button.Width,
                    button.Height),
                font,
                smallTextBox: true
            );
            packButton.TextBox.Text = "PACK";

            visorButton = new Button(
                button,
                button,
                new Rectangle(
                    buttonX,
                    buttonY + (buttonGap * 2),
                    button.Width,
                    button.Height),
                font,
                smallTextBox: true
            );
            visorButton.TextBox.Text = "VISOR";

            play = new Button(
                button,
                button,
                new Rectangle(
                    buttonX,
                    buttonY + (buttonGap * 3),
                    button.Width,
                    button.Height),
                font,
                smallTextBox: true
            );
            play.TextBox.Text = "PLAY";
        }


        /// <summary>
        /// Updates all buttons, checks for play
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="buttonPressed">Mouse down</param>
        /// <returns></returns>
        public bool Update(GameTime gameTime, bool buttonPressed)
        {
            //Update all buttons
            bodyButton.Update();
            packButton.Update();
            visorButton.Update();
            play.Update();

            if (bodyButton.IsHovering && buttonPressed) 
            {
                ChangeLayer(Layer.Body);
            }
            if (packButton.IsHovering && buttonPressed)
            {
                ChangeLayer(Layer.Backpack);
            }
            if (visorButton.IsHovering && buttonPressed)
            {
                ChangeLayer(Layer.Visor);
            }
            if (play.IsHovering && buttonPressed)
            {
                Color[] colorSelected = new Color[]
                {
                    Color.Black,
                    colors[layerColorNum[(int)Layer.Body]],
                    colors[layerColorNum[(int)Layer.Backpack]],
                    colors[layerColorNum[(int)Layer.Visor]],
                    Color.White
                };

                return true;
            }

            return false;
        }

        /// <summary>
        /// Draw player customizer
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void Draw(SpriteBatch _spriteBatch)
        {
            //Draw the player holder
            _spriteBatch.Draw(
                UiAssets[19],
                holderRect,
                Color.White);

            //Draw each layer of the player
            playerRect.X = 0;
            for (int i = 0; i < numLayers; i++)
            {
                _spriteBatch.Draw(
                    asset,
                    new Vector2(holderRect.X + 200, holderRect.Y + 100),
                    playerRect,
                    colors[layerColorNum[i]],
                    0,
                    Vector2.Zero,
                    10,
                    SpriteEffects.None,
                    0);

                //Move to the next layer
                playerRect.X += asset.Width / numLayers;
            }
            bodyButton.Draw(_spriteBatch);
            packButton.Draw(_spriteBatch);
            visorButton.Draw(_spriteBatch);
            play.Draw(_spriteBatch);
        }

        /// <summary>
        /// Increases the color to the next in the array
        /// </summary>
        /// <param name="layer"></param>
        private void ChangeLayer(Layer layer)
        {
            //Get the current color index
            int currIndex = layerColorNum[(int)layer];

            //Wrap the index around if it hit the end of the array
            if (currIndex < colors.Length - 1)
            {
                currIndex++;
            }
            else
            {
                //Wrap
                currIndex = 0;
            }

            //Assign the new color to that layer
            layerColorNum[(int)layer] = currIndex;
        }

    }
}
