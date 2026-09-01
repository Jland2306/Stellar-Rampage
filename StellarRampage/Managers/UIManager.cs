using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using StellarRampage.HelperClasses;
using StellarRampage.Miscellaneous;
using Microsoft.Xna.Framework.Input;
using StellarRampage.Managers;
using StellarRampage.GameObjects;
using StellarRampage.FlexItems;
using Microsoft.Xna.Framework.Content;

namespace StellarRampage.Managers
{
    //---------------------------------------------------------------------
    //                           Static Attributes
    //---------------------------------------------------------------------

    //Class cannot be inherited from
    public sealed class UIManager
    {
        //Creates a new static instance of this manager, there will only be one
        private static UIManager instance = null;

        //The working instance of the class
        public static UIManager Instance
        {
            //Returns the instance if it exists, creates it if not
            get
            {
                if (instance == null)
                {
                    instance = new UIManager();
                }
                return instance;
            }
        }

        //---------------------------------------------------------------------
        //                          Class Attributes
        //---------------------------------------------------------------------
        //Assets
        private List<Texture2D> UiAssets;

        //ScreenDimensions
        int width;
        int height;

        //Lists to help repeating tasks
        private List<Button> buttonList = new List<Button>();
        private List<UpgradePanel> upgradePanelList = new List<UpgradePanel>();

        //Upgrades
        private Texture2D upgradePanel;
        private Button testDisplayButton;
        private bool isUpgrading;
        private SpriteFont font;
        private SpriteFont m6;
        private Texture2D blueSheet;
        private Texture2D frames;
        private int gap = 8;

        //Only 3 upgrades at a time
        private UpgradePanel leftUpgrade;
        private UpgradePanel rightUpgrade;
        private UpgradePanel middleUpgrade;
        Upgrade chosenUpgrade = null;

        //Tween positions
        private int startY = -600;
        private int endY;
        private Tween.ObjectTween fallIn;

        //Input
        MouseState mouseState;
        MouseState prevMouseState;

        //Customizable
        private int maxItem = 15;
        private Rectangle hudLocation;
        private Rectangle timeLocation;
        private TextBox timerBox;

        //XP
        private Slider xpSlider;
        float xpPercent;
        private TextBox xpDebugText;

        //Health
        private Slider healthSlider;
        float healthPercent;

        //Health
        private Player player;
        private Slider boostSlider;
        float boostPercent;


        //Debug
        private int debugUpgradeX = 25;
        private int debugUpgradeY = 25;
        private int upgradeGap = 70;
        private List<UpgradePanel> upgradeTests = new List<UpgradePanel>();

        //Fonts
        SpriteFont edosz;
        /// <summary>
        /// If upgrading, pause game
        /// </summary>
        public bool IsUpgrading
        {
            get { return isUpgrading; }
        }

        public int IconSize
        {
            get 
            {
                int count = UpgradeManager.Instance.PlayerUpgrades.Count;

                //cap the expansion to the max items in column
                count = Math.Min(count, maxItem);
                int width = ((height - 500) - (count * gap))  / count;
                if(count > 5)
                {
                    return width ;
                }
                else
                {
                    return 32 * 3;
                }
            }
        }
        /// <summary>
        /// Initialize replaces constructor. Should only be called once on creation.
        /// </summary>
        public void Initialize(List<Texture2D> UiAssets, SpriteFont font, int width, int height, Player player, SpriteFont m6, ContentManager content)
        {
            this.width = width;
            this.height = height;
            this.font = font;
            this.player = player;
            this.UiAssets = UiAssets;
            upgradePanel = UiAssets[11];
            this.m6 = m6;

            edosz = content.Load<SpriteFont>("Fonts/edosz");
            testDisplayButton = new Button(
                UiAssets[0],                    //normal texture
                UiAssets[1],                     //hovering
                new Rectangle(30, 120, 114, 38), //put in top left
                font);                          //Font if there's text

            //add button to update list
            //buttonList.Add(testDisplayButton);

            testDisplayButton.TextBox.Text = "TEST UPGRADE";

            blueSheet = UpgradeManager.Instance.Sheet;
            frames = UpgradeManager.Instance.Frames;
            UpgradeStartUp(blueSheet);

            //Set asset locations
            hudLocation = new Rectangle(
                25,
                height - ((2*UiAssets[12].Height / 3) + 25),
                2* UiAssets[12].Width / 3,
                2* UiAssets[12].Height / 3);

            timeLocation = new Rectangle(
                860,
                35,
                4*UiAssets[18].Width/9,
                4*UiAssets[18].Height/9);

            timerBox = new TextBox(m6, timeLocation, Color.White);


            //----------------------------------------------------
            //              SLIDER BARS
            //----------------------------------------------------

            int fullWidth = 2 / 3 * UiAssets[16].Width;

            xpSlider = new Slider(
                UiAssets[26],
                new Vector2(
                    18,
                    4),
                    UiAssets[26].Width,
                    UiAssets[26].Height,
                    1
                );

            healthSlider = new Slider(
                UiAssets[13],
                new Vector2(
                    hudLocation.X + 144,
                    hudLocation.Y + 86),
                    UiAssets[13].Width,
                    UiAssets[13].Height,
                    2 / (float) 3
                );

            boostSlider = new Slider(
                UiAssets[15],
                new Vector2(
                    hudLocation.X + 148,
                    hudLocation.Y + 52),
                    UiAssets[15].Width,
                    UiAssets[15].Height,
                    2 / (float)3
                );

            //----------------------------------------------------
            //              DEBUG UPGRADES
            //----------------------------------------------------

            List<Upgrade> upgrades = UpgradeManager.Instance.AvailableUpgrades;

            Upgrade[] newUps = UpgradeManager.Instance.CreateTestUpgrades();
            //Add a button for each upgrade in the list
            for (int i = 0; i <upgrades.Count; i++)
            {

                upgradeTests.Add(new UpgradePanel(
                    upgradePanel,
                    upgradePanel,
                    new Rectangle(debugUpgradeX + (upgradeGap * i), debugUpgradeY, 75, 75),
                    font,
                    m6,
                    null,
                    blueSheet,
                    UiAssets[23],
                    UiAssets[24],
                    UiAssets[25],
                    edosz));
                upgradeTests[i].Upgrade = newUps[i];

                upgradeTests[i].SpriteScale = 1;
                upgradeTests[i].UpdateTextBox();
            }

            xpDebugText = new TextBox(
                font,
                new Rectangle(
                    250, height - 57, 100, 30),
                Color.White);

        }

        public void DrawHud(SpriteBatch _spriteBatch, double timer)
        {
            //Draw the Hud
            _spriteBatch.Draw(
                UiAssets[22],
                new Rectangle(0, 0, width, height),
                Color.White);

            //Health Bar Hud
            _spriteBatch.Draw(
                UiAssets[12],
                hudLocation,
                Color.White);

            //Health Bar
            healthSlider.Draw(_spriteBatch);

            // Boost Bar
            _spriteBatch.Draw(
                UiAssets[14],
                hudLocation,
                Color.White);

            //Draw boost percent
            boostSlider.Draw(_spriteBatch);

            //Xp
            xpSlider.Draw(_spriteBatch);

            _spriteBatch.End();

            //Start a new spriteBatch with no filtering
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            //Player Icon
            _spriteBatch.Draw(
                player.Asset,
                new Vector2(hudLocation.X + 34, hudLocation.Y + 25),
                new Rectangle(0, 0, player.Asset.Width, 35),
                Color.White,
                0,
                Vector2.Zero,
                2.75f,
                SpriteEffects.None,
                0);
            _spriteBatch.End();
            _spriteBatch.Begin();
            /*

            //Time
            _spriteBatch.Draw(
                UiAssets[18],
                timeLocation,
                Color.White);    
            */

            int totalSeconds = (int)timer;
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            timerBox.Text = $"{minutes:00}:{seconds:00}";

            timerBox.Draw(_spriteBatch);
        }

        /// <summary>
        /// Creates 3 panels, adds them to the lists, and position them
        /// </summary>
        private void UpgradeStartUp(Texture2D sheet)
        {
            //Create the 3 upgrade panels
            leftUpgrade = new UpgradePanel(
                upgradePanel,
                upgradePanel,
                new Rectangle(
                    0,                          //x
                    0,                          //y
                    405,                        //width
                    378),                       //height
                    font,
                    m6,
                    null,
                    sheet,
                    UiAssets[23],
                    UiAssets[24],
                    UiAssets[25]
                    ,edosz);

            rightUpgrade = new UpgradePanel(
                upgradePanel,
                upgradePanel,
                new Rectangle(
                    0,                          //x
                    0,                          //y
                    405,                        //width
                    378),                       //height
                    font,
                    m6,
                    null,
                    sheet,
                    UiAssets[23],
                    UiAssets[24],
                    UiAssets[25]
                    , edosz);

            middleUpgrade = new UpgradePanel(
                upgradePanel,
                upgradePanel,
                new Rectangle(
                    0,                          //x
                    0,                          //y
                    405,                        //width
                    378),                       //height
                    font,
                    m6,
                    null,
                    sheet,
                    UiAssets[23],
                    UiAssets[24],
                    UiAssets[25]
                    , edosz);

            //Add the panels to the list
            upgradePanelList.Add(leftUpgrade);
            upgradePanelList.Add(rightUpgrade);
            upgradePanelList.Add(middleUpgrade);

            //Scale the panels down to max
            foreach (UpgradePanel up in upgradePanelList)
            {
                ScalePanel(up);
            }

            //Position all the panels
            Center(middleUpgrade);
            FloatLeft(leftUpgrade);
            FloatRight(rightUpgrade);

            //tween end position
            endY = leftUpgrade.Y;

            foreach (UpgradePanel up in upgradePanelList)
            {
                up.UpdateTextBox();
            }
        }

        /// <summary>
        /// Creates 3 random upgrades, and adds them to a panel
        /// </summary>
        public void CreateNewUpgrades()
        {
            Upgrade[] upgrades = UpgradeManager.Instance.CreateUpgrades();
            for (int i = 0; i < upgrades.Length; i++) 
            {
                upgradePanelList[i].Upgrade = upgrades[i];
            }
        }

        /// <summary>
        /// Remove the 3 upgrades from the current selection
        /// </summary>
        public void DeleteUpgrades()
        {
            //run 3 times
            for (int i = 0; i < 2; i++)
            {
                //set panel upgrade to be nothing
                upgradePanelList[i].Upgrade = null;
            }
        }

        //Check if button is being hovered or clicked
        public void Update(bool mousePress, bool debugOn)
        {
            //----------------------------------------------------
            //              SLIDER BARS
            //----------------------------------------------------

            //Get percent completion for xp
            xpPercent = UpgradeManager.Instance.XP / (float)UpgradeManager.Instance.XPRequired;
            xpSlider.SetPercent(xpPercent);

            //Get health percentage of player
            healthPercent = player.Health / player.MaxHealth;
            healthSlider.SetPercent(healthPercent);

            //Get boost left
            boostPercent = player.BoostPercent / player.BoostMax;
            boostSlider.SetPercent(boostPercent);

            //Update the test button
            foreach (Button b in buttonList)
            {
                b.Update();
            }

            
            if(testDisplayButton.IsHovering && mousePress)
            {
                isUpgrading = true;
            }

            if (UpgradeManager.Instance.CanLevelUp)
            {
                isUpgrading = true;
                //Reset player stats
                player.Health = player.MaxHealth;
                player.BoostPercent = player.BoostMax;
                fallIn = Tween.CreateTween(startY, endY, 0.6f, EaseType.EaseOut);
                UpgradeManager.Instance.ResetXP();
            }

            //User is no longer upgrading, but the upgrades have not been removed.
            //Delete them
            if (!isUpgrading && leftUpgrade.HasUpgrade) 
            {
                DeleteUpgrades();
            }
            //Update the panel upgrade animations
            foreach (UpgradePanel up in upgradePanelList)
            {
                if (isUpgrading)
                {
                    //Update panel size/hover
                    up.Update();
                }


                //Update animation position
                if (fallIn != null)
                {
                    up.Y = (int)fallIn.currValue;
                }
            }

            //If the player is currently upgrading, check to see if they push a button
            //If the animation hasnt finished, dont let player select, otherwise theyll 
            //accidentally click one
            if (isUpgrading && fallIn != null && fallIn.IsCompleted)
            {

                if (leftUpgrade.IsHovering && mousePress)
                {
                    chosenUpgrade = leftUpgrade.Upgrade;
                    isUpgrading = false;
                }
                else if (rightUpgrade.IsHovering && mousePress)
                {
                    chosenUpgrade = rightUpgrade.Upgrade;
                    isUpgrading = false;

                }
                else if (middleUpgrade.IsHovering && mousePress)
                {
                    chosenUpgrade = middleUpgrade.Upgrade;
                    isUpgrading = false;
                }
                //User selected, give them upgrade
                if (!isUpgrading)
                {
                    UpgradeManager.Instance.GiveUpgrade(chosenUpgrade);
                }

            }

            if (debugOn)
            {
                foreach(UpgradePanel u in upgradeTests)
                {
                    u.Update();
                    if (mousePress && u.IsHovering) 
                    {
                        UpgradeManager.Instance.GiveUpgrade(u.Upgrade);
                    }
                }
            }
        }

        /// <summary>
        /// Reinstance the upgrades so they match players
        /// </summary>
        public void UpdateTestUpgrades()
        {
            Upgrade[] newUps = UpgradeManager.Instance.CreateTestUpgrades();

            //Reassign
            for (int i = 0; i < upgradeTests.Count; i++)
            {
                upgradeTests[i].Upgrade = newUps[i];
            }
        }

        /// <summary>
        /// Draw the debug test upgrade, and panels
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void DebugDraw(SpriteBatch _spriteBatch)
        {

            //Draw all test upgrades
            foreach (UpgradePanel u in upgradeTests)
            {
                u.Draw(_spriteBatch);
                u.DrawUpgradeIcon(_spriteBatch);
            }

            //Draw the xp required on top of xp bar
            xpDebugText.Text = UpgradeManager.Instance.XPRequired.ToString();
            xpDebugText.Draw(_spriteBatch);

            foreach(Button b in buttonList)
            {
                b.Draw(_spriteBatch);
            }
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            if (isUpgrading)
            {
                //Draw all upgrades
                DisplayUpgrade(_spriteBatch);

            }
            //Draw the upgrade icons
            DrawUpgrades(_spriteBatch);
        }

        /// <summary>
        /// Draw all upgrade panels
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void DisplayUpgrade(SpriteBatch _spriteBatch)
        {

            if (!leftUpgrade.HasUpgrade)
            {
                CreateNewUpgrades();
            }
            foreach (UpgradePanel up in upgradePanelList)
            {
                up.Draw(_spriteBatch);
                if (up.Rectangle.Y == endY)
                {
                    up.DrawUpgradeText(_spriteBatch);
                }
            }
        }

        /// <summary>
        /// Make the panel go left
        /// </summary>
        /// <param name="up"></param>
        public void FloatLeft(UpgradePanel up)
        {
            up.X = width / 2 - up.Width / 2 - up.Width;
            up.Y = (height - up.Height) / 2;
        }

        /// <summary>
        /// Center the panel
        /// </summary>
        /// <param name="up">panel</param>
        public void Center(UpgradePanel up)
        {
            up.X = width / 2 - up.Width / 2;
            up.Y = (height - up.Height) / 2;
        }

        /// <summary>
        /// Make the panel go right
        /// </summary>
        /// <param name="up"></param>
        public void FloatRight(UpgradePanel up)
        {
            up.X = width / 2 - up.Width / 2 + up.Width;
            up.Y = (height - up.Height) / 2;
        }

        /// <summary>
        /// Scales panels down to the size that fits the screen
        /// </summary>
        /// <param name="up"></param>
        public void ScalePanel(UpgradePanel up)
        {
            // Get the smallest needed scale factor to fit 3 panels on screen
            // at one given time.
            float scale = Math.Min((float)width / (up.Width * 4), (float)height / (up.Height + 200));

            //Scale the panel down to its maximum size
            int newWidth = (int)(up.Width * scale);
            up.Width = newWidth;
            int newHeight = (int)(up.Height * scale);
            up.Height = newHeight;
        }

        public void DrawUpgrades(SpriteBatch _spriteBatch)
        {
            _spriteBatch.End();
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            List<Upgrade> playerUpgrades = UpgradeManager.Instance.PlayerUpgrades;

            //How far off the wall the icons start
            int x = 50;

            //draw each upgrade the player has
            for (int i = 0; i < playerUpgrades.Count; i++)
            {
                //the row to position icon in
                int row = i % maxItem;
                //the column to position icon in
                int column = i / maxItem;

                // Move x based on column count
                x = 25 + (column * (IconSize + gap + 10));

                // move y based on icon size and gap
                int y = 150 + (row * (IconSize + gap));


                //Draw the box
                 _spriteBatch.Draw(
                    frames,
                    new Rectangle(
                        x,
                        y,
                        (int)(IconSize),
                        (int)(IconSize)),
                    new Rectangle(
                        32 *5,
                        32,
                        32,
                        32),
                    Color.White);

                //Draw the icon
                _spriteBatch.Draw(
                    blueSheet,
                    new Rectangle(
                        x,
                        y,
                        IconSize,
                        IconSize),
                    playerUpgrades[i].SourceRect,
                    Color.White);

                //Only show how many if there is more than 1
                if (playerUpgrades[i].CurrLevel != 1)
                {
                    TextBox box = new TextBox(
                        m6,                       //Font
                        new Rectangle
                        (x +  IconSize,             //X
                        y,                          //Y
                        IconSize / 2,               //Width
                        IconSize / 2),               //Height
                        Color.White,                //Color
                        centerText: false);         //Left align

                    //Text should be the current level the player is at for that upgrade
                    box.Text = "x" + playerUpgrades[i].CurrLevel.ToString();

                    box.DrawRotated(_spriteBatch, -0.35f);
                }
            }
            _spriteBatch.End();
            _spriteBatch.Begin();
        }
    }

}
