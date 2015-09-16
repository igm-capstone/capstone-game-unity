using UnityEngine;
using System.Collections;
using System.Linq;

public class Node
{
    private ArrayList visibleLights;
    private Vector3 _position;
    private Vector2 _coord;
    private float _weight;
    private bool _canWalk;
    private float gC = -1;
    private Node _nodeParent;

    public Node(Vector3 __position, bool __canWalk, Vector2 __coord, int __weight)
    {
        visibleLights = new ArrayList();
        _position = __position;
        _canWalk = __canWalk;
        _coord = __coord;
        _weight = __weight;
    }

    public void Reset() {
        _weight = 1;
        gC = -1;
        _nodeParent = null;
    }

    public float weight
    {
        get { return (!canWalk || !hasLight) ? int.MaxValue : _weight; }
        set { _weight = value; }
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

    public bool hasLight
    {
        get { return visibleLights.Count > 0; }
    }

    public float gCost
    {
        get { return gC; }
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

    public void OnLightUpdate(Light2D[] lights)
    {
        if (!canWalk) {
            return;
        }

        visibleLights.Clear();
        foreach (Light2D light in lights)
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


}
