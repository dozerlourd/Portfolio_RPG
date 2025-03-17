using UnityEngine;

public class Node
{
    public int gridX;
    public int gridY;

    public bool walkable;
    public Vector3 worldPosition;

    public int gCost;
    public int hCost;
    public Node parent;

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
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
}