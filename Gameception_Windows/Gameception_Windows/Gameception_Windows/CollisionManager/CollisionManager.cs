using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gameception
{
    class CollisionManager
    {
        #region Attributes

        /// <summary>
        /// Game CollisionManager belongs to
        /// </summary>
        Gameception game;
        public Gameception Game
        {
            get { return game; }
            set { game = value; }
        }

        /// <summary>
        /// SoundManager associated with Game so that CollisionManager can play sounds
        /// </summary>
        SoundManager soundManager;
        public SoundManager SoundManager
        {
            get { return soundManager; }
            set { soundManager = value; }
        }

        /// <summary>
        /// Grid used to check for collisions
        /// </summary>
        List<GameObject>[,] grid;
        public List<GameObject>[,] Grid
        {
            get { return grid; }
            set { grid = value; }
        }

        /// <summary>
        /// Total number of X-coords in the grid
        /// </summary>
        int x;
        public int X
        {
            get { return x; }
            set { x = value; }
        }

        /// <summary>
        /// Total number of Y-coords in the grid
        /// </summary>
        int y;
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        /// <summary>
        /// Width of grid cell
        /// </summary>
        int width;
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        /// Used to ensure grid positions are not in the negatives.
        /// Defines the centre of the grid.
        /// </summary>
        int offset_X;
        int offset_Y;

        #endregion

        #region Initialization

        public CollisionManager(Gameception game_, int x_, int y_, int w_)
        {
            Game = game_;
            X = x_;
            Y = y_;
            offset_X = (int)X / 2;
            offset_Y = (int)Y / 2;
            width = w_;
            grid = new List<GameObject>[Y, X];
            for (int y = 0; y < Y; y++)
            {
                for (int x = 0; x < X; x++)
                {
                    grid[y, x] = new List<GameObject>();
                }
            }
        }

        #endregion

        #region Add/Remove

        public void Add(GameObject o)
        {
            // corners
        }

        #endregion
    }
}
