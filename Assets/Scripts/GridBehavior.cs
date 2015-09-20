using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using PathFinder;

[ExecuteInEditMode]
public class GridBehavior : MonoBehaviour, ISearchSpace
{
    private Vector2 maxSize;
    public float nodeRadius;

    private int numSpheresX, numSpheresY;
    private Vector3 startingCorner;
    
    public LayerMask obstacleLayer = 1 << 9 | 1 << 12;

    Node[,] areaOfNodes;
    private GameObject ShadowColliderGroup;

    public bool dirty = true;

    public IEnumerable<INode> Nodes
    {
        get
        {
            // cast will convert the 2D array into IEnumerable ;)
            return areaOfNodes.Cast<INode>();
        }
    }
    
    void Start()
    {
        var originalCol = gameObject.GetComponent<Collider>();

        maxSize = new Vector2(originalCol.bounds.size.x, originalCol.bounds.size.y);
        numSpheresX = Mathf.RoundToInt(maxSize.x / nodeRadius) / 2;
        numSpheresY = Mathf.RoundToInt(maxSize.y / nodeRadius) / 2;

        startingCorner = new Vector3(originalCol.bounds.min.x, originalCol.bounds.min.y, gameObject.transform.position.z);

        if (Application.isPlaying)
        {
            ShadowColliderGroup = new GameObject("Shadows");
            ShadowColliderGroup.layer = 10;
            ShadowColliderGroup.transform.parent = gameObject.transform;
        }
        createGrid();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying || dirty)
        {
            UpdateGrid();
            dirty = false;
        }
    }
#endif

    public void createGrid()
    {
        areaOfNodes = new Node[numSpheresX,numSpheresY];
        for (int x = 0; x < numSpheresX; x++)
        {
            for (int y = 0; y < numSpheresY; y++)
            {
                Vector3 nodePos = startingCorner + new Vector3((nodeRadius * 2) * x + nodeRadius, (nodeRadius * 2) * y + nodeRadius, 0);
                areaOfNodes[x, y] = new Node(this, nodePos, new Vector2(x, y), 1);

                //ShadowCollider
                if (Application.isPlaying)
                {
                    BoxCollider2D col = ShadowColliderGroup.AddComponent<BoxCollider2D>();
                    col.size = new Vector2(nodeRadius * 2, nodeRadius * 2);
                    col.offset = areaOfNodes[x, y].position;
                    col.isTrigger = true;
                    areaOfNodes[x, y].shadowCollider = col;
                }
            }
        }
        UpdateGrid();
    }

    public void UpdateGrid()
    {
        LightController[] sceneLights = FindObjectsOfType(typeof(LightController)) as LightController[];

        for (int x = 0; x < numSpheresX; x++)
        {
            for (int y = 0; y < numSpheresY; y++)
            {
                Vector3 nodePos = areaOfNodes[x, y].position;
                //Check for obstacles
                var cols = Physics2D.OverlapAreaAll((Vector2)nodePos - new Vector2(nodeRadius, nodeRadius), (Vector2)nodePos + new Vector2(nodeRadius, nodeRadius), obstacleLayer | 1 << 8);

                areaOfNodes[x, y].canWalk = true;
                areaOfNodes[x, y].hasLight = false;
                foreach (Collider2D col in cols)
                {
                    if (col.gameObject.layer != LayerMask.NameToLayer("Lights"))
                        areaOfNodes[x, y].canWalk = false;
                }
                if (areaOfNodes[x, y].canWalk && cols.Length > 0)
                {
                    areaOfNodes[x, y].hasLight = true;
                }

                if (Application.isPlaying)
                {
                    areaOfNodes[x, y].shadowCollider.enabled = !areaOfNodes[x, y].hasLight;
                }
                //Check for lights
                //areaOfNodes[x, y].OnLightUpdate(sceneLights);
            }
        }

        if (Application.isPlaying)
        {
            //Let AI know what to do based on visibility status
            PlayerController player = FindObjectOfType(typeof(PlayerController)) as PlayerController;
            bool playerIsAccessible = false;
            if (player)
            {
                Node playerNode = getNodeAtPos(player.transform.position);
                playerIsAccessible = playerNode.hasLight && playerNode.canWalk;
            }

            AIController[] robots = FindObjectsOfType(typeof(AIController)) as AIController[];
            foreach (AIController robot in robots)
            {
                if (getNodeAtPos(robot.transform.position).hasLight)
                {
                    if (playerIsAccessible) {
                        robot.StartFollow();
                    } else {
                        robot.StartPatrol();
                    }
                }
                else {
                    robot.TurnOff();
                }
            }
        }
    }

    public Node getNodeAtPos(Vector3 pos)
    {
        Vector2 percent = new Vector2(Mathf.Clamp01((pos.x + maxSize.x / 2) / maxSize.x), Mathf.Clamp01((pos.y + maxSize.y / 2) / maxSize.y));

        int gridCoordX = Mathf.RoundToInt((numSpheresX - 1) * percent.x);
        int gridCoordY = Mathf.RoundToInt((numSpheresY - 1) * percent.y);

        return areaOfNodes[gridCoordX, gridCoordY];
    }

    public IEnumerable<NodeConnection> GetNodeConnections(int x, int y)
    {
        var notLeftEdge = x > 0;
        var notRightEdge = x < areaOfNodes.GetLength(0) - 1;
        var notBottomEdge = y > 0;
        var notTopEdge = y < areaOfNodes.GetLength(1) - 1;

        var connections = new List<NodeConnection>();

        if (notTopEdge) CreateConnectionIfValid(connections, areaOfNodes[x, y], areaOfNodes[x, y + 1]);
        if (notRightEdge && notTopEdge) CreateConnectionIfValid(connections, areaOfNodes[x, y], areaOfNodes[x + 1, y + 1]);
        if (notRightEdge) CreateConnectionIfValid(connections, areaOfNodes[x, y], areaOfNodes[x + 1, y]);
        if (notRightEdge && notBottomEdge) CreateConnectionIfValid(connections, areaOfNodes[x, y], areaOfNodes[x + 1, y - 1]);
        if (notBottomEdge) CreateConnectionIfValid(connections, areaOfNodes[x, y], areaOfNodes[x, y - 1]);
        if (notLeftEdge && notBottomEdge) CreateConnectionIfValid(connections, areaOfNodes[x, y], areaOfNodes[x - 1, y - 1]);
        if (notLeftEdge) CreateConnectionIfValid(connections, areaOfNodes[x, y], areaOfNodes[x - 1, y]);
        if (notLeftEdge && notTopEdge) CreateConnectionIfValid(connections, areaOfNodes[x, y], areaOfNodes[x - 1, y + 1]);

        return connections;
    }

    void CreateConnectionIfValid(List<NodeConnection> list, Node nodeFrom, Node nodeTo)
    {
        if (nodeTo.Weight < Single.MaxValue)
        {
            var conn = new NodeConnection
            {
                // 1.4 for diagonals and 1 for horizontal or vertical connections
                Cost = nodeFrom.coord.x == nodeTo.coord.x || nodeFrom.coord.y == nodeTo.coord.y ? 1 : 1.4f,
                From = nodeFrom,
                To = nodeTo,
            };

            list.Add(conn);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (areaOfNodes != null)
        {
            //Node playerNode = getNodeAtPos(player.position);
            foreach (Node node in areaOfNodes)
            {
                if (node.canWalk && node.hasLight)
                {
                    Handles.color = new Color(0, ((11.0f - node.Weight) / 10.0f)*0.6f+0.4f, 0, 0.4f);
                }
                if (!node.hasLight)
                {
                    Handles.color = new Color(0.5f, 0.5f, 0.5f, 0.4f);
                }
                if (!node.canWalk)
                {
                    Handles.color = new Color(0.1f, 0.1f, 0.1f, 0.4f);
                    Handles.DrawLine(node.position + new Vector3(nodeRadius * .9f, nodeRadius * .9f, 0f), node.position + new Vector3(-nodeRadius * .9f, -nodeRadius * .9f, 0f));
                    Handles.DrawLine(node.position + new Vector3(-nodeRadius * .9f, nodeRadius * .9f, 0f), node.position + new Vector3(nodeRadius * .9f, -nodeRadius * .9f, 0f));
                }
                Handles.DrawPolyLine(new[] { 
                    node.position + new Vector3(nodeRadius*.9f, nodeRadius*.9f, 0f),
                    node.position + new Vector3(nodeRadius*.9f, -nodeRadius*.9f, 0f),
                    node.position + new Vector3(-nodeRadius*.9f, -nodeRadius*.9f, 0f),
                    node.position + new Vector3(-nodeRadius*.9f, nodeRadius*.9f, 0f),
                    node.position + new Vector3(nodeRadius*.9f, nodeRadius*.9f, 0f) });

            }
        }
    }

    public int GetMaxSize
    {
        get { return numSpheresX * numSpheresY; }
    }


    public static float Heuristic(INode NodeA, INode NodeB)
    {
        var A = NodeA as Node;
        var B = NodeB as Node;
        
        float xDist = Mathf.Abs(A.coord.x - B.coord.x);
        float yDist = Mathf.Abs(A.coord.y - B.coord.y);
        if(xDist > yDist)
            return 1.4f*yDist + (xDist-yDist);
        return 1.4f*xDist + (yDist-xDist);
    }

    
    public IEnumerable<INode> GetFringePath(GameObject Start, GameObject End)
    {
        IEnumerable<INode> path;

        PathFinder.Fringe fringe = new PathFinder.Fringe(Heuristic);

        Node startNode = getNodeAtPos(Start.transform.position);
        Node endNode = getNodeAtPos(End.transform.position);

        path = fringe.FindPath((INode)startNode, (INode)endNode);

        return path;
    }

}
