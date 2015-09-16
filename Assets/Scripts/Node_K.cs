using UnityEngine;
using System.Collections;

public class Node_K : IHeapObj<Node_K>
{
    private Vector3 myPosition;
    private Vector2 coord;
    private int myCost;
    private bool canWalk;
    private int gC, hC, xCoord, zCoord, hIndex;
    private Node_K nodeParent;

    public Node_K(Vector3 _myPosition, bool _canWalk, Vector2 _coord, int _myCost)
    {
        myPosition = _myPosition;
        canWalk = _canWalk;
        coord = _coord;
        myCost = _myCost;
    }

    public int nodeCost
    {
        get { return myCost; }
    }

    public Vector3 myPos
    {
        get{ return myPosition; }
    }

    public bool walkable
    {
        get { return canWalk; }
    }

    public int fCost
    {
        get { return (gC + hC); }
    }

    public int gCost
    {
        get { return gC; }
        set { gC = value; }
    }

    public int hCost
    {
        get { return hC; }
        set { gC = value; }
    }

    public Node_K parent
    {
        get { return nodeParent; }
        set { nodeParent = value; }
    }

    public Vector2 myCoord
    {
        get { return coord; }
    }

    public int heapIndex
    {
        get { return hIndex; }
        set { hIndex = value; }
    }

    public int CompareTo(Node_K nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
            compare = hCost.CompareTo(nodeToCompare.hCost);

        return -compare;
    }
}
