using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.GameObjects.Enemies
{
    public class Bomber : Enemy
    {
        public Bomber(List<Texture2D> assets, float health, Vector2 position, float speed, SpriteFont font) : base(assets, health, position, speed, font)
        {
        }
        public override void EnemyStartUp()
        {
            //rotate
            base.EnemyStartUp();
            animHandler.SetIndex(1);
        }

        public override void Update(GameTime gameTime, Vector2 playerPosition)
        {
            base.Update(gameTime, playerPosition);

            //Bombers have shields until they hit 1 health
            if (health == 1)
            {
                //remove the shield
                animHandler.SetIndex(0);
            }
        }
    }
}
