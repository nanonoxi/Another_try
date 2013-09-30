using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gameception
{
    class CollisionManager
    {
        #region Attributes

        /// <summary>
        /// Game instance CollisionManager belongs to
        /// </summary>
        TemplateLevel game;
        public TemplateLevel Game
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

        /// <summary>
        /// Create a CollisionManager
        /// </summary>
        /// <param name="game_"></param> Game instance this collision manager belongs to
        /// <param name="x_"></param> Total number of X-coords in the grid
        /// <param name="y_"></param> Total number of Y-coords in the grid
        /// <param name="w_"></param> Width of a cell in pixels
        public CollisionManager(TemplateLevel game_, int x_, int y_, int w_)
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
            // get object bounds
            Vector3 center = o.Position;
            float radius = o.getBoundingSphere().Radius;
            
            // corners
            int xMin = (int)(center.X - radius + offset_X) / width;
            int xMax = (int)(center.X + radius + offset_X - 1) / width;
            int yMin = (int)(center.Y - radius + offset_Y) / width;
            int yMax = (int)(center.Y + radius + offset_Y - 1) / width;

            // snap to grid to prevent object from going beyond the game's borders
            if (xMin > X - 1) xMin = X - 1;
            else if (xMin < 0) xMin = 0;
            if (xMax > X - 1) xMax = X;
            else if (xMax < 0) xMax = 0;
            if (yMin > Y - 1) yMin = Y - 1;
            else if (yMin < 0) yMin = 0;
            if (yMax > Y - 1) yMax = Y - 1;
            else if (yMax < 0) yMax = 0;

            // top left
            if (!grid[yMin, xMin].Contains(o))
                grid[yMin, xMin].Add(o);
            // top right
            if (!grid[yMin, xMax].Contains(o))
                grid[yMin, xMax].Add(o);
            // bottom left
            if (!grid[yMax, xMin].Contains(o))
                grid[yMax, xMin].Add(o);
            // bottom right
            if (!grid[yMax, xMax].Contains(o))
                grid[yMax, xMax].Add(o);
        }

        public void Remove(GameObject o)
        {
            // get object bounds
            Vector3 center = o.Position;
            float radius = o.getBoundingSphere().Radius;

            // corners
            int xMin = (int)(center.X - radius + offset_X) / width;
            int xMax = (int)(center.X + radius + offset_X - 1) / width;
            int yMin = (int)(center.Y - radius + offset_Y) / width;
            int yMax = (int)(center.Y + radius + offset_Y - 1) / width;

            // snap to grid to prevent object from going beyond the game's borders
            if (xMin > X - 1) xMin = X - 1;
            else if (xMin < 0) xMin = 0;
            if (xMax > X - 1) xMax = X;
            else if (xMax < 0) xMax = 0;
            if (yMin > Y - 1) yMin = Y - 1;
            else if (yMin < 0) yMin = 0;
            if (yMax > Y - 1) yMax = Y - 1;
            else if (yMax < 0) yMax = 0;

            // top left
            grid[yMin, xMin].Remove(o);
            // top right
            grid[yMin, xMax].Remove(o);
            // bottom left
            grid[yMax, xMin].Remove(o);
            // bottom right
            grid[yMax, xMax].Remove(o);
        }

        public void RemoveOld(GameObject o)
        {
            // get object bounds
            Vector3 center = o.PreviousPosition;
            float radius = o.getBoundingSphere().Radius;

            // corners
            int xMin = (int)(center.X - radius + offset_X) / width;
            int xMax = (int)(center.X + radius + offset_X - 1) / width;
            int yMin = (int)(center.Y - radius + offset_Y) / width;
            int yMax = (int)(center.Y + radius + offset_Y - 1) / width;

            // top left
            grid[yMin, xMin].Remove(o);
            // top right
            grid[yMin, xMax].Remove(o);
            // bottom left
            grid[yMax, xMin].Remove(o);
            // bottom right
            grid[yMax, xMax].Remove(o);
        }

        #endregion

        #region Collisions

        public void CheckCollisions(GameObject o)
        {
            Game.displayCollisions(false); // reset
            // get object bounds
            BoundingSphere sphere = o.getBoundingSphere();
            Vector3 center = o.Position;
            float radius = sphere.Radius;

            // corners
            int xMin = (int)(center.X - radius + offset_X) / width;
            int xMax = (int)(center.X + radius + offset_X - 1) / width;
            int yMin = (int)(center.Y - radius + offset_Y) / width;
            int yMax = (int)(center.Y + radius + offset_Y - 1) / width;

            // check all 4 adjacent blocks
            // top left
            if (grid[yMin, xMin].Count != 0)
            {
                for (int i = 0; i < grid[yMin, xMin].Count; i++)
                {
                    if (sphere.Intersects(grid[yMin, xMin].ElementAt(i).getBoundingSphere()))
                    {
                        processCollision(o, grid[yMin, xMin].ElementAt(i));
                    }
                }
            }
            // top right
            if (grid[yMin, xMax].Count != 0)
            {
                for (int i = 0; i < grid[yMin, xMax].Count; i++)
                {
                    if (sphere.Intersects(grid[yMin, xMax].ElementAt(i).getBoundingSphere()))
                    {
                        processCollision(o, grid[yMin, xMax].ElementAt(i));
                    }
                }
            }
            // bottom left
            if (grid[yMax, xMin].Count != 0)
            {
                for (int i = 0; i < grid[yMax, xMin].Count; i++)
                {
                    if (sphere.Intersects(grid[yMax, xMin].ElementAt(i).getBoundingSphere()))
                    {
                        processCollision(o, grid[yMax, xMin].ElementAt(i));
                    }
                }
            }
            // bottom right
            if (grid[yMax, xMax].Count != 0)
            {
                for (int i = 0; i < grid[yMax, xMax].Count; i++)
                {
                    if (sphere.Intersects(grid[yMax, xMax].ElementAt(i).getBoundingSphere()))
                    {
                        processCollision(o, grid[yMax, xMax].ElementAt(i));
                    }
                }
            }
        }

        private void processCollision(GameObject o1, GameObject o2)
        {
            Game.displayCollisions(true);
            /*if (o1.GetType().Name.Equals(""))
            {
            }
            else
            {
                o1.Collision = true;
                RemoveOld(o1);
                o2.Collision = true;
                RemoveOld(o2);
            }*/
        }

        #endregion

        public void Update(GameObject o)
        {
            Remove(o);
            RemoveOld(o);
            CheckCollisions(o);
            Add(o);
        }
    }
}
