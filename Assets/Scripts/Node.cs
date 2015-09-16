using UnityEngine;
using System.Collections;
using System.Linq;

public class Node
{
    private ArrayList visibleLights;
    private Vector3 _position;
    private Vector2 _coord;
    private int _cost;
    private bool _canWalk;       // The node can be walked on at some point;
    private int gC, hC, xCoord, zCoord, hIndex;
    private Node _nodeParent;

    public Node(Vector3 __position, bool __canWalk, Vector2 __coord, int __cost)
    {
        visibleLights = new ArrayList();
        _position = __position;
        _canWalk = __canWalk;
        _coord = __coord;
        _cost = __cost;
    }

    public int cost
    {
        get { return _cost; }
        set { _cost = value; }
    }

    public Vector3 position
    {
        get{ return _position; }
    }

    public bool canWalk
    {
        get { return _canWalk; }
        set { _canWalk = value; }
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
        get { return _nodeParent; }
        set { _nodeParent = value; }
    }

    public Vector2 coord
    {
        get { return _coord; }
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
            if (Physics.Raycast(_position, Vector3.Normalize(light.transform.position - _position), out hit))
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
