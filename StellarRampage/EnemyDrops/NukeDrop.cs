using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StellarRampage.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellarRampage.Particles;
using StellarRampage.Miscellaneous;
using StellarRampage.Managers;

namespace StellarRampage.EnemyDrops
{
    internal class NukeDrop : Drop
    {
        private int radius = 500;
        public NukeDrop(Vector2 pos, Texture2D asset, Player player, float radius)
            : base(pos, asset, player,radius, 2)
        {
        }

        public override void GiveEffect()
        {
            base.GiveEffect();
            SoundManager.PlaySound("Wave", 2);
            Game1.Cam.TriggerShake(1, 60);
            ShockwaveManager.Instance.AddWave(pos);
        }
    }
}
