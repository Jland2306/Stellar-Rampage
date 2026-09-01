using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StellarRampage.GameObjects;

namespace StellarRampage.Managers
{
    public class BossManager
    {
        //All possible bosses
        private List<Boss> allBosses;
        private Boss activeBoss;

        public BossManager(ContentManager content, Player player, EnemyManager enemyManager)
        {
            activeBoss = null;
            allBosses = new List<Boss>();

            //Add dreadnought to possible bosses
            allBosses.Add(new Dreadnought(content, player, enemyManager));
            allBosses.Add(new BattleCrusier(content));
        }

        /// <summary>
        /// Draw all the bosses
        /// </summary>
        /// <param name="_spriteBatch"></param>
        public void Draw(SpriteBatch _spriteBatch, bool debugOn)
        {
            if (activeBoss != null)
            {
                activeBoss.Draw(_spriteBatch, debugOn);
            }
        }

        /// <summary>
        /// Update the time on the boss sprites
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime, Vector2 playerPos)
        {
            if (activeBoss != null)
            {
                activeBoss.Update(playerPos, gameTime);

                //Boss is dead
                if (activeBoss.Health <= 0)
                {
                    KillBoss();
                    //win the game
                    Game1.GameMode = GameMode.Win;
                }
            }
        }

        private void LoadBosses()
        {

        }

        public void SpawnBoss()
        {
            activeBoss = allBosses[0];
            activeBoss.Reset();
        }

        public void SpawnCrusier()
        {
            activeBoss = allBosses[1];
            activeBoss.Reset();
        }

        public void KillBoss()
        {
            activeBoss = null;
        }
    }
}
