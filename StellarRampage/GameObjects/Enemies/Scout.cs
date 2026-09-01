using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.GameObjects.Enemies
{

    /// <summary>
    /// A scout is an enemy that has no special attributes.
    /// It can shoot but it doesn't
    /// </summary>
    public class Scout : Enemy
    {
        public Scout(List<Texture2D> assets, float health, Vector2 position, float speed, SpriteFont font) : base(assets, health, position, speed, font)
        {
        }

        public override void EnemyStartUp()
        {
            //rotate
            base.EnemyStartUp();

        }
    }
}
