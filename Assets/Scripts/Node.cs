using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PathFinder;

public class Node : INode
{
    private ArrayList visibleLights;
    private Vector3 _position;
    private Vector2 _coord;
    private float _weight;
    private bool _canWalk, _hasLight;
    private float gC = -1;
    private Node _nodeParent;
    
    public BoxCollider2D shadowCollider;

    public readonly GridBehavior Grid;

    public Node(GridBehavior grid, Vector3 position, Vector2 coord, float weight)
    {
        visibleLights = new ArrayList();
        _position = position;
        _coord = coord;
        _weight = weight;
        Grid = grid;
    }

    public IEnumerable<NodeConnection> Connections
    {
        get
        {
            return Grid.GetNodeConnections((int)_coord.x, (int)_coord.y);
        }
    }

    public void Reset() {
        _weight = 1;
        gC = -1;
        _nodeParent = null;
    }

    public float Weight
    {
        get { return (!canWalk || !hasLight) ? float.MaxValue : _weight; }
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
        get { return _hasLight; }
        set { _hasLight = value; }
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

    public void OnLightUpdate(LightController[] lights)
    {
        if (!canWalk) {
            return;
        }

        visibleLights.Clear();
        foreach (LightController light in lights)
        {
            if (light.CurrentStatus != LightController.Status.On) continue;
            Vector2 p = new Vector2(_position.x, _position.y);
            Vector2 d = new Vector2(light.transform.position.x, light.transform.position.y) - p;
            d.Normalize();

            RaycastHit2D hit;
            hit = Physics2D.Raycast(p, d, 1000.0f, light.shadowMask | (1<<8));
            if (hit && hit.collider && hit.collider.gameObject.tag == "Light")
            {
                visibleLights.Add(hit.collider.gameObject);
            }
        }

        if (Application.isPlaying)
        {
            shadowCollider.enabled = !hasLight;
        }
    }


}
