using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    public Vector3 seekerPos, targetPos;

    AStarGrid grid;
    public List<AStarNode> path = new List<AStarNode>();

    private GameObject owner;

    private void Awake()
    {
        grid = GameObject.Find("Game World Manager").GetComponent<AStarGrid>(); 
        owner = gameObject;
    }


    public void FindPath(Vector3 start, Vector3 target)
    {
        seekerPos = start;
        targetPos = target;

        // First, try to find the path with enemies set as unwalkable.
        if (!AStarAlgorithm(targetPos, true))
        {
            // If that doesn't work, let's see if an enemy was blocking.
            bool enemyBlocking = false;
            foreach (AStarNode n in path)
            {
                if (Physics.CheckSphere(n.worldPosition, 0.1f, LayerMask.GetMask("Physical")))
                {
                    enemyBlocking = true;
                    break;
                }
            }

            // If there WAS an enemy blocking, run the algorithm again, setting enemies as walkable nodes.
            if (enemyBlocking)
            {
                // If you still don't have a clear path at all to the player, return.
                if (!AStarAlgorithm(targetPos, false)) return;
                else
                {
                    // Otherwise, you have a path to the player, enemies be damned.
                    // Time to find an open space along that path to travel to.
                    FindOpenSpace();
                }
            }
            else
            {
                // If you don't have any feasible path to the player, you probably SHOULD NOT BE TRYING TO PATHFIND.
                return;
            }



        }
    }

    private bool FindOpenSpace()
    {
        Vector3 newTarget;
        // This function only runs if an enemy can't find a path to the player by treating enemies as unwalkable.

        // for each node in the path,
        // we're looking for the first node that has an enemy on it.
        AStarNode nodeOfImportance = null;

        foreach (AStarNode n in path)
        {
            // If one of those path nodes has an enemy on it, that's the one we care about.
            if (Physics.CheckSphere(n.worldPosition, 0.1f, LayerMask.GetMask("Physical")))
            {
                nodeOfImportance = n;
                break;
            }
        }

        // Enemy picks a random point along its current hypothetical path UP UNTIL this node of importance.

        int index = path.FindIndex(a => a.Equals(nodeOfImportance));

        // If you can't find anything, return false.
        if (index < 0 || path == null) Debug.Break();

        AStarNode randomNodeAlongPath = path[UnityEngine.Random.Range(0, index)];

        if (randomNodeAlongPath == null || grid == null) return false;

        AStarNode closestOpenNode = grid.BFS(randomNodeAlongPath);

        // If you can't find an open node, give up.
        if (closestOpenNode == null) return false;


        newTarget = grid.BFS(randomNodeAlongPath).worldPosition;

        if (newTarget != null)
        {
            // So, for one time only, the new target to pathfind to is this particular open node you found.
            // Notice how we don't replace targetPos with this because it's TEMPORARY.
            AStarAlgorithm(newTarget, true);
        }

        // Shoot, otherwise the enemy just can't move.
        return false;
    }


    //public AStarNode RandomDirection(Vector3 start)
    //{
    //    AStarNode startNode = grid.AStartNodeFromWorldPoint(seekerPos);
    //    List<AStarNode> neighbours = grid.GetNeighbours(startNode, true);
    //    AStarNode randomNode = neighbours[Random.Range(0, neighbours.Count)];
    //    return randomNode;
    //}

    private bool AStarAlgorithm(Vector3 target, bool treatEnemiesAsUnwalkables)
    {
        AStarNode startNode = grid.AStartNodeFromWorldPoint(seekerPos);
        AStarNode targetNode = grid.AStartNodeFromWorldPoint(targetPos);

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
                // Then the path has been found
                RetracePath(startNode, targetNode);
                return true;
            }

            foreach (AStarNode neighbour in grid.GetNeighbours(currentNode, false))
            {
                // Code to check if the node is already in the closed set, if the node is walkable, or if the node is an enemy and we're treating as unwalkable.
                if (!(neighbour.walkable) || closedSet.Contains(neighbour))
                {
                    continue;
                }
                else if (treatEnemiesAsUnwalkables)
                {
                    RaycastHit hit;

                    if (Physics.Raycast(currentNode.worldPosition, neighbour.worldPosition - currentNode.worldPosition, out hit, 2f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.UseGlobal))
                    {
                        EnemyScript e = hit.collider.GetComponent<EnemyScript>();
                        if (e != null) { 
                            // If the enemy you're looking at is in your dimension, then you can't pass thru them
                            if (e.homeWorld == gameObject.GetComponent<EnemyScript>().homeWorld)
                            {
                                continue;
                            }
                        }
                    }
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

        return false;
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

    private int GetDistance(AStarNode nodeA, AStarNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);


        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }


}
