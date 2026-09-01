using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StellarRampage.GameObjects;
using StellarRampage.GameObjects.Enemies;
using StellarRampage.HelperClasses;
using StellarRampage.Managers;
using StellarRampage.Particles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StellarRampage.EnemyDrops
{

    //Class cannot be inherited from
    //Class cannot be inherited from
    public sealed class DropManager
    {

        //Creates a new static instance of this manager, there will only be one
        private static DropManager instance = null;
        public static DropManager Instance
        {
            //Returns the instance if it exists, creates it if not
            get
            {
                if (instance == null)
                {
                    instance = new DropManager();
                }
                return instance;
            }
        }


        public void Initialize(Player player)
        {
            this.player = player;
        }

        private Player player;
        private float radius = 140;

        //Randombly select a power up
        List<Type> dropTypes = new List<Type>
        {
            typeof(HealthDrop),
            typeof(NukeDrop),
            typeof(BoostDrop),
        };

        List<Texture2D> assets = new List<Texture2D>();

        private List<Drop> drops = new List<Drop>();
        private Random rng = new Random();

        public void LoadDrops(ContentManager content)
        {
            assets.Add(content.Load<Texture2D>("Drops/HeartDrop"));
            assets.Add(content.Load<Texture2D>("Drops/NukeDrop"));
            assets.Add(content.Load<Texture2D>("Drops/ShieldDrop"));
        }
        /// <summary>
        /// Should an enemy drop something
        /// </summary>
        public void TryToDrop(Vector2 pos)
        {
            //Get a random drop num
            int dropNum = rng.Next(dropTypes.Count);

            //Get that random drop type
            Type dropType = dropTypes[dropNum];

            //Create the drop from a random type
            drops.Add((Drop)Activator.CreateInstance(dropType,
                new object[] {
                    new Vector2(pos.X + 15, pos.Y + 15),
                    assets[dropNum],
                    player,
                    radius
                }));
        }

        public void Update(GameTime gameTime)
        {
            //Iterate backwards to prevent errors when removing
            for (int i = drops.Count - 1; i >= 0; i--)
            {
                drops[i].Update(gameTime, player.Position);

                if (drops[i].PickedUp)
                {
                    
                }
                //If the drop is expired, remove it
                if (drops[i].IsComplete)
                {
                    drops.RemoveAt(i);
                }
            }
        }

        public void IncreaseRange(float rangeIncrease)
        {
            foreach(Drop d in drops)
            {
                d.Radius *= rangeIncrease;
            }
            radius *= rangeIncrease;
        }
        public void ResetRange(float amount)
        {
            foreach (Drop d in drops)
            {
                d.Radius = amount;
            }
            radius = amount;
        }
        public void Draw(SpriteBatch spriteBatch, bool debugOn)
        {
            //Draw the drops
            foreach(Drop d in drops)
            {
                d.Draw(spriteBatch, debugOn);
            }
        }
    }
}
