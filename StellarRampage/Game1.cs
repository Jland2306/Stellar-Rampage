// Jordan Landvesicht, Alexander Pooler, Mat Wargacki, Nick Branscombe
// Stellar Rampage 106 Game

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StellarRampage.EnemyDrops;
using StellarRampage.FlexItems;
using StellarRampage.GameObjects;
using StellarRampage.HelperClasses;
using StellarRampage.Managers;
using StellarRampage.Miscellaneous;
using StellarRampage.Particles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace StellarRampage
{
    // Enum for different game states
    public enum GameMode
    { 
        Menu,
        Game,
        Pause,
        Creator,
        GameOverTransition,
        GameOver,
        Win,
        Upgrade,
        Credits,
        Settings
    }

    public class Game1 : Game
    {
        #region VARIABLES
        //---------------------------------------------------------------------
        //                          VARIABLES
        //---------------------------------------------------------------------

        // Base monogame variables
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Debug
        private static bool debugOn;
        private bool pauseTime;
        private SpriteFont arial20;

        // fonts
        private SpriteFont lucidaConsole120;
        private SpriteFont ocra100;
        private SpriteFont arial100;
        private SpriteFont toreks60;
        private SpriteFont toreks80;

        // Cursor
        private Texture2D cursor;

        // Extra Assets
        private static Texture2D pixel;
        private Texture2D pixelTexture;
        private Texture2D menuBackground;
        private Texture2D asteroid1;
        private Texture2D asteroid2;
        private Texture2D keyboardExtras;
        private Texture2D keyboardLetters;
        private Texture2D mouseLeftClick;
        private AsteroidSpawner asteroids;

        //Gradients
        private Texture2D leftGradient;

        // Player data
        private Player player;
        private Texture2D playerSprite;
        private Texture2D bodySprite;
        private Texture2D originalPlayer;
        private Texture2D gun;
        private Texture2D bullet;
        private Texture2D wave;
        private Texture2D orbitalAsset;
        private Texture2D orbitalTop;
        private Texture2D shield;
        private Texture2D satellite;

        //Matrix to center player
        private static Camera camera;
        private static int width;
        private static int height;
        public static int Width
        {
            get { return width; }
        }

        public static int Height
        {
            get { return height; }
        }

        public static Texture2D Pixel
        {
            get { return pixel; }
        }

        public static Camera Cam
        {
            get { return camera; }
        }

        public static bool IsDebugging
        {
            get { return debugOn; }
        }

        public static SubtitleManager Subtitles
        {
            get { return subtitles; }
        }

        //Do not call certain methods if trying to hit button instead
        public static bool HoveringButton;


        //This will disable certain functions of the game, such as camera
        public static bool InBoss = false;

        //Input
        private KeyboardState kbState;
        private KeyboardState prevKbState;  
        private MouseState mouseState;
        private MouseState prevMouseState;
        private bool anyKeyPressed;

        // Initial game state
        private static GameMode gameMode = GameMode.Menu;

        public static GameMode GameMode
        {
            get { return gameMode; }
            set {  gameMode = value; }
        }

        // Game timer
        private double timer;

        //UI Assets
        private List<Texture2D> UiAssets = new List<Texture2D>();
        private Texture2D buttonNormal;         //0
        private Texture2D buttonHover;          //1
        private Texture2D upgradeCard;          //2
        private Texture2D largePanel;           //3
        private Texture2D iconButton;           //4
        private Texture2D buttonPanel;          //5
        private Texture2D checkBoxFalse;        //6
        private Texture2D checkBoxTrue;         //7
        private Texture2D buttonAlt;            //8
        private Texture2D buttonAltHover;       //9
        private Texture2D squareCard;           //10 
        private Texture2D squareCardAlt;        //11
        private Texture2D healthBar;            //12
        private Texture2D health;               //13
        private Texture2D boostBar;             //14
        private Texture2D boost;                //15
        private Texture2D xpBar;                //16
        private Texture2D playerIcon;           //17
        private Texture2D timeHolder;           //18
        private Texture2D playerHolder;         //19
        private Texture2D sliderFront;          //20
        private Texture2D sliderBack;           //21
        private Texture2D HUD;                  //22
        private Texture2D upgradeOutline;       //23
        private Texture2D upgradeSquare;        //24
        private Texture2D squareFilled;         //25
        private Texture2D xpBarTop;             //26
        
        // Game over buttons
        private Texture2D RedButtonNormal;      
        private Texture2D RedButtonHover;       
        private Texture2D RedTitle;             

        // Button fields
        private Button playButton;
        private Button controlsButton;
        private Button creditsButton;
        private Button quitButton;
        private Button mainMenuButton;
        private Button gameOverQuitButton;
        private Button godModeButton;
        private Button spawnBoss;
        private Button pauseTimerButton;
        private Button spawnCrusier;
        private Button winMenuButton;
        private Button winQuitButton;

        // Background Assets
        private Texture2D spaceBackground;
        private Texture2D spaceMidground1;
        private Texture2D spaceMidground2;
        private Texture2D staticMenuBackground;
        private Texture2D dynamicMenuBackground1;
        private Texture2D dynamicMenuBackground2;
        private Texture2D staticGameOverBackground;
        private Texture2D staticControlsBackground;
        private Texture2D dynamicControlsBackground1;
        private Texture2D dynamicControlsBackground2;
        private Texture2D staticCreditsBackground;
        private Texture2D dynamicCreditsBackground1;
        private Texture2D dynamicCreditsBackground2;
        private Texture2D staticWinBackground;
        private Texture2D dynamicWinBackground1;
        private Texture2D dynamicWinBackground2;

        //Upgrades 
        private Texture2D blueUpgrades;
        private Texture2D frames;

        // Letter sprite sheet
        int spriteWidth;
        int spriteHeight;
        Rectangle wSpriteSource;
        Rectangle aSpriteSource;
        Rectangle sSpriteSource;
        Rectangle dSpriteSource;
        Rectangle escSpriteSource;
        Rectangle fSpriteSource;

        // Title card
        private Texture2D titleMenu;

        // Settings Menu
        private Settings settings;
        private bool inSettings;
        bool isFullscreen = false;
        static SubtitleManager subtitles;

        // Offset for menu paralax
        private Vector2 dynamicOffSet1;
        private Vector2 dynamicOffSet2;

        // Switch interval for menu color
        private double elapsedTime;
        private Color textColor;

        // Animated planet
        private AnimatedSprite planet;
        private Texture2D planetTexture;
        private AnimatedSprite menuPlanet;
        private Texture2D menuPlanetTexture;
        private AnimatedSprite blackHole;
        private Texture2D blackHoleTexture;
        private AnimatedSprite earth;
        private Texture2D earthTexture;

        // Fields for menu character
        private Vector2 characterPos1;
        private Vector2 characterPos2;
        private Vector2 characterPos3;
        private float characterRotation;
        private float characterRotation2;
        private float characterRotation3;
        private float characterSpeed;
        private float rotationSpeed;
        private float animationTimer1;
        private float animationTimer2;
        private float animationTimer3;
        private const float AnimationLoopDuration1 = 500f;
        private const float AnimationLoopDuration2 = 500f;
        private Vector2 startPos1;
        private Vector2 endPos1;
        private Vector2 startPos2;
        private Vector2 endPos2;
        private Vector2 startPos3;
        private Vector2 endPos3;
        private Random randy = new Random();
        private Color menuColor;
        private Color menuColor2;
        private Color menuColor3;

        //Random colors for menu character
        Color[] colorPalette = new Color[]
        {
            Color.Red,
            Color.Blue,
            Color.Green,
            Color.Yellow,
            Color.Purple,
            Color.Orange,
            Color.Cyan,
            Color.Magenta
        };

        private SpriteFont m6;

        //Creator
        CharacterSelect selector;

        // Title font
        private SpriteFont titleFont;

        // List to hold credits
        private List<string> creditsLines;

        // Position for scrolling in the y direction
        private float scrollY;

        // Value for scrolling speed
        private float scrollSpeed;

        private float impactTime = 1.0f;
        private float fallDelay = 0.3f;
        private float currDelay;
        private float fallDuration = 2.0f;
        private bool transitionOver;
        private Tween.ColorTween colorTween;
        private Tween.ObjectTween rotateTween;
        private Tween.ObjectTween scaleTween;
        private Tween.ObjectTween fallTween;

        //Drops
        private DropManager dropManager;
        private ShockwaveManager shockManager;
        private Texture2D ring;

        // Timer text box
        private TextBox timerText;

        // new characters
        private Texture2D buddy;
        private Texture2D buggy;
        private Texture2D benjy;
        private Texture2D buffy;
        private Texture2D bessy;
        private Texture2D[] playerSprites;
        private Texture2D currentPlayerSprite1;
        private Texture2D currentPlayerSprite2;
        private Texture2D currentPlayerSprite3;
        
        // Random object
        Random random;
        #endregion

        #region GAME1
        //---------------------------------------------------------------------
        //                          GAME1
        //---------------------------------------------------------------------
        public Game1() 
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = false;

            width = _graphics.PreferredBackBufferWidth;
            height = _graphics.PreferredBackBufferHeight;
        }
        #endregion

        #region INITIALIZE
        //---------------------------------------------------------------------
        //                          INITIALIZE
        //---------------------------------------------------------------------
        protected override void Initialize()
        {
            // Code goes between asterisks
            //*****************************************************************

            //Creates a 1x1 pixel that can be used to draw border boxes/grid
            pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });

            //Create a new camera with a smooth of 0.1.
            //Smooth will create a more dynamic camera.
            // smooth of 1 = instant, 0 = no center
            camera = new Camera(width,height, 0.05f);

            // initialize timer
            timer = 0;

            // initialize menu character animation fields
            characterPos1 = new Vector2(0, GraphicsDevice.Viewport.Height / 2);
            characterPos2 = new Vector2(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height / 2 + 200);
            characterPos3 = new Vector2(0, GraphicsDevice.Viewport.Height / 2 - 400);
            characterRotation = 0f;
            characterRotation2 = 0f;
            characterSpeed = 2f;
            rotationSpeed = 0.0035f;
            animationTimer1 = 0f;
            animationTimer2 = 0f;
            animationTimer3 = 0f;

            // initialze any key pressed
            anyKeyPressed = false;

            pauseTime = false;

            // Scroll speed
            scrollSpeed = 60f;
            //*****************************************************************

            base.Initialize();
        }
        #endregion

        #region LOAD
        //---------------------------------------------------------------------
        //                          LOAD
        //---------------------------------------------------------------------
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Code goes between asterisks
            //*****************************************************************


            //Load the audio manager
            SoundManager.LoadContent(Content);

            //TEST FONT
            titleFont = Content.Load<SpriteFont>("TestFont");
            //Loads debug font and initiates grid
            arial20 = Content.Load<SpriteFont>("arial-20");

            // Cursor
            cursor = Content.Load<Texture2D>("Cursor");

            // initialize Enemy Manager
            EnemyManager.Instance.Initialize(arial20);

            gun = Content.Load<Texture2D>("Gun");
            // Load fonts
            lucidaConsole120 = Content.Load<SpriteFont>("Lucida-Console-120");
            arial100 = Content.Load<SpriteFont>("arial-100");
            ocra100 = Content.Load<SpriteFont>("ocra-100");
            toreks60 = Content.Load<SpriteFont>("Toreks-100");
            toreks80 = Content.Load<SpriteFont>("torek-80");
            m6 = Content.Load<SpriteFont>("Fonts/m6");

            //load UI
            buttonNormal = Content.Load<Texture2D>("UiAssets/NewButton");
            buttonHover = Content.Load<Texture2D>("UiAssets/NewButton");
            upgradeCard = Content.Load<Texture2D>("UiAssets/TallCard");
            iconButton = Content.Load<Texture2D>("UiAssets/Icon Button");
            largePanel = Content.Load<Texture2D>("UiAssets/LargePanel");
            buttonPanel = Content.Load<Texture2D>("UiAssets/Button Panel 1 Normal");
            checkBoxFalse = Content.Load<Texture2D>("UiAssets/CheckBox False");
            checkBoxTrue = Content.Load<Texture2D>("UiAssets/CheckBox True");
            buttonAlt = Content.Load<Texture2D>("UiAssets/Button Normal");
            buttonAltHover = Content.Load<Texture2D>("UiAssets/Button Hover 1");
            squareCard = Content.Load<Texture2D>("UiAssets/Card X2");
            squareCardAlt = Content.Load<Texture2D>("UiAssets/Card X3");
            healthBar = Content.Load<Texture2D>("UiAssets/HealthBar");
            health = Content.Load<Texture2D>("UiAssets/Health");
            xpBar = Content.Load<Texture2D>("UiAssets/Xp");
            boostBar = Content.Load<Texture2D>("UiAssets/BoostBar");
            boost =  Content.Load<Texture2D>("UiAssets/Boost");
            playerIcon = Content.Load<Texture2D>("UiAssets/Player");
            timeHolder = Content.Load<Texture2D>("UiAssets/TimeHolder");
            playerHolder = Content.Load<Texture2D>("UiAssets/PlayerHolder");
            RedButtonNormal = Content.Load<Texture2D>("GOButtonRedNormal1");
            RedButtonHover = Content.Load<Texture2D>("GoButtonRedHover1");
            RedTitle = Content.Load<Texture2D>("gameOverTitle1");
            sliderFront = Content.Load<Texture2D>("UiAssets/SliderFront");
            sliderBack = Content.Load<Texture2D>("UiAssets/SliderBack");
            HUD = Content.Load<Texture2D>("UiAssets/Hud");
            upgradeOutline = Content.Load<Texture2D>("UiAssets/UpgradeOutline");
            upgradeSquare = Content.Load<Texture2D>("UiAssets/UpgradeSquare");
            squareFilled = Content.Load<Texture2D>("UiAssets/SquareFilled");
            xpBarTop = Content.Load<Texture2D>("UiAssets/XpBarTop");

            UiAssets.Add(buttonNormal);
            UiAssets.Add(buttonHover);
            UiAssets.Add(upgradeCard);
            UiAssets.Add(largePanel);
            UiAssets.Add(iconButton);
            UiAssets.Add(buttonPanel);
            UiAssets.Add(checkBoxFalse);
            UiAssets.Add(checkBoxTrue);
            UiAssets.Add(buttonAlt);
            UiAssets.Add(buttonAltHover);
            UiAssets.Add(squareCard);
            UiAssets.Add(squareCardAlt);
            UiAssets.Add(healthBar);
            UiAssets.Add(health);
            UiAssets.Add(boostBar);
            UiAssets.Add(boost);
            UiAssets.Add(xpBar);
            UiAssets.Add(playerIcon);
            UiAssets.Add(timeHolder);
            UiAssets.Add(playerHolder);
            UiAssets.Add(sliderFront);
            UiAssets.Add(sliderBack);
            UiAssets.Add(HUD);
            UiAssets.Add(upgradeOutline);
            UiAssets.Add(upgradeSquare);
            UiAssets.Add(squareFilled);
            UiAssets.Add(xpBarTop);

            //Load upgrade sprites
            frames = Content.Load<Texture2D>("Upgrades/Frameset");
            blueUpgrades = Content.Load<Texture2D>("Upgrades/BlueUpgrades");

            wave = Content.Load<Texture2D>("Player/SwordWave");
            bullet = Content.Load<Texture2D>("Bullets/SpaceBullet");
            ProjectileManager.Instance.Initialize(pixel, bullet, wave);

            //load player
            // Player texture
            playerSprite = Content.Load<Texture2D>("PlayerSheet");
            originalPlayer = Content.Load<Texture2D>("Player");
            bodySprite = Content.Load<Texture2D>("PlayerBody");
            orbitalAsset = Content.Load<Texture2D>("Upgrades/Orbital");
            orbitalTop = Content.Load<Texture2D>("Upgrades/OrbitalTop");
            shield = Content.Load<Texture2D>("Upgrades/Shield");
            satellite = Content.Load<Texture2D>("Upgrades/Satellite");

            //Create the asteroid manager
            asteroid1 = Content.Load<Texture2D>("asteroid-1");
            asteroid2 = Content.Load<Texture2D>("asteroid-2");
            asteroids = new AsteroidSpawner(asteroid1, asteroid2);

            //Startup orbs
            OrbManager orbManager = new OrbManager(orbitalAsset, orbitalTop, shield, satellite);

            player = new Player(playerSprite, pixel, GraphicsDevice, arial20, orbManager, gun);

            //Gradients
            leftGradient = Content.Load<Texture2D>("Player/LeftToRight");


            UpgradeManager.Instance.Initialize(frames, blueUpgrades, player, Content);

            //Pass in the uiAssets list for any textures
            UIManager.Instance.Initialize(UiAssets, arial100, width, height, player, m6, Content);

            //load grid
            Grid.Instance.Initialize(pixel, arial20, player, camera);

            // Load background textures
            spaceBackground = Content.Load<Texture2D>("Space Background");
            spaceMidground1 = Content.Load<Texture2D>("SpaceBackground_Parallax");
            spaceMidground2 = Content.Load<Texture2D>("SpaceBackground_Parallax2");
            menuBackground = Content.Load<Texture2D>("menuBackground2");
            staticControlsBackground = Content.Load<Texture2D>("controlStatic");
            dynamicControlsBackground1 = Content.Load<Texture2D>("controlDynamic1");
            dynamicControlsBackground2 = Content.Load<Texture2D>("controlDynamic2");
            staticCreditsBackground = Content.Load<Texture2D>("staticSpaceCredits");
            dynamicCreditsBackground1 = Content.Load<Texture2D>("dynamicSpaceCredits1");
            dynamicCreditsBackground2 = Content.Load<Texture2D>("dynamicSpaceCredits2");
            staticWinBackground = Content.Load<Texture2D>("staticWinBackground");
            dynamicWinBackground1 = Content.Load<Texture2D>("dynamicWinBackground1");
            dynamicWinBackground2 = Content.Load<Texture2D>("dynamicWinBackground2");

            // Load border for title
            pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            pixelTexture.SetData(new[] { Color.White });

            //Button Constants
            int menuButtonsX = 40;
            int buttonWidth = 453;
            int buttonHeight = 100;

            // Load everything needed for play button on main menu
            int playButtonY = (_graphics.PreferredBackBufferHeight - buttonHeight) / 2 - 50;
            playButton = new Button(buttonNormal, buttonHover, new Rectangle(menuButtonsX, playButtonY, buttonWidth, buttonHeight), toreks60, smallTextBox: true);
            playButton.TextBox.Text = "PLAY";
            playButton.ButtonColor = Color.White;

            // Load everything nedded for controls button on main menu
            int controlsButtonY = (_graphics.PreferredBackBufferHeight - buttonHeight) / 2 + 70;
            controlsButton = new Button(buttonNormal, buttonHover, 
                new Rectangle(menuButtonsX, controlsButtonY, buttonWidth, buttonHeight), toreks60, smallTextBox: true);
            controlsButton.TextBox.Text = "SETTINGS";
            playButton.ButtonColor = Color.White;

            // Load everything for credits button on main menu
            int creditButtonY = (_graphics.PreferredBackBufferHeight - buttonHeight) / 2 + 190;
            creditsButton = new Button(buttonNormal, buttonHover, new Rectangle(menuButtonsX, creditButtonY, buttonWidth, buttonHeight), toreks60, smallTextBox: true);
            creditsButton.TextBox.Text = "CREDITS";
            creditsButton.ButtonColor = Color.White;

            // Load everything for quit button on main menu

            int quitButtonY = (_graphics.PreferredBackBufferHeight - buttonHeight) / 2 + 310;
            quitButton = new Button(buttonNormal, buttonHover, new Rectangle(menuButtonsX, quitButtonY, buttonWidth, buttonHeight), toreks60, smallTextBox: true);
            quitButton.TextBox.Text = "QUIT";
            quitButton.ButtonColor = Color.White;

            // Load everything for main menu button on game over screen
            int gameOverButtonWidth = 400;
            int gameOverButtonHeight = 150;
            int mainMenuButtonX = (_graphics.PreferredBackBufferWidth - gameOverButtonWidth) - 30;
            int mainMenuButtonY = (_graphics.PreferredBackBufferHeight - gameOverButtonHeight) / 2 + 210;
            mainMenuButton = new Button(RedButtonNormal, RedButtonHover, 
                new Rectangle(mainMenuButtonX, mainMenuButtonY, gameOverButtonWidth, gameOverButtonHeight), toreks60);
            mainMenuButton.TextBox.Text = "MENU";
            mainMenuButton.ButtonColor = Color.White;

            // Load everything for quit button in game over menu
            int quitGameOverButtonX = (_graphics.PreferredBackBufferWidth - gameOverButtonWidth) - 30;
            int quitGameOverButtonY = (_graphics.PreferredBackBufferHeight - gameOverButtonHeight) / 2 + 390;
            gameOverQuitButton = new Button(RedButtonNormal, RedButtonHover, 
                new Rectangle(quitGameOverButtonX, quitGameOverButtonY, gameOverButtonWidth, gameOverButtonHeight), toreks60);
            gameOverQuitButton.TextBox.Text = "QUIT";
            gameOverQuitButton.ButtonColor = Color.White;

            // Load everything for menu button in win menu
            int menuWinButtonY = (_graphics.PreferredBackBufferHeight - buttonHeight) / 2 + 150;
            winMenuButton = new Button(buttonAlt, buttonAltHover,
                new Rectangle(menuButtonsX, menuWinButtonY, 350, 120), toreks60);
            winMenuButton.TextBox.Text = "MENU";
            winMenuButton.ButtonColor = Color.White;

            // Load everything for quit button in win menu
            int quitWinButtonY = (_graphics.PreferredBackBufferHeight - buttonHeight) / 2 + 300;
            winQuitButton = new Button(buttonAlt, buttonAltHover,
                new Rectangle(menuButtonsX, quitWinButtonY, 350, 120), toreks60);
            winQuitButton.TextBox.Text = "QUIT";
            winQuitButton.ButtonColor = Color.White;

            // Load GodMode button
            int godModeButtonWidth = 300;
            int godModeButtonHeight = 75;
            int godModeButtonX = (width - godModeButtonWidth - 100);
            int godModeButtonY = 50;
            godModeButton = new Button(
                buttonNormal,
                buttonNormal,
                new Rectangle(
                    godModeButtonX,
                    godModeButtonY,
                    godModeButtonWidth,
                    godModeButtonHeight),
                toreks60,
                smallTextBox: true);
            godModeButton.TextBox.Text = "GODMODE: OFF";

            // Load Boss Spawner button
            int spawnerButtonWidth = 300;
            int spawnerButtonHeight = 75;
            int spawnerButtonX = (width - spawnerButtonWidth * 2 - 150);
            int spawnerButtonY = 50;
            spawnBoss = new Button(
                buttonNormal,
                buttonNormal,
                new Rectangle(
                    spawnerButtonX,
                    spawnerButtonY,
                    spawnerButtonWidth,
                    spawnerButtonHeight),
                toreks60,
                smallTextBox: true);
            spawnBoss.TextBox.Text = "SPAWN BOSS";

            // Load Crusier Spawner button
            int crusierButtonWidth = 300;
            int crusierButtonHeight = 75;
            int crusierButtonX = (width - crusierButtonWidth * 2 - 150);
            int crusierButtonY = 100 + spawnerButtonHeight;
            spawnCrusier = new Button(
                buttonNormal,
                buttonNormal,
                new Rectangle(
                    crusierButtonX,
                    crusierButtonY,
                    crusierButtonWidth,
                    crusierButtonHeight),
                toreks60,
                smallTextBox: true);
            spawnCrusier.TextBox.Text = "SPAWN CRUSIER";


            // Load PauseTimer button
            int pauseTimerButtonWidth = 300;
            int pauseTimerButtonHeight = 75;
            int pauseTimerButtonX = (width - godModeButtonWidth - 100);
            int pauseTimerButtonY = 100 + godModeButtonHeight;
            pauseTimerButton = new Button(
                buttonNormal,
                buttonNormal,
                new Rectangle(
                    pauseTimerButtonX,
                    pauseTimerButtonY,
                    pauseTimerButtonWidth,
                    pauseTimerButtonHeight),
                toreks60,
                smallTextBox: true);
            pauseTimerButton.TextBox.Text = "PAUSE TIMER: OFF";

            // Load keyboard icons
            keyboardExtras = Content.Load<Texture2D>("KeyboardExtras");
            keyboardLetters = Content.Load<Texture2D>("KeyboardLetters");

            // Load Enemy data
            EnemyManager.Instance.LoadContent("../../../TextFiles/Enemies.txt", Content, player);

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

            // Load menu card
            titleMenu = Content.Load<Texture2D>("menuCard1");

            // Load in menu backgrounds
            staticMenuBackground = Content.Load<Texture2D>("menuSpaceBackground1");
            dynamicMenuBackground1 = Content.Load<Texture2D>("menuSpaceBackground7");
            dynamicMenuBackground2 = Content.Load<Texture2D>("menuSpaceBackground6");

            // Initialize text color
            textColor = Color.White;

            // Load in game over background
            staticGameOverBackground = Content.Load<Texture2D>("gameOverBackground1");

            // load in planet spritesheets
            planetTexture = Content.Load<Texture2D>("gameOverPlanet2");
            menuPlanetTexture = Content.Load<Texture2D>("mainMenuPlanet");
            blackHoleTexture = Content.Load<Texture2D>("blackHole");
            earthTexture = Content.Load<Texture2D>("winPlanet");

            // Initialize animated planet sprite
            planet = new AnimatedSprite(planetTexture, 300, 300, 50, 0.2f, new Rectangle(110, 160, 800, 800));
            menuPlanet = new AnimatedSprite(menuPlanetTexture, 300, 300, 50, 0.15f, new Rectangle(750, -200, 1200, 1200));
            blackHole = new AnimatedSprite(blackHoleTexture, 320, 300, 50, 0.15f, new Rectangle(50, 500, 600, 600));
            earth = new AnimatedSprite(earthTexture, 150, 150, 50, 0.2f, new Rectangle(850, 500, 1500, 1500));

            // Settings menu
            settings = new Settings(UiAssets, toreks60, _graphics, this, Content);
            subtitles = new SubtitleManager(arial20, new Vector2(width, height), GraphicsDevice);
            subtitles.Enabled = settings.ShowSubtitles;


            // Load character position for first animation
            startPos1 = new Vector2(-250, GraphicsDevice.Viewport.Height / 2);
            endPos1 = new Vector2(width - playerSprite.Width, GraphicsDevice.Viewport.Height / 2);
            characterPos1 = startPos1;
            menuColor = GetRandomColor();

            // Load character position for second animation
            startPos2 = new Vector2(GraphicsDevice.Viewport.Width + 200, GraphicsDevice.Viewport.Height / 2 + 200);
            endPos2 = new Vector2(-1500, GraphicsDevice.Viewport.Height / 2 + 200);
            characterPos2 = startPos2;
            menuColor2 = GetRandomColor();

            // Load character position for third animation
            startPos3 = new Vector2(-3000, GraphicsDevice.Viewport.Height / 2 - 300);
            endPos3 = new Vector2(width, GraphicsDevice.Viewport.Height / 2 - 300);
            characterPos3 = startPos3;
            menuColor3 = GetRandomColor();

            //Character Creator:
            selector = new CharacterSelect(UiAssets,player, m6, buttonNormal, Content, pixel, this);

            // Credit Lines list
            creditsLines = new List<string>(File.ReadAllLines(Path.Combine("TextFiles", "Credits.txt")));

            // Y starting position
            scrollY = GraphicsDevice.Viewport.Height;

            // Timer text box
            timerText = new TextBox(toreks60, new Rectangle(100, 100, 300, 300), Color.White, true);
            timerText.Text = "Timer:" + timer;

            ring = Content.Load<Texture2D>("Drops/Ring");

            DropManager.Instance.Initialize(player);
            DropManager.Instance.LoadDrops(Content);
            ShockwaveManager.Instance.Initialize(ring);

            //Load saved settings
            settings.Load();
            settings.CreateTabs();

            // Load the characters into array
            playerSprites = new Texture2D[]
            {
                Content.Load<Texture2D>("Player"),
                Content.Load<Texture2D>("Player/Purple"),
                Content.Load<Texture2D>("Player/Red"),
                Content.Load<Texture2D>("Player/Yellow"),
                Content.Load<Texture2D>("Player/Pink")
            };

            // Get first random characters
            random = new Random();
            int index1 = random.Next(playerSprites.Length);
            int index2 = random.Next(playerSprites.Length);
            int index3 = random.Next(playerSprites.Length);
            currentPlayerSprite1 = GetUniqueRandomSprite();
            currentPlayerSprite2 = GetUniqueRandomSprite(currentPlayerSprite1);
            currentPlayerSprite3 = GetUniqueRandomSprite(currentPlayerSprite1, currentPlayerSprite2);

            //*****************************************************************
        }
        #endregion

        #region UPDATE
        //---------------------------------------------------------------------
        //                          UPDATE
        //---------------------------------------------------------------------
        protected override void Update(GameTime gameTime)
        {
            //Get the current input
            kbState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            //Sound manager needs to exist everywhere
            SoundManager.Update(gameTime);

            //Allow player to toggle screen with f
            if (SingleKeyPress(Keys.F))
            {
                ToggleFullscreen();
            }
            // Base FSM for the different game states
            switch (gameMode)
            {
                case GameMode.Menu:

                    // Play menu song if its not already playing
                    if (SoundManager.CurrentSongName != "menuSong")
                    {
                        // start new song
                        SoundManager.PlaySong("menuSong");
                    }

                    // Get total elapsed time
                    elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

                    // Update planet sprite
                    menuPlanet.Update(gameTime);

                    // Switch interval for menu text
                    double switchInterval = 0.5f;

                    // Swich color every second
                    if (elapsedTime >= switchInterval)
                    {
                        elapsedTime = 0; // Reset timer
                        
                        // Switch color with if statment
                        if (textColor == Color.White)
                        {
                            textColor = Color.DarkTurquoise;
                        }
                        else
                        {
                            textColor = Color.White;
                        }
                    }

                    // Update animation timer
                    animationTimer1 += (float)elapsedTime;

                    // Reset animation after 15 seconds
                    if (animationTimer1 >= AnimationLoopDuration1)
                    {
                        animationTimer1 = 0f;
                        characterPos1 = startPos1;
                        characterRotation = 0f;
                        menuColor = GetRandomColor();

                        // Update which sprite animates
                        int index = random.Next(playerSprites.Length);
                        currentPlayerSprite1 = GetUniqueRandomSprite();
                    }

                    // Move character from start to end position
                    characterPos1.X += characterSpeed;

                    // Update menu character rotation
                    characterRotation += rotationSpeed;

                    // Check buttons hover state
                    if (gameMode == GameMode.Menu)
                    {
                        playButton.Update();
                        controlsButton.Update();
                        creditsButton.Update();
                        quitButton.Update();
                    }

                    // If statement for play button click
                    if (playButton.IsHovering && SingleMousePress())
                    {
                        // Reset Game
                        ResetGame();

                        // Change game state
                        gameMode = GameMode.Creator;   
                    }

                    // if statement for control button click
                    if (controlsButton.IsHovering && SingleMousePress())
                    {
                        // Change game state
                        gameMode = GameMode.Settings;
                    }

                    // If statment for credits button
                    if (creditsButton.IsHovering && SingleMousePress())
                    {
                        // Reset game
                        ResetGame();
                        
                        // Change game state
                        gameMode = GameMode.Credits;
                    }

                    // If statment for quit button
                    if (quitButton.IsHovering && SingleMousePress())
                    {
                        // Quits program
                        settings.Save();
                        Exit();
                    }

                    // If statement to switch to game state when enter is pressed
                    if (SingleKeyPress(Keys.Enter))
                    {
                        // Switch game mode
                        gameMode = GameMode.Creator;
                    }

                    // Sensitivity for menu paralax
                    float movementFactor1 = 0.008f;
                    float movementFactor2 = 0.004f;

                    // Offset background based on mouse position
                    dynamicOffSet1 = new Vector2(
                        (mouseState.X - GraphicsDevice.Viewport.Width / 2) * movementFactor1,
                        (mouseState.Y - GraphicsDevice.Viewport.Height / 2) * movementFactor1);
                    dynamicOffSet2 = new Vector2(
                        (mouseState.X - GraphicsDevice.Viewport.Width / 2) * movementFactor2,
                        (mouseState.Y - GraphicsDevice.Viewport.Height / 2) * movementFactor2);
                    break;

                case GameMode.Settings:

                    settings.Update();

                    // Sensitivity for menu paralax
                    movementFactor1 = 0.008f;
                    movementFactor2 = 0.004f;

                    // Offset background based on mouse position
                    dynamicOffSet1 = new Vector2(
                        (mouseState.X - GraphicsDevice.Viewport.Width / 2) * movementFactor1,
                        (mouseState.Y - GraphicsDevice.Viewport.Height / 2) * movementFactor1);
                    dynamicOffSet2 = new Vector2(
                        (mouseState.X - GraphicsDevice.Viewport.Width / 2) * movementFactor2,
                        (mouseState.Y - GraphicsDevice.Viewport.Height / 2) * movementFactor2);

                    break;
                case GameMode.Creator:

                    //Update returns the play button value
                    if (selector.Update(gameTime, SingleMousePress()))
                    {
                        // Reset Game
                        ResetGame();

                        selector.GiveUpgrades();
                        // Switch game mode

                        Mouse.SetCursor(MouseCursor.FromTexture2D(cursor, 16, 16));
                        gameMode = GameMode.Game;

                        // Play game song if its not already playing
                        if (SoundManager.CurrentSongName != "gameSong")
                        {
                            // start new song
                            SoundManager.PlaySong("gameSong");
                        }

                    }
                    break;

                case GameMode.Credits:

                    // Play credits song if its not already playing
                    if (SoundManager.CurrentSongName != "creditsMusic")
                    {
                        // start new song
                        SoundManager.PlaySong("creditsMusic");
                    }

                    // Update planet
                    blackHole.Update(gameTime);

                    // Update animation timer
                    animationTimer2 += (float)elapsedTime;
                    animationTimer3 += (float)elapsedTime;

                    // Reset first animation once it ends
                    if (animationTimer2 >= AnimationLoopDuration1)
                    {
                        animationTimer2 = 0f;
                        characterPos2 = startPos2;
                        characterRotation2 = 0f;
                        menuColor2 = GetRandomColor();

                        // Update which sprite animates
                        int index = random.Next(playerSprites.Length);
                        currentPlayerSprite2 = GetUniqueRandomSprite(currentPlayerSprite1);
                    }

                    // Reset second animation once it ends
                    if (animationTimer3 >= AnimationLoopDuration2)
                    {
                        animationTimer3 = 0f;
                        characterPos3 = startPos3;
                        characterRotation3 = 0f;
                        menuColor3 = GetRandomColor();

                        // Update which sprite animates
                        int index = random.Next(playerSprites.Length);
                        currentPlayerSprite3 = GetUniqueRandomSprite(currentPlayerSprite1, currentPlayerSprite2);
                    }

                    // Move characters from start to end position
                    characterPos2.X -= characterSpeed;
                    characterPos3.X += characterSpeed;

                    // Update menu character rotation
                    characterRotation2 -= rotationSpeed;
                    characterRotation3 += rotationSpeed;

                    // Force reset animation if it goes off screen
                    if (characterPos2.X < -currentPlayerSprite2.Width - 3000)
                    {
                        animationTimer2 = AnimationLoopDuration1; // Force reset next frame
                    }
                    if (characterPos3.X > GraphicsDevice.Viewport.Width + currentPlayerSprite3.Width + 3000)
                    {
                        animationTimer3 = AnimationLoopDuration2;
                    }

                    // Check if any key is pressed
                    if (kbState.GetPressedKeys().Length > 0)
                    {
                        anyKeyPressed = true;
                    }

                    // If statment for returning to menu if any key is pressed
                    if (anyKeyPressed)
                    {
                        // return to menu state
                        gameMode = GameMode.Menu;

                        // return value to false
                        anyKeyPressed = false;
                    }

                    // get gametime time
                    float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

                    // Caluclate the scroll in the y direction
                    scrollY -= scrollSpeed * time;

                    // Loop when credits finish
                    if (scrollY + creditsLines.Count * toreks60.LineSpacing < 0)
                    {
                        // Reset y
                        scrollY = GraphicsDevice.Viewport.Height;
                    }

                    // Sensitivity for menu paralax
                    movementFactor1 = 0.008f;
                    movementFactor2 = 0.004f;

                    // Offset background based on mouse position
                    dynamicOffSet1 = new Vector2(
                        (mouseState.X - GraphicsDevice.Viewport.Width / 2) * movementFactor1,
                        (mouseState.Y - GraphicsDevice.Viewport.Height / 2) * movementFactor1);
                    dynamicOffSet2 = new Vector2(
                        (mouseState.X - GraphicsDevice.Viewport.Width / 2) * movementFactor2,
                        (mouseState.Y - GraphicsDevice.Viewport.Height / 2) * movementFactor2);

                    break;

                case GameMode.Game:

                    //------------ Game Logic ---------------------

                    // Adjust timer
                    if (!pauseTime)
                    {
                        timer += gameTime.ElapsedGameTime.TotalSeconds;
                    }

                    //------------ Debug --------------------------

                    //Checks if G (Debug) was hit once
                    if (SingleKeyPress(Keys.G))
                    {
                        //Flip debug on/off
                        debugOn = !debugOn;
                    }

                    // If debug mode is on
                    if (debugOn)
                    {
                        // Show GodMode button
                        godModeButton.Update();

                        // If user presses GodMode button, toggle godmode
                        if (SingleMousePress() && godModeButton.IsHovering)
                        {
                            player.GodMode = !player.GodMode;

                            // Update button text
                            if (player.GodMode)
                            {
                                godModeButton.TextBox.Text = "GODMODE: ON ";
                            }
                            else
                            {
                                godModeButton.TextBox.Text = "GODMODE: OFF";
                            }
                        }

                        // Show boss spawner
                        spawnBoss.Update();

                        // If user presses Boss Spawn button, clear screen, spawn boss
                        if(SingleMousePress() && spawnBoss.IsHovering)
                        {

                            if (!InBoss)
                            {
                                //Spawn the boss
                                EnemyManager.Instance.SpawnBoss();
                            }
                            else
                            {
                                //End the boss
                                EnemyManager.Instance.EndBoss();
                                //Turn the music back to normal
                                SoundManager.PlaySong("gameSong");
                            }

                        }

                        // Show boss spawner
                        spawnCrusier.Update();

                        // If user presses Boss Spawn button, clear screen, spawn boss
                        if (SingleMousePress() && spawnCrusier.IsHovering)
                        {

                            if (!InBoss)
                            {
                                //Spawn the boss
                                EnemyManager.Instance.SpawnCrusier();
                            }
                            else
                            {
                                //End the boss
                                EnemyManager.Instance.EndBoss();
                            }

                        }

                        // Show PauseTimer button
                        pauseTimerButton.Update();

                        // If user presses pause timer button, toggle pause time
                        if (SingleMousePress() && pauseTimerButton.IsHovering)
                        {
                            pauseTime = !pauseTime;

                            // Update button text
                            if (pauseTime)
                            {
                                pauseTimerButton.TextBox.Text = "PAUSE TIME: ON ";
                            }
                            else
                            {
                                pauseTimerButton.TextBox.Text = "PAUSE TIME: OFF";
                            }
                        }

                        // Kill button
                        if (SingleKeyPress(Keys.H))
                        {
                            Mouse.SetCursor(MouseCursor.Arrow);
                            gameMode = GameMode.GameOverTransition;
                        }

                        // Win button
                        if (SingleKeyPress(Keys.T))
                        {
                            Mouse.SetCursor(MouseCursor.Arrow);
                            gameMode = GameMode.Win;
                        }

                        // Spawns a lot of enemies
                        if (SingleKeyPress(Keys.U))
                        {
                            EnemyManager.Instance.IncreaseClock();
                        }
                    }


                    //Player wants to pause
                    if (SingleKeyPress(Keys.Escape))
                    {
                        Mouse.SetCursor(MouseCursor.Arrow);
                        gameMode = GameMode.Pause;
                        inSettings = true;
                    }

                    //Player is upgrading, pause movement
                    if (UIManager.Instance.IsUpgrading)
                    {
                        Mouse.SetCursor(MouseCursor.Arrow);
                        gameMode = GameMode.Upgrade;
                    }

                    //------------ Player -------------------------

                    //Updates player position
                    player.Update(
                        gameTime, 
                        kbState,
                        prevKbState,
                        mouseState, 
                        prevMouseState, 
                        camera.CameraPosition,
                        new Vector2(width, height));


                    //Updates camera matrix to follow player, do not update in boss
                    if (!InBoss)
                    {
                        camera.CameraFollow(player, gameTime);
                    }


                    //player needs to update based on player position
                    Grid.Instance.Update();

                    //------------ Projectiles --------------------

                    //Updates Projectiles
                    //TODO: Projectile manager update call
                    ProjectileManager.Instance.UpdateAll(gameTime);

                    //------------ Enemies ------------------------

                    //Updates Enemies
                    EnemyManager.Instance.Update(gameTime, timer, player.Position);
                    DropManager.Instance.Update(gameTime);
                    ShockwaveManager.Instance.Update(gameTime);
                    UpgradeManager.Instance.UpdateOrbs(gameTime);

                    //------------ UI ------------------------

                    UIManager.Instance.Update(SingleMousePress(),debugOn);
                    subtitles?.Update(gameTime);

                    //Lose condition, end game if player health drops under 0
                    if (player.Health <= 0)
                    {
                        Mouse.SetCursor(MouseCursor.Arrow);
                        // Move player to game over state
                        gameMode = GameMode.GameOverTransition;
                    }

                    asteroids.Update(gameTime);

                    break;

                case GameMode.Pause:
                    settings.Update();
                    //Unpause
                    if (SingleKeyPress(Keys.Escape))
                    {
                        Mouse.SetCursor(MouseCursor.FromTexture2D(cursor, 16, 16));

                        gameMode = GameMode.Game;
                        inSettings = false;
                    }
                    break;

                case GameMode.Upgrade:

                    UIManager.Instance.Update(SingleMousePress(), debugOn);

                    if (!UIManager.Instance.IsUpgrading)
                    {
                        Mouse.SetCursor(MouseCursor.FromTexture2D(cursor, 16, 16));

                        //Return to game
                        gameMode = GameMode.Game;
                    }

                    break;
                case GameMode.GameOverTransition:

                    Cam.UpdateShakeOnly(gameTime);
                    if(scaleTween == null)
                    {
                        scaleTween = Tween.CreateTween(1f, 80f, impactTime, EaseType.EaseIn);
                    }
                    if(rotateTween == null)
                    {
                        rotateTween = Tween.CreateTween(0f, MathF.PI * 2f + 0.25f, impactTime, EaseType.EaseOut);
                    }
                    if(colorTween == null)
                    {
                        //Turn screen red
                        TintScreen(new Color(60,14,11,100));
                    }
                    rotateTween.OnComplete = () =>
                    {
                        Cam.TriggerShake(0.5f, 70f);
                        scaleTween = null;
                        rotateTween = null;
                        SoundManager.PlaySound("Smack", 1f);
                        gameMode = GameMode.GameOver;
                    };
                    if(scaleTween != null && rotateTween != null)
                    {
                        player.Transition(scaleTween.currValue, rotateTween.currValue);
                    }


                    break;
                case GameMode.Win:

                    // Get total elapsed time
                    elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

                    // Play win song if its not already playing
                    if (SoundManager.CurrentSongName != "winMusic")
                    {
                        // start new song
                        SoundManager.PlaySong("winMusic");
                    }

                    // Update planet
                    earth.Update(gameTime);

                    // Update buttons
                    if (gameMode == GameMode.Win)
                    {
                        winMenuButton.Update();
                        winQuitButton.Update();
                    }

                    // Sensitivity for menu paralax
                    movementFactor1 = 0.008f;
                    movementFactor2 = 0.004f;

                    // Offset background based on mouse position
                    dynamicOffSet1 = new Vector2(
                        (mouseState.X - GraphicsDevice.Viewport.Width / 2) * movementFactor1,
                        (mouseState.Y - GraphicsDevice.Viewport.Height / 2) * movementFactor1);
                    dynamicOffSet2 = new Vector2(
                        (mouseState.X - GraphicsDevice.Viewport.Width / 2) * movementFactor2,
                        (mouseState.Y - GraphicsDevice.Viewport.Height / 2) * movementFactor2);

                    // Switch interval for win text
                    switchInterval = 0.5f;

                    // Swich color every second
                    if (elapsedTime >= switchInterval)
                    {
                        elapsedTime = 0; // Reset timer

                        // Switch color with if statment
                        if (textColor == Color.White)
                        {
                            textColor = Color.DarkTurquoise;
                        }
                        else
                        {
                            textColor = Color.White;
                        }
                    }

                    // If statement for main menu button click
                    if (winMenuButton.IsHovering && SingleMousePress())
                    {
                        // Change game state
                        gameMode = GameMode.Menu;
                    }

                    // If statement for quit button click
                    if (winQuitButton.IsHovering && SingleMousePress())
                    {
                        // Change game state
                        settings.Save();
                        Exit();
                    }

                    break;
                case GameMode.GameOver:

                    if(fallTween == null)
                    {
                        fallTween = Tween.CreateTween(player.Position.Y, player.Position.Y + 5000, fallDuration, EaseType.EaseIn);
                    }

                    Cam.UpdateShakeOnly(gameTime);
                    Tween.Update(gameTime);
                    player.Fall(fallTween.currValue);

                    // Play game over song if its not already playing
                    if (SoundManager.CurrentSongName != "gameOverSong")
                    {
                        // start new song
                        SoundManager.PlaySong("gameOverSong");
                    }

                    // update planet animation
                    planet.Update(gameTime);

                    // Transition from game over to menu
                    if (SingleKeyPress(Keys.Enter))
                    {
                        // Change state
                        gameMode = GameMode.Menu;
                    }

                    // Check buttons hover state
                    if (gameMode == GameMode.GameOver)
                    {
                        mainMenuButton.Update();
                        gameOverQuitButton.Update();
                    }

                    // If statement for main menu button click
                    if (mainMenuButton.IsHovering && SingleMousePress())
                    {
                        // Change game state
                        gameMode = GameMode.Menu;
                    }

                    // If statement for quit button click
                    if (gameOverQuitButton.IsHovering && SingleMousePress())
                    {
                        // Change game state
                        settings.Save();
                        Exit();
                    }

                    // Get total elapsed time
                    elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

                    // Switch interval for menu text
                    switchInterval = 0.5f;

                    // Swich color every second
                    if (elapsedTime >= switchInterval)
                    {
                        elapsedTime = 0; // Reset timer

                        // Switch color with if statment
                        if (textColor == Color.Red)
                        {
                            textColor = Color.White;
                        }
                        else
                        {
                            textColor = Color.Red;
                        }
                    }

                    break;
            }
            //Tweens need to be updated in every case
            Tween.Update(gameTime);

            //Sets previous states for next frame
            prevKbState = kbState;
            prevMouseState = mouseState;

            //*****************************************************************

            base.Update(gameTime);
        }   
        #endregion

        #region DRAW
        //---------------------------------------------------------------------
        //                          DRAW
        //---------------------------------------------------------------------
        protected override void Draw(GameTime gameTime)
        { 
            GraphicsDevice.Clear(Color.Black);

            // Code goes between asterisks
            //*****************************************************************

            // Base FSM for the different game states
            switch (gameMode)
            {
                case GameMode.Menu:

                    // Change background color
                    GraphicsDevice.Clear(Color.Black);

                    // Begin sprite batch
                    _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

                    // Draw static menu background
                    _spriteBatch.Draw(
                        staticMenuBackground,
                        new Vector2(0, 0),
                        Color.White);

                    // base position for offset
                    Vector2 basePosition = new Vector2(-50, -50);

                    // Draw dynamic background2
                    _spriteBatch.Draw(
                        dynamicMenuBackground2,
                        basePosition + dynamicOffSet2,
                        Color.White);

                    // Draw dynamic background1
                    _spriteBatch.Draw(
                        dynamicMenuBackground1,
                        basePosition + dynamicOffSet1,
                        Color.White);

                    // Draw planet
                    menuPlanet.Draw(_spriteBatch);

                    // Calculate origin of player in order to rotate properly
                    Vector2 origin = new Vector2(currentPlayerSprite1.Width / 2, currentPlayerSprite1.Height / 2);

                    //Gradients
                    _spriteBatch.Draw(
                        leftGradient,
                        new Rectangle(-100, 0, 600, height),
                        new Color(Color.White, 100f));

                    // Draw character1 animation
                    _spriteBatch.Draw(
                        currentPlayerSprite1,
                        characterPos1 + origin,
                        null,
                        Color.White,
                        characterRotation,
                        origin,
                        1.5f,
                        SpriteEffects.None,
                        0f);

                    // Title words position
                    Vector2 titlePosition1 = new Vector2( 130, GraphicsDevice.Viewport.Height / 2 - 460);
                    Vector2 titlePosition2 = new Vector2( 100, GraphicsDevice.Viewport.Height / 2 - 320);

                    // Rectangle position
                    Rectangle rectangle = new Rectangle(20, GraphicsDevice.Viewport.Height / 2 - 470, 800, 350);

                    // Draw title card
                    _spriteBatch.Draw(
                        titleMenu,
                        rectangle,
                        Color.White);

                    // Draw Menu text 
                    _spriteBatch.DrawString(
                        titleFont,
                        "STELLAR",
                        titlePosition1,
                        textColor);

                    // Draw Menu text
                    _spriteBatch.DrawString(
                        titleFont,
                        "RAMPAGE",
                        titlePosition2,
                        textColor);

                    // Draw buttons
                    if (gameMode == GameMode.Menu)
                    {
                        playButton.Draw(_spriteBatch);
                        controlsButton.Draw(_spriteBatch);
                        creditsButton.Draw(_spriteBatch);
                        quitButton.Draw(_spriteBatch);
                    }

                    // End spritebatch
                    _spriteBatch.End();

                    break;

                case GameMode.Settings:

                    // Begin sprite batch
                    _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

                    // Draw static menu background
                    _spriteBatch.Draw(
                        staticControlsBackground,
                        new Vector2(0, 0),
                        Color.White);

                    // base position for backgrounds
                    basePosition = new Vector2(-50, -50);

                    // Drawy dynamic background2
                    _spriteBatch.Draw(
                        dynamicControlsBackground1,
                        basePosition + dynamicOffSet2,
                        Color.White);

                    // Draw dynamic background1
                    _spriteBatch.Draw(
                        dynamicControlsBackground2,
                        basePosition + dynamicOffSet1,
                        Color.White);

                    settings.Draw(_spriteBatch);
                    // End spritebatch
                    _spriteBatch.End();
                    
                    break;

                    case GameMode.Credits:

                        // Begin sprite batch
                        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

                        // Draw static menu background
                        _spriteBatch.Draw(
                            staticCreditsBackground,
                            new Vector2(0, 0),
                            Color.White);

                        // base position for offset
                        basePosition = new Vector2(-50, -50);

                        // Draw dynamic background2
                        _spriteBatch.Draw(
                            dynamicCreditsBackground1,
                            basePosition + dynamicOffSet2,
                            Color.White);

                        // Draw dynamic background1
                        _spriteBatch.Draw(
                            dynamicCreditsBackground2,
                            basePosition + dynamicOffSet1,
                            Color.White);

                        // Draw planet
                        blackHole.Draw(_spriteBatch);

                        // Calculate origin of player in order to rotate properly
                        Vector2 origin1 = new Vector2(currentPlayerSprite2.Width / 2, currentPlayerSprite2.Height / 2);
                        Vector2 origin2 = new Vector2(currentPlayerSprite3.Width / 2, currentPlayerSprite3.Height / 2);

                        // Draw character1 animation
                        _spriteBatch.Draw(
                            currentPlayerSprite2,
                            characterPos2 + origin1,
                            null,
                            Color.White,
                            characterRotation2,
                            origin1,
                            1.5f,
                            SpriteEffects.None,
                            0f);

                        // Draw character2 animation
                        _spriteBatch.Draw(
                            currentPlayerSprite3,
                            characterPos3 + origin2,
                            null,
                            Color.White,
                            characterRotation3,
                            origin2,
                            1.5f,
                            SpriteEffects.None,
                            0f);
                        
                        // Space each line
                        float lineHeight = toreks60.LineSpacing;

                        // Iterate through credits file
                        for (int i = 0; i < creditsLines.Count; i++)
                        {
                            // Position text in the center
                            Vector2 position = new Vector2(
                                GraphicsDevice.Viewport.Width / 2,
                                scrollY + i * lineHeight);

                            // Aligns the text to the center
                            Vector2 textSize = toreks60.MeasureString(creditsLines[i]);

                            // Draw text
                            _spriteBatch.DrawString(
                                toreks60,
                                creditsLines[i],
                                position - textSize / 2,
                                Color.White);
                        }

                        // End draw sprite batch
                        _spriteBatch.End();

                        break;

                case GameMode.Creator:
                    // Change background color back
                    GraphicsDevice.Clear(Color.CornflowerBlue);

                    // Draw background
                    _spriteBatch.Begin(
                        sortMode: SpriteSortMode.Deferred,
                        samplerState: SamplerState.LinearWrap);

                    // Draw background first, then parallax
                    _spriteBatch.Draw(
                        spaceBackground,
                        new Vector2(0, 0),
                        new Rectangle(
                            (int)(camera.CameraPosition.X / 2 + (int)(gameTime.TotalGameTime.TotalMilliseconds / 100)),
                            (int)(camera.CameraPosition.Y / 2 + (int)(gameTime.TotalGameTime.TotalMilliseconds / 100)),
                            spaceBackground.Width,
                            spaceBackground.Height),
                        Color.White);

                    _spriteBatch.Draw(
                        spaceMidground1,
                        new Vector2(0, 0),
                        new Rectangle(
                            (int)(camera.CameraPosition.X / 1.8 + (int)(gameTime.TotalGameTime.TotalMilliseconds / 80)),
                            (int)(camera.CameraPosition.Y / 1.8 + (int)(gameTime.TotalGameTime.TotalMilliseconds / 80)),
                            spaceBackground.Width,
                            spaceBackground.Height),
                        Color.White);

                    _spriteBatch.Draw(
                        spaceMidground2,
                        new Vector2(0, 0),
                        new Rectangle(
                            (int)(camera.CameraPosition.X / 1.5 + (int)(gameTime.TotalGameTime.TotalMilliseconds / 50)),
                            (int)(camera.CameraPosition.Y / 1.5 + (int)(gameTime.TotalGameTime.TotalMilliseconds / 50)),
                            spaceBackground.Width,
                            spaceBackground.Height),
                        Color.White);

                    _spriteBatch.End();


                    //Draw player
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, samplerState: SamplerState.PointClamp);
                    selector.Draw(_spriteBatch);
                    _spriteBatch.End();
                    break;
                case GameMode.Pause:
                case GameMode.Upgrade:
                case GameMode.GameOverTransition:
                case GameMode.Game:

                    // Change background color back
                    GraphicsDevice.Clear(Color.CornflowerBlue);

                    // Draw background
                    _spriteBatch.Begin(
                        sortMode: SpriteSortMode.Deferred, 
                        samplerState: SamplerState.LinearWrap);

                    // Draw background first, then parallax
                    _spriteBatch.Draw(
                        spaceBackground,
                        new Vector2(0, 0),
                        new Rectangle(
                            (int)(camera.CameraPosition.X/2 + (int)(gameTime.TotalGameTime.TotalMilliseconds / 100)),
                            (int)(camera.CameraPosition.Y/2 + (int)(gameTime.TotalGameTime.TotalMilliseconds / 100)), 
                            spaceBackground.Width, 
                            spaceBackground.Height),
                        Color.White);

                    _spriteBatch.Draw(
                        spaceMidground1,
                        new Vector2(0, 0),
                        new Rectangle(
                            (int)(camera.CameraPosition.X / 1.8 + (int)(gameTime.TotalGameTime.TotalMilliseconds / 80)),
                            (int)(camera.CameraPosition.Y / 1.8 + (int)(gameTime.TotalGameTime.TotalMilliseconds / 80)),
                            spaceBackground.Width,
                            spaceBackground.Height),
                        Color.White);

                    _spriteBatch.Draw(
                        spaceMidground2,
                        new Vector2(0, 0),
                        new Rectangle(
                            (int)(camera.CameraPosition.X / 1.5 + (int)(gameTime.TotalGameTime.TotalMilliseconds / 50)),
                            (int)(camera.CameraPosition.Y / 1.5 + (int)(gameTime.TotalGameTime.TotalMilliseconds / 50)),
                            spaceBackground.Width,
                            spaceBackground.Height),
                        Color.White);

                    _spriteBatch.End();

                    // Begin sprite batch draw
                    //NonPremultiplied is needed to allow particles to fade.
                    //With premultiplied values, white can never fade out of screen
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, transformMatrix: camera.Transform, samplerState: SamplerState.PointClamp);


                    asteroids.Draw(_spriteBatch);

                    _spriteBatch.End();

                    // Draw background
                    _spriteBatch.Begin(
                        sortMode: SpriteSortMode.Deferred,
                        samplerState: SamplerState.LinearWrap);



                    _spriteBatch.End();

                    // Begin sprite batch draw
                    //NonPremultiplied is needed to allow particles to fade.
                    //With premultiplied values, white can never fade out of screen
                    _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, transformMatrix: camera.Transform, samplerState: SamplerState.PointClamp);

                    ProjectileManager.Instance.DrawBullets(_spriteBatch, debugOn);

                    //Draw XP
                    UpgradeManager.Instance.DrawOrbs(_spriteBatch, debugOn);

                    // Draw Enemy
                    EnemyManager.Instance.Draw(_spriteBatch, debugOn);
                    DropManager.Instance.Draw(_spriteBatch, debugOn);
                    ShockwaveManager.Instance.Draw(_spriteBatch);


                    //Changes the origin of the player
                    if(gameMode == GameMode.GameOverTransition)
                    {
                        player.DrawTransition(_spriteBatch);
                    }
                    else
                    {
                        // Draw player
                        player.Draw(_spriteBatch, debugOn);
                    }


                    // End previous camera spritbatch
                    _spriteBatch.End();

                    // New one with no transformation matrix to keep timer on screen
                    _spriteBatch.Begin();

                    UIManager.Instance.DrawHud(_spriteBatch, timer);



                    // End 
                    _spriteBatch.End();

                    
                    //if debugging, draw all debug features
                    if (debugOn)
                    {

                        //--------------------
                        //    Move with world
                        //--------------------
                        // Begin new sprite batch draw for camera
                        _spriteBatch.Begin(transformMatrix: camera.Transform);

                        //draw grid lines, and cells
                        Grid.Instance.DisplayGrid(_spriteBatch);

                        _spriteBatch.End();

                        //--------------------
                        //    Fixed UI
                        //--------------------
                        _spriteBatch.Begin();

                        DebugDraw(_spriteBatch);

                        // Draw godmode & pauseTime button
                        pauseTimerButton.Draw(_spriteBatch);
                        godModeButton.Draw(_spriteBatch);
                        spawnBoss.Draw(_spriteBatch);
                        spawnCrusier.Draw(_spriteBatch);

                        //draw test upgrade button
                        UIManager.Instance.DebugDraw(_spriteBatch);
                        //Draw normal upgrade
                        UIManager.Instance.Draw(_spriteBatch);

                        _spriteBatch.End();
                    }
                    _spriteBatch.Begin();

                    //draw UI
                    UIManager.Instance.Draw(_spriteBatch);
                    subtitles?.Draw(_spriteBatch);
                    ;
                    if (inSettings)
                    {
                        _spriteBatch.Draw(
                            pixel,          //Texture
                            new Rectangle(
                                0,          //X
                                0,          //Y
                                width,      //Screen width
                                height),    //Screen height
                            new Color(      //Transparent overlay
                                0,
                                0,
                                0,
                                150));      //Alpha

                        settings.Draw(_spriteBatch);
                    }

                    //Tint the screen on death
                    if(colorTween != null)
                    {
                        _spriteBatch.Draw(
                            pixel,
                            new Rectangle(0, 0, width, height),
                            colorTween.currColor);
                    }
                    _spriteBatch.End();
                    break;

                case GameMode.GameOver:

                    // Change background color to black
                    GraphicsDevice.Clear(Color.White);

                    // New sprite batch for game over
                    _spriteBatch.Begin(samplerState: SamplerState.PointClamp);


                    // Draw background
                    _spriteBatch.Draw(
                        staticGameOverBackground,
                        new Vector2(0, 0),
                        Color.White);

                    // draw animated planet
                    planet.Draw(_spriteBatch);

                    // Draw game over card
                    _spriteBatch.Draw(
                        RedTitle,
                        new Rectangle(GraphicsDevice.Viewport.Width - 1000, GraphicsDevice.Viewport.Height / 2 - 480, 1000, 300),
                        Color.White);

                    // Draw time card
                    _spriteBatch.Draw(
                        RedTitle,
                        new Rectangle(GraphicsDevice.Viewport.Width - 900, GraphicsDevice.Viewport.Height / 2 - 150, 900, 200),
                        Color.White);

                    // Game over position
                    Vector2 gameOverPosition = new Vector2(GraphicsDevice.Viewport.Width - 915, GraphicsDevice.Viewport.Height / 2 - 400);

                    // Draw Game over message (more later)
                    _spriteBatch.DrawString(
                        toreks80,
                        "GAME OVER",
                        gameOverPosition,
                        textColor);
                    
                    // Draw timer text
                    _spriteBatch.DrawString(
                        toreks60,
                        "TIME:",
                        new Vector2(GraphicsDevice.Viewport.Width - 810, GraphicsDevice.Viewport.Height / 2 - 100),
                        Color.White);

                    // Timer info
                    int totalSeconds = (int)timer;
                    int minutes = totalSeconds / 60;
                    int seconds = totalSeconds % 60;

                    // Draw timer
                    _spriteBatch.DrawString(
                        toreks60,
                        $"{minutes:00}:{seconds:00}",
                        new Vector2(GraphicsDevice.Viewport.Width - 500, GraphicsDevice.Viewport.Height / 2 - 100),
                        textColor);
                    

                    // Draw buttons
                    if (gameMode == GameMode.GameOver)
                    {
                        mainMenuButton.Draw(_spriteBatch);
                        gameOverQuitButton.Draw(_spriteBatch);
                    }

                    // End sprite batch
                    _spriteBatch.End();

                    // New sprite batch for game over
                    _spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: camera.Transform);

                    //Draw the player transition
                    player.DrawTransition(_spriteBatch);


                    // End sprite batch
                    _spriteBatch.End();
                    break;
                case GameMode.Win:

                    // Change background color to black
                    GraphicsDevice.Clear(Color.White);

                    // Begin sprite batch
                    _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

                    // Draw static menu background
                    _spriteBatch.Draw(
                        staticWinBackground,
                        new Vector2(0, 0),
                        Color.White);

                    // base position for offset
                    basePosition = new Vector2(-50, -50);

                    // Draw dynamic background1
                    _spriteBatch.Draw(
                        dynamicWinBackground1,
                        basePosition + dynamicOffSet2,
                        Color.White);

                    // Draw dynamic background2
                    _spriteBatch.Draw(
                        dynamicWinBackground2,
                        basePosition + dynamicOffSet1,
                        Color.White);

                    // Draw planet
                    earth.Draw(_spriteBatch);

                    // Draw win card
                    _spriteBatch.Draw(
                        titleMenu,
                        new Rectangle(20, 100, 900, 250),
                        Color.White);

                    // Draw win text
                    _spriteBatch.DrawString(
                        toreks80,
                        "YOU WIN",
                        new Vector2(150, 150),
                        textColor);

                    // Draw timer card
                    _spriteBatch.Draw(
                        titleMenu,
                        new Rectangle(20, 380, 800, 200),
                        Color.White);

                    // Timer text
                    _spriteBatch.DrawString(
                        toreks60,
                        "TIME:",
                        new Vector2(80, 430),
                        Color.White);

                    // Timer info
                    totalSeconds = (int)timer;
                    minutes = totalSeconds / 60;
                    seconds = totalSeconds % 60;

                    // Draw timer
                    _spriteBatch.DrawString(
                        toreks60,
                        $"{minutes:00}:{seconds:00}",
                        new Vector2(380, 430),
                        textColor);

                    // Draw buttons
                    if (gameMode == GameMode.Win)
                    {
                        winMenuButton.Draw(_spriteBatch);
                        winQuitButton.Draw(_spriteBatch);
                    }

                    // End sprite batch
                    _spriteBatch.End();

                    break;
            }

            //*****************************************************************

            base.Draw(gameTime);
        }
        #endregion

        #region METHODS
        //---------------------------------------------------------------------
        //                              METHODS
        //---------------------------------------------------------------------

        /// <summary>
        /// Checks if a key was pressed, returns true the frame it does
        /// </summary>
        /// <param name="key">The key to check</param>
        /// <returns>True value the frame a key is pressed</returns>
        private bool SingleKeyPress(Keys key)
        {
            return (kbState.IsKeyDown(key) && prevKbState.IsKeyUp(key));
        }

        /// <summary>
        /// Checks if the mouse was just released 
        /// </summary>
        /// <returns>true if mouse was pressed and released</returns>
        private bool SingleMousePress()
        {
            if (mouseState.LeftButton == ButtonState.Released &&
               prevMouseState.LeftButton == ButtonState.Pressed)
            {
                //Reset the hovering bool. Once the button is pressed,
                //The update is killed before it can be removed
                if (HoveringButton)
                {
                    SoundManager.PlaySound("Pressed", 1f);
                    HoveringButton = false;
                }
                //Button was pressed
                return true;
            }
            //Button not pressed
            return false;
        }

        /// <summary>
        /// Reset the game by reseting only the timer for now to test gameover state
        /// </summary>
        private void ResetGame()
        {
            // Center player
            player.CenterPlayer(GraphicsDevice);

            // Center floating guy and restart rotation
            characterPos1 = startPos1;
            characterRotation = 0f;
            characterPos2 = startPos2;
            characterRotation2 = 0f;
            characterPos3 = startPos3;
            
            // Reset timer
            timer = 0;

            // Reset credits
            scrollY = GraphicsDevice.Viewport.Height;

            // Clear remaining Enemies
            EnemyManager.Instance.ResetEnemies();

            //Remove any active bosses
            EnemyManager.Instance.EndBoss();

            Player.CanMove = true;
            
            
            UpgradeManager.Instance.ResetUpgrades();

            //Turn off debug
            debugOn = false;


            HoveringButton = false;

            //Reset the transitions
            scaleTween = null;
            fallTween = null;
            rotateTween = null;
            colorTween = null;
            player.ResetTransition();

            //Turn boss off
            EnemyManager.Instance.EndBoss();

            //Reset XP
            UpgradeManager.Instance.XPRequired = 50;
            UpgradeManager.Instance.XP = 0;
        }

        /// <summary>
        /// Allows player to enter and exit fullscreen
        /// </summary>
        private void ToggleFullscreen()
        {
            isFullscreen = !isFullscreen;
            _graphics.IsFullScreen = isFullscreen;
            _graphics.ApplyChanges();
        }

        /// <summary>
        ///  Gets a random color for floating man in space
        /// </summary>
        /// <returns>random color</returns>
        private Color GetRandomColor()
        {
            int index = randy.Next(colorPalette.Length);
            return colorPalette[index];
        }

        public void EnterMenu()
        {
            gameMode = GameMode.Menu;
            inSettings = false;
        }

        private void DebugDraw(SpriteBatch _spriteSheet)
        {
            _spriteBatch.DrawString(
                arial20,
                HoveringButton.ToString(),
                new Vector2(width / 2, 10),
                Color.White);
        }

        /// <summary>
        /// Turn the whole screen red
        /// </summary>
        private void TintScreen(Color color)
        {
            //Create a new red tint
            colorTween = Tween.CreateColorTween(Color.Transparent, color, 0.15f, EaseType.EaseOut);
            colorTween.OnComplete = () =>
            {
                colorTween = Tween.CreateColorTween(color, Color.Transparent, 0.5f, EaseType.EaseIn);
            };
        }

        /// <summary>
        /// Prevent the same sprite from appearing
        /// </summary>
        /// <param name="excludeSprites">excluded sprites</param>
        /// <returns>non duplicate sprite</returns>
        private Texture2D GetUniqueRandomSprite(params Texture2D[] excludeSprites)
        {
            Texture2D sprite;
            do
            {
                sprite = playerSprites[random.Next(playerSprites.Length)];
            }
            while (excludeSprites.Contains(sprite));
            return sprite;
        }
        #endregion

    }

}
