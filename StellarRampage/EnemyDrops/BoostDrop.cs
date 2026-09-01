using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StellarRampage.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.EnemyDrops
{
    internal class BoostDrop : Drop
    {
        private int boostAmount = 75;
        public BoostDrop(Vector2 pos, Texture2D asset, Player player, float radius)
            : base(pos, asset, player, radius, 2)
        {
        }

        /// <summary>
        /// Give the user boost
        /// </summary>
        public override void GiveEffect()
        {
            base.GiveEffect();

            //dont increase passed max
            if (player.BoostPercent <= player.BoostMax - boostAmount)
            {
                //Add boost
                player.BoostPercent += boostAmount;
            }
            else
            {
                //Top the boost off
                player.BoostPercent = player.BoostMax;
            }
        }
    }
}
