using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarNode : IHeapItem<AStarNode>
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;

    public AStarNode parent;

    int heapIndex;

    public AStarNode(bool _walkable, Vector3 _worldPos, int gridX, int gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        this.gridX = gridX;
        this.gridY = gridY;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(AStarNode nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
        {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }

        return -compare;
    }
}
