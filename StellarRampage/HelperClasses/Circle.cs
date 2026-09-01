using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Threading.Tasks;

namespace StellarRampage.HelperClasses
{
    /// <summary>
    /// Circles have a size and position. Can be used as hitboxes
    /// </summary>
    public class Circle
    {
        //Radius of the circle
        public float Radius;

        //The position the circle is at
        public Vector2 Center;

        /// <summary>
        /// Create a new circle struct
        /// </summary>
        /// <param name="center">position</param>
        /// <param name="radius">size</param>
        public Circle( Vector2 center, float radius)
        {
            //assign the fields of the struct
            this.Radius = radius;
            this.Center = center;
        }
    }
}
