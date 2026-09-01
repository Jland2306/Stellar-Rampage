using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace StellarRampage.Miscellaneous
{
    /// <summary>
    /// An upgrade contains all data related to 1 boost, player can have
    /// many upgrades at any given time
    /// </summary>
    public class Upgrade
    {
        private string name;
        private int maxLevel;
        private int currLevel;
        private Rectangle sourceRect;
        private string description;
        private string text;
        private string type;
        /// <summary>
        /// Returns the name, helps check for duplicate upgrades
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Returns the source rectangle for the sheet
        /// </summary>
        public Rectangle SourceRect
        {
            get { return sourceRect; }
        }

        //Current level get incremented every time the player gets a duplicate upgrade
        public int CurrLevel
        {
            get { return currLevel; }
            set { currLevel = value; }
        }

        public int MaxLevel
        {
            get { return maxLevel; }
        }

        //Return the perks of the upgrade
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// Add an upgrade to the player
        /// </summary>
        /// <param name="name">Upgrade name</param>
        /// <param name="maxLevel">How many tiers are there</param>
        /// <param name="cords">Where is the sprite in the sheet</param>
        public Upgrade(string name, int maxLevel, Rectangle sourceRect, string description, string text, string type)
        {
            this.name = name;
            this.maxLevel = maxLevel;
            this.sourceRect = sourceRect;
            currLevel = 0;
            this.description = description;
            this.text = text;
            this.type = type;
        }


        /// <summary>
        /// Copy constructor to duplicate Upgrade
        /// </summary>
        /// <param name="u"></param>
        public Upgrade(Upgrade u) 
        {
            this.name = u.name;
            this.maxLevel = u.maxLevel;
            this.currLevel = u.currLevel;
            this.sourceRect = u.sourceRect;
            this.description = u.description;
            this.text = u.text;
            this.type = u.type;
        }

    }
}
