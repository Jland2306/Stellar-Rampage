using Microsoft.Xna.Framework.Graphics;
using StellarRampage.FlexItems;
using Microsoft.Xna.Framework;
using StellarRampage.GameObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using StellarRampage.HelperClasses;
using StellarRampage.Managers;

namespace StellarRampage.Miscellaneous
{
    internal class CharacterSelect
    {

        //---------------------------------------------------------------------
        //                           FIELDS
        //---------------------------------------------------------------------

        //File to use
        private string characterFile = Path.Combine("TextFiles", "Character.txt");

        //Sprites
        private Texture2D playerSprites;

        //Controls
        private Rectangle bottomBar = new Rectangle(0, Game1.Height - 70, Game1.Width, 43);
        private Rectangle screen = new Rectangle(0, 0, Game1.Width, Game1.Height);
        private Rectangle infoPanel = new Rectangle(400, 400, 2000, 350);
        private Rectangle selectionPanel;
        private Texture2D bottomGradient;
        private Texture2D smallGradient;
        private Texture2D pixel;

        //Assets for selecting
        private List<Texture2D> UiAssets;
        private Player player;
        private Texture2D button;
        private SpriteFont font;
        private Texture2D dash;
        private int spriteWidth = 32;

        private TexturedButton[] buttons;
        private int numButtons = 5;
        private int currIndex;

        private Button selectButton;
        private Button confirm;
        private Button back;

        //Characters
        private Vector2 charPos = new Vector2(150,200);
        private float scale = 15;
        Texture2D sword;
        Texture2D swordWave;

        //Text
        private TextBox title;
        private TextBox name;
        private TextBox description;
        private TextBox ability;

        //State
        private Game1 game;


        private Dictionary<string, Color> Colors = new()
        {
            { "Red", Color.PaleVioletRed },
            { "Pink", Color.Pink },
            { "Yellow", Color.LightGoldenrodYellow },
            { "Blue", Color.Aqua},
            { "Purple", Color.Violet},

        };

        private List<PlayerType> players = new List<PlayerType>();
        //---------------------------------------------------------------------
        //                          CONSTRUCTOR
        //---------------------------------------------------------------------
        public CharacterSelect(List<Texture2D> UiAssets, Player player, SpriteFont font, Texture2D button, ContentManager content, Texture2D pixel, Game1 game)
        {
            //Assign fields
            this.UiAssets = UiAssets;
            this.player = player;
            this.button = button;
            this.font = font;
            this.pixel = pixel;
            this.game = game;

            int buttonY = 850;
            int buttonWidth = 130;
            int buttonHeight = 170;
            int padding = 10;
            int space = 10;

            //Center the X, this will adjust with screen
            int buttonX = ((Game1.Width - (numButtons * buttonWidth + (space * numButtons - 1) + padding)) / 2) + 200;

            buttons = new TexturedButton[numButtons];

            ReadFile(content);

            //Make the panel match the number of buttons
            selectionPanel = new Rectangle(buttonX - padding , buttonY - padding, numButtons * buttonWidth + (space * numButtons - 1) + padding, buttonHeight - padding * 2);

            //Create each player
            for(int i = 0; i < numButtons; i++)
            {
                //Create 5 buttons for each player
                buttons[i] = new TexturedButton(smallGradient, smallGradient, new Rectangle(buttonX + ((buttonWidth + space) * i), buttonY, buttonWidth, buttonHeight), font, players[i].Asset);
            }

            title = new TextBox(font, new Rectangle(20, 20, 400, 90), Color.White);
            title.Text = "Character Select";

            confirm = new Button(null, null, new Rectangle(Game1.Width - 170, bottomBar.Y - 10, 150, 70), font);
            confirm.TextBox.Text = "CONFIRM";

            back = new Button(null, null, new Rectangle(40, bottomBar.Y , 150, 50), font);
            back.TextBox.Text = "BACK";

            description = new TextBox(font, new Rectangle(860, 550, 600, 500), Color.White);
            ability = new TextBox(font, new Rectangle(860, 460, 700, 60), Color.Lime, false);

            name = new TextBox(font, new Rectangle(760, 75, 800, 300), Color.White);
        }


        //---------------------------------------------------------------------
        //                          READ FILE
        //---------------------------------------------------------------------
        // <summary>
        /// Open the file and add the character choices
        /// </summary>
        private void ReadFile(ContentManager content)
        {
            try
            {
                StreamReader reader = new StreamReader(characterFile);

                // Get string variables ready for file lines being split!
                string line = "";
                string[] splitData = null;

                while ((line = reader.ReadLine()) != null)
                {
                    //Ignore any line that starts with a slash or dash
                    if (!(line[0] == '/' || line[0] == '-'))
                    {
                        //File data
                        if (line[0] == '_')
                        {
                            //Split the data using a bar as a separator
                            splitData = line.Split('|');

                            //Load the first line skipping the first char
                            playerSprites = content.Load<Texture2D>(splitData[0][1..^1]);

                            bottomGradient = content.Load<Texture2D>(splitData[1][1..]);

                            smallGradient = content.Load<Texture2D>(splitData[2].Trim());

                            dash = content.Load<Texture2D>(splitData[3].Trim());

                            sword = content.Load<Texture2D>(splitData[4].Trim());
                        }
                        //Data
                        else
                        {
                            //Split the data using a bar as a separator
                            splitData = line.Split('|');

                            //Location of sprite cord
                            Texture2D asset = content.Load<Texture2D>("Player/" + splitData[1].Trim());
                            Texture2D sheet = content.Load<Texture2D>("Player/" + splitData[3].Trim());
                            SpriteFont font = content.Load<SpriteFont>("Fonts/" + splitData[5].Trim());
                            //Create a new player type
                            players.Add(new PlayerType(asset, splitData[0].Trim(), 100, 1, Colors[splitData[1].Trim()], sheet, splitData[2].Trim(), splitData[4].Trim(), font));
                        }

                    }
                }


                // Close the stream
                reader.Close();
            }
            //File error
            catch
            {
                System.Diagnostics.Debug.WriteLine("FILE-READING ERROR!");
            }
        }

        //---------------------------------------------------------------------
        //                          METHODS
        //---------------------------------------------------------------------


        /// <summary>
        /// Draw player customizer
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void Draw(SpriteBatch _spriteBatch)
        {
            _spriteBatch.Draw(
                pixel,
                infoPanel,
                new Color(players[currIndex].color, 70));

            //Draw the character
            
            _spriteBatch.Draw(
                players[currIndex].Asset,
                charPos,
                new Rectangle(0, 0, spriteWidth, playerSprites.Height),
                Color.White,
                0,
                Vector2.Zero,
                scale,
                SpriteEffects.None,
                0);


            //players[currIndex].Draw(_spriteBatch); 
            //Bottom bar
            _spriteBatch.Draw(
                pixel,
                bottomBar,
                Color.Black);

            //Gradients
            _spriteBatch.Draw(
                bottomGradient,
                screen,
                new Color(Color.White, 200));


            _spriteBatch.Draw(
                pixel,
                selectionPanel,
                new Color(Color.Black, 100));

            

            title.Draw(_spriteBatch);
            confirm.Draw(_spriteBatch);
            back.Draw(_spriteBatch);
            

            for (int i = 0; i < buttons.Length; i++)
            {
                //Highlight the selected player
                if(i == currIndex)
                {
                    buttons[i].ButtonColor = players[currIndex].color;
                }
                else
                {
                    buttons[i].ButtonColor = Color.Black;
                }

                buttons[i].Draw(_spriteBatch);
            }

            name.Font = players[3].Font;
            //Switch to the latest color and name
            name.Color = players[currIndex].color;
            name.Text = players[currIndex].Name;
            //name.Font = players[currIndex].Font;
            name.Draw(_spriteBatch);

            description.Text = players[currIndex].Description;
            description.DrawWrapped(_spriteBatch);

            ability.Text = players[currIndex].Ability;
            ability.Color = players[currIndex].color;
            ability.Draw(_spriteBatch);
        }


        /// <summary>
        /// Updates the buttons/animations
        /// </summary>
        public bool Update(GameTime gameTime, bool mousePress)
        {
            title.Update();
            confirm.Update();
            back.Update();

            //Start the game
            if (confirm.IsHovering && mousePress)
            {
                player.SetNewSprite(players[currIndex].Asset);
                return true;
            }

            //Leave the game
            if (back.IsHovering && mousePress)
            {
                game.EnterMenu();
            }

            //Update each button
            for (int i = 0; i< buttons.Length; i++)
            {
                buttons[i].Update();

                //If user pressed the button, switch to that character
                if (buttons[i].IsHovering && mousePress)
                {
                    //Move the index
                    currIndex = i;
                }
            }

            //Update the animations
            players[currIndex].Update(gameTime);

            return false;
        }

        public void GiveUpgrades()
        {
            switch (currIndex)
            {
                case 0:
                    //Increase health by 50
                    player.MaxHealth += 50;
                    player.ResetHealth();
                    break;
                case 1:
                    //2 drones
                    UpgradeManager.Instance.GiveUpgrade(UpgradeManager.Instance.AvailableUpgrades[5]);
                    UpgradeManager.Instance.GiveUpgrade(UpgradeManager.Instance.AvailableUpgrades[5]);
                    break;
                case 2:
                    //100 boost
                    UpgradeManager.Instance.GiveUpgrade(UpgradeManager.Instance.AvailableUpgrades[7]);
                    UpgradeManager.Instance.GiveUpgrade(UpgradeManager.Instance.AvailableUpgrades[7]);
                    break;
                case 3:
                    //Give buffy a sword
                    //UpgradeManager.Instance.GiveUpgrade(UpgradeManager.Instance.AvailableUpgrades[3]);
                    //UpgradeManager.Instance.GiveUpgrade(UpgradeManager.Instance.AvailableUpgrades[3]);
                    player.CreateSword(sword);
                    break;
                case 4:
                    //Give 5 Shields
                    UpgradeManager.Instance.GiveShield();
                    UpgradeManager.Instance.GiveShield();
                    UpgradeManager.Instance.GiveShield();
                    break;
            }
        }
    }

}
