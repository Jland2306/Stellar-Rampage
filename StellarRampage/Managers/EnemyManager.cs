using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StellarRampage.EnemyDrops;
using StellarRampage.GameObjects;
using StellarRampage.GameObjects.Enemies;
using StellarRampage.HelperClasses;
using StellarRampage.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace StellarRampage.Managers
{

    //Class cannot be inherited from
    public sealed class EnemyManager
    {
        //Creates a new static instance of this manager, there will only be one
        private static EnemyManager instance = null;

        //Allows each enemy to spawn from there type, not the enemy
        List<Type> enemyTypes = new List<Type>
        {
            typeof(Scout),
            typeof(Frigate),
            typeof(Fighter),
            typeof(Bomber),
        };

        // Enemy Data
        private List<Enemy> enemies;
        private List<string> enemyNames;
        private List<List<Texture2D>> enemyAssets;
        private List<int> enemyMaxHealth;
        private List<int> contactDamage;
        private List<float> movementSpeed;
        private List<Enemy> explodingEnemies;


        // Spawn Data
        private double clock;
        private double bossClock;
        private float bossSpawn = 0.75f;
        private int bossEnemyCount = 10;
        private int currEnemy;
        private float totalSpawnrate = 3;

        //Stop spawns at start of boss
        private bool canSpawn = true;

        private Rectangle topSpawnArea;
        private Rectangle bottomSpawnArea;
        private Rectangle rightSpawnArea;
        private Rectangle leftSpawnArea;

        private Random rng;

        private Vector2 spawnLocation;

        private double spawnRateAmpOverTime;

        //Used to show enemy position in debug
        private SpriteFont debugFont;

        //Controls all the bosses
        private BossManager bossManager;
        private float bossSpawnTime = 300;
        private float timeTillBoss = 0;

        //Explosion
        private bool isExploding;

        //Chance for something to drop over xp
        private int dropChance = 10;
        public bool KeepSpawning
        {
            get { return currEnemy < bossEnemyCount; }
        }
        //The working instance of the class
        public static EnemyManager Instance
        {
            //Returns the instance if it exists, creates it if not
            get
            {
                if (instance == null)
                {
                    instance = new EnemyManager();
                }
                return instance;
            }
        }

        /// <summary>
        /// Initialize replaces constructor. Should only be called once on creation.
        /// </summary>
        public void Initialize(SpriteFont debugFont)
        {

            // Enemy Data
            enemies = new List<Enemy>();
            enemyNames = new List<string>();
            explodingEnemies = new List<Enemy>();

            // Asset data
            enemyAssets = new List<List<Texture2D>>();

            // Numerical Data
            enemyMaxHealth = new List<int>();
            contactDamage = new List<int>();
            movementSpeed = new List<float>();


            // Spawn Data
            rng = new Random();

            topSpawnArea = new Rectangle(0, 0, Game1.Width, 1);
            bottomSpawnArea = new Rectangle(0, 0, Game1.Width, 1);
            rightSpawnArea = new Rectangle(0, 0, 1, Game1.Height);
            leftSpawnArea = new Rectangle(0, 0, 1, Game1.Height);

            spawnLocation = new Vector2();

            spawnRateAmpOverTime = 1;

            this.debugFont = debugFont;
        }

        /// <summary>
        /// loads the file and textures, should only be called once, in LoadContent.
        /// </summary>
        /// <param name="file">The file to load enemy data from</param>
        /// <param name="contentManager"> A reference to the contentManager</param>
        public void LoadContent(string file, ContentManager contentManager, Player player)
        {
            string enemiesPath = Path.Combine("TextFiles", "Enemies.txt");

            StreamReader streamReader = new StreamReader(enemiesPath);

            //Load the boss manager
            bossManager = new BossManager(contentManager, player, this);

            // Holder for asset File string
            List<string> enemyTextureFiles = new List<string>();

            try
            {
                string lineData;
                string[] splitData;

                while ((lineData = streamReader.ReadLine()) != null)
                {

                    // Skip lines that begin with '-'
                    if (lineData[0] != '-')
                    {
                        // Data is seperated by '|'
                        splitData = lineData.Split('|');

                        enemyNames.Add(splitData[0]);
                        enemyTextureFiles.Add(splitData[1]);
                        enemyMaxHealth.Add(int.Parse(splitData[2]));
                        contactDamage.Add(int.Parse(splitData[3]));
                        movementSpeed.Add(float.Parse(splitData[4]));
                    }
                }

                // Load all of the assets
                foreach (string assetPath in enemyTextureFiles)
                {
                    string trimmedAssetPath = assetPath.Trim();

                    //Create a new texture sheet list
                    List<Texture2D> sheets = new List<Texture2D>();

                    //Add all the sprite sheets to that list
                    sheets.Add(contentManager.Load<Texture2D>(trimmedAssetPath + "Single"));
                    sheets.Add(contentManager.Load<Texture2D>(trimmedAssetPath + "Shield"));
                    sheets.Add(contentManager.Load<Texture2D>(trimmedAssetPath + "Death"));
                    sheets.Add(contentManager.Load<Texture2D>(trimmedAssetPath + "Trail"));

                    try
                    {
                        sheets.Add(contentManager.Load<Texture2D>(trimmedAssetPath + "Fire"));
                    }
                    catch
                    {
                        //Bomber does not have the fire sheet
                    }

                    //Add the texture sheet list to the list of enemies
                    enemyAssets.Add(sheets);
                }

            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.Write(error.Message);
            }
            finally
            {
                streamReader.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="player"></param>
        public void Update(GameTime gameTime, double gameClock, Vector2 playerPosition)
        {
            updateSpawnLocation(playerPosition);
            IncreaseSpawnRateWithTime(gameClock);

            //Sort the list of enemies by their Y value.
            //The enemies higher up should be drawn first
            enemies.Sort((enemyA, enemyB) =>
            {
                //Compare the two enemies Y 
                return enemyA.Position.Y.CompareTo(enemyB.Position.Y);
            });

            if (canSpawn)
            {
                //Increase spawnRate clock
                clock += gameTime.ElapsedGameTime.TotalSeconds;

                if (clock >= (totalSpawnrate * spawnRateAmpOverTime))
                {
                    // Spawn behind the player
                    if (Player.PlayerDirection.Length() >= 550)
                    {
                        RigSpawnLocation(Player.PlayerDirection);
                    }
                    // Use normal RNG
                    else
                    {
                        chooseSpawnLocation();
                    }

                    ChooseRandomEnemy();
                }

                foreach (Enemy enemy in enemies)
                {
                    enemy.Update(gameTime, playerPosition);
                }
            }

            //Increase time till boss
            timeTillBoss += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Enough time has passed to spawn boss
            if(timeTillBoss >= bossSpawnTime && !Game1.InBoss)
            {
                SpawnBoss();
                Game1.InBoss = true;
            }

            //Update the animation if any enemies got killed
            if (explodingEnemies.Count != 0)
            {
                //iterate backwards to remove any enemies finished exploding
                for (int i = explodingEnemies.Count - 1; i >= 0; i--)
                {
                    if (explodingEnemies[i].Explode(gameTime))
                    {
                        //Finished exploding, remove it
                        explodingEnemies.Remove(explodingEnemies[i]);
                    }
                }
            }


            //update any active boss
            bossManager.Update(gameTime, playerPosition);

            RemoveDestroyedEnemies();

            // Empty out the grid and refill
            Grid.Instance.EmptyCells();
            Grid.Instance.FillCells(enemies);
        }

        /// <summary>
        /// Picks a random enemy to spawn
        /// </summary>
        private void ChooseRandomEnemy()
        {
            //Get a random enemy num
            int enemyNum = rng.Next(enemyMaxHealth.Count);

            //Get that random enemy type
            Type enemyType = enemyTypes[enemyNum];

            //Create an enemy of random type, with the data driven variables
            //This will allow each enemy to initialize using its inherited class
            enemies.Add((Enemy)Activator.CreateInstance(enemyType,
                new object[] {
                    enemyAssets[enemyNum],          //Sheets
                    enemyMaxHealth[enemyNum],       //Health
                    spawnLocation,                  //Position
                    movementSpeed[enemyNum],        //Speed
                    debugFont                       //Debug font
                }));

            //Reduce the clock back to normal
            clock -= (totalSpawnrate * spawnRateAmpOverTime);

            //Add the sheets to the sprite
            enemies[enemies.Count - 1].AddSheets(enemyAssets[enemyNum]);

            //Start any special fields of that enemy
            enemies[enemies.Count - 1].EnemyStartUp();
        }

        public void Draw(SpriteBatch sb, bool debugOn)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Draw(sb, debugOn);
            }
            //draw the death animation
            foreach (Enemy e in explodingEnemies)
            {
                e.Draw(sb, debugOn);
            }

            if (debugOn)
            {
                Debug(sb);
            }

            //Draw any active bosses, bosses always go on top
            bossManager.Draw(sb, debugOn);
        }

        /// <summary>
        /// Clears all enemies, does not clear enemy information
        /// </summary>
        public void ResetEnemies()
        {
            spawnRateAmpOverTime = 1;
            enemies.Clear();
        }

        /// <summary>
        /// Updates the spawn Area to be relative to the player's Position
        /// </summary>
        /// <param name="playerPosition">a vector of the player's world position</param>
        private void updateSpawnLocation(Vector2 playerPosition)
        {
            // Top Spawner's Location
            topSpawnArea.X = (int)(playerPosition.X - Game1.Width / 2);
            topSpawnArea.Y = (int)(playerPosition.Y - Game1.Height / 2);

            // Bottom Spawner's Location
            bottomSpawnArea.X = (int)(playerPosition.X - Game1.Width / 2);
            bottomSpawnArea.Y = (int)(playerPosition.Y + Game1.Height / 2);

            // Right Spawner's Location
            rightSpawnArea.X = (int)(playerPosition.X + Game1.Width / 2);
            rightSpawnArea.Y = (int)(playerPosition.Y - Game1.Height / 2);

            // Left Spawner's Location
            leftSpawnArea.X = (int)(playerPosition.X - Game1.Width / 2);
            leftSpawnArea.Y = (int)(playerPosition.Y - Game1.Height / 2);
        }

        /// <summary>
        /// Spawn behind the player
        /// </summary>
        /// <param name="playerDirection">the direction the player is moving in</param>
        private void RigSpawnLocation(Vector2 playerDirection)
        {
            //Get the angle and convert it to degrees
            float angle = MathF.Atan2(playerDirection.Y, playerDirection.X);
            angle *= 180;
            angle /= MathF.PI;
            //Give advantage to top and bottom spawners, they reach the player faster
            //Left
            if (angle < -140 || angle > 140)
            {
                spawnLocation.X = rng.Next(leftSpawnArea.Left, leftSpawnArea.Right);
                spawnLocation.Y = leftSpawnArea.Y + leftSpawnArea.Height / 2;

            }
            //Top
            else if (angle < -40)
            {
                //Spawn dead middle of top
                spawnLocation.X = topSpawnArea.X + topSpawnArea.Width / 2;
                spawnLocation.Y = rng.Next(topSpawnArea.Top, topSpawnArea.Bottom);
            }
            //Bottom
            else if (angle >= 40)
            {
                //Spawn dead middle of bottom
                spawnLocation.X = bottomSpawnArea.X + bottomSpawnArea.Width / 2;
                spawnLocation.Y = rng.Next(bottomSpawnArea.Top, bottomSpawnArea.Bottom);
            }
            //Right
            else
            {
                spawnLocation.X = rng.Next(rightSpawnArea.Left, rightSpawnArea.Right);
                //Spawn dead middle of right
                spawnLocation.Y = rightSpawnArea.Y + rightSpawnArea.Height / 2;
            }
        }
        /// <summary>
        /// Changes the spawn location to a random spot in the
        /// top, bottom, left or right, spawn area
        /// </summary>
        private void chooseSpawnLocation()
        {
            switch (rng.Next(0, 4))
            {
                // Spawn at Top
                case 0:
                    spawnLocation.X = rng.Next(topSpawnArea.Left, topSpawnArea.Right);
                    spawnLocation.Y = rng.Next(topSpawnArea.Top, topSpawnArea.Bottom);
                    break;

                // Spawn at Bottom
                case 1:
                    spawnLocation.X = rng.Next(bottomSpawnArea.Left, bottomSpawnArea.Right);
                    spawnLocation.Y = rng.Next(bottomSpawnArea.Top, bottomSpawnArea.Bottom);
                    break;

                // Spawn at Right
                case 2:
                    spawnLocation.X = rng.Next(rightSpawnArea.Left, rightSpawnArea.Right);
                    spawnLocation.Y = rng.Next(rightSpawnArea.Top, rightSpawnArea.Bottom);
                    break;

                // Spawn at Left
                case 3:
                    spawnLocation.X = rng.Next(leftSpawnArea.Left, leftSpawnArea.Right);
                    spawnLocation.Y = rng.Next(leftSpawnArea.Top, leftSpawnArea.Bottom);
                    break;
            }
        }

        /// <summary>
        /// Draws debug information of the EnemyManager class (draws the spawn area)
        /// </summary>
        /// <param name="sb"></param>
        private void Debug(SpriteBatch sb)
        {
            DebugLib.DrawRectFill(sb, topSpawnArea, Color.GreenYellow);
            DebugLib.DrawRectFill(sb, bottomSpawnArea, Color.GreenYellow);
            DebugLib.DrawRectFill(sb, rightSpawnArea, Color.GreenYellow);
            DebugLib.DrawRectFill(sb, leftSpawnArea, Color.GreenYellow);

            sb.DrawString(debugFont, $"{spawnRateAmpOverTime}", Vector2.Zero, Color.White);
        }

        /// <summary>
        /// Removes Enemies with 0 health from the enemies list
        /// </summary>
        private void RemoveDestroyedEnemies()
        {
            List<Enemy> enemiesStillAlive = new List<Enemy>();
            //Iterate backwards so the list can be changed if needed
            for(int i = enemies.Count - 1; i >= 0; i--)
            {
                if (enemies[i].Health > 0)
                {
                    enemiesStillAlive.Add(enemies[i]);
                }
                else
                {
                    //Leave the animation for a little so it can animate out
                    if (enemies[i].isExploding)
                    {
                        explodingEnemies.Add(enemies[i]);
                        SoundManager.PlayEnvironmentalSound("ExplodeMono", enemies[i].Position, 1f);

                        if (enemies[i] is Frigate)
                        {
                            enemiesStillAlive.AddRange(SplitEnemy(enemies[i].Position));
                        }
                    }

                    //Pick a random number, if its still 0, drop something other than xp
                    if(rng.Next(dropChance) == 0)
                    {
                        DropManager.Instance.TryToDrop(enemies[i].Position);
                    }
                    //drop xp
                    else
                    {
                        UpgradeManager.Instance.DropXP(enemies[i].Position);
                    }

                }
            }

            enemies = enemiesStillAlive;
        }

        private void IncreaseSpawnRateWithTime(double gameClock)
        {
            spawnRateAmpOverTime = 1 / Math.Pow(1.3, gameClock / 60);
        }

        /// <summary>
        /// removes all enemies from screen. Allows for bosses
        /// to start on a clean slate
        /// </summary>
        private void ClearEnemies()
        {
            enemies.Clear();
        }

        /// <summary>
        /// Clear screen and spawn a new boss
        /// </summary>
        public void SpawnBoss()
        {
            //Turn off enemies
            canSpawn = false;
            Game1.InBoss = true;

            ClearEnemies();
            bossManager.SpawnBoss();
        }

        /// <summary>
        /// Clear screen and spawn a new boss
        /// </summary>
        public void SpawnCrusier()
        {
            //Turn off enemies
            canSpawn = false;
            Game1.InBoss = true;

            ClearEnemies();
            bossManager.SpawnCrusier();
        }


        /// <summary>
        /// Turns off boss mode
        /// </summary>
        public void EndBoss()
        {
            canSpawn = true;
            timeTillBoss = 0;
            Game1.InBoss = false;
        }

        /// <summary>
        /// Adds a lot of time to the clock
        /// </summary>
        public void IncreaseClock()
        {
            clock += 100;
        }

        /// <summary>
        /// Lets a boss summon an enemy for an attack
        /// </summary>
        /// <param name="pos">Where to summon from, usually the boss itself</param>
        public void SummonEnemies(Vector2 pos, GameTime gameTime, Vector2 playerPos)
        {
            //Increase spawnRate clock
            bossClock += gameTime.ElapsedGameTime.TotalSeconds;

            //Check if an enemy can be spawned
            if (bossClock >= (bossSpawn))
            {
                chooseSpawnLocation();
                ChooseRandomEnemy();
                bossClock = 0;
                currEnemy++;
            }

            foreach (Enemy enemy in enemies)
            {
                enemy.Update(gameTime, playerPos);
            }
        }


        /// <summary>
        /// Spawns 2 small enemies in place of a big one
        /// </summary>
        /// <param name="pos"></param>
        private List<Enemy> SplitEnemy(Vector2 pos)
        {
            spawnLocation = pos;

            List<Enemy> newEnemy = new List<Enemy>();

            //Create 2 enemies from the big guy
            for(int i = 0; i < 2; i++)
            {
                newEnemy.Add(new Fighter(
                        enemyAssets[2],          //Sheets
                        enemyMaxHealth[2],       //Health
                        spawnLocation,           //Position
                        movementSpeed[2],        //Speed
                        debugFont                //Debug font
                    ));

                //Add the sheets to the sprite
                newEnemy[i].AddSheets(enemyAssets[2]);

                //Start any special fields of that enemy
                newEnemy[i].EnemyStartUp();

                //add varition so they arent stacked
                spawnLocation = new Vector2(spawnLocation.X + 35, spawnLocation.Y + 35);
            }
            return newEnemy;
        }
        /// <summary>
        /// Update enemy position
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="playerPos"></param>
        public void UpdateEnemies(GameTime gameTime, Vector2 playerPos)
        {
            foreach (Enemy enemy in enemies)
            {
                enemy.Update(gameTime, playerPos);
            }
        }


        /// <summary>
        /// Check if an enemy should blow up from a shockwave
        /// </summary>
        public void ShockwaveCollisionCheck(Vector2 center, float currRadius)
        {
            //Check each enemy collision
            foreach(Enemy e in enemies)
            {
                //Check if the distance between the enemy and center is less than radius.
                //this means the enemy is inside the shockwave
                if((e.Position - center).Length() <= currRadius)
                {
                    //kill the enemy
                    e.TakeDamage(100);
                }
            }
        }
    }
}
