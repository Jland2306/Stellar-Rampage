using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using StellarRampage.GameObjects.Enemies;

namespace StellarRampage.Miscellaneous
{

    /// <summary>
    /// A cell that will hold a collection of enemies present in it
    /// </summary>
    internal class Cell
    {
        public List<Enemy> enemiesInside;

        /// <summary>
        /// Returns true if no enemies are in the cell
        /// </summary>
        public bool IsEmpty
        {
            get { return enemiesInside.Count == 0; }
        }

        /// <summary>
        /// Add an enemy to the list of enemies present in the cell
        /// </summary>
        /// <param name="e">1 enemy</param>
        public void AddEnemy(Enemy e)
        {
            enemiesInside.Add(e);
        }


        /// <summary>
        /// Empties the list of enemies
        /// </summary>
        public void ClearCell()
        {
            enemiesInside.Clear();
        }

        /// <summary>
        /// Creates a new cell and a new list of enemies
        /// </summary>
        public Cell()
        {
            enemiesInside = new List<Enemy>();
        }
    }
}
