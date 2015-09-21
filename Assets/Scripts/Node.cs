using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PathFinder;

public class Node : INode
{
    private Vector3 _position;
    private Vector2 _coord;
    private float _weight;
    private bool _canWalk, _hasLight;
    private float gC = -1;
    private Node _nodeParent;
    private bool _isStartEnd = false;

    public BoxCollider2D shadowCollider;

    public readonly GridBehavior Grid;

    public Node(GridBehavior grid, Vector3 position, Vector2 coord, float weight)
    {
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
        get { return ((!hasLight || !canWalk) && !_isStartEnd) ? float.MaxValue : _weight; }
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

    public bool isStartEnd
    {
        get { return _isStartEnd; }
        set { _isStartEnd = value; shadowCollider.enabled = value; }
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

}
