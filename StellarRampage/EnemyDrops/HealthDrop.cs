using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellarRampage.GameObjects;

namespace StellarRampage.EnemyDrops
{
    internal class HealthDrop : Drop
    {
        private int healthAmount = 15;
        public HealthDrop(Vector2 pos, Texture2D asset, Player player, float radius) 
            : base(pos, asset, player, radius)
        {
        }

        public override void GiveEffect()
        {
            base.GiveEffect();

            //dont increase passed max
            if(player.Health <= player.MaxHealth - healthAmount)
            {
                //Add health
                player.Health += healthAmount;
            }
            else
            {
                //Top the health off
                player.Health = player.MaxHealth;
            }


        }

    }
}
