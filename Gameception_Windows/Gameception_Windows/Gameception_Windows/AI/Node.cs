using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gameception
{
    class Node
    {
        public Cell currentCell;
        public Node parentNode;
        public int movementCost, functionscore;
        public bool closed = false;

        public Node(Cell currentCell, Node parentNode, int movementCost, int functionscore)
        {
            this.currentCell = currentCell;
            this.parentNode = parentNode;
            this.movementCost = movementCost;
            this.functionscore = functionscore;
        }

        public String toString()
        {
            if (parentNode == null)
                return "==================================\n" +
                    "Cell: " + currentCell.z + ":" + currentCell.x + "\n" +
                    "Movement Cost: " + movementCost + "\n" +
                    "F score: " + functionscore + "\n" +
                    "==================================\n";

            else
                return "==================================\n" +
                "Cell: " + currentCell.z + ":" + currentCell.x + "\n" +
                "Parent: " + parentNode.currentCell.z + ":" + parentNode.currentCell.x + "\n" +
                "Movement Cost: " + movementCost + "\n" +
                "F score: " + functionscore + "\n" +
                "==================================\n";
        }

    }
}
