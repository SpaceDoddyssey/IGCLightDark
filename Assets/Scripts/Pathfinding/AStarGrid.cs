using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGrid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    AStarNode[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    public bool onlyDisplayPathGizmos;
    public bool displayTargetGizmos;
    public Transform target;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }


    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid()
    {
        grid = new AStarNode[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(worldPoint, nodeRadius - 0.01f, unwalkableMask);
                grid[x,y] = new AStarNode(walkable, worldPoint, x, y);

            }
        }

        
    }

    public List<AStarNode> GetNeighbours(AStarNode node, bool treatEnemiesAsUnwalkable)
    {
        List<AStarNode> neighbours = new List<AStarNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) 
                    continue;

                if (Mathf.Abs(x) == Mathf.Abs(y))
                {
                    continue;

                }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                // Here we filter out the unwalkable enemies, if need be.
                if (treatEnemiesAsUnwalkable && (Physics.CheckSphere(grid[checkX, checkY].worldPosition, 0.1f, LayerMask.GetMask("Physical"))))
                    continue;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    // Figure this one out, I understand everything else
    public AStarNode AStartNodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;

        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

        int x = Mathf.FloorToInt(Mathf.Clamp((gridSizeX - 1) * percentX, 0, gridSizeX ));

        int y = Mathf.FloorToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public List<AStarNode> path;
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (onlyDisplayPathGizmos)
        {
            if (path != null)
            {
                foreach (AStarNode n in path)
                {
                    Gizmos.color = Color.black;
                    if (displayTargetGizmos)
                    {
                        if (n == AStartNodeFromWorldPoint(target.position))
                            Gizmos.color = Color.blue;
                    }
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }

        }
        else
        {

            if (grid != null)
            {
                foreach (AStarNode n in grid)
                {
                    Gizmos.color = (n.walkable) ? Color.white : Color.red;
                    if (path != null)
                        if (path.Contains(n))
                            Gizmos.color = Color.black;
                    if (displayTargetGizmos)
                    {

                        if (n == AStartNodeFromWorldPoint(target.position))
                            Gizmos.color = Color.blue;
                    }
                    Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
    }


    public AStarNode BFS(AStarNode node)
    {

        // Abysmal time complexity, n squared. But whatever. Best idea I have right now as we slapdash this together.
        for (int x = 0; x < gridSizeX; x++) 
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                grid[x, y].BFSNode = new AStarNode.BFSObject();
            }
        }

        // Discover the source s
        node.BFSNode.color = 'g';
        node.BFSNode.depth = 0;
        node.BFSNode.parent = null;

        Queue<AStarNode> Q = new Queue<AStarNode>();
        Q.Enqueue(node);

        while (!(Q.Count == 0))
        {
            AStarNode current = Q.Dequeue();
            foreach (AStarNode n in GetNeighbours(current, true))
            {
                if (n.BFSNode.color == 'w')
                {
                    n.BFSNode.color = 'g';
                    n.BFSNode.depth = current.BFSNode.depth + 1;
                    n.BFSNode.parent = current.BFSNode;

                    // If that particular node we're looking at is empty, then we're good :3
                    if (!Physics.CheckSphere(n.worldPosition, 0.1f, LayerMask.GetMask("Physical")))
                    {
                        return n;
                    }

                }
                current.BFSNode.color = 'b';
            }
        }

        return null;
    }
}
