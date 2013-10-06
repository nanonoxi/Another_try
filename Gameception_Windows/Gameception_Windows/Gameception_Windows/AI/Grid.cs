using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gameception
{
    class Grid
    {
        public readonly int ROWS, COLLUMNS,WIDTH,HEIGHT;
        private static Cell[,] cells;
        private int tileWidth, tileHeight;
        private List<GameObject> dataAddedQueue;
        private List<GameObject> neighbors;

        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        public List<Vector3> shortestPath = new List<Vector3>();
        Node parentNode;
        public float yDim = 3.5f;

 
        public Grid(int width, int height, int tileX, int tileZ)
        {

            WIDTH = width ;
            HEIGHT = height ;
            ROWS = (int)Math.Floor( (float)(height/tileZ) );
            COLLUMNS = (int)Math.Floor((float)(width/tileX));
            tileWidth = tileX;
            tileHeight = tileZ;
            cells = new Cell[ROWS, COLLUMNS];
            dataAddedQueue = new List<GameObject>();
            neighbors = new List<GameObject>();

            initialize();
        }

        public void initialize()
        {
            for (int x = 0; x < ROWS; x++)
                for (int z = 0; z < COLLUMNS; z++)
                    cells[x,z] = new Cell(x, z);
        }

        public void addDataToGrid(GameObject gameobject)
        {
            dataAddedQueue.Add(gameobject);
          //Console.WriteLine("added: "+gameobject.GetType());
        }

        public void displayGrid()
        { 
            Console.WriteLine("//******************************************//");
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLLUMNS; j++)
                {

                    Console.WriteLine("cell[" + i + "," + j + "] :");
                    foreach (GameObject go in cells[i, j].itemAtPosition)
                    {
                        Console.WriteLine(go.GetHashCode() + " "+ go.GetType() + " "+ cells[i,j].walkable);
                   
                    }
                }
                Console.WriteLine();
            }

        }

        public void updateGrid()
        { 

            //loop through all elements in queue
            while(dataAddedQueue.Count>0)
            {
                GameObject currentObject = dataAddedQueue.ElementAt(0);

                //if element contains anything  other than a player/creep, walkable = false
               
                    Cell currentCell = getPositionInGrid(currentObject);
                
                    if(!currentCell.itemAtPosition.Contains(currentObject))
                    {
                        if (!(currentObject is Player) && !(currentObject is Creep))
                            currentCell.walkable = false;

                        currentCell.itemAtPosition.Add(currentObject);
                    }
             
                /*
                //if position changed, clear previous position in grid
                if(currentObject.PreviousPosition != currentObject.Position)
                {
                    cells[currentCell.x, currentCell.z].itemAtPosition.Remove(currentObject);
                                    
                    //this didnt work
                    Cell newCell = getPositionInGrid(currentObject);
                    cells[newCell.x, newCell.z].itemAtPosition.Add(currentObject);
                }
                */
                //remove evaluated element
                dataAddedQueue.RemoveAt(0);
            }
        }

        public void clearCells()
        { 
            for (int x = 0; x < ROWS; x++)
                for (int z = 0; z < COLLUMNS; z++)
                    cells[x,z].itemAtPosition.Clear();
        }

        //might not need this method
        public bool checkBounds(GameObject gameobject)
        {
            int xVal = (int)(gameobject.getBoundingSphere().Center).X;
            int zVal = (int)(gameobject.getBoundingSphere().Center).Z;

            if(xVal < 0 || xVal> WIDTH)
                return false;
            if (zVal < 0 || zVal > HEIGHT)
                return false;

            return true;
        }

        public void remove(GameObject gameobject)
        {
            Cell currentCell = getPositionInGrid(gameobject);
            currentCell.itemAtPosition.Remove(gameobject);

            if(gameobject is Projectile)
            {
                for(int i = 0; i <ROWS;i++)
                {
                    for (int j = 0; j < COLLUMNS;j++ )
                    {
                        for (int k = 0; k < cells[i, j].itemAtPosition.Count; k++)
                            if (cells[i, j].itemAtPosition.ElementAt(k) is Projectile)
                                cells[i, j].itemAtPosition.RemoveAt(k);
                    }
                }

            }

        }

        public Cell getPositionInGrid(GameObject gameobject)
        {
            float x = gameobject.Position.X +106;
            float z = gameobject.Position.Z +55;
            Cell returnCell;
            
            if( (x <= 140 && x >=0) && (z>=0 && z<=120))
            {
                int positionInColumn = (int)Math.Floor( x/tileWidth );
                int positionInRow = (int)Math.Floor( z/tileHeight );
               
                Cell currentCell = cells[Math.Abs(positionInRow), Math.Abs(positionInColumn)];
                returnCell = currentCell;
            }
            else
            {
                returnCell = null;
                Console.WriteLine(gameobject.GetType() + " was outside bounds: x->"+x+" z->"+z);
            }
            return returnCell;
        }

        public Cell getPointInGrid(double x, double z)
        {
            int x_value = (int)x;
            int z_value = (int)z;

            if (x_value < 0 || x_value > WIDTH)
                return null;
            if (z_value < 0 || z_value > HEIGHT)
                return null;

            return cells[Math.Abs(x_value / tileWidth),Math.Abs(z_value / tileHeight)];
        }

        public void setWalkability(Cell currentCell, GameObject gameobject)
        {
            currentCell.walkable = false;
            float radius = gameobject.getBoundingSphere().Radius;

            Cell surroundingCells= getPointInGrid( (gameobject.getBoundingSphere().Center.X-radius),
                                                    (gameobject.getBoundingSphere().Center.Z-radius));
            if(surroundingCells!=null)
            { surroundingCells.walkable = false;}
           
            surroundingCells = getPointInGrid((gameobject.getBoundingSphere().Center.X + radius),
                                                    (gameobject.getBoundingSphere().Center.Z - radius));
            if (surroundingCells != null)
            { surroundingCells.walkable = false; }

            surroundingCells = getPointInGrid((gameobject.getBoundingSphere().Center.X - radius),
                                                    (gameobject.getBoundingSphere().Center.Z + radius));
            if (surroundingCells != null)
            { surroundingCells.walkable = false; }

            surroundingCells = getPointInGrid((gameobject.getBoundingSphere().Center.X + radius),
                                                    (gameobject.getBoundingSphere().Center.Z + radius));
            if (surroundingCells != null)
            { surroundingCells.walkable = false; }

           //might need to add diagonals
        }

        public List<GameObject> getNeighbors(Cell currentCell)
        {
            if (!(neighbors.Count > 0))
                neighbors.Clear();

            Cell current = cells[currentCell.x, currentCell.z];
            foreach (GameObject go in current.itemAtPosition)
            {
                neighbors.Add(go);
            }

            if (!(currentCell.x + 1 >= WIDTH / tileWidth))
            { 
                Cell right = cells[currentCell.x+1,currentCell.z];
                foreach (GameObject go in right.itemAtPosition)
                {
                    neighbors.Add(go);
                }
            }

            if (!(currentCell.z + 1 >= HEIGHT/ tileHeight))
            {
                Cell top = cells[currentCell.x, currentCell.z+1];
                foreach (GameObject go in top.itemAtPosition)
                {
                    neighbors.Add(go);
                }
            }

            if (!(currentCell.x - 1 <= WIDTH/ tileWidth))
            {
                Cell left = cells[currentCell.x-1, currentCell.z];
                foreach (GameObject go in left.itemAtPosition)
                {
                    neighbors.Add(go);
                }
            }


            if (!(currentCell.z - 1 <= HEIGHT/ tileHeight))
            {
                Cell bottom = cells[currentCell.x, currentCell.z-1];
                foreach (GameObject go in bottom.itemAtPosition)
                {
                    neighbors.Add(go);
                }
            }

            if (!(currentCell.z + 1 <= HEIGHT / tileHeight) && !(currentCell.x + 1 >= WIDTH / tileWidth))
            {
                Cell topright = cells[currentCell.x + 1,currentCell.z + 1];
                foreach (GameObject go in topright.itemAtPosition)
                {
                    neighbors.Add(go);
                }
            }

            if (!(currentCell.z + 1 <= HEIGHT / tileHeight) && !(currentCell.x - 1 >= WIDTH / tileWidth))
            {
                Cell topLeft = cells[currentCell.x - 1,currentCell.z + 1];
                foreach (GameObject go in topLeft.itemAtPosition)
                {
                    neighbors.Add(go);
                }
            }

            if (!(currentCell.z - 1 <= HEIGHT / tileHeight) && !(currentCell.x + 1 >= WIDTH / tileWidth))
            {
                Cell bottomRight = cells[currentCell.x + 1,currentCell.z - 1];
                foreach (GameObject go in bottomRight.itemAtPosition)
                {
                    neighbors.Add(go);
                }
            }

            if (!(currentCell.z - 1 <= HEIGHT / tileHeight) && !(currentCell.x - 1 >= WIDTH / tileWidth))
            {
                Cell bottomLeft = cells[currentCell.x - 1,currentCell.z - 1];
                foreach (GameObject go in bottomLeft.itemAtPosition)
                {
                    neighbors.Add(go);
                }
            }
            return neighbors;
        }

        public List<Cell> getAdjacentCells(Cell currentCell)
        {
            List<Cell> adjacent = new List<Cell>();

            if ( (currentCell.x + 1 < WIDTH/tileWidth) )
            {
                Cell right = cells[currentCell.x + 1,currentCell.z];
                //Cell right = cells[currentCell.z, currentCell.x + 1];
                
                currentCell.movementCost = 10;

                if (!checkClosedContainsCell(right))
                    adjacent.Add(right);
            }

            if ( (currentCell.z + 1 < HEIGHT / tileHeight))
            {
                Cell top = cells[currentCell.x,currentCell.z + 1];
                //Cell top = cells[currentCell.z + 1, currentCell.x];
                currentCell.movementCost = 10;

                if (!checkClosedContainsCell(top))
                    adjacent.Add(top);
            }

            if (((currentCell.x - 1) < WIDTH / tileWidth))
            {
                Cell left = cells[currentCell.x - 1,currentCell.z];
                //Cell left = cells[currentCell.z, currentCell.x - 1];
                currentCell.movementCost = 10;

                if (!checkClosedContainsCell(left))
                    adjacent.Add(left);
            }

            if ((currentCell.z - 1 < HEIGHT / tileHeight))
            {
                Cell bottom = cells[currentCell.x,currentCell.z - 1];
                //Cell bottom = cells[currentCell.z-1, currentCell.x];
                
                currentCell.movementCost = 10;

                if (!checkClosedContainsCell(bottom))
                    adjacent.Add(bottom);
            }

            if ((currentCell.z + 1 < HEIGHT / tileHeight) && !(currentCell.x + 1 >= WIDTH / tileWidth))
            {
                 Cell topright = cells[currentCell.x + 1,currentCell.z + 1];
                //Cell topright = cells[currentCell.z + 1, currentCell.x + 1];
                
                currentCell.movementCost = 14;

                if (!checkClosedContainsCell(topright))
                    adjacent.Add(topright);
            }

            if ((currentCell.z + 1 < HEIGHT / tileHeight) && !(currentCell.x - 1 >= WIDTH / tileWidth))
            {
                Cell topLeft = cells[currentCell.x - 1,currentCell.z + 1];
                //Cell topLeft = cells[currentCell.z + 1, currentCell.x - 1];

                currentCell.movementCost = 14;

                if (!checkClosedContainsCell(topLeft))
                    adjacent.Add(topLeft);
            }

            if ((currentCell.z - 1 < HEIGHT / tileHeight) && !(currentCell.x + 1 >= WIDTH / tileWidth))
            {
                Cell bottomRight = cells[currentCell.x + 1,currentCell.z - 1];
                //Cell bottomRight = cells[currentCell.z - 1, currentCell.x+1];
                
                currentCell.movementCost = 14;

                if (!checkClosedContainsCell(bottomRight))
                    adjacent.Add(bottomRight);
            }

            if ((currentCell.z - 1 < HEIGHT / tileHeight) && !(currentCell.x - 1 >= WIDTH / tileWidth))
            {
                Cell bottomLeft = cells[currentCell.x - 1,currentCell.z - 1];
                //Cell bottomLeft = cells[currentCell.z - 1, currentCell.x - 1];
                
                currentCell.movementCost = 14;

                if (!checkClosedContainsCell(bottomLeft))
                    adjacent.Add(bottomLeft);
            }
            return adjacent;
        }

        public int getHeuristic(Cell currentCell, Cell destination)
        {
            return Math.Max(Math.Abs(currentCell.x - destination.x), Math.Abs(currentCell.z - destination.z));
        }

        public Vector3 getShortestPath(Cell currentCell, GameObject player)
        {
            closedList.Clear();
            openList.Clear();
            shortestPath.Clear();
            float radius = player.getBoundingSphere().Radius;

            if (currentCell == getPositionInGrid(player))
            {
                Vector3 point = new Vector3(player.getBoundingSphere().Center.X - radius, yDim,
                                            player.getBoundingSphere().Center.Z - radius);
                return point;
            }

            else 
            {
                Cell destinationCell = getPositionInGrid(player);
                int functionScore = currentCell.movementCost + getHeuristic(currentCell, destinationCell);

                if (!destinationCell.walkable)
                    destinationCell.walkable = true;

                parentNode = new Node(currentCell, null, 0, functionScore);
                openList.Add(parentNode);

                List<Cell> adjacent = getAdjacentCells(currentCell);
                foreach(Cell c in adjacent)
                {
                    functionScore = (parentNode.movementCost + currentCell.movementCost) + getHeuristic(c, destinationCell);
                    openList.Add(new Node(c, parentNode,(parentNode.movementCost + currentCell.movementCost), functionScore));
                }

                    for (int i = 0; i < openList.Count;i++ )
                    {
                        Node n = openList.ElementAt(i);


                        openList.Remove(parentNode);
                        closedList.Add(parentNode);
                        parentNode.currentCell.CellState = Cell.state.Closed;

                        parentNode = openList.ElementAt(getLowestScore());
                        currentCell = parentNode.currentCell;

                        if (currentCell == destinationCell)
                        {
                            openList.Remove(parentNode);
                            closedList.Add(parentNode);
                            parentNode.currentCell.CellState = Cell.state.Closed;
                            break;
                        }

                        adjacent = getAdjacentCells(currentCell);

                        foreach (Cell c in adjacent)
                        {
                            if (currentCell.walkable)
                            {
                                functionScore = currentCell.movementCost + getHeuristic(c, destinationCell);
                                Node currentAdjacentNode = new Node(c, parentNode, (parentNode.movementCost + currentCell.movementCost), functionScore);
                                openList.Add(currentAdjacentNode);
                            }

                            else
                            {
                                if (openList.Count() > 0)
                                {
                                    openList.Remove(parentNode);
                                    closedList.Add(parentNode);
                                    parentNode.currentCell.CellState = Cell.state.Closed;
                                }
                            }
                        }
                    }

            }

            Node nextNode = generateShortestPathNode(parentNode);
            Vector3 newPoint = new Vector3(nextNode.currentCell.x * tileWidth, yDim, nextNode.currentCell.z * tileHeight);

            return newPoint;
          

            return Vector3.Zero;
        }

        public int getLowestScore()
        { 
            Node firstNode = openList.ElementAt(0);
            int lowestScore = firstNode.functionscore;
            int position = 0;

            for (int i = 1; i < openList.Count; i++)
            {
                Node currentNode = openList.ElementAt(i);
                if (currentNode.functionscore < lowestScore)
                {
                    lowestScore = currentNode.functionscore;
                    position = i;
                }
            }
            return position;
        }

        public Node generateShortestPathNode(Node destinationReachedNode)
        {
            Node currentNode = destinationReachedNode;
            Node previousNode = null;

            while(currentNode.parentNode!=null)
            {
                shortestPath.Add(new Vector3(currentNode.currentCell.x * tileWidth,yDim,
                                              currentNode.currentCell.z * tileHeight));
                previousNode = currentNode;
                currentNode = currentNode.parentNode;
            }
            return previousNode;
        }

        public bool checkClosedContainsCell(Cell cellToCheck)
        { 
            foreach(Node n in closedList)
            {
                if (n.currentCell == cellToCheck)
                    return true;
            }
            return false;
        }

    }
}
