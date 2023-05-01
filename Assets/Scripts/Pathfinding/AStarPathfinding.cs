using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class AStarPathfinding : MonoBehaviour
{
    public Transform seeker, target;

    AStarGrid grid;
    public List<AStarNode> path = new List<AStarNode>();

    private void Awake()
    {
        grid = GameObject.Find("World Manager").GetComponent<AStarGrid>(); 
    }

    private void Update()
    {

    }

    public void FindPath()
    {


        Stopwatch sw = new Stopwatch();
        sw.Start();
        AStarNode startNode = grid.AStartNodeFromWorldPoint(seeker.position);
        AStarNode targetNode = grid.AStartNodeFromWorldPoint(target.position);

        Heap<AStarNode> openSet = new Heap<AStarNode>(grid.MaxSize);
        HashSet<AStarNode> closedSet = new HashSet<AStarNode>();
        openSet.Add(startNode);

        // Our main loop
        while (openSet.Count > 0)
        {
            AStarNode currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);
            if (currentNode == targetNode)
            {
                sw.Stop();
                print("Path found: " + sw.ElapsedMilliseconds + "ms");

                // Then the path has been found
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (AStarNode neighbour in grid.GetNeighbours(currentNode))
            {
                if (!(neighbour.walkable) || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);

                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    void RetracePath(AStarNode startNode, AStarNode endNode)
    {
        path.Clear();
        AStarNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        grid.path = path;
    }

    int GetDistance(AStarNode nodeA, AStarNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
        

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
