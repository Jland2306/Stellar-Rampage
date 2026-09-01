using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using StellarRampage.GameObjects;
using StellarRampage.Managers;
using StellarRampage.GameObjects.Enemies;

namespace StellarRampage.Miscellaneous
{
    //---------------------------------------------------------------------
    //                           Static Attributes
    //---------------------------------------------------------------------
    //Class cannot be inherited from
    public sealed class Grid
    {
        //Creates a new static instance of this manager, there will only be one
        private static Grid instance = null;

        //The working instance of the class
        public static Grid Instance
        {
            //Returns the instance if it exists, creates it if not
            get
            {
                if (instance == null)
                {
                    instance = new Grid();
                }
                return instance;
            }
        }

        /// <summary>
        /// Initialize replaces constructor. Should only be called once on creation.
        /// Grid needs pixel to draw grid lines, and a font to display cell
        /// </summary>
        public void Initialize(Texture2D pixel, SpriteFont arial20, Player player, Camera cam)
        {
            lineThickness = 3;
            this.pixel = pixel;
            grid = new Cell[numWide, numHigh];
            this.arial20 = arial20;
            this.player = player;
            this.cam = cam;

            for (int i = 0; i < numWide; i++)
            {
                    for (int j = 0; j < numHigh; j++)
                    {
                        grid[i, j] = new Cell();
                    }
            }
        }

        //---------------------------------------------------------------------
        //                          Class Attributes
        //---------------------------------------------------------------------


        //Variables needed to draw grid lines/text
        private Texture2D pixel;
        private int lineThickness;
        private Color color = Color.White;
        private SpriteFont arial20;

        //The underlying cell grid which will hold enemies
        private Cell[,] grid;

        //The number of cells and their respective size
        const int cellWidth = 100;
        const int numWide = 24;
        private int numHigh = 14;

        //Moves the grid with the player
        private Vector2 playerPos;
        private Player player;
        private Camera cam;

        /// <summary>
        /// Clears the enemies in each cell
        /// </summary>
        public void EmptyCells()
        {
            for (int x = 0; x < numWide; x++)
            {
                for (int y = 0; y < numHigh; y++)
                {
                    grid[x, y].ClearCell();
                }
            }
        }

        /// <summary>
        /// Finds the cell an enemy is in, and adds it to that cells list
        /// </summary>
        /// <param name="enemies"></param>
        public void FillCells(List<Enemy> enemies)
        {
            Point cellCords;
            foreach (Enemy e in enemies)
            {
                cellCords = FindCell(e.Center);

                //Only add the enemy if its withing the grid.
                //If FindCell accidently returns int outside of array,
                //it would result in a runtime error.
                if (cellCords.X < numWide &&
                   cellCords.Y < numHigh &&
                   cellCords.X > 0 &&
                   cellCords.Y > 0)
                {
                    grid[cellCords.X, cellCords.Y].AddEnemy(e);
                }
            }
        }

        /// <summary>
        /// Gets the list of enemies at a specified position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public List<Enemy> GetEnemies(Vector2 pos)
        {
            //Gets cord position of a provided vector, bullet, player...
            Point cellCords = FindCell(pos);

            //This list will contain all 9 cells near a position.
            List<Enemy> allEnemies = new List<Enemy>();

            //Iterate over the cell to the left, center, and right
            for (int x = cellCords.X - 1; x <= cellCords.X + 1; x++)
            {
                //Iterate over the cell to the top, center, and bottom
                for (int y = cellCords.Y - 1; y <= cellCords.Y + 1; y++)
                {
                    //Checks if the coordinate is in the array before adding it
                    if (x >= 0 &&
                        y >= 0 &&
                        x < grid.GetLength(0) &&
                        y < grid.GetLength(1))
                    {
                        //Add all enemies at that cell
                        allEnemies.AddRange(grid[x, y].enemiesInside);
                    }
                }
            }

            //returns the list of enemies at or near that cell
            return allEnemies;
        }


        /// <summary>
        /// Returns a point with the cell that an enemy is within
        /// </summary>
        /// <param name="enemyCenter">The center of an enemies pos</param>
        /// <returns>2 int values (x, y)</returns>
        private Point FindCell(Vector2 enemyCenter)
        {
            Vector2 localPosition = enemyCenter - GetWorldOffset() ;

            //Returns the number of times an enemies center fits within the cell width
            //For example:
            // enemyCenter = Vector2(930,490)
            // 930 / 100 = 9
            // 490 / 100 = 4
            // cell position = new Point(4,9)
            return new Point(
                (int)(localPosition.X / cellWidth),
                (int)(localPosition.Y / cellWidth));
        }

        /// <summary>
        /// Gets the position of the upper left hand corner of the world, 
        /// based on the players position
        /// </summary>
        /// <returns></returns>
        private Vector2 GetWorldOffset()
        {
            //Figures out how far the player is. And positions the grid
            //Centered around them
            return new Vector2(
                    playerPos.X - numWide * cellWidth / 2,
                    playerPos.Y - numHigh * cellWidth / 2);
        }

        /// <summary>
        /// Updates the grid to match player position
        /// </summary>
        public void Update()
        {
            playerPos = cam.CameraPosition;

        }

        /// <summary>
        /// Draws the cell lines along with text inside to show the position it is in
        /// </summary>
        /// <param name="sb"></param>
        public void DisplayGrid(SpriteBatch sb)
        {
            //Number of cells wide
            for (int x = 0; x < numWide; x++)
            {
                //Number of cells high
                for (int y = 0; y < numHigh; y++)
                {
                    int xPos = x * cellWidth + (int)GetWorldOffset().X;
                    int yPos = y * cellWidth + (int)GetWorldOffset().Y;

                    //The number of enemies in a cell
                    int count = grid[x, y].enemiesInside.Count;

                    //Draw the color based on number of enemies
                    Color heatColor = new Color(count * 50, 0, 0, 0);
                    sb.Draw(
                        pixel,
                        new Rectangle(
                            xPos,
                            yPos,
                            cellWidth,
                            cellWidth),
                        heatColor);
                }
            }
        }
    }
}
