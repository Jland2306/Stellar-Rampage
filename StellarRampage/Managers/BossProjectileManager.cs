using Microsoft.Xna.Framework.Graphics;
using StellarRampage.GameObjects;
using StellarRampage.GameObjects.Crusier_Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.Managers
{
    internal sealed class BossProjectileManager
    {

        private static BossProjectileManager instance = null;

        private List<BossProjectile> projectiles; 
        private List<Texture2D> projectileTexture;
        private List<Projectile> removalList;
        private Texture2D pixel;

        public static BossProjectileManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new BossProjectileManager();
                }

                return instance;
            }
        }

        public void Initialize(Texture2D pixel, List<Texture2D> assetlist)
        {
            this.pixel = pixel;
            this.projectileTexture = assetlist;
        }
    }
}
