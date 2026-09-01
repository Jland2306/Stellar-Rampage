using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace StellarRampage.GameObjects.Crusier_Projectiles
{
    internal class Bomb : BossProjectile
    {
        Vector2 drag;

        public Bomb(Texture2D asset, Vector2 pos, Vector2 dirVector, float angle, Texture2D explosion, float speed, float maxTime)
            :base(1, pos, dirVector, angle, explosion, speed, maxTime)
        {
            drag = new Vector2(-1, -1);
        }

        /// <summary>
        /// Moves the bomb with drag
        /// </summary>
        public override void MoveProjectile()
        {
            if (speed > 0)
            {
                AddVelocity(movement * speed, drag, out movement, out speed);
            }
        }
    }
}