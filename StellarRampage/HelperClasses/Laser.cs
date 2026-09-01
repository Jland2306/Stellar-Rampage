using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using StellarRampage.Miscellaneous;
using StellarRampage.GameObjects;
using StellarRampage.Managers;

namespace StellarRampage.HelperClasses
{

    /// <summary>
    /// A laser is a collection of laser segments, by offsetting and combining
    /// segments, a solid laser can be created
    /// </summary>
    internal class Laser
    {
        //The collection containing all the laser pieces
        private List<LaserSegment> laserSegments;

        //The spriteSheet of 1 laser segment
        private Texture2D sheet;

        //Which way is the laser moving
        private Vector2 laserDir;

        //If its finished, draw them permanently
        //Else, only draw lasers behind boss
        public bool finishedPlacing = false;

        private int numSegments = 60;

        //normal hitbox so intersects can be used
        private Rectangle hitbox;

        //A flashing laser represents a laser that toggles on and
        //off to allow the player to dodge
        public bool isFlashing;

        //Color to flash with
        private Tween.ColorTween flashColor;

        //How fast does the laser spawn after flashing once
        private float flashDelay = 0.75f;
        private float currDelay;
        private bool startFlash;
        private float duration = 2;
        public bool finished;
 
        private Player player;
        private Vector2 pos;


        /// <summary>
        /// Create a new laser beam
        /// </summary>
        /// <param name="laserAsset"></param>
        public Laser(Texture2D laserAsset, Player player)
        {
            laserSegments = new List<LaserSegment>();
            //Add the asset to field
            sheet = laserAsset;
            this.player = player;
        }

        /// <summary>
        /// Create a set amount of laser segments and 
        /// add them to the list
        /// </summary>
        public void SpawnLaser(Vector2 pos, float scale, float rotation, bool isEdge = false)
        {
            laserSegments.Clear();

            this.pos = pos;

            // Get direction from rotation. Must add an extra 90 degrees as the laser asset it vertical,
            // not horizontal
            Vector2 direction = new Vector2((float)Math.Cos(rotation + MathF.PI / 2), (float)Math.Sin(rotation + MathF.PI / 2));

            //Set laser direction, this will be used to only draw lasers behind the boss, not in front
            laserDir = direction;

            for (int i = 0; i < numSegments; i++)
            {
                //pos represents the start location of the boss.
                //direction is the angle the laser is moving,
                //i is the current laser, and 38 is the size of one laser segment
                //This figures out how far off each segment needs to be to form a full line
                Vector2 position = pos + direction * i * 38;
                laserSegments.Add(new LaserSegment(sheet, (int)position.X, (int)position.Y, scale, rotation));
            }

            // The end position of the laser
            Vector2 laserEnd = pos + direction * numSegments * 38;

            // The laser rotates around the screen. taking the min
            // will indicate which x and y value to use at the start
            float minX = Math.Min(pos.X, laserEnd.X);
            float minY = Math.Min(pos.Y, laserEnd.Y);
            float maxX = Math.Max(pos.X, laserEnd.X);
            float maxY = Math.Max(pos.Y, laserEnd.Y);

            // how many pixels wide the laser is
            float laserWidth = 18;

            // Get the perpendicular vector of the direction vector. using -b/a
            Vector2 perp = new Vector2(-direction.Y, direction.X);

            // go out half the width in the direction perpendicular to the laser
            Vector2 offset = perp * (laserWidth / 2f);

            //Create the rectangle using the top left x and y.
            hitbox = new Rectangle(
                (int)(minX - offset.X),               
                (int)(minY - offset.Y),              
                (int)((maxX - minX) + (offset.X * 2)), //Width
                (int)((maxY - minY) + (offset.Y * 2))  //Height
            );
        }


        /// <summary>
        /// Draw each segment of the laser
        /// </summary>
        /// <param name="_spriteBatch"></param>
        /// <param name="debugOn"></param>
        public void Draw(SpriteBatch _spriteBatch, bool debugOn, Vector2 bossPos)
        {
            foreach (LaserSegment l in laserSegments)
            {
                //Draw all the segments
                if (finishedPlacing)
                {
                    DrawLaser(_spriteBatch, debugOn, l);
                }
                else
                {
                    //Get the distance from the segment to the laser
                    Vector2 bossToSegment = l.Position - bossPos;

                    //The dot product tells which way a vector is pointing
                    //int relation to another. If this vector is negative,
                    //That means the segment is behind the boss. In that case, draw it
                    //Otherwise, ignore it and wait until boss has passed over
                    if (Vector2.Dot(bossToSegment, laserDir) < 0)
                    {
                        DrawLaser(_spriteBatch, debugOn, l);
                    }
                }
            }

            if (debugOn)
            {
                DebugLib.DrawRectOutline(_spriteBatch, hitbox, 2, Color.Red);
            }
        }

        private void DrawLaser(SpriteBatch _spriteBatch, bool debugOn, LaserSegment l)
        {
            if (isFlashing && flashColor == null)
            {
                //Create a new flash color
                flashColor = Tween.CreateColorTween(
                    Color.Transparent,
                    new Color(Color.Orange, 60),
                    1f,
                    EaseType.Linear);

                flashColor.OnComplete = () =>
                {
                    flashColor = Tween.CreateColorTween(new Color(Color.Orange, 60), Color.Transparent, 0.5f, EaseType.EaseOutCubic);
                    //Begin a delay to spawn the real laser
                    startFlash = true;
                };

            }
            else if (isFlashing)
            {
                //Draw with flashing color
                l.Draw(_spriteBatch, debugOn, flashColor.currColor);
            }
            else
            {
                //Draw normal laser
                l.Draw(_spriteBatch, debugOn, Color.White);
            }
        }

        /// <summary>
        /// Checks if a player hits the laser
        /// </summary>
        /// <param name="playerHitbox">the player hitbox</param>
        private void CheckCollision()
        {
            //Check if the player hit the laser and not in godmode
            if (Player.Hitbox.Intersects(hitbox))
            {
                // Get the perpendicular vector of the direction vector. using -b/a
                Vector2 perp = new Vector2(-laserDir.Y, laserDir.X);
                perp.Normalize();

                if (!player.GodMode)
                {
                    //zap the player
                    player.TakeDamage(1, new Color(255, 200, 0, 50));
                }
                //push the player back
                player.Knockback(perp, 10);



                SoundManager.PlaySound("Bounce");
            }
        }

        /// <summary>
        /// Update the animation of the laser sprites
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            foreach (LaserSegment l in laserSegments)
            {
                l.Update(gameTime);
            }

            //Start the delay for spawning
            if (startFlash)
            {
                currDelay += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            //The delay has ended, spawan laser
            if (currDelay > flashDelay && startFlash)
            {
                startFlash = false;
                isFlashing = false;
                //Play with directional
                SoundManager.PlayEnvironmentalSound("ImpactMono", pos);

                SoundManager.PlaySound("ImpactMono", 0.1f);
                Game1.Cam.TriggerShake(0.3f, 5);
                currDelay = 0;
            }

            //Dont check collision until laser is fully down
            if (!isFlashing)
            {
                CheckCollision();
                currDelay += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (currDelay >= duration)
                {
                    finished = true;
                }
            }
        }

        /// <summary>
        /// Remove all laser segments
        /// </summary>
        public void Clear()
        {
            laserSegments.Clear();
            finishedPlacing = false;
        }
    }
}