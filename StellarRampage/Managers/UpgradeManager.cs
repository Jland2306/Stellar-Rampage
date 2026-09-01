using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StellarRampage.GameObjects;
using StellarRampage.Miscellaneous;
using System.Linq;
using StellarRampage.GameObjects.Enemies;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework.Content;
using StellarRampage.EnemyDrops;
using StellarRampage.HelperClasses;



namespace StellarRampage.Managers
{
    //---------------------------------------------------------------------
    //                           Static Attributes
    //---------------------------------------------------------------------

    //Class cannot be inherited from
    public sealed class UpgradeManager
    {
        //Creates a new static instance of this manager, there will only be one
        private static UpgradeManager instance = null;

        //The working instance of the class
        public static UpgradeManager Instance
        {
            //Returns the instance if it exists, creates it if not
            get
            {
                if (instance == null)
                {
                    instance = new UpgradeManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// Initialize replaces constructor. Should only be called once on creation.
        /// </summary>
        public void Initialize(Texture2D frames, Texture2D blueUpgrades, Player player, ContentManager content)
        {
            this.frames = frames;
            this.blueUpgrades = blueUpgrades;
            randy = new Random();
            availableUpgrades = new List<Upgrade>();
            playerUpgrades = new List<Upgrade>();

            upgradeFile = Path.Combine("TextFiles", "Upgrades.txt");
            ReadFile(content);
            this.player = player;
        }


        //---------------------------------------------------------------------
        //                          Class Attributes
        //---------------------------------------------------------------------

        enum UpgradeType
        {
            BulletPierce,
            AttackSpeed,
            BoostStrength,
            ExtraAmmo,
            BulletSpeed,
            Orbital,
            Shield,
            BoostDuration,
            BoostRecharge,
            Stabilizer,
            BurstShot,
            Magnetism
        }

        //Assets
        Texture2D frames;
        Texture2D blueUpgrades;
        int spriteWidth = 32;
        private Texture2D xpTexture;

        //Upgrade requirements
        private Random randy;
        private int xp;
        private int level;
        private int XpRequired = 50;
        private float XpGrowth = 1.02f;

        //Data driven
        private string upgradeFile;

        //Orbs
        private List<XpOrb> orbs = new List<XpOrb>();
        private float startingPitch = -0.5f;
        private float lastPickUp;
        private float pickUpTimeRange = 0.4f;
        private int numPickUps;
        private float pitchIncrease = 0.1f;
        private bool giveBonus;
        private float radius = 140;

        private float rangeIncrease = 1.3f;

        //Contains all possible upgrades a player can get
        private List<Upgrade> availableUpgrades;
        //Contains all upgrades the player has
        private List<Upgrade> playerUpgrades;

        private Player player;
        public bool CanLevelUp
        {
            get { return XP >= XpRequired; }
        }

        public Texture2D Sheet
        {
            get { return blueUpgrades; }
        }

        public Texture2D Frames
        {
            get { return frames; }
        }

        public int XP
        {
            get { return xp; }
            set { xp = value; }
        }

        /// <summary>
        /// The amount of Xp needed to level up
        /// </summary>
        public int XPRequired
        {
            get { return XpRequired; }
            set { XpRequired = value; } 
        }

        /// <summary>
        /// This is a risk, but will allow the display of icons in UI Manager
        /// DO NOT CHANGE THIS OUT OF CLASS
        /// </summary>
        public List<Upgrade> PlayerUpgrades
        {
            get { return playerUpgrades; }
        }

        //ONLY USE IN DEUBG
        public List<Upgrade> AvailableUpgrades
        {
            get { return availableUpgrades; }
        }

        /// <summary>
        /// Based on the type of enemy, give a set amount of xp
        /// </summary>
        /// <param name="enemies"></param>
        public void CalculateXP(int amount)
        {
            //Dont give xp in debug
            if (!Game1.IsDebugging)
            {
                xp += amount;
            }
        }

        /// <summary>
        /// Open the upgrade file and create a new upgrade for each line
        /// </summary>
        private void ReadFile(ContentManager content)
        {
            try
            {
                StreamReader reader = new StreamReader(upgradeFile);

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

                            xpTexture = content.Load<Texture2D>(splitData[0][1..^1]);
                        }
                        else
                        {
                            //Split the data using a bar as a separator
                            splitData = line.Split('|');

                            //Location of sprite cord
                            int x = int.Parse(splitData[2]);
                            int y = int.Parse(splitData[3]);

                            //Upgrade effects
                            string desc = splitData[4].Trim();

                            string text = splitData[5].Trim();

                            string type = splitData[6].Trim();

                            //Create a new upgrade from the file
                            availableUpgrades.Add                       //Add to upgrade list
                                (new Upgrade(                           //Upgrade
                                    splitData[0],                       //Name
                                    int.Parse(splitData[1]),            //Max Level
                                    new Rectangle(                      //Source Rect
                                    x * spriteWidth,                    //X
                                    y * spriteWidth,                    //Y
                                    spriteWidth,                        //Width
                                    spriteWidth)                        //Height
                                    , desc,
                                    text,
                                    type)
                                );
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

        /// <summary>
        /// Removes any upgrades that are additions
        /// </summary>
        public void ResetUpgrades()
        {
            //Resets bullets to default
            ProjectileManager.Instance.extraBullets = 0;
            ProjectileManager.Instance.bulletHealth = 1;
            //Reset any fields changed in player
            player.ResetStats();

            //Remove the level
            foreach (Upgrade p in PlayerUpgrades)
            {
                p.CurrLevel = 0;

            }
            PlayerUpgrades.Clear();

            numPickUps = 0;
            orbs.Clear();
            player.UsingSword = false;
            if (player.Sword != null)
            {
                player.Sword.Scale = 1;
            }
            ResetRange(140);
        }

        /// <summary>
        /// Gives pacifist shields
        /// </summary>
        public void GiveShield()
        {
            GiveUpgrade(availableUpgrades[6]);
        }
        /// <summary>
        /// Give the perk bonus to player
        /// </summary>
        public void GiveUpgrade(Upgrade upgrade)
        {
            //Check if player already has a copy
            if (!CheckDuplicate(upgrade))
            {
                //Add the upgrade to the player list for drawing.
                playerUpgrades.Add(upgrade);

                //Reset the current level, its brand new
                upgrade.CurrLevel = 0;
            }

            upgrade.CurrLevel++;
            //Change the amount of XP needed for the next upgrade
            XpRequired = (int)MathF.Pow(XpRequired, XpGrowth);

            Game1.HoveringButton = false;

            //Gives all the debug upgrades a new copy to match the player
            UIManager.Instance.UpdateTestUpgrades();

            string upgradeName = RemoveSpaces(upgrade.Name);
            UpgradeType upgradeChosen;
            Enum.TryParse(upgradeName, out upgradeChosen);

            switch (upgradeChosen)
            {
                case UpgradeType.BulletPierce:
                    //Change the health on bullet to increase
                    ProjectileManager.Instance.bulletHealth++;
                    break;
                case UpgradeType.AttackSpeed:
                    player.ShootDownTime *= 0.8f;
                    //Change the timer on player to reduce speed
                    break;
                case UpgradeType.BoostStrength:
                    //Change the velocity vector on max player boost
                    player.BoostTerminalVelocity *= 1.15f;
                    break;
                case UpgradeType.ExtraAmmo:
                    //Increase the bullets the player fires by 1
                    ProjectileManager.Instance.extraBullets++;
                    player.ShootDownTime /= 0.9f;
                    player.Recoil += 1f;
                    //Increase by 0.5 per bullet
                    player.StabilizerRecoil = 1f * (ProjectileManager.Instance.extraBullets + 1);

                    //Change the size of the sword
                    if (player.UsingSword)
                    {
                        //Increase sword size
                        player.Sword.Scale *= 1.15f;
                    }
                    break;
                case UpgradeType.BulletSpeed:
                    player.BulletSpeed += 0.5f;
                    //Increase the bullets speed after firing
                    break;
                case UpgradeType.Orbital:
                    //Add an orb around player
                    player.SpawnOrbital();
                    break;
                case UpgradeType.Shield:
                    player.SpawnShield();
                    break;
                case UpgradeType.BoostDuration:
                    player.BoostMax += 100;
                    player.BoostPercent = player.BoostMax;
                    break;
                case UpgradeType.BoostRecharge:
                    player.BoostRechargeAmount += 0.1f;
                    break;
                case UpgradeType.Stabilizer:
                    player.UnlockedStabilizer = true;
                    //Increase by 0.5 per bullet
                    player.StabilizerRecoil = 1f * (ProjectileManager.Instance.extraBullets + 1);
                    break;
                case UpgradeType.BurstShot:
                    // Increase player burst shot by 1
                    player.BurstShot++;
                    player.ShootDownTime /= 0.8f;
                    break;
                case UpgradeType.Magnetism:
                    IncreaseRange();
                    break;
            }
        }

        /// <summary>
        /// Returns a bool on whether the player has an upgrade
        /// </summary>
        /// <param name="newUpgrade"></param>
        /// <returns></returns>
        private bool CheckDuplicate(Upgrade newUpgrade)
        {
            //Checks if player has an upgrade already
            foreach (Upgrade u in playerUpgrades)
            {
                //Player has the upgrade return true
                if (u.Name == newUpgrade.Name)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes spaces from the text string
        /// </summary>
        /// <param name="s">string to change</param>
        /// <returns></returns>
        private string RemoveSpaces(string s)
        {
            //Create string builder to attach chars
            StringBuilder newString = new StringBuilder();
            bool shouldCap = true;

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c != ' ')
                {
                    //Make the letter upper or lower case
                    if (shouldCap)
                    {
                        newString.Append(char.ToUpper(c));
                        
                    }
                    else
                    {
                        newString.Append(char.ToLower(c));
                    }
                    //First letter ended, make lower case again
                    shouldCap = false;
                }
                else
                {
                    //Start of new word, make it capital
                    shouldCap = true;
                }
            }
            //Return the built string
            return newString.ToString();
        }


        /// <summary>
        /// Selects random Upgrades from the available choices
        /// </summary>
        /// <returns></returns>
        public Upgrade[] CreateUpgrades()
        {
            //Only 3 upgrades at a time
            Upgrade[] upgrades = new Upgrade[3];

            //Create 3 upgrades
            for (int i = 0; i < 3; i++)
            {
                //Select a random int based on how many upgrades exist
                int randomNum = randy.Next(availableUpgrades.Count);

                //Use copy constructor to get a new Upgrade
                upgrades[i] = CheckPlayerUpgrade(availableUpgrades[randomNum]);
            }

            return upgrades;
        }

        public Upgrade[] CreateTestUpgrades()
        {
            Upgrade[] upgrades = new Upgrade[availableUpgrades.Count];

            //Create all upgrades
            for (int i = 0; i < availableUpgrades.Count; i++)
            {
                //Use copy constructor to get a new Upgrade
                upgrades[i] = CheckPlayerUpgrade(availableUpgrades[i]);
            }

            return upgrades;
        }
        public void ResetXP()
        {
            xp = 0;
        }

        /// <summary>
        /// Checks to see if the player already has a certain upgrade before
        /// giving them another one. That way it can add on, instead of giving
        /// double
        /// </summary>
        private Upgrade CheckPlayerUpgrade(Upgrade newUpgrade)
        {
            //Checks if player has an upgrade already
            foreach(Upgrade u in playerUpgrades)
            {
                //Player has the upgrade return that reference
                if(u.Name == newUpgrade.Name)
                {
                    return u;
                }
            }
            //Player does not have the upgrade, keep the new instance
            return new Upgrade(newUpgrade);
        }


        /// <summary>
        /// Create a new xp orb
        /// </summary>
        /// <param name="pos"></param>
        public void DropXP(Vector2 pos)
        {
            //Add a new xb orb
            orbs.Add(new XpOrb(pos, xpTexture, radius));
        }

        public void DrawOrbs(SpriteBatch _spriteBatch, bool debugOn)
        {
            foreach(XpOrb o in orbs)
            {
                o.Draw(_spriteBatch, debugOn);
            }
        }


        /// <summary>
        /// Update each orb
        /// </summary>
        /// <param name="gameTime"></param>
        public void UpdateOrbs(GameTime gameTime)
        {
            //Iterate backwards to prevent errors when removing
            for(int i = orbs.Count -1; i>= 0; i--)
            {
                orbs[i].Update(gameTime, player.Position);

                if (orbs[i].PickedUp)
                {
                    SoundManager.PlaySound("PickUpAlt", 1f, GetPitch());
                }
                //If the orb is expired, remove it
                if (orbs[i].IsComplete)
                {
                    orbs.RemoveAt(i);
                }
            }

            //Check if player hit the number needed to bonus
            CheckBonus();
        }

        /// <summary>
        /// Player picked up a magnet, increase the hitbox size
        /// </summary>
        public void IncreaseRange()
        {
            foreach(XpOrb o in orbs)
            {
                o.Radius *= rangeIncrease;
            }
            radius *= rangeIncrease;

            DropManager.Instance.IncreaseRange(rangeIncrease);
        }

        public void ResetRange(float amount)
        {
            foreach (XpOrb o in orbs)
            {
                o.Radius = amount;
            }
            radius = amount;

            DropManager.Instance.ResetRange(amount);
        }
        /// <summary>
        /// Increases pitch with the more orbs picked up
        /// </summary>
        private float GetPitch()
        {
            //Just picked up
            lastPickUp = 0;

            //Check if player picked up within the max time
            if (lastPickUp > pickUpTimeRange)
            {
                numPickUps = 0;
            }
            else
            {
                //Increas pick up pitch
                numPickUps++;
            }

            //Pitch after increase
            float pitch = startingPitch + (pitchIncrease * numPickUps);

            //Sound can not go above 1 pitch
            if(pitch > 0.6f)
            {
                //The player has reached max
                numPickUps = 0;
                giveBonus = true;
                pitch = 0.6f;
            }

            return pitch;
        }

        /// <summary>
        /// Check if extra XP should be given
        /// </summary>
        private void CheckBonus()
        {
            if (giveBonus)
            {
                //Give the player their boost back
                player.BoostPercent = player.BoostMax;
                giveBonus = false;
                SoundManager.PlaySound("PickUp", 1f);
            }
        }



    }
}
