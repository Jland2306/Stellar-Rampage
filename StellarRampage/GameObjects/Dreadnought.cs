using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellarRampage.HelperClasses;
using StellarRampage.Managers;
using StellarRampage.Miscellaneous;
namespace StellarRampage.GameObjects
{
    public class Dreadnought : Boss
    {
        /// <summary>
        /// The different phases Dreadnought has
        /// </summary>
        public enum BossPhase
        {
            Start,
            Enclose,
            Wave,
            Beam,
            Spawn
        }

        // Hitbox points
        Vector2 tip;
        Vector2 topLeft;
        Vector2 topRight;
        Vector2 backLeft;
        Vector2 backRight;
        Vector2 backMiddle;

        //Triangle accounting for rotation and position
        Vector2 newTip;
        Vector2 newTopLeft;
        Vector2 newTopRight;
        Vector2 newBackLeft;
        Vector2 newBackRight;
        Vector2 newBackMiddle;

        //If the boss is hit, highlight in debug
        bool isHit;

        //UI HEALTH BAR
        private float maxHealth = 500;
        private Texture2D healthBar;
        private Texture2D barAsset;
        private Slider healthSlider;
        private Rectangle healthPos;

        /// <summary>
        /// Returns an array of the hitbox points
        /// </summary>
        public Vector2[] BossVertices
        {
            get
            {
                //the hitbox to return
                Vector2[] hitbox = new Vector2[6];

                //the 5 points in the boss hitbox
                //Must be in order of checks
                hitbox[0] = newTip;
                hitbox[1] = newTopLeft;
                hitbox[2] = newBackLeft;
                hitbox[3] = newBackMiddle;
                hitbox[4] = newBackRight;
                hitbox[5] = newTopRight;

                return hitbox;
            }
        }

        //Allows stages to be random
        private int numPhases = 4;
        private BossPhase phase = BossPhase.Spawn;

        //Load the content
        public string contentName = "Bosses/Dreadnought/";

        private int spriteWidth = 128;
        private Texture2D laserAsset;
        private List<Laser> lasers;

        private bool canShoot;
        private Vector2 kickback;
        private float drag = 0.99f;
        private int laserCount;
        private float laserDelay = 1;
        private float currDelay = 1;
        private Vector2 playerPos;
        private Vector2 screenCenter;

        private float scale = 3;

        //Fly to back then back in
        private Tween.Vector2Tween resetCenter;
        private Tween.ObjectTween scaleSize;
        private Tween.ColorTween colorTween;

        //Wall of layers
        private int laserGap = 60;
        private int numInWall = 20;
        private bool wallDone;
        private float xOff;
        private Tween.ObjectTween rotationTween;

        //Wave dash
        private bool isLeft;
        private float speed = 25f;
        private Vector2 dir;
        private float yMax;
        private float yMin;
        private float numWaves = 7;
        private int waveCount;
        private float speedUP = 0.8f;
  
        //Start horizontal
        private float laserAngle = MathF.PI / 2;
        private int lastLaser = 0;

        //This allows the boss to start anything needed just once.
        //Then leave it out of the update loop
        private bool startUp = true;
        private bool isFinished = false;

        private Player player;

        private Random randy = new Random();

        //Drifting
        private Vector2 anchor;
        private float time;
        private float amplitudeX = 20f; 
        private float amplitudeY = 5f;
        private float frequencyX = 0.5f;
        private float frequencyY = 1.2f;

        private bool movingOver;

        private EnemyManager enemyManager;

        private float lastHit;
        private float hitDelay = 0.3f;

        private float intialDelay = 0.8f;
        private float startUpCurr;

        //Animation
        private AnimatedSprite sprite;
        public Dreadnought(ContentManager content, Player player, EnemyManager enemyManager)
            : base(null, 100, Vector2.Zero)
        {
            laserAsset = content.Load<Texture2D>(contentName + "Laser");

            //Load the animation states
            baseSheet = content.Load<Texture2D>(contentName + "Base");
            fireSheet = content.Load<Texture2D>(contentName + "Fire");
            destroySheet = content.Load<Texture2D>(contentName + "Destroy");
            shieldSheet = content.Load<Texture2D>(contentName + "Shield");
            trailSheet = content.Load<Texture2D>(contentName + "Trail");
            barAsset = content.Load<Texture2D>(contentName + "BossHealthBar");
            healthBar = content.Load<Texture2D>(contentName + "BossRedHealth");

            //Create an animated sprite
            sprite = new AnimatedSprite(baseSheet, 128, 128, 1, 0.1f, new Rectangle(28, 20, 72, 89), scale: 3);

            //Load the sprites into the sprite
            List<Texture2D> sheets = new List<Texture2D>();
            sheets.Add(baseSheet);
            sheets.Add(trailSheet);
            sheets.Add(destroySheet);
            sheets.Add(fireSheet);
            sheets.Add(shieldSheet);

            sprite.AddSheets(sheets);

            sprite.IsBoss = true;

            //Start out firing lasers
            sprite.SetIndex((int)AnimState.Base);

            //Get a reference to the player. This will allow boss to hover near the player
            this.player = player;

            //Create a new list for all lasers
            lasers = new List<Laser>();

            //Collision Load
            LoadHitbox();

            //Create the health bar
            healthPos = new Rectangle((Game1.Width - (2 * healthBar.Width / 3)) / 2 - 30, 10, 2 * healthBar.Width / 3, 2 * healthBar.Height / 3);
            healthSlider = new Slider(healthBar, new Vector2(healthPos.X, healthPos.Y), healthBar.Width,healthBar.Height, 2/3f);

            this.enemyManager = enemyManager;
        }

        private void LoadHitbox()
        {
            //The dreadnought is scaled 3x, so half is not just half the asset pixels
            float halfWidth = 3 * baseSheet.Width / 2f;
            float halfHeight = 3 * baseSheet.Height / 2f;

            //Create a hitbox for the boss. Base sheet represents 1 asset
            tip = new Vector2(0, -halfHeight + 65);
            topLeft = new Vector2(-halfWidth / 2 + 30, -halfHeight / 2);
            topRight = new Vector2(halfWidth / 2 - 30, -halfHeight / 2);
            backLeft = new Vector2(-halfWidth / 2 + 15, 80);
            backRight = new Vector2(halfWidth / 2 - 15, 80);
            backMiddle = new Vector2(0, halfHeight - 50);
        }

        private void UpdateHitbox()
        {

            //Create a rotation matrix to adjust the triangle points
            Matrix rotationMatrix = Matrix.CreateRotationZ(sprite.Rotation);

            // Update Hitbox
            newTip = Vector2.Transform(tip, rotationMatrix) + position;
            newBackLeft = Vector2.Transform(backLeft, rotationMatrix) + position;
            newBackRight = Vector2.Transform(backRight, rotationMatrix) + position;
            newTopLeft = Vector2.Transform(topLeft, rotationMatrix) + position;
            newTopRight = Vector2.Transform(topRight, rotationMatrix) + position;
            newBackMiddle = Vector2.Transform(backMiddle, rotationMatrix) + position;
        }
        public override void Draw(SpriteBatch _spriteBatch, bool debugOn)
        {
            //Draw the laser segments
            //Must be first so boss is on top
            foreach (Laser laser in lasers)
            {
                laser.Draw(_spriteBatch, debugOn, position);
            }

            sprite.DrawRotated(_spriteBatch);

            //Draw the collider
            if (debugOn)
            {
                //Draw each of the points in the hitbox
                foreach(Vector2 p in BossVertices)
                {
                    //Draw the outline
                    DebugLib.DrawCircleOutline(_spriteBatch, p, 2, 10, 2, Color.Red);
                }

                //Starting vertex
                int next = 0;

                for (int i = 0; i < BossVertices.Length; i++)
                {
                    //Get the next vertex in array
                    next = i + 1;
                    //Hit end of list, restart
                    if (next == BossVertices.Length) next = 0;

                    Vector2 currentVertex = BossVertices[i];
                    Vector2 nextVertex = BossVertices[next];

                    //draw the hitbox red if they are hit by a bullet
                    if (isHit)
                    {
                        DebugLib.DrawLine(_spriteBatch, currentVertex, nextVertex, 2f, Color.Red);
                    }
                    else
                    {
                        DebugLib.DrawLine(_spriteBatch, currentVertex, nextVertex, 2f, Color.Blue);
                    }

                }
            }

            //End the moving spritebatch
            _spriteBatch.End();
            //Begin one for UI
            _spriteBatch.Begin();
            //Draw the health bar
            _spriteBatch.Draw(barAsset, healthPos, Color.White);
            healthSlider.Draw(_spriteBatch);
            _spriteBatch.End();
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, transformMatrix:Game1.Cam.Transform, samplerState: SamplerState.PointClamp);
        }

        /// <summary>
        /// Check if a bullet hit the boss
        /// </summary>
        /// <returns></returns>
        private bool CheckCollision()
        {
            //Check if each bullet hit
            foreach (Projectile p in ProjectileManager.Instance.Bullets)
            {
                //Check if bullet hit boss for the first time
                if(CollisionDetection.Instance.CircleVsPoly(p.Circle, BossVertices) && !p.HitBoss)
                {
                    //Make sure the bullet cant hit more than once
                    p.BossCollided();

                    //Collision occurred
                    return true;
                }
            }
            //Made it through all the bullets, no collision
            return false;
        }

        /// <summary>
        /// Checks if there was player collision
        /// </summary>
        /// <returns>True if the player collided with the boss</returns>
        private bool PlayerCollision()
        {
            // If player is not in godmode and the 2 objects collide, return true
            if (!player.GodMode && CollisionDetection.Instance.PlayerVsPoly(BossVertices))
            {
                return true;
            }
            return false;
        }

        public override void Update(Vector2 playerPos, GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            this.playerPos = playerPos;

            //Update health bar
            healthSlider.SetPercent(health / maxHealth);

            //The center will always be based on the camera
            screenCenter = Game1.Cam.CameraPosition;

            //Move the hitbox points with boss
            UpdateHitbox();

            //Check if the enemy was hit by a bullet
            if (CheckCollision())
            {
                isHit = true;
                TakeDamage(3);
            }
            else
            {
                isHit = false;
            }

            //Check if the boss smacked into the player
            if (PlayerCollision())
            {
                if(lastHit >= hitDelay)
                {
                    lastHit = 0;
                    player.TakeDamage(5);
                }
            }

            lastHit += (float)gameTime.ElapsedGameTime.TotalSeconds;

            //Set the position to the movement tween
            if(resetCenter != null)
            {
                position = resetCenter.currVector;
            }

            if (colorTween != null)
            {
                sprite.Color = colorTween.currColor;
                if (colorTween.IsCompleted)
                {
                    colorTween = null;
                }
            }

            if(scaleSize != null)
            {
                scale = scaleSize.currValue;
                sprite.Scale = scaleSize.currValue;
                if (scaleSize.IsCompleted)
                {
                    scaleSize = null;
                }
            }

            //Boss made it to center, remove the tween
            if(position == screenCenter)
            {
                resetCenter = null;

            }

            //Update any spawned enemies
            enemyManager.UpdateEnemies(gameTime, playerPos);


            //What stage is the boss in?
            switch (phase)
            {
                //Fly up to the top of screen
                case BossPhase.Start:

                    if (startUp)
                    {
                        //Disable player movement
                        Player.CanMove = false;
                        startUp = false;
                        Game1.Cam.TriggerShake(1f, 100);
                        position = new Vector2(screenCenter.X + 5000, screenCenter.Y + 5000);
                    }
                    if (startUpCurr >= intialDelay)
                    {
                        //Slow down the momentum, add to enemy
                        kickback *= drag;
                        position += kickback;

                        //Update delay
                        currDelay += time;

                        if (currDelay >= laserDelay)
                        {
                            SpawnLasers(playerPos);
                            SoundManager.PlayEnvironmentalSound("ImpactMono", position);
                            SoundManager.PlaySong("OutOfTime", true);
                        }
                    }
                    startUpCurr += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    //Camera follow is disabled in a boss fight;
                    //however this state should allow it so the camera can shake
                    Game1.Cam.CameraFollow(playerPos, gameTime);
                    break;
                //Laser off the walls
                case BossPhase.Enclose:

                    Game1.Cam.UpdateShakeOnly(gameTime);
                    if (startUp)
                    {
                        startUp = false;

                        //Anchor at the top mid
                        anchor = new Vector2((screenCenter.X), screenCenter.Y - 500);
                        sprite.Rotation = 0;

                        laserDelay = 3f;

                        //Start lasers immediate
                        currDelay = laserDelay - 0.1f;
                        waveCount = 0;
                    }

                    //Update delay
                    currDelay += time;

                    //Spawn a wall if there are still rows to show
                    if (currDelay >= laserDelay)
                    {
                        RandomLaser();
                        currDelay = 0f;
                        waveCount++;
                        laserDelay = MathF.Pow(laserDelay, speedUP);
                    }
                    //Check if the number of waves has finished
                    if(waveCount >= numWaves)
                    {
                        NextPhase();
                    }

                    //Float them around the anchor
                    Float(gameTime);
                    break;
                //ZigZag Motion
                case BossPhase.Wave:
                    yMax = screenCenter.Y + 500;
                    yMin = screenCenter.Y - 500;

                    if (startUp)
                    {
                        position = new Vector2(screenCenter.X - 900, screenCenter.Y - 490);
                        waveCount = 0;
                        isFinished = false;
                        NextWave();
                    }
                    //Check if the bounces have finished
                    if(numWaves * 2 == waveCount && !isFinished)
                    {
                        dir = Vector2.Zero;
                        sprite.Rotation = 0;
                        isFinished = true;
                        NextPhase();
                    }
                    if (position.Y > yMax || position.Y < yMin && numWaves * 2 > waveCount && !isFinished)
                    {
                       NextWave();
                        SoundManager.PlayEnvironmentalSound("ImpactMono", position);
                    }
                    Game1.Cam.UpdateShakeOnly(gameTime);
                    position += dir * speed;
                    break;
                //Start at the far left, beam to right. Shut it off every 0.5 seconds
                case BossPhase.Beam:
                    break;
                //Spawn enemies 
                case BossPhase.Spawn:
                    if (startUp)
                    {
                        //Anchor at the top mid
                        anchor = new Vector2((screenCenter.X), screenCenter.Y - 500);
                        startUp = false;
                        movingOver = true;
                    }
                    //Fly center finished
                    if (movingOver)
                    {
                        //Is the number of enemies under the limit?
                        if (enemyManager.KeepSpawning)
                        {
                            //Summon enemies inside
                            enemyManager.SummonEnemies(screenCenter, gameTime, playerPos);
                        }
                        else
                        {
                            NextPhase();
                        }
                        //Hover center
                        Float(gameTime);
                    }
                    break;
            }
            player.WrapScreen();

            //Update the animation frame
            sprite.Update(gameTime);

            //Update the sprite position
            sprite.Position = new Rectangle(
                (int)position.X,                            //X
                (int)position.Y,                            //Y
                sprite.Position.Width,          //Width
                sprite.Position.Height);        //Height

            //Update laser animation
            foreach (Laser l in lasers)
            {
                l.Update(gameTime);
            }

            for (int i = lasers.Count - 1; i > 3; i--)
            {
                if (lasers[i].finished)
                {
                    //Laser has finished, remove
                    lasers.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Choose the next lasers to spawn down
        /// </summary>
        private void RandomLaser()
        {
            if(lastLaser == 0)
            {
                SpawnLasers(0f, 5, 700f);
                lastLaser = 1;
            }
            else
            {
                SpawnLasers(MathF.PI / 2, 3, 300f);
                lastLaser = 0;
            }
        }

        /// <summary>
        /// Takes a direction and number of lasers, spread the lasers across the screen
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="num"></param>
        /// <param name="offset"></param>
        private void SpawnLasers(float dir, int num, float offset)
        {
            //Add the number of lasers requested
            for (int i = 0; i < num; i++)
            {
                Vector2 position = screenCenter;


                //Max of half the offset
                float newOffset = (float)(randy.NextDouble() * offset - offset / 2f);

                //Vertical laser
                if (dir == 0) 
                {
                    //the area to spread across
                    float spread = 1000;
                    //
                    float distance = spread / (num - 1);
                    float x = screenCenter.X - spread / 2f + i * distance + newOffset;
                    position = new Vector2(x, screenCenter.Y - 700);
                }
                //Horizontal laser
                else if (dir == MathF.PI / 2)
                {
                    //the area to spread across. Horizontal has less area to work with
                    float spread = 400;
                    float distance = spread / (num - 1);
                    float y = screenCenter.Y - spread / 2f + i * distance + newOffset;
                    position = new Vector2(screenCenter.X + 1000, y);
                }

                //Spawn a flashing laser
                FlashingLaser(dir, position, 10);
                //Set it finished so it shows right away
                lasers[lasers.Count - 1].finishedPlacing = true;
            }
        }

        /// <summary>
        /// Set the boss back to normal
        /// </summary>
        public override void Reset()
        {
            SoundManager.PlaySound("CinematicShort");
            health = 500;
            kickback = Vector2.Zero;
            laserCount = 0;
            currDelay = 1;
            sprite.Rotation = 0;
            startUp = true;
            foreach (Laser l in lasers)
            {
                l.Clear();
            }

            //Start horizontal
            laserAngle = MathF.PI / 2;
            lasers.Clear();
            laserDelay = 1;
            wallDone = false;
            waveCount = 0;
            phase = BossPhase.Start;
            isFinished = false;
            scale = 3;
            startUpCurr = 0f;
        }

        private void NextWave()
        {
            if (isLeft)
            {
                dir = new Vector2(0.2f, -0.8f);
                
            }
            else
            {
                dir = new Vector2(0.2f, 0.8f);
            }

            if(waveCount >= numWaves)
            {
                dir.X = -dir.X;
                speed = 55;
            }

            //Rotate the sprite in the direction of the angle
            sprite.Rotation = MathF.Atan2(dir.Y, dir.X) + MathF.PI / 2;

            isLeft = !isLeft;
            waveCount++;
            startUp = false;
            Game1.Cam.TriggerShake(0.3f, 10f);
        }
        private void SpawnLasers(Vector2 playerPos)
        {

            switch (laserCount)
            {
                case 0:
                    //Shoot boss from right to left
                    position = new Vector2(playerPos.X + 1000, playerPos.Y + 500);
                    kickback = new Vector2(-44, 0);
                    //Makes a pound on the screen
                    Game1.Cam.TriggerShake(1f, 50f);
                    SoundManager.PlayEnvironmentalSound("ImpactMono", position);

                    SoundManager.PlaySound("ImpactMono", 0.3f);
                    //Add laser
                    AddLaser(laserAngle, position);
                    break;
                case 1:
                    //Makes sure the lasers stay place down
                    lasers[0].finishedPlacing = true;

                    //Shoot boss from bottom left to top left
                    position = new Vector2(playerPos.X - 870, playerPos.Y + 650);
                    kickback = new Vector2(0, -32);
                    //Makes a pound on the screen
                    Game1.Cam.TriggerShake(1f, 50f);
                    SoundManager.PlayEnvironmentalSound("ImpactMono", position);

                    SoundManager.PlaySound("ImpactMono", 0.3f);
                    //Add laser
                    AddLaser(laserAngle, position);
                    break;
                case 2:
                    //Makes sure the lasers stay place down
                    lasers[1].finishedPlacing = true;

                    //Shoot boss from top left to top right
                    position = new Vector2(playerPos.X - 1000, playerPos.Y - 475);
                    kickback = new Vector2(44, 0);
                    //Makes a pound on the screen
                    Game1.Cam.TriggerShake(1f, 50f);
                    SoundManager.PlayEnvironmentalSound("ImpactMono", position);

                    SoundManager.PlaySound("ImpactMono", 0.3f);
                    //Add laser
                    AddLaser(laserAngle, position);
                    break;
                case 3:
                    //Makes sure the lasers stay place down
                    lasers[2].finishedPlacing = true;

                    //Shoot boss from top right to bottom right
                    position = new Vector2(playerPos.X + 900, playerPos.Y - 650);
                    kickback = new Vector2(0, 32);
                    //Makes a pound on the screen
                    Game1.Cam.TriggerShake(1f, 50f);
                    SoundManager.PlayEnvironmentalSound("ImpactMono", position);

                    SoundManager.PlaySound("ImpactMono", 0.3f);
                    //Add laser
                    AddLaser(laserAngle, position);
                    break;
                case 4:
                    //Makes sure the lasers stay place down
                    lasers[3].finishedPlacing = true;


                    //The center will always be based on the camera
                    screenCenter = Game1.Cam.CameraPosition;

                    //Enable movement
                    Player.CanMove = true;

                    //Move to next phase
                    NextPhase();

                    //Reset next phase start up
                    startUp = true;
                    laserCount = -1;
                    laserDelay = 0.4f;
                    break;
            }

            //Increase the angle
            laserAngle += MathF.PI / 2;
            sprite.Rotation = laserAngle + MathF.PI / 2;

            //Restart timer
            currDelay = 0;

            //Move to the next laser in order
            laserCount++;
        }

        /// <summary>
        /// Pick a random boss phase
        /// </summary>
        private void NextPhase()
        {
            //Set the phase equal to the current one
            BossPhase nextPhase = phase;

            //Dont give the same phase, always a new one
            while(nextPhase == phase)
            {
                int ranNum = randy.Next(1, numPhases);
                startUp = true;

                //Pick a random phase
                switch (ranNum)
                {
                    case 1:
                        nextPhase = BossPhase.Enclose;
                        break;
                    case 2:
                        nextPhase = BossPhase.Wave;
                        break;
                    case 3:
                        nextPhase = BossPhase.Spawn;
                        break;
                }
            }

            //Set the new phase
            phase = nextPhase;
        }
        /// <summary>
        /// Adds a laser to barricade the player out
        /// </summary>
        /// <param name="angle">what direction is the laser facing</param>
        /// <param name="pos">where is the laser</param>
        private Laser AddLaser(float angle, Vector2 pos)
        {
            Laser lase = new Laser(laserAsset, player);
            lasers.Add(lase);
            lase.SpawnLaser(pos, 3, angle);

            //Makes a pound on the screen
            Game1.Cam.TriggerShake(1f, 30f);

            return lase;
        }

        /// <summary>
        /// Adds a laser to barricade the player out
        /// </summary>
        /// <param name="angle">what direction is the laser facing</param>
        /// <param name="pos">where is the laser</param>
        /// <param name="shakeAmount">how large the shake is once the laser spawns</param>
        private Laser AddLaser(float angle, Vector2 pos, float shakeAmount)
        {
            Laser lase = new Laser(laserAsset, player);
            lasers.Add(lase);
            lase.SpawnLaser(pos, 3, angle);

            //Makes a pound on the screen
            Game1.Cam.TriggerShake(1f, shakeAmount);

            return lase;
        }

        /// <summary>
        /// Flies behind the screen then back in
        /// </summary>
        private void FlyCenter()
        {
            movingOver = false;
            resetCenter = Tween.CreateVectorTween(position, new Vector2(screenCenter.X - 5300, position.Y), 1f,  EaseType.Linear);

            //Fade back into screen
            resetCenter.OnComplete = () =>
            {

                //Fly back in
                colorTween = Tween.CreateColorTween(Color.Black, Color.White, 3, EaseType.EaseOutCubic);
                scaleSize = Tween.CreateTween(0.2f, 3, 3f, EaseType.EaseOutCubic);
                resetCenter = Tween.CreateVectorTween(position, screenCenter, 2f, EaseType.EaseOut);

                scaleSize.OnComplete = () =>
                {
                    movingOver = true;
                };
            };

        }

        /// <summary>
        /// A flashing laser represents a laser that disables its hitbox
        /// after a certain time, then restarts.
        /// </summary>
        /// <returns></returns>
        private Laser FlashingLaser(float angle, Vector2 pos, float shakeAmount)
        {
            //Create the laser
            Laser lase = AddLaser(angle, pos, 0);
            lase.isFlashing = true;
            return lase;
        }

        /// <summary>
        /// Spawns a wall of lasers to block out player
        /// </summary>
        private void LaserWall()
        {
            //Add the laser with a gap
            FlashingLaser(0, new Vector2((screenCenter.X  - 215 + Game1.Width / 2) - (laserCount * laserGap), screenCenter.Y - 700), 10);
            //Set the laser to show right away
            lasers[lasers.Count - 1].finishedPlacing = true; 
            //Restart timer
            currDelay = 0;

            //Move to the next laser in order
            laserCount++;
        }


        /// <summary>
        /// Remove health from the boss
        /// </summary>
        /// <param name="damage">How much damage should the boss take</param>
        private void TakeDamage(int damage)
        {
            health -= damage;
        }


        /// <summary>
        /// Moves the enemy around so the enemy feels alive
        /// </summary>
        private void Float(GameTime gameTime)
        {
            //increase time
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            float x = MathF.Sin(time * frequencyX) * amplitudeX;
            float y = MathF.Cos(time * frequencyY) * amplitudeY;
            
            //Sets the position to the floating value
            position = anchor + new Vector2(x, y);
        }
    }
}
