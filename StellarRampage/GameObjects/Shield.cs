using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StellarRampage.GameObjects.Enemies;
using StellarRampage.Miscellaneous;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.GameObjects
{
    /// <summary>
    /// Shields are orbitals that do not shoot, but instead block damage that hits them
    /// </summary>
    internal class Shield : Orbital
    {
        //Can the shield deflect?
        private bool canHit;

        // Hitbox points
        Vector2 tip;
        Vector2 backLeft;
        Vector2 backRight;

        //Triangle accounting for rotation and position
        Vector2 newTip;
        Vector2 newBackLeft;
        Vector2 newBackRight;

        /// <summary>
        /// Returns an array of the hitbox points
        /// </summary>
        public Vector2[] ShieldVertices
        {
            get
            {
                //the hitbox to return
                Vector2[] hitbox = new Vector2[3];

                //the 3 points in the triangle
                hitbox[0] = newTip;
                hitbox[1] = newBackLeft;
                hitbox[2] = newBackRight;
                return hitbox;
            }
        }

        private float toPlayer;
        public override float TotalAngle
        {
            get { return toPlayer; }
        }

        /// <summary>
        /// Create a new shield
        /// </summary>
        /// <param name="asset">the texture</param>
        /// <param name="health">amount of damage before it breaks/ currently doesnt break</param>
        /// <param name="position">player location</param>
        /// <param name="top">unused for shields, could change the color to match player</param>
        /// <param name="color">Tint of player, unused</param>
        public Shield(Texture2D asset, float health, Vector2 position, Texture2D top, Color color) 
            : base(asset, health, position, top, color)
        {
            //How far off the shield sits
            radiusOffset = 35;

            //Create a hitbox for the shield
            tip = new Vector2(asset.Width, 0);     
            backLeft = new Vector2(-asset.Width / 2, - asset.Height / 2); 
            backRight = new Vector2(-asset.Width / 2, asset.Height / 2);     
        }

        /// <summary>
        /// Shields will not shoot, but still inherit from orb so this 
        /// method can track if the shield can hit an enemy
        /// </summary>
        public override void Shoot(float time)
        {
            //Update time
            timeTillShoot += time;
            //Check if the shield has waited the full duration before hitting again
            if(timeTillShoot >= timeMax)
            {
                //Check if shield hits enemy
                if (CheckCollision())
                {

                }

            }
            return;
        }

        /// <summary>
        /// Draw the shield
        /// </summary>
        /// <param name="_spriteBatch"></param>
        /// <param name="debugOn"></param>
        public override void Draw(SpriteBatch _spriteBatch, bool debugOn)
        {
            //Draw the shield
            base.Draw(_spriteBatch, debugOn);

            //Draw the hitbox in debug
            if(debugOn) DebugLib.DrawTriangleOutline(_spriteBatch, newTip, newBackLeft, newBackRight, 2, Color.Red);
        }

        /// <summary>
        /// Update shield location and hitbox points
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="playerPos"></param>
        /// <param name="angle"></param>
        public override void Update(GameTime gameTime, Vector2 playerPos, float angle)
        {
            //base.Update(gameTime, playerPos, angle);

            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            MouseState mState = Mouse.GetState();

            //Allows the orb to bob around player
            float radius = baseRadius + radiusOffset;


            //Creates a new direction vector by getting the player position and
            //subtracting that from the mouses current position. Need to add screen offset
            //since the map is infinite
            Vector2 dirVector = new Vector2(
                 (Game1.Cam.CameraPosition.X - Game1.Width / 2 + mState.Position.X) - playerPos.X,
                (Game1.Cam.CameraPosition.Y - Game1.Height / 2 + mState.Position.Y) - playerPos.Y);

            float newAngle = MathF.Atan2(dirVector.Y, dirVector.X);


            newAngle += AngleOffset;

            //Polar offset, follow circle
            Vector2 offset = new Vector2(
                (float)Math.Cos(newAngle),
                (float)Math.Sin(newAngle)) * radius;


            //the orb will always follow player position
            position = playerPos + offset;

            //Tries to deflect
            Shoot(time);

            // Angle to face the player
            Vector2 playerDir =  position - playerPos;
            toPlayer = MathF.Atan2(playerDir.Y, playerDir.X);

            //Create a rotation matrix to adjust the triangle points
            Matrix rotationMatrix = Matrix.CreateRotationZ(toPlayer);

            // Update Hitbox
            newTip = Vector2.Transform(tip, rotationMatrix) + position;
            newBackLeft = Vector2.Transform(backLeft, rotationMatrix) + position; 
            newBackRight = Vector2.Transform(backRight, rotationMatrix) + position;
        }



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
                bool collision = PolyVsPoly(e.Hitbox);

                //Only hit an enemy once
                if (collision)
                {
                    timeTillShoot = 0;
                    e.TakeDamage(1);
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
        private bool PolyVsPoly(Vector2[] vertices)
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
                if (LineVsPoly(currentVertex, nextVertex))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Compares all edges of a polygon vs a line
        /// </summary>
        /// <param name="currV"></param>
        /// <param name="nextV"></param>
        /// <returns></returns>
        private bool LineVsPoly(Vector2 currV, Vector2 nextV)
        {

            // go through each of the vertices, plus the next
            // vertex in the list
            int next = 0;
            for (int current = 0; current < ShieldVertices.Length; current++)
            {

                // get next vertex in list
                // if it hits the end, wrap around to 0
                next = current + 1;
                if (next == ShieldVertices.Length) next = 0;

                // get the points at current position
                float x3 = ShieldVertices[current].X;
                float y3 = ShieldVertices[current].Y;
                float x4 = ShieldVertices[next].X;
                float y4 = ShieldVertices[next].Y;

                // do a Line/Line comparison
                // if true, return 'true' immediately
                if (LineVsLine(currV.X, currV.Y, nextV.X, nextV.Y, x3, y3, x4, y4))
                {
                    return true;
                }
            }
            // never got a hit
            return false;
        }

        /// <summary>
        /// Checks if two lines are colliding
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="x3"></param>
        /// <param name="y3"></param>
        /// <param name="x4"></param>
        /// <param name="y4"></param>
        /// <returns></returns>
        private bool LineVsLine(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
        {

            // calculate the direction of the lines
            float uA = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));
            float uB = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));

            // if uA and uB are between 0-1, lines are colliding
            if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
            {
                return true;
            }
            return false;
        }
    }



}

