using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StellarRampage.GameObjects.Enemies;
using StellarRampage.HelperClasses;
using StellarRampage.Managers;
using StellarRampage.Miscellaneous;
using StellarRampage.Particles;
using System;
using System.Collections.Generic;

namespace StellarRampage.GameObjects
{
    /// <summary>
    /// Direction used for vertical player movement
    /// </summary>
    public enum VerticalDirection
    {
        None,
        Up,
        Down,
    }

    /// <summary>
    /// Direction used for Horizontal player movement
    /// </summary>
    public enum HorizontalDirection
    {
        None,
        Left,
        Right,
    }

    /// <summary>
    /// The player character controlled using wasd and mouse. Implements GameObject
    /// </summary>
    public class Player : GameObject
    {
        private Rectangle playerRect;

        // Visual player direction state
        private SpriteEffects playerDirection;
        private float gunAngle;

        // Player velocity movement state
        private HorizontalDirection horizontalMovement;
        private VerticalDirection verticalMovement;
        private float diagnalUnitVectorCompontent = (float)Math.Sin(Math.PI / 4);

        // Dust cloud on move
        private ParticleSystem particleSystem;
        private Texture2D particleAsset;

        private Texture2D gun;
        private Texture2D swordTexture;
        private bool usingSword;
        private Sword sword;

        // Speed fields
        private const float drag = 0.965f;
        private float recoil;
        private float bulletSpeed;

        // Boost fields
        private float boostSpeed;
        private float boostRecoil;
        private float boostTerminalVelocity = 4;
        private float boostPercent;
        private float boostMax;
        private Vector2 boostVelocity;
        private float boostRechargeAmount;
        private const float BoostRechargeCooldown = 1000;
        private float boostCooldownTimer;

        // Shoot Modifiers
        private int burstShot;
        private int bulletsAlreadyShot;
        private float burstShotCountdown;

        // Shoot Downtime fields
        private bool autoShoot;
        private bool canShoot;
        private float shootDowntime = 500;
        private float shootTimeCounter;

        // God Mode
        private bool godMode;

        //Collision
        private static Rectangle hitbox;
        private Tween.ColorTween tintTween;

        //Get direction player is moving
        private double maxTime = 3;
        private double currTime;
        private Vector2 startPos;
        private Vector2 endPos;
        private static Vector2 playerDir;

        //Upgrade- orbs
        private OrbManager orbManager;

        //Upgrade- Stabilizer
        private bool unlockedStabilizer;
        private bool stabilizerOn;
        private float stabilizeRecoil = 0.5f;

        //boss fight
        private static bool canMove = true;

        //Transition
        private float scale = 1;
        private float rotation;
        private Vector2 center;

        //Sound Properties
        private bool wasBoosting;
        private bool isBoosting;

        //Rotate on shoot
        private float shotRot = 0f;
        private float rotRecovery = 3f;

        private float maxHealth = 50;
        #region Properties

        /// <summary>
        /// Use the sword sprite
        /// </summary>
        public bool UsingSword
        {
            get { return usingSword; }
            set { usingSword = value; }
        }
        public Sword Sword
        {
            get { return sword; }
        }
        public Texture2D Gun
        {
            set { gun = value; }
        }

        //Buddy gets extra health at the start
        public float MaxHealth
        {
            get { return maxHealth; }
            set { maxHealth = value; }
        }
        //The amount of recoil when stabilizer on,
        //Increase with extra bullets
        public float StabilizerRecoil
        {
            //The recoil should scale with extra bullets
            get { return stabilizeRecoil; }
            set { stabilizeRecoil = value; }
        }

        /// <summary>
        /// Unlock stabilizer
        /// </summary>
        public bool UnlockedStabilizer
        {
            get { return unlockedStabilizer; }
            set {  unlockedStabilizer = value; }
        }

        /// <summary>
        /// Toggle this off when a boss starts
        /// </summary>
        public static bool CanMove
        {
            set { canMove = value; }
        }

        //Allows enemy manager to get the location the player is heading in
        public static Vector2 PlayerDirection
        {
            get { return playerDir; }
        }

        private SpriteFont font;

        /// <summary>
        /// This property is used for upgrades. 
        /// This will allow bullets to become more responsive and snappy
        /// </summary>
        public float BulletSpeed
        {
            get { return bulletSpeed; }
            set { bulletSpeed = value; }
        }

        /// <summary>
        /// This is the downtime in milliseconds between shots. Base value is 500
        /// </summary>
        public float ShootDownTime
        {
            get { return shootDowntime; }
            set { shootDowntime = value; }
        }

        /// <summary>
        /// This is the amount of recoil the player takes per shot
        /// </summary>
        public float Recoil
        {
            get { return recoil; }
            set { recoil = value; }
        }

        /// <summary>
        /// This is a debug GodMode that makes the player invincible and have infinite boost
        /// </summary>
        public bool GodMode
        {
            get { return godMode; }
            set { godMode = value; }
        }

        /// <summary>
        /// The amount of boost the player has left
        /// </summary>
        public float BoostPercent
        {
            get { return boostPercent; }
            set { boostPercent = value; }
        }

        /// <summary>
        /// The max amount of boost
        /// </summary>
        public float BoostMax
        {
            get { return boostMax; }
            set { boostMax = value; }
        }

        /// <summary>
        /// Acceleration value for player boost
        /// </summary>
        public float BoostRecoil
        {
            get { return boostRecoil; }
            set { boostRecoil = value; }
        }

        /// <summary>
        /// The max speed that can be gained with boost
        /// </summary>
        public float BoostTerminalVelocity
        {
            get { return boostTerminalVelocity; }
            set { boostTerminalVelocity = value; }
        }

        /// <summary>
        /// The amount of boost recharged per update call
        /// </summary>
        public float BoostRechargeAmount
        {
            get { return boostRechargeAmount; }
            set { boostRechargeAmount = value; }
        }

        /// <summary>
        /// The player rectangle bounds
        /// </summary>
        public static Rectangle Hitbox
        {
            get { return hitbox; }
        }

        /// <summary>
        /// Amount of bullets burst from a shot
        /// </summary>
        public int BurstShot
        {
            get { return burstShot; }
            set { burstShot = value; }
        }

        #endregion

        public Player(Texture2D asset, Texture2D particleAsset, GraphicsDevice graphicsDevice, SpriteFont font, OrbManager orbManager, Texture2D gun)
            : base(asset, 100, Vector2.Zero)
        {
            //Split the Asset into separate chunks based on number of layers
            playerRect = new Rectangle(
                0,                                  //X
                0,                                  //Y
                32,                                 //Width
                42);                                //Height

            particleSystem = new ParticleSystem(particleAsset);
            CenterPlayer(graphicsDevice);

            //Center of the player
            center = new Vector2(asset.Width / 10, asset.Height / 2);

            burstShot = 1;

            speed = 0;

            // Initialize starting stats
            ResetStats();

            this.font = font;

            hitbox = new Rectangle(
                (int)position.X,
                (int)position.Y,
                asset.Width,
                asset.Height);

            this.orbManager = orbManager;

            this.gun = gun;

            //Remove any active bosses
            EnemyManager.Instance.EndBoss();
        }

        /// <summary>
        /// Draws the Player to screen
        /// </summary>
        /// <param name="_spriteBatch">Monogame Sprite Batch</param>
        /// <param name="debugOn">Whether Debug mode is on</param>
        public override void Draw(SpriteBatch _spriteBatch,bool debugOn)
        {
            //dust first, so player goes on top
            particleSystem.Draw(_spriteBatch);

            _spriteBatch.Draw(
                asset,
                position,
                playerRect,
                Color.White,
                shotRot,
                Vector2.Zero,
                1,
                playerDirection,
                0);
            if (debugOn)
            {
                //Draw the position above player
                _spriteBatch.DrawString(
                    font,
                    RoundedPos,
                    DebugPosition,
                    Color.White
                );
                
                //Draw hitbox
                DebugLib.DrawRectOutline(
                     _spriteBatch,
                     hitbox,
                     3,
                     Color.Red
                 );
            }


            DrawTint(_spriteBatch);

            orbManager.Draw(_spriteBatch, debugOn);

            //------------------------------------
            ///             SWORD               ///
            //------------------------------------
            if (usingSword)
            {
                sword.Draw(_spriteBatch);
                return;
            }


            //Check which way the player is looking
            if (playerDirection == SpriteEffects.FlipHorizontally)
            {
                //Flip the gun to match
                _spriteBatch.Draw(
                    gun,
                    new Vector2((int)position.X + 10, (int)position.Y + 25),
                    new Rectangle(0, 0, gun.Width, gun.Height),
                    Color.White,
                    gunAngle,
                    new Vector2(6, 6),
                    1,
                    SpriteEffects.FlipVertically,
                    0);
            }
            //Facing right
            else
            {
                _spriteBatch.Draw(
                    gun,
                    new Vector2((int)position.X + 15, (int)position.Y + 20),
                    new Rectangle(0, 0, gun.Width, gun.Height),
                    Color.White,
                    gunAngle,
                    new Vector2(6, 6),
                    1,
                    SpriteEffects.None,
                    0);
            }
        }

        public void DrawTransition(SpriteBatch _spriteBatch)
        {
            //dust first, so player goes on top
            particleSystem.Draw(_spriteBatch);

            //Draw each layer of the player
            playerRect.X = 0;

            _spriteBatch.Draw(
                asset,
                position,
                playerRect,
                Color.White,
                rotation,
                center,
                scale,
                playerDirection,
                0);

            //Check which way the player is looking
            if (playerDirection == SpriteEffects.FlipHorizontally)
            {
                //Flip the gun to match
                _spriteBatch.Draw(
                    gun,
                    new Vector2((int)position.X, (int)position.Y + 5),
                    new Rectangle(0, 0, gun.Width, gun.Height),
                    Color.White,
                    rotation,
                    new Vector2(6, 6),
                    scale,
                    SpriteEffects.FlipVertically,
                    0);
            }
            //Facing right
            else
            {
                _spriteBatch.Draw(
                    gun,
                    new Vector2((int)position.X, (int)position.Y),
                    new Rectangle(0, 0, gun.Width, gun.Height),
                    Color.White,
                    rotation,
                    new Vector2(6, 6),
                    scale,
                    SpriteEffects.None,
                    0);
            }
        }

            /// <summary>
            /// Updates the players position and speed
            /// </summary>
            /// <param name="gameTime">GameTime</param>
            /// <param name="kbState">Current Keyboard State</param>
            /// <param name="prevKbState">Previous Keyboard State</param>
            /// <param name="msState">Current Mouse State</param>
            /// <param name="prevMsState">Previous Mouse State</param>
            /// <param name="cameraPos">Camera Position</param>
            /// <param name="screenSize">Screen Size</param>
            public void Update(
            GameTime gameTime,
            KeyboardState kbState,
            KeyboardState prevKbState,
            MouseState msState,
            MouseState prevMsState,
            Vector2 cameraPos,
            Vector2 screenSize)
        {
            //Get the current kb and ms states
            kbState = Keyboard.GetState();
            msState = Mouse.GetState();

            //Update the dust system
            particleSystem.Update(gameTime);

            // Get duration of last game frame
            float lastFrameDuration = (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            shootTimeCounter += lastFrameDuration;
            boostCooldownTimer += lastFrameDuration;
            burstShotCountdown += lastFrameDuration;

            //Rotate back to center
            shotRot = MathHelper.Lerp(shotRot,0, lastFrameDuration / 1000 * rotRecovery);

            // If user is hovering a button, player cannot shoot
            if (Game1.HoveringButton)
            {
                canShoot = false;
            }

            //Toggle the stabilizer
            if(kbState.IsKeyDown(Keys.Space) && prevKbState.IsKeyUp(Keys.Space) && unlockedStabilizer)
            {
                //Switch the stabilizer on/off
                stabilizerOn = !stabilizerOn;
            }

            // If user presses E, toggle autoshoot
            if (kbState.IsKeyDown(Keys.E) && prevKbState.IsKeyUp(Keys.E))
            {
                autoShoot = !autoShoot;
            }

            // If user clicks, holds, or autoshoot is on, and,
            // 0.5 second has elapsed, or, a few milliseconds have passed and still have burst shots,
            // and, player can move
            if ((msState.LeftButton == ButtonState.Pressed || autoShoot) &&
                (canShoot && shootTimeCounter >= 20) && canMove)
            {
                Shoot(msState, cameraPos, screenSize);
                shootTimeCounter = 0;

                // For first shot in burst, start burst countdown
                if(bulletsAlreadyShot == 0)
                {
                    burstShotCountdown = 0;
                }

                bulletsAlreadyShot++;

                // If player shot all bullets from its burst shot
                if (bulletsAlreadyShot >= burstShot)
                {
                    // Start bullet cooldown
                    canShoot = false;
                }
            }

            //The sword position needs to be updated
            if (usingSword)
            {
                //Should the sword be rotated
                if (playerDirection == SpriteEffects.FlipHorizontally)
                {
                    sword.Update(gunAngle, position, gameTime, true);
                }
                else
                {
                    sword.Update(gunAngle, position, gameTime, false);
                }
            }
            // If player stopped holding fire and still has extra bullets in burst,
            // stop allowing player to shoot after some time proportial to bullets in burst
            if (bulletsAlreadyShot >= 0 && burstShotCountdown >= (40 * burstShot))
            {
                canShoot = false;
            }

            //Check if stabilizer is on, then reduce it
            CalculateStabilize();

            #region User Boost
            // Update User Boost directions
            switch (verticalMovement)
            {
                case VerticalDirection.Up:
                    // If user is pressing down 
                    if (kbState.IsKeyDown(Keys.S))
                    {
                        // If user is not pressing up, change state to down
                        if (kbState.IsKeyUp(Keys.W))
                        {
                            verticalMovement = VerticalDirection.Down;
                        }
                        // Else if user is holding down both buttons, switch direction to none
                        else
                        {
                            verticalMovement = VerticalDirection.None;
                        }
                    }
                    else
                    {
                        // If user isnt pressing a button, switch direction to none
                        if (kbState.IsKeyUp(Keys.W))
                        {
                            verticalMovement = VerticalDirection.None;
                        }
                    }

                    break;
                case VerticalDirection.Down:
                    // If user is pressing up 
                    if (kbState.IsKeyDown(Keys.W))
                    {
                        // If user is not pressing down, change state to up
                        if (kbState.IsKeyUp(Keys.S))
                        {
                            verticalMovement = VerticalDirection.Up;
                        }
                        // Else if user is holding down both buttons, switch direction to none
                        else
                        {
                            verticalMovement = VerticalDirection.None;
                        }
                    }
                    else
                    {
                        // If user isnt pressing a button, switch direction to none
                        if (kbState.IsKeyUp(Keys.S))
                        {
                            verticalMovement = VerticalDirection.None;
                        }
                    }

                    break;
                case VerticalDirection.None:
                    // If user is holding up and not down, switch state to up
                    if(kbState.IsKeyDown(Keys.W) && kbState.IsKeyUp(Keys.S))
                    {
                        verticalMovement = VerticalDirection.Up;
                    }
                    // If user is holding down and not up, switch state to down
                    if (kbState.IsKeyDown(Keys.S) && kbState.IsKeyUp(Keys.W))
                    {
                        verticalMovement = VerticalDirection.Down;
                    }
                    break;
            }

            switch (horizontalMovement)
            {
                case HorizontalDirection.Left:
                    // If user is pressing right 
                    if (kbState.IsKeyDown(Keys.D))
                    {
                        // If user is not pressing left, change state to right
                        if (kbState.IsKeyUp(Keys.A))
                        {
                            horizontalMovement = HorizontalDirection.Right;
                        }
                        // Else if user is holding down both buttons, switch direction to none
                        else
                        {
                            horizontalMovement = HorizontalDirection.None;
                        }
                    }
                    else
                    {
                        // If user isnt pressing a button, switch direction to none
                        if (kbState.IsKeyUp(Keys.A))
                        {
                            horizontalMovement = HorizontalDirection.None;
                        }
                    }

                    break;
                case HorizontalDirection.Right:
                    // If user is pressing left 
                    if (kbState.IsKeyDown(Keys.A))
                    {
                        // If user is not pressing right, change state to left
                        if (kbState.IsKeyUp(Keys.D))
                        {
                            horizontalMovement = HorizontalDirection.Left;
                        }
                        // Else if user is holding down both buttons, switch direction to none
                        else
                        {
                            horizontalMovement = HorizontalDirection.None;
                        }
                    }
                    else
                    {
                        // If user isnt pressing a button, switch direction to none
                        if (kbState.IsKeyUp(Keys.D))
                        {
                            horizontalMovement = HorizontalDirection.None;
                        }
                    }

                    break;
                case HorizontalDirection.None:
                    // If user is holding left and not right, switch state to left
                    if (kbState.IsKeyDown(Keys.A) && kbState.IsKeyUp(Keys.D))
                    {
                        horizontalMovement = HorizontalDirection.Left;
                    }
                    // If user is holding right and not left, switch state to right
                    if (kbState.IsKeyDown(Keys.D) && kbState.IsKeyUp(Keys.A))
                    {
                        horizontalMovement = HorizontalDirection.Right;
                    }
                    break;
            }

            // Check if player is using boost
            if((horizontalMovement != HorizontalDirection.None 
                || verticalMovement != VerticalDirection.None ) && canMove)
            {
                // Player is boosting vertically
                if(horizontalMovement == HorizontalDirection.None)
                {
                    if(verticalMovement == VerticalDirection.Up)
                    {
                        Boost(
                            new Vector2(0, -1),
                            boostRecoil);
                    }
                    if (verticalMovement == VerticalDirection.Down)
                    {
                        Boost(
                            new Vector2(0, 1),
                            boostRecoil);
                    }
                }
                // Player is boosting horizontally
                else if (verticalMovement == VerticalDirection.None)
                {
                    if (horizontalMovement == HorizontalDirection.Left)
                    {
                        Boost(
                            new Vector2(-1, 0),
                            boostRecoil);
                    }
                    if (horizontalMovement == HorizontalDirection.Right)
                    {
                        Boost(
                            new Vector2(1, 0),
                            boostRecoil);
                    }
                }
                // Player is boosting diagnally
                else
                {
                    if (horizontalMovement == HorizontalDirection.Left &&
                        verticalMovement == VerticalDirection.Up)
                    {
                        Boost(
                            new Vector2(-1 * diagnalUnitVectorCompontent,
                            -1 * diagnalUnitVectorCompontent),
                            boostRecoil);
                    }
                    if (horizontalMovement == HorizontalDirection.Left && 
                        verticalMovement == VerticalDirection.Down)
                    {
                        Boost(
                            new Vector2(-1 * diagnalUnitVectorCompontent, 
                            diagnalUnitVectorCompontent),
                            boostRecoil);
                    }
                    if (horizontalMovement == HorizontalDirection.Right && 
                        verticalMovement == VerticalDirection.Up)
                    {
                        Boost(
                            new Vector2(diagnalUnitVectorCompontent, -1 * 
                            diagnalUnitVectorCompontent),
                            boostRecoil);
                    }
                    if (horizontalMovement == HorizontalDirection.Right && 
                        verticalMovement == VerticalDirection.Down)
                    {
                        Boost(
                            new Vector2(diagnalUnitVectorCompontent, 
                            diagnalUnitVectorCompontent),
                            boostRecoil);
                    }
                }
            }

            #endregion

            // Move player
            UpdatePosition();
            //Check if player takes damage
            CheckCollision();

            // Check if bullet downtime is over
            if (shootTimeCounter >= shootDowntime)
            {
                // Allow user to shoot again
                bulletsAlreadyShot = 0;
                canShoot = true;
            }

            // Check if boost recharge downtime is over
            if(boostCooldownTimer >= BoostRechargeCooldown)
            {
                if (boostPercent < boostMax)
                {
                    boostPercent += boostRechargeAmount;
                }
            }

            //Update hitbox
            hitbox.X = (int)position.X;
            hitbox.Y = (int)position.Y;

            // Update previous ms state
            prevMsState = msState;

            //Calculate players average position
            if(currTime >= maxTime)
            {
                //Reset timer and get position
                currTime = 0;
                startPos = endPos;
                //Set the new end position
                endPos = position;

            }
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
            //Find out which direction the player is heading
            playerDir = endPos - startPos;
            //update timer
            currTime += time;


            //Creates a new direction vector by getting the player position and
            //subtracting that from the mouses current position
            Vector2 dirVector = new Vector2(
                 (cameraPos.X - screenSize.X / 2 + msState.Position.X) - position.X,
                (cameraPos.Y - screenSize.Y / 2 + msState.Position.Y) - position.Y);

            //Get the angle of the direction vector to rotate the gun
            gunAngle = (float)Math.Atan2(dirVector.Y, dirVector.X);

            // Make player look in direction the mouse is facing
            if (msState.Position.X >= screenSize.X / 2)
            {
                playerDirection = SpriteEffects.None;
            }
            else
            {
                playerDirection = SpriteEffects.FlipHorizontally;
            }

            orbManager.Update(gameTime, speed, new Vector2(position.X + 16, position.Y + asset.Height / 2));
        }

        /// <summary>
        /// Method to center player
        /// </summary>
        /// <param name="graphicsDevice">graphics device</param>
        public void CenterPlayer(GraphicsDevice graphicsDevice)
        {
            int screenWidth = graphicsDevice.Viewport.Width;
            int screenHeight = graphicsDevice.Viewport.Height;

            // center player
            position = new Vector2((screenWidth - asset.Width) / 2, (screenHeight - asset.Height) / 2);

            //Reset health
            health = maxHealth;

        }

        /// <summary>
        /// Method to center player
        /// </summary>
        public void CenterPlayer()
        {
            Vector2 screenCenter = Game1.Cam.CameraPosition;

            // center player
            position = screenCenter;
        }

        //---------------------------------------------------------------------
        //                         Stabilizer
        //---------------------------------------------------------------------

        /// <summary>
        /// Checks if the player has the upgrade, then reduces it
        /// </summary>
        private void CalculateStabilize()
        {
            //Stabilizer uses boost slowly over time
            if (stabilizerOn)
            {
                //Reduce boost
                if (boostPercent <= 0)
                {
                    //toggle stabilizer off
                    //stabilizerOn = false;
                }
                else
                {
                    //Only show stabilizer if player is moving
                    if (movement.Length() > 0.5f)
                    {
                        //decide where the particles will go
                        Vector2 direction;
                        Vector2 assetPos;

                        //Figure out which direction to show the boost particle
                        if (playerDirection == SpriteEffects.None)
                        {
                            //Flow to the left from top left
                            direction = new Vector2(-50, 0);
                            assetPos = new Vector2(0, 15);
                        }
                        else
                        {
                            //Flow to the right from top right
                            direction = new Vector2(50, 0);
                            assetPos = new Vector2(27, 15);
                        }

                        //add the location of the player
                        assetPos += position;

                        //Show the boost particles
                        particleSystem.CreateParticle(assetPos, direction, Color.CadetBlue, 3, 0.75f);
                    }

                }

            }
        }

        //---------------------------------------------------------------------
        //                         SWORD
        //---------------------------------------------------------------------

        public void CreateSword(Texture2D texture)
        {
            usingSword = true;
            sword = new Sword(texture);
        }

        //---------------------------------------------------------------------
        //                         ORBITAL
        //---------------------------------------------------------------------

        public void SpawnOrbital()
        {
            orbManager.SpawnOrbital(position, Color.BlueViolet);
        }

        public void SpawnShield()
        {
            orbManager.SpawnShield(position, Color.BlueViolet);
        }
        //---------------------------------------------------------------------
        //                         Customization
        //---------------------------------------------------------------------

        /// <summary>
        /// Changes the player asset being used
        /// </summary>
        public void SetNewSprite(Texture2D asset)
        {
            this.asset = asset;

            //Make the new hitbox slightly smaller, to be generous
            hitbox = new Rectangle(
                0,
                0,
                asset.Width - 2,
                asset.Width - 2);

            //Update the rectangle
            playerRect = new Rectangle(
                0, 0, asset.Width, asset.Height);
        }

        #region Collision Detection
        //---------------------------------------------------------------------
        //                         Collision Detection
        //---------------------------------------------------------------------

        /// <summary>
        /// Checks if if a player hit an enemy
        /// </summary>
        /// <returns></returns>
        private bool CheckCollision()
        {
            // Gets the 9 cells near the player
            List<Enemy> enemiesNear = Grid.Instance.GetEnemies(position);

            //Iterates over all nearby enemies
            foreach (Enemy e in enemiesNear)
            {
                bool collision = TriangleVsSquare(e.Hitbox);

                //Only hit an enemy once
                if (collision)
                {
                // If godmode, dont take damage collisions
                if (!godMode)
                {
                    TakeDamage(10);
                    }
                    e.TakeDamage(100);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks collisions of a square vs all lines of a triangle
        /// </summary>
        /// <param name="vertices"></param>
        /// <returns></returns>
        private bool TriangleVsSquare(Vector2[] vertices)
        {
            //Starting vertice
            int next = 0;

            for (int i = 0; i < vertices.Length; i++)
            {
                //Get the next vertex in array
                next = i + 1;
                //Hit end of list, restart
                if (next == vertices.Length) next = 0;

                Vector2 currentVertex = vertices[i];
                Vector2 nextVertex = vertices[next];

                //Check if the square collided with that line
                if (LineVsRect(currentVertex, nextVertex))
                {
                    return true;
                }
            }
            return false;
        }

        private bool LineVsRect(Vector2 currV, Vector2 nextV)
        {
            //Check if a line has hit any rectangle side
            bool left = LineVsLine(currV, nextV, hitbox.X, hitbox.Y, hitbox.X, hitbox.Y + hitbox.Height);
            bool right = LineVsLine(currV, nextV, hitbox.X + hitbox.Width, hitbox.Y, hitbox.X + hitbox.Width, hitbox.Y + hitbox.Height);
            bool top = LineVsLine(currV, nextV, hitbox.X, hitbox.Y, hitbox.X + hitbox.Width, hitbox.Y);
            bool bottom = LineVsLine(currV, nextV, hitbox.X, hitbox.Y + hitbox.Height, hitbox.X + hitbox.Width, hitbox.Y + hitbox.Height);

            //One of the rectangle lines intersects
            if (left || right || top || bottom)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Takes the two vertices of a line, plus the line that makes one edge of a rectangle
        /// </summary>
        /// <param name="currV"></param>
        /// <param name="nextV"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <returns></returns>
        private bool LineVsLine(Vector2 currV, Vector2 nextV, float x, float y, float w, float h)
        {
            //Compute the 2D parametric line intersection formula
            float intersect1 = ((w - x) * (currV.Y - y) - (h - y) * (currV.X - x)) /
                ((h - y) * (nextV.X - currV.X) - (w - x)*(nextV.Y - currV.Y));

            float intersect2 = ((nextV.X - currV.X) * (currV.Y - y) - (nextV.Y - currV.Y) * (currV.X - x)) /
             ((h - y) * (nextV.X - currV.X) - (w - x) * (nextV.Y - currV.Y));

            //Line intersected
            if(intersect1 >= 0 && intersect1 <= 1 && intersect2 >= 0 && intersect2 <= 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Applies damage to player with tinted screen
        /// </summary>
        /// <param name="damage"></param>
        public override void TakeDamage(int damage)
        {
            health -= damage;
            TintScreen();
        }

        /// <summary>
        /// Applies damage to player with custom tint
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage, Color color)
        {
            health -= damage;
            TintScreen(color);
        }


        /// <summary>
        /// Turn the whole screen red
        /// </summary>
        private void TintScreen(Color color)
        {
            //Create a new red tint
            tintTween = Tween.CreateColorTween(Color.Transparent, color, 0.15f, EaseType.EaseOut);
            tintTween.OnComplete = () =>
            {
                tintTween = Tween.CreateColorTween(color, Color.Transparent, 0.5f, EaseType.EaseIn);
            };
        }

        /// <summary>
        /// Fixes maxhealth after buddy gets an upgrade
        /// </summary>
        public void ResetHealth()
        {
            health = maxHealth;
        }
        /// <summary>
        /// Turn the whole screen red
        /// </summary>
        private void TintScreen()
        {
            //Apply red tint, standard
            TintScreen(new Color(255, 0, 0, 50));
        }

        private void DrawTint(SpriteBatch _spriteBatch)
        {
            if (tintTween != null)
            {
                _spriteBatch.End();
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);
                _spriteBatch.Draw(
                    Game1.Pixel,
                    new Rectangle(
                        0,
                        0,
                        Game1.Width,
                        Game1.Height),
                    tintTween.currColor);
                _spriteBatch.End();
                //Restart the old spritebatch
                _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, transformMatrix: Game1.Cam.Transform, samplerState: SamplerState.PointClamp);
            }

        }
        #endregion

        //---------------------------------------------------------------------
        //                         Player Movement
        //---------------------------------------------------------------------

        /// <summary>
        /// Calculates added velocity to the total velocity
        /// </summary>
        /// <param name="initalVelocityDirection">Unit vector of direction of inital velocity</param>
        /// <param name="initalVelocityMagnitude">Magnitude of inital velocity</param>
        /// <param name="addedVelocityDirection">Unit vector of direction of added velocity</param>
        /// <param name="addedVelocityMagnitude">Magnitude of added velocity</param>
        /// <param name="totalVelocityDirection">Unit vector of direction of total calculated velocity</param>
        /// <param name="totalVelocityMagnitude">Magnitude of total calculated velocity</param>
        public void AddVelocity(
            Vector2 initalVelocityDirection, float initalVelocityMagnitude, 
            Vector2 addedVelocityDirection, float addedVelocityMagnitude, 
            out Vector2 totalVelocityDirection, out float totalVelocityMagnitude)
        {
            // Add to current movement vector
            Vector2 combinedVector = new Vector2(
                addedVelocityDirection.X * addedVelocityMagnitude +
                initalVelocityDirection.X * initalVelocityMagnitude,
                addedVelocityDirection.Y * addedVelocityMagnitude +
                initalVelocityDirection.Y * initalVelocityMagnitude);

            float combinedVectorMagnitude = (float)Math.Sqrt(
                Math.Pow(combinedVector.X, 2.0) + Math.Pow(combinedVector.Y, 2.0));

            //Only normalize if its not zero. Normalization uses division, so 0,0 will cause the game to bug
            if (combinedVector != Vector2.Zero)
            {
                combinedVector.Normalize();
            }

            // Update speed and movement
            addedVelocityDirection.Normalize();
            totalVelocityMagnitude = combinedVectorMagnitude;
            totalVelocityDirection = combinedVector;
        }

        /// <summary>
        /// Shoots a bullet, causing the player to recoil
        /// </summary>
        /// <param name="msState">Current Mouse state</param>
        /// <param name="cameraPos">Previous Mouse state</param>
        /// <param name="screenSize">Screen size</param>
        public void Shoot(MouseState msState, Vector2 cameraPos, Vector2 screenSize)
        {
            // Obtain recoil Vector based of shooting direction
            Vector2 recoilVector = new Vector2(
                position.X - (cameraPos.X - screenSize.X / 2 + msState.Position.X),
                position.Y - (cameraPos.Y - screenSize.Y / 2 + msState.Position.Y));


            // Reduce down to unit vector
            //Only normalize if its not zero. Normalization uses division, so 0,0 will cause the game to bug
            if (recoilVector != Vector2.Zero)
            {
                recoilVector.Normalize();
            }

            //Stabilizer is on and there is boost left
            if (stabilizerOn && boostPercent > 0 && unlockedStabilizer)
            {
                // Add minimal velocity for stabilizer
                AddVelocity(movement, speed, recoilVector, stabilizeRecoil, out movement, out speed);
                boostCooldownTimer = 0f;
                //reduce boost caused by stabilizer
                boostPercent -= 5f;
            }
            else
            {
                // Add velocity to total velocity
                AddVelocity(movement, speed, recoilVector, recoil, out movement, out speed);
            }

            //Going left or right
            if(recoilVector.X > 0)
            {
                //Add rotation recoil
                shotRot += recoil / 20 * 1;
            }
            else
            {
                //Add rotation recoil
                shotRot += recoil/ 20 * -1;
            }


            //swing a sword if it exists
            if (usingSword)
            {
                sword.Swing();
                ProjectileManager.Instance.AddWave(new Point((int)position.X + 13, (int)position.Y + 21), recoilVector * -bulletSpeed, 0.2f + sword.Scale);
                SoundManager.PlaySoundRandomPitch("SwordSwing", 1.3f);
            }
            else
            {
                // Creates a bullet and shoots in direction of mouse
                ProjectileManager.Instance.AddBullet(new Point((int)position.X + 13, (int)position.Y + 21), recoilVector * -bulletSpeed);
                SoundManager.PlaySoundRandomPitch("Shoot", 0.5f, -0.9f);
            }
        }

        /// <summary>
        /// Knocks the player backwards. Can be used to emulate a laser zap
        /// </summary>
        /// <param name="dir">where to push the player</param>
        /// <param name="amount">how much to knock back</param>
        public void Knockback(Vector2 dir, float amount)
        {
            AddVelocity(movement,speed/10, dir, amount, out movement, out speed);
        }

        /// <summary>
        /// Adds boost to total velocity
        /// </summary>
        /// <param name="boostDirection">Direction of boost</param>
        /// <param name="boostMagnitude">Magnitude of boost</param>
        public void Boost(Vector2 boostDirection, float boostMagnitude)
        {
            // If boost isnt empty
            if(boostPercent > 0)
            {
                // Calculate boost speed and direction
                AddVelocity(boostVelocity, boostSpeed, boostDirection, boostMagnitude, out boostVelocity, out boostSpeed);

                // Limit boost speed to terminal velocity
                if(boostSpeed > boostTerminalVelocity)
                {
                    boostSpeed = boostTerminalVelocity;
                }

                // If not in godMode, lower boost percent
                if (!godMode)
                {
                    boostPercent -= 1;
                }

                //Going left or right
                if (boostDirection.X > 0)
                {
                    //Add rotation from boost
                    shotRot += 0.015f;
                }
                else
                {
                    //Add rotation from boost
                    shotRot += -0.015f;
                }

                //Create a new particle at the center of the player
                particleSystem.CreateParticle(
                    new Vector2(position.X + 16, position.Y + 16),
                    boostDirection,
                    Color.White,                    //Color
                    5,                              //size
                    0.7f);                          //lifeTime

                // Set boost Timer to 0
                boostCooldownTimer = 0;

                isBoosting = true;
            }
        }

        /// <summary>
        /// Update position of player accounting for boost and bullet recoil
        /// </summary>
        public override void UpdatePosition()
        {
            position += (movement * speed) + (boostVelocity * boostSpeed);
            // Slow speed down by a percentage of the speed
            speed *= drag;
            boostSpeed *= drag;
        }

        /// <summary>
        /// Resets stats to starting stats
        /// </summary>
        public void ResetStats()
        {
            // Boost values
            boostSpeed = 0;
            boostRecoil = 1;
            boostMax = 100;
            boostTerminalVelocity = 8;
            boostPercent = boostMax;
            boostRechargeAmount = 0.1f;
            ShootDownTime = 500;
            if(orbManager != null)
            {
                orbManager.Clear();
            }
            recoil = 6f;
            bulletSpeed = 1;

            canShoot = false;
            shootTimeCounter = 500;
            unlockedStabilizer = false;
            autoShoot = false;

            burstShot = 1;
        }


        /// <summary>
        /// Wrap the player to the other side of the screen
        /// </summary>
        public void WrapScreen()
        {
            //left side
            if (position.X < Game1.Cam.CameraPosition.X - Game1.Width / 2)
            {
                //add screen size
                position.X += Game1.Width;
            }
            //Right
            else if (position.X > Game1.Cam.CameraPosition.X + Game1.Width / 2)
            {
                //subtract screen size
                position.X -= Game1.Width;
            }
            //top
            else if (position.Y < Game1.Cam.CameraPosition.Y - Game1.Height / 2)
            {
                //add screen size
                position.Y += Game1.Height;
            }
            //bottom
            else if (position.Y > Game1.Cam.CameraPosition.Y + Game1.Height / 2)
            {
                //subtract screen size
                position.Y -= Game1.Width;
            }
        }

        /// <summary>
        /// Rather than changing the player position directly, call this
        /// method to update using tweens
        /// </summary>
        /// <param name="scale">What size to scale the player</param>
        /// <param name="rot">What rotation</param>
        public void Transition(float scale, float rot)
        {
            this.scale = scale;
            rotation = rot;
        }

        /// <summary>
        /// Puts all changed transition fields back to normal
        /// </summary>
        public void ResetTransition()
        {
            this.scale = 1;
            this.rotation = 0;
        }

        /// <summary>
        /// Moves the player down slowly given an amount
        /// </summary>
        /// <param name="amount"></param>
        public void Fall(float amount)
        {
            position.Y = amount;
        }
    }
}
