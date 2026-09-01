using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StellarRampage.GameObjects;
using StellarRampage.HelperClasses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.Miscellaneous
{
    /// <summary>
    /// Checks collisons between 2 objects
    /// </summary>
    public class CollisionDetection
    {
        //Creates a new static instance of this manager, there will only be one
        private static CollisionDetection instance = null;

        //The working instance of the class
        public static CollisionDetection Instance
        {
            //Returns the instance if it exists, creates it if not
            get
            {
                if (instance == null)
                {
                    instance = new CollisionDetection();
                }
                return instance;
            }
        }

        private Player player;

        /// <summary>
        /// Initialize replaces constructor. Should only be called once on creation.
        /// Grid needs pixel to draw grid lines, and a font to display cell
        /// </summary>
        public void Initialize(Texture2D pixel, SpriteFont arial20, Player player, Camera cam)
        {
            this.player = player;
        }

        /// <summary>
        /// Check a collision for a polygon vs polygon
        /// </summary>
        /// <returns>True if a collision happened</returns>
        public bool PolyVsPoly()
        {

            return true;
        }

        /// <summary>
        /// Checks a collision for a circle vs the player
        /// </summary>
        /// <param name="circle">Returns true is a collision occured</param>
        /// <returns></returns>
        public bool CircleVsPlayer(Circle circle)
        {
            Rectangle playerRect = Player.Hitbox;
            Vector2[] vertices = new Vector2[4];

            //Get the corners of the rectangle
            vertices[0] = new Vector2(playerRect.X, playerRect.Y);
            vertices[1] = new Vector2(playerRect.X, playerRect.Y + playerRect.Height);
            vertices[2] = new Vector2(playerRect.X + playerRect.Width, playerRect.Y + playerRect.Height);
            vertices[3] = new Vector2(playerRect.X + playerRect.Width, playerRect.Y);

            // go through each of the vertices, plus
            // the next vertex in the list
            int n = 0;

            //Go through all the points
            for (int c = 0; c < vertices.Length; c++)
            {
                // get next vertex in list
                // if we've hit the end, wrap around to 0
                n = c + 1;
                if (n == vertices.Length)
                {
                    //Wrap around other side
                    n = 0;
                }

                // get the Vectors at the current position
                Vector2 vc = vertices[c];
                Vector2 vn = vertices[n];

                // check for collision between the circle and
                // a line formed between the two vertices
                bool collision = LineVsCircle(vc, vn, circle);

                //check if a collision happened
                if (collision)
                {
                    return true;
                }
            }
            //Made it to end, no collision
            return false;

        }
        /// <summary>
        /// Check a collision for a circle vs polygon
        /// </summary>
        /// <returns>True if a collision happened</returns>
        public bool CircleVsPoly(Circle circle, Vector2[] vertices)
        {
            // go through each of the vertices, plus
            // the next vertex in the list
            int n = 0;

            //Go through all the points
            for (int c = 0; c < vertices.Length; c++)
            {
                // get next vertex in list
                // if we've hit the end, wrap around to 0
                n = c + 1;
                if (n == vertices.Length)
                {
                    //Wrap around other side
                    n = 0;
                }

                // get the Vectors at the current position
                Vector2 vc = vertices[c];
                Vector2 vn = vertices[n];

                // check for collision between the circle and
                // a line formed between the two vertices
                bool collision = LineVsCircle(vc, vn, circle);

                //check if a collision happened
                if (collision)
                {
                    return true;
                }
            }
            //Made it to end, no collision
            return false;
        }

        /// <summary>
        /// Check a collision for a player vs a polygon
        /// </summary>
        /// <returns>True if a collision happened</returns>
        public bool PlayerVsPoly(Vector2[] vertices)
        {
            // go through each of the vertices, plus
            // the next vertex in the list
            int n = 0;

            //Go through all the points
            for (int c = 0; c < vertices.Length; c++)
            {
                // get next vertex in list
                // if we've hit the end, wrap around to 0
                n = c + 1;
                if (n == vertices.Length)
                {
                    //Wrap around other side
                    n = 0;
                }

                // get the Vectors at the current position
                Vector2 vc = vertices[c];
                Vector2 vn = vertices[n];

                // check for collision between the rect and
                // a line formed between the two vertices
                bool collision = LineVsRect(vc, vn, Player.Hitbox);

                //check if a collision happened
                if (collision)
                {
                    return true;
                }
            }
            //Made it to end, no collision
            return false;
        }

        private bool LineVsRect(Vector2 currV, Vector2 nextV, Rectangle hitbox)
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
        /// Check the collision of a line vs a polygon
        /// </summary>
        /// <param name="lineStart"></param>
        /// <param name="lineEnd"></param>
        /// <param name="vertices"></param>
        public bool LineVsPoly(Vector2 lineStart, Vector2 lineEnd, Vector2[] vertices)
        {
            //Iterate over all the vertexes
            for (int c = 0; c < vertices.Length; c++)
            {
                int n = (c + 1) % vertices.Length;

                // get the Vectors at the current position
                Vector2 vc = vertices[c];
                Vector2 vn = vertices[n];

                // check for collision between the rect and
                // a line formed between the two vertices
                bool collision = LineVsLine(lineStart, lineEnd, vc.X,vc.Y,vn.X,vn.Y);

                //check if a collision happened
                if (collision)
                {
                    return true;
                }
            }
            //Made it to end, no collision
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
        /// <returns>True if collision</returns>
        private bool LineVsLine(Vector2 currV, Vector2 nextV, float x, float y, float w, float h)
        {
            //Compute the 2D parametric line intersection formula
            float intersect1 = ((w - x) * (currV.Y - y) - (h - y) * (currV.X - x)) /
                ((h - y) * (nextV.X - currV.X) - (w - x) * (nextV.Y - currV.Y));

            float intersect2 = ((nextV.X - currV.X) * (currV.Y - y) - (nextV.Y - currV.Y) * (currV.X - x)) /
             ((h - y) * (nextV.X - currV.X) - (w - x) * (nextV.Y - currV.Y));

            //Line intersected
            if (intersect1 >= 0 && intersect1 <= 1 && intersect2 >= 0 && intersect2 <= 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Check a collision for a line vs circle
        /// </summary>
        /// <param name="startVertex">current vector</param>
        /// <param name=" endVertex">next vector</param>
        /// <param name="circle">the circle to check against</param>
        /// <returns></returns>
        private bool LineVsCircle(Vector2 startVertex, Vector2 endVertex, Circle circle)
        {
            //Gets length of line
            float distX = startVertex.X - endVertex.X;
            float distY = startVertex.Y - endVertex.Y;

            //Distance formula
            float len = MathF.Sqrt((distX * distX) + (distY * distY));

            //Dot product from line to circle
            float x = circle.Center.X;
            float y = circle.Center.Y;
            float r = circle.Radius;
            float dot = ((x - startVertex.X) * (endVertex.X - startVertex.X) +
                (y - startVertex.Y) * (endVertex.Y - startVertex.Y)) / MathF.Pow(len, 2);

            dot = Math.Clamp(dot, 0, 1);

            //Find closest point
            float closestX = startVertex.X + (dot * (endVertex.X - startVertex.X));
            float closestY = startVertex.Y + (dot * (endVertex.Y - startVertex.Y));

            distX = closestX - x;
            distY = closestY - y;

            float distance = MathF.Sqrt((distX * distX) + (distY * distY));
            if (distance <= r)
            {
                return true;
            }
            return false;
        }
    }
}
