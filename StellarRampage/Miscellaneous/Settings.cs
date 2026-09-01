using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using StellarRampage.HelperClasses;
using StellarRampage.FlexItems;
using StellarRampage.Managers;
using StellarRampage.Particles;
using System.IO;
using System;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework.Content;

namespace StellarRampage.Miscellaneous
{
    public class Settings
    {
        enum MenuState
        {
            Video,
            Sound,
            Controls,
        }

        //Gets the appdata folder
        private readonly string filePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "StellarRampage",
            "settings.txt"
        );
        private Game1 game;


        //Input
        MouseState mouseState;
        MouseState prevMouseState;

        //Current menu tab
        private MenuState menuState;

        //Container
        private List<Texture2D> UiAssets;

        //Settings textures
        private Texture2D panel;
        private Texture2D activeButton;
        private Texture2D hoverButton;
        private Texture2D exit;

        //Tabs
        private FlexBox tabBox;
        private Button video;
        private Button audio;
        private Button controls;
        private Button voidGap;

        //Side Buttons
        private FlexBox menuButtons;
        private Button resume;
        private Button menu;
        private Button quit;


        //Position/Scale
        private Rectangle panelRect;

        //Text font
        private SpriteFont font;

        //Screen Properties
        int width;
        int height;

        //Video Data
        private FlexBox flexBoxLeft;

        //Audio data
        public bool ShowSubtitles = true;

        private List<Texture2D> assets;

        private Panel debugToggle;


        // Store a list of pages per menu tab
        private Dictionary<MenuState, List<Panel>> menuPages;

        // Letter sprite sheet
        int spriteWidth;
        int spriteHeight;
        Rectangle wSpriteSource;
        Rectangle aSpriteSource;
        Rectangle sSpriteSource;
        Rectangle dSpriteSource;
        Rectangle escSpriteSource;
        Rectangle fSpriteSource;
        private Texture2D keyboardExtras;
        private Texture2D keyboardLetters;
        private Texture2D mouseLeftClick;
        public Settings(List<Texture2D> UiAssets, SpriteFont font, GraphicsDeviceManager _graphics, Game1 game, ContentManager Content)
        {
            this.game = game;
            assets = UiAssets;
            panel = UiAssets[3];
            activeButton = UiAssets[8];
            hoverButton = UiAssets[8];
            exit = UiAssets[4];
            this.font = font;
            width = _graphics.PreferredBackBufferWidth;
            height = _graphics.PreferredBackBufferHeight;

            this.UiAssets = UiAssets;
            menuPages = new Dictionary<MenuState, List<Panel>>();
            panelRect = CenterRectangle();

            // Load keyboard icons
            keyboardExtras = Content.Load<Texture2D>("KeyboardExtras");
            keyboardLetters = Content.Load<Texture2D>("KeyboardLetters");

            // Load mouse left click image
            mouseLeftClick = Content.Load<Texture2D>("mouseLeftClick3");

            // Letter sprite sheet
            int spriteWidth = 16;
            int spriteHeight = 16;
            int escSpriteWidth = 32;
            int escSpriteHeight = 16;
            wSpriteSource = new Rectangle(0, 0, spriteWidth, spriteHeight);
            aSpriteSource = new Rectangle(0, 0, spriteWidth, spriteHeight);
            sSpriteSource = new Rectangle(0, 0, spriteWidth, spriteHeight);
            dSpriteSource = new Rectangle(0, 0, spriteWidth, spriteHeight);
            escSpriteSource = new Rectangle(0, 0, spriteWidth, spriteHeight);
            fSpriteSource = new Rectangle(0, 0, spriteWidth, spriteHeight);

            // Index for W key
            int wSpriteIndexX = 6;
            int wSpriteIndexY = 4;
            wSpriteSource = new Rectangle(wSpriteIndexX * spriteWidth, wSpriteIndexY * spriteHeight, spriteWidth, spriteHeight);

            // Index for A key
            int aSpriteIndexX = 0;
            int aSpriteIndexY = 2;
            aSpriteSource = new Rectangle(aSpriteIndexX * spriteWidth, aSpriteIndexY * spriteHeight, spriteWidth, spriteHeight);

            // Index for S key
            int sSpriteIndexX = 2;
            int sSpriteIndexY = 4;
            sSpriteSource = new Rectangle(sSpriteIndexX * spriteWidth, sSpriteIndexY * spriteHeight, spriteWidth, spriteHeight);

            // Index for D key
            int dSpriteIndexX = 3;
            int dSpriteIndexY = 2;
            dSpriteSource = new Rectangle(dSpriteIndexX * spriteWidth, dSpriteIndexY * spriteHeight, spriteWidth, spriteHeight);

            // Index for TAB key
            int escSpriteIndexX = 2;
            int escSpriteIndexY = 0;
            escSpriteSource = new Rectangle(escSpriteIndexX * spriteWidth, escSpriteIndexY * spriteHeight, escSpriteWidth, escSpriteHeight);

            // Index for F key
            int fSpriteIndexX = 5;
            int fSpriteIndexY = 2;
            fSpriteSource = new Rectangle(fSpriteIndexX * spriteWidth, fSpriteIndexY * spriteHeight, spriteWidth, spriteHeight);

            // Load mouse left click image
            mouseLeftClick = Content.Load<Texture2D>("mouseLeftClick3");

        }

        public void CreateTabs()
        {
            //Creates the tabs
            CreateSelections();

            //Creates video panel
            CreateVideoSettings();

            //Creates audio panel
            CreateAudioSettings();

            //Controls
            CreateControlSettings();
        }

        /// <summary>
        /// Adds all active buttons to the settings page
        /// </summary>
        private void CreateSelections()
        {
            //The navbar
            tabBox = new FlexBox(new Rectangle(panelRect.X + 40, panelRect.Y + 40, panelRect.Width - 80, (panelRect.Height) / 11), 10);

            //Active tabs
            video = new Button(activeButton, hoverButton, Rectangle.Empty, font);
            audio = new Button(activeButton, hoverButton, Rectangle.Empty, font);
            controls = new Button(activeButton, hoverButton, Rectangle.Empty, font);
            //Label the tabs
            audio.TextBox.Text = "AUDIO";
            video.TextBox.Text = "VIDEO";
            controls.TextBox.Text = "CONTROLS";

            //Create placeholder button, invisible
            voidGap = new Button(null, null, Rectangle.Empty, font);

            //Add active and blank tabs
            tabBox.Add(video);
            tabBox.Add(audio);
            tabBox.Add(controls);
            tabBox.Add(voidGap);
            tabBox.Add(voidGap);
            tabBox.Add(voidGap);

            //menu holds all options for leaving/reentering game
            menuButtons = new FlexBox(
                new Rectangle(
                    100,
                    panelRect.Y + 20,
                    UiAssets[0].Width,
                    UiAssets[0].Height * 2),
                15,
                isVertical: true);

            //Create menu buttons
            menu = new Button(UiAssets[0], UiAssets[0], Rectangle.Empty, font, smallTextBox: true);
            menu.TextBox.Text = "MENU";
            quit = new Button(UiAssets[0], UiAssets[0], Rectangle.Empty, font, smallTextBox: true);
            quit.TextBox.Text = "QUIT";

            //Add the menu options
            menuButtons.Add(menu);
            menuButtons.Add(quit); 
        }

        /// <summary>
        /// Creates 4 panels in each tab
        /// </summary>
        /// <returns></returns>
        private List<Panel> CreateGridSections()
        {
            //The panels to be returned
            List<Panel> sections = new List<Panel>();

            //number of columns and rows, separated by gap. Can pass this as field eventually
            int gridCol = 1;
            int gridRows = 1;
            int sectionGap = 20;

            //Add together the amount of gaps to get the total available space,
            //In this case, theres only one; however grids bigger than 2 could exist
            int totalGapX = sectionGap * (gridCol - 1);
            int totalGapY = sectionGap * (gridRows - 1);

            //The gaps from the walls
            int paddingX = 40;
            int paddingY = 100;

            //Divide the total space among the number of panels.
            int sectionWidth = (panelRect.Width - (paddingX * 2) - totalGapX) / gridCol;
            int sectionHeight = (panelRect.Height - (paddingY * 2) - totalGapY) / gridRows;

            //Find out where panel 0,0 starts
            //Add a magic number for how far from the top the panel is
            int startX = panelRect.X + paddingX;
            int startY = panelRect.Y + paddingY + 40;

            //Iterate over the rows => 2
            for (int row = 0; row < gridRows; row++)
            {
                //Iterate over the columns => 2
                for (int col = 0; col < gridCol; col++)
                {
                    //X pos
                    int x = startX + col * (sectionWidth + sectionGap);
                    //Y pos
                    int y = startY + row * (sectionHeight + sectionGap);

                    //Creates a rectangle positioned where the panel will be
                    Rectangle sectionRect = new Rectangle(x, y, sectionWidth, sectionHeight);

                    // Create a FlexBox inside the panel for vertical stacking of settings
                    Panel section = new Panel(assets[5], sectionRect, null, isVerticle: true);

                    //If there's a title, align it
                    section.AlignTextHeader();

                    //Add the panel to the list
                    sections.Add(section);
                }
            }
            //Return the list containing the 4 panels
            return sections;
        }

        /// <summary>
        /// Creates the sections that define the video tab
        /// </summary>
        private void CreateVideoSettings()
        {
            //Creates 4 panels, each will have different settings
            List<Panel> videoSections = CreateGridSections();

            PanelButton panel = new PanelButton("Subtitles ", font, null, UiAssets[7], UiAssets[6], Rectangle.Empty, true, ToggleSubtitles);
            videoSections[0].FlexBox.Add(panel);

            PanelButton particles = new PanelButton("Particles ", font, null, UiAssets[7], UiAssets[6], Rectangle.Empty, true, ToggleParticles);
            videoSections[0].FlexBox.Add(particles);

            //Create a new settings row
            Panel empty1 = new Panel(
                null,                           //No Texture
                Rectangle.Empty,                //Rectangle Doesn't matter
                    new TextBox(                //TextBox
                        font,                   //Font
                        Rectangle.Empty,        //Rectangle Doesn't matter
                        Color.White,            //Text Color
                        centerText: false       //Left aligned
                    )
                );

            videoSections[0].FlexBox.Add(empty1);

            //Create a new settings row
            Panel empty2 = new Panel(
                null,                           //No Texture
                Rectangle.Empty,                //Rectangle Doesn't matter
                    new TextBox(                //TextBox
                        font,                   //Font
                        Rectangle.Empty,        //Rectangle Doesn't matter
                        Color.White,            //Text Color
                        centerText: false       //Left aligned
                    )
                );

            videoSections[0].FlexBox.Add(empty2);
            //Create a new settings row
            Panel empty3 = new Panel(
                null,                           //No Texture
                Rectangle.Empty,                //Rectangle Doesn't matter
                    new TextBox(                //TextBox
                        font,                   //Font
                        Rectangle.Empty,        //Rectangle Doesn't matter
                        Color.White,            //Text Color
                        centerText: false       //Left aligned
                    )
                );

            videoSections[0].FlexBox.Add(empty3);

            //Add the new page to the dictionary
            menuPages[MenuState.Video] = videoSections;
        }

        /// <summary>
        /// Creates the sections that define the video tab
        /// </summary>
        private void CreateControlSettings()
        {
            //Creates 4 panels, each will have different settings
            List<Panel> controlSections = CreateGridSections();

            PanelButton walk = new PanelButton("Move ", font, null, null, null, Rectangle.Empty, true, null);
            controlSections[0].FlexBox.Add(walk);

            PanelButton escape = new PanelButton("Settings", font, null, null, null, Rectangle.Empty, true, null);
            controlSections[0].FlexBox.Add(escape);

            PanelButton shoot = new PanelButton("Shoot ", font, null, null, null, Rectangle.Empty, true, null);
            controlSections[0].FlexBox.Add(shoot);


            PanelButton full = new PanelButton("Fullscreen ", font, null, null, null, Rectangle.Empty, true, null);
            controlSections[0].FlexBox.Add(full);

            //Add the new page to the dictionary
            menuPages[MenuState.Controls] = controlSections;
        }
        private void ControlSettings(SpriteBatch _spriteBatch)
        {
            int xOff = 220;
            int yOff = 250;

            // Draw W icon for controls
            _spriteBatch.Draw(
                keyboardLetters,
                new Rectangle(880 + xOff, 50 + yOff, 100, 100),
                wSpriteSource,
                Color.White);

            // Draw A icon for controls
            _spriteBatch.Draw(
                keyboardLetters,
                new Rectangle(790 + xOff, 130 + yOff, 100, 100),
                aSpriteSource,
                Color.White);

            // Draw S icon for controls
            _spriteBatch.Draw(
                keyboardLetters,
                new Rectangle(880 + xOff, 130 + yOff, 100, 100),
                sSpriteSource,
                Color.White);

            // Draw D icon for controls
            _spriteBatch.Draw(
                keyboardLetters,
                new Rectangle(970 + xOff, 130 + yOff, 100, 100),
                dSpriteSource,
                Color.White);

            // Draw ESC icon for controls
            _spriteBatch.Draw(
                keyboardExtras,
                new Rectangle(1155, 490, 150, 100),
                escSpriteSource,
                Color.White);

                        // Draw mouse icon
            _spriteBatch.Draw(
                mouseLeftClick,
                new Rectangle(1090, 590, 80, 120),
                Color.White);

            // Draw F icon for controls
            _spriteBatch.Draw(
                keyboardLetters,
                new Rectangle(1310, 710, 100, 100),
                fSpriteSource,
                Color.White);
        }
        /// <summary>
        /// Creates the sections that define the audio tab
        /// </summary>
        private void CreateAudioSettings()
        {
            // Creates 4 panels, each will have different settings
            List<Panel> audioSections = CreateGridSections();

            SliderPanel volumeSlider = new SliderPanel(
                "Master Volume",
                font,
                null,
                UiAssets[20],
                UiAssets[21],
                new Rectangle(100, 100, 300, 60),
                0.5f,
                SoundManager.SetVolume
                );

            volumeSlider.Value = SoundManager.MasterVolume;
            audioSections[0].FlexBox.Add(volumeSlider);

            

            SliderPanel musicVolume = new SliderPanel(
                "MusicVolume",
                font,
                null,
                UiAssets[20],
                UiAssets[21],
                new Rectangle(100, 100, 300, 60),
                0.5f,
                SoundManager.SetMusicVolume
                );
            musicVolume.Value = SoundManager.MusicVolume;
            audioSections[0].FlexBox.Add(musicVolume);


            SliderPanel soundSlider = new SliderPanel(
                "Effects Volume",
                font,
                null,
                UiAssets[20],
                UiAssets[21],
                new Rectangle(100, 100, 300, 60),
                0.5f,
                SoundManager.SetEffectsVolume
                );
            soundSlider.Value = SoundManager.SoundVolume;
            audioSections[0].FlexBox.Add(soundSlider);


            //Create a new settings row
            Panel empty1 = new Panel(
                null,                           //No Texture
                Rectangle.Empty,                //Rectangle Doesn't matter
                    new TextBox(                //TextBox
                        font,                   //Font
                        Rectangle.Empty,        //Rectangle Doesn't matter
                        Color.White,            //Text Color
                        centerText: false       //Left aligned
                    )
                );

            audioSections[0].FlexBox.Add(empty1);

            //Create a new settings row
            Panel empty2 = new Panel(
                null,                           //No Texture
                Rectangle.Empty,                //Rectangle Doesn't matter
                    new TextBox(                //TextBox
                        font,                   //Font
                        Rectangle.Empty,        //Rectangle Doesn't matter
                        Color.White,            //Text Color
                        centerText: false       //Left aligned
                    )
                );

            audioSections[0].FlexBox.Add(empty2);
            //Create a new settings row
            Panel empty3 = new Panel(
                null,                           //No Texture
                Rectangle.Empty,                //Rectangle Doesn't matter
                    new TextBox(                //TextBox
                        font,                   //Font
                        Rectangle.Empty,        //Rectangle Doesn't matter
                        Color.White,            //Text Color
                        centerText: false       //Left aligned
                    )
                );

            audioSections[0].FlexBox.Add(empty3);
            // Add the new page to the dictionary
            menuPages[MenuState.Sound] = audioSections;
        }

        /// <summary>
        /// Update all settings visible
        /// </summary>
        public void Update()
        {
            mouseState = Mouse.GetState();

            //Update all tabs
            tabBox.Update();
            menuButtons.Update();

            //Check for state change
            if (quit.IsHovering && SingleMousePress())
            {
                Save();
                game.Exit();
            }
            if (menu.IsHovering && SingleMousePress())
            {
                game.EnterMenu();
            }

            // Switch between menu tabs
            if (video.IsHovering && SingleMousePress())
            {
                menuState = MenuState.Video;
            }
            else if (audio.IsHovering && SingleMousePress())
            {
                menuState = MenuState.Sound;
            }
            else if (controls.IsHovering && SingleMousePress())
            {
                menuState = MenuState.Controls;
            }


            // Update the active settings page
            switch (menuState)
            {
                //Video
                case MenuState.Video:
                    foreach (FlexItem p in menuPages[MenuState.Video])
                    {
                        p.Update();
                    }
                    break;
                //Audio
                case MenuState.Sound:
                    foreach (Panel p in menuPages[MenuState.Sound])
                    {
                        p.FlexBox.Update();
                    }
                    break;
                case MenuState.Controls:
                    foreach (Panel p in menuPages[MenuState.Controls])
                    {
                        p.FlexBox.Update();
                    }
                    break;
            }
            //Set the previous input
            prevMouseState = mouseState;
        }

        /// <summary>
        /// Draws the settings menu
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void Draw(SpriteBatch _spriteBatch)
        {
            //Draw the large settings panel
            _spriteBatch.Draw(panel, panelRect, Color.White);

            //Draw menu options
            tabBox.Draw(_spriteBatch);
            menuButtons.Draw(_spriteBatch);

            //Draw the active panel
            switch (menuState)
            {
                case MenuState.Video:
                    foreach (FlexItem p in menuPages[MenuState.Video])
                        p.Draw(_spriteBatch);
                    break;
                case MenuState.Sound:
                    foreach (Panel p in menuPages[MenuState.Sound])
                        p.Draw(_spriteBatch);
                    break;
                case MenuState.Controls:
                    foreach (Panel p in menuPages[MenuState.Controls])
                    {
                        p.Draw(_spriteBatch);
                        ControlSettings(_spriteBatch);
                    }

                    break;
            }
        }

        /// <summary>
        /// Saves the settings
        /// </summary>
        public void Save()
        {
            StreamWriter writer = null!;
            try
            {
                //Creates a folder at the appdata level
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                //Creates a new writer using the filename provided
                writer = new StreamWriter(filePath);

                //Write the sound settings
                writer.Write("SOUND "+ "|");
                writer.Write(SoundManager.MasterVolume +"|");
                writer.Write(SoundManager.MusicVolume + "|");
                writer.Write(SoundManager.SoundVolume);
            }

            catch (Exception e)
            {
                Console.WriteLine("There was an error writing to this file: " + e);
            }

            //if the writer was created properly, close it
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                }
            }
        }

        /// <summary>
        /// Takes a file and copies the text contents to fill the game board
        /// </summary>
        public void Load()
        {
            //No appdata exists
            if (!File.Exists(filePath))
            {
                return;
            }
            //This list will be used to keep track of each line, then using the index of data
            //to assign the proper data
            List<string> lineList = new List<string>();

            //Instantiates the reader outside so it will be available throughout the scope
            StreamReader reader = null;
            try
            {
                //Creates the streamReader object by going to the provided file
                reader = new StreamReader(filePath);

                string line = "";

                while ((line = reader.ReadLine()) != null)
                {
                    lineList.Add(line);
                }

                string[] splitData;
                for (int i = 0; i < lineList.Count; i++)
                {
                    splitData = lineList[i].Split('|');

                    //Sound
                    if (i == 0)
                    {
                        SoundManager.MasterVolume = float.Parse(splitData[1]);
                        SoundManager.MusicVolume = float.Parse(splitData[2]);
                        SoundManager.SoundVolume = float.Parse(splitData[3]);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("There was an error reading the file: " + e);
            }

            //If the reader was instantiated properly, close it
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
        }

        /// <summary>
        /// Center the panel
        /// </summary>
        public Rectangle CenterRectangle()
        {
            //Creates a centered panel
            return new Rectangle(
                2 * width / 3 - panel.Width / 2,
                height / 2 - panel.Height / 2,
                panel.Width,
                panel.Height);

        }

        /// <summary>
        /// Checks if the mouse was pressed once
        /// </summary>
        /// <returns></returns>
        private bool SingleMousePress()
        {
            //Mouse button was just released
            return mouseState.LeftButton == ButtonState.Released &&
                    prevMouseState.LeftButton == ButtonState.Pressed;
        }
        
        /// <summary>
        /// Turn subtitles off/on
        /// </summary>
        /// <param name="value"></param>
        public void ToggleSubtitles(bool value)
        {
            Game1.Subtitles.Enabled = value;
        }

        /// <summary>
        /// Toggle the particles on/off
        /// </summary>
        /// <param name="value"></param>
        public void ToggleParticles(bool value)
        {
            ParticleSystem.DrawParticles = value;
        }
    }

}
