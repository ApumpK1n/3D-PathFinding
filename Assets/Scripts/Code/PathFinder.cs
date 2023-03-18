using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Pumpkin
{
    public enum PathfindingAlgorith
    {
        AStar,
        Greedy
    }

    public class PathFinder
    {
        private PathfindingAlgorith algorithm = PathfindingAlgorith.AStar;

        private HashSet<Element> tmpDraw;

        private Element tmpNode;

        public List<Element> FindPath(Element start, Element target, PathfindingAlgorith pathfindingAlgorith)
        {
            switch (pathfindingAlgorith)
            {
                case PathfindingAlgorith.AStar:
                    return FindPathAStar(start, target);
            }
            return null;
        }

        // A star algorithm
        // https://en.wikipedia.org/wiki/A*_search_algorithm
        private List<Element> FindPathAStar(Element start, Element target)
        {
            List<Element> foundPath = new List<Element>();

            List<Element> openSet = new List<Element>();
            HashSet<Element> closedSet = new HashSet<Element>();

            //We start adding to the open set
            start.gCost = 0;
            start.hCost = GetDistance(start, target);

            openSet.Add(start);

            while (openSet.Count > 0)
            {
                Element currentNode = openSet[0];

                for (int i = 0; i < openSet.Count; i++)
                {
                    //We check the costs for the current node
                    //You can have more opt. here but that's not important now
                    if (openSet[i].fCost < currentNode.fCost ||
                        (openSet[i].fCost == currentNode.fCost &&
                        openSet[i].hCost < currentNode.hCost))
                    {
                        //and then we assign a new current node
                        if (!currentNode.Equals(openSet[i]))
                        {
                            currentNode = openSet[i];
                        }
                    }
                }

                //we remove the current node from the open set and add to the closed set
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                //if the current node is the target node
                if (currentNode.Equals(target))
                {
                    //that means we reached our destination, so we are ready to retrace our path
                    foundPath = RetracePath(start, target);
                    break;
                }

                if (currentNode.Neigbors == null) continue;
                //if we haven't reached our target, then we need to start looking the neighbours
                foreach (Element neighbour in currentNode.Neigbors)
                {
                    if (neighbour == null) continue;

                    if (closedSet.Contains(neighbour)) continue;

                    //we create a new movement cost for our neighbours
                    float newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                    //and if it's lower than the neighbour's cost
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        //we calculate the new costs
                        neighbour.gCost = newMovementCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, target);
                        //Assign the parent node
                        neighbour.searchingParentNode = currentNode;
                        //And add the neighbour node to the open set
                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }

            //we return the path at the end
            return foundPath;
        }

            private List<Element> RetracePath(Element startNode, Element endNode)
        {
            //Retrace the path, is basically going from the endNode to the startNode
            List<Element> path = new List<Element>();
            Element currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                //by taking the parentNodes we assigned
                currentNode = currentNode.searchingParentNode;
            }

            //then we simply reverse the list
            path.Reverse();

            return path;
        }

        public float GetDistance(Element elementA, Element elementB)
        {
            return Vector3.Distance(elementA.Bounds.center, elementB.Bounds.center);
        }

#if UNITY_EDITOR
        
        public void Draw()
        {
            if (tmpDraw == null) return;

            if (tmpNode == null) return;


            foreach (var element in tmpDraw)
            {
                element.DrawSelf();
            }

            tmpNode.DrawSelf(Color.green);
        }
#endif
    }
}
