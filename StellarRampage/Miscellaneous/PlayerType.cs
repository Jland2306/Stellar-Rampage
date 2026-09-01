using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StellarRampage.HelperClasses;

namespace StellarRampage.Miscellaneous
{

    /// <summary>
    /// A player type is used to show all information in character select
    /// Everything will be public as it has no effect on the actual player
    /// </summary>
    class PlayerType
    {
        public Texture2D Asset;
        public string Name;
        public string Description;
        public int Health;
        public int Damage;
        public Color color;
        public AnimatedSprite sprite;
        public string Ability;
        public SpriteFont Font;

        public PlayerType(Texture2D asset, string name, int health, int damage, Color color, Texture2D sheet, string description, string ability,SpriteFont font)
        {
            Asset = asset;
            Name = name;
            Health = health;
            Damage = damage;
            this.color = color;
            this.Font = font;
            //Create an animation for the selection screen
            sprite = new AnimatedSprite(sheet, sheet.Width / 4, sheet.Height, 4, 0.15f, new Rectangle(350, 550, 32, 42), 13);
            //Calculate by using the width
            sprite.UpdateFrameCount();
            Description = description;
            Ability = ability;
        }

        public void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);
        }

        public void Draw(SpriteBatch _spriteBatch)
        {
            sprite.DrawRotated(_spriteBatch);
        }
    }
}
