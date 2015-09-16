using UnityEngine;
using System.Collections;
using System.Linq;

public class Node
{
    private ArrayList visibleLights;
    private Vector3 myPosition;
    private Vector2 coord;
    private int myCost;
    private bool canWalk;       // The node can be walked on at some point;
    private int gC, hC, xCoord, zCoord, hIndex;
    private Node nodeParent;

    public Node(Vector3 _myPosition, bool _canWalk, Vector2 _coord, int _myCost)
    {
        visibleLights = new ArrayList();
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

    public Node parent
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

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0)
            compare = hCost.CompareTo(nodeToCompare.hCost);

        return -compare;
    }

    public void OnLightUpdate(GameObject[] lights)
    {
        if (!canWalk) {
            return;
        }

        visibleLights.Clear();
        foreach (GameObject light in lights)
        {
            RaycastHit hit;
            if (Physics.Raycast(myPosition, Vector3.Normalize(light.transform.position - myPosition), out hit))
            {
                if (hit.collider.gameObject.tag == "Light")
                {
                    visibleLights.Add(hit.collider.gameObject);
                }
            }
        }
    }

    public bool IsVisible()
    {
        return (visibleLights.Count > 0);
    }

}
