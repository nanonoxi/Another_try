using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gameception
{
    class Cell
    {
        public int x, z;
     //public int y
        public List<GameObject> itemAtPosition;
        public int movementCost;
        public bool walkable = true;
        public state CellState;

        public enum state
        { 
            Open,Closed
        }

        public Cell(int x, int z)
        {
            this.x = x;
            this.z = z;

            itemAtPosition = new List<GameObject>();
            CellState = state.Open;
        }

        public void add(GameObject gameobject)
        {
            itemAtPosition.Add(gameobject);
        }

        public bool containsPlayer()
        {
            foreach(GameObject go in itemAtPosition)
            {
                if(go is Player)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
