using UnityEngine;
using UnityEngine.Networking;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

using System.Collections.Generic;
using System.Deployment.Internal;
using System.Linq;
using PathFinder;


[ExecuteInEditMode]
public class GridBehavior : NetworkBehaviour, ISearchSpace
{
    private Vector2 maxSize;
    public float nodeRadius;

    private int numSpheresX, numSpheresY;
    private Vector3 startingCorner;
    
    public LayerMask opaqueObstacleMask = 1 << 9 | 1 << 12;
    public LayerMask lightmeshMask = 1 << 16;
    public LayerMask transparentMask = 1 << 17;

    public float lightMaxWeight = 1;
    public float lightInfluence = 10;

    Node[,] areaOfNodes;
    private GameObject ShadowColliderGroup;

    [SyncVar]
    private bool dirty = true;
    [SyncVar]
    private bool checkAI = true;

    [SerializeField]
    private bool ShowDebugLines = false;

    [SerializeField] private Rect DebugArea = Rect.MinMaxRect(-25, -25, 25, 25);

    public IEnumerable<INode> Nodes
    {
        get
        {
            // cast will convert the 2D array into IEnumerable ;)
            return areaOfNodes.Cast<INode>();
        }
    }

    public static GridBehavior Instance { get; private set; }

    void Start()
    {
        Instance = this;
        var mRenderer = gameObject.GetComponent<Renderer>();

        maxSize = new Vector2(mRenderer.bounds.size.x, mRenderer.bounds.size.y);
        numSpheresX = Mathf.RoundToInt(maxSize.x / nodeRadius) / 2;
        numSpheresY = Mathf.RoundToInt(maxSize.y / nodeRadius) / 2;

        startingCorner = new Vector3(mRenderer.bounds.min.x, mRenderer.bounds.min.y, gameObject.transform.position.z);

        if (Application.isPlaying)
        {
            ShadowColliderGroup = new GameObject("Shadows");
            ShadowColliderGroup.layer = 10;
            ShadowColliderGroup.transform.parent = gameObject.transform;
        }
        createGrid();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            if (areaOfNodes == null)
            {
                Start();
            }

           UpdateGrid();
        }
#endif

        if(isServer)
        {
            if (dirty)
            {
                UpdateGrid();
                dirty = false;
            }
            if (checkAI)
            {
                UpdateAI();
                checkAI = false;
            }
        }
    }

    public void SetGridDirty()
    {
        dirty = true;
    }

    public void SetAIDirty()
    {
        checkAI = true;
    }

    public void createGrid()
    {
        areaOfNodes = new Node[numSpheresX,numSpheresY];
        for (int x = 0; x < numSpheresX; x++)
        {
            for (int y = 0; y < numSpheresY; y++)
            {
                Vector3 nodePos = startingCorner + new Vector3((nodeRadius * 2) * x + nodeRadius, (nodeRadius * 2) * y + nodeRadius, 0);
                nodePos.z = -500;

                float zIndex = 0;
                RaycastHit hit;
                if (Physics.Raycast(nodePos, Vector3.forward, out hit, 1000, LayerMask.GetMask("Floor")))
                {
                    zIndex = hit.point.z;
                }

                areaOfNodes[x, y] = new Node(this, nodePos, new Vector2(x, y), 1, zIndex);

                //ShadowCollider
                //if (Application.isPlaying)
                //{
                //    BoxCollider2D col = ShadowColliderGroup.AddComponent<BoxCollider2D>();
                //    col.size = new Vector2(nodeRadius * 2, nodeRadius * 2);
                //    col.offset = areaOfNodes[x, y].position;
                //    col.isTrigger = true;
                //    areaOfNodes[x, y].shadowCollider = col;
                //}
            }
        }
        UpdateGrid();
        UpdateAI();
    }

    private void UpdateGrid()
    {
        var inv_c2 = 1.0f / (lightInfluence * lightInfluence);

        var radiusVec2 = Vector2.one * nodeRadius;

        //Debug.Log("Grid update: "+isServer.ToString());
        for (int x = 0; x < numSpheresX; x++)
        {
            for (int y = 0; y < numSpheresY; y++)
            {
                Node node = areaOfNodes[x, y];
                Vector3 nodePos = node.position;

                var cols = Physics2D.OverlapAreaAll((Vector2)nodePos - radiusVec2, (Vector2)nodePos + radiusVec2, opaqueObstacleMask | lightmeshMask | transparentMask);

                node.Weight = 1;
                node.canWalk = true;
                node.hasLight = false;

                foreach (var col in cols)
                {
                    var polygon = col as PolygonCollider2D;
                    var colLayer = 1 << col.gameObject.layer;

                    // cant walk if collided with obstacle layer object
                    node.canWalk &= (colLayer & opaqueObstacleMask) == 0;

                    // next checks deppend on polygon collider
                    if (polygon == null)
                        continue;

                    var isLightmeshPolygon = (colLayer & lightmeshMask) != 0;

                    // has light if collided with any light
                    if (isLightmeshPolygon && node.canWalk)
                    {
                        node.hasLight = true;
                        var d2 = (col.transform.position - node.position).sqrMagnitude;
                        
                        // add extra weight for each light using the formula: 1 / (dist^2/c^2 + 1). Function has max of 1 at 0 distance for positive numbers
                        // c is proportional to the inverse of the decay of the curve
                        node.Weight += lightMaxWeight / (1 + d2 * inv_c2);
                    }
                }

                if (!node.canWalk) node.Weight = 1000;

                //if (Application.isPlaying)
                //{
                //    areaOfNodes[x, y].shadowCollider.enabled = !areaOfNodes[x, y].hasLight || !areaOfNodes[x, y].canWalk;
                //}
            }
        }
    }

    private void UpdateAI() {
        //Debug.Log("AI update: " + isServer.ToString());
        if (Application.isPlaying)
        {
            //Let AI know what to do based on visibility status
            //AvatarController player = FindObjectOfType(typeof(AvatarController)) as AvatarController;
            //bool playerIsAccessible = false;
            //if (player)
            //{
            //    Node playerNode = getNodeAtPos(player.transform.position);
            //    playerIsAccessible = (playerNode != null);
            //}

            //MinionController[] robots = FindObjectsOfType(typeof(MinionController)) as MinionController[];
            //foreach (MinionController robot in robots)
            //{
            //    if (robot.enabled)
            //    {
            //        var robotNode = getNodeAtPos(robot.transform.position);
            //        if (robotNode != null)
            //        {
            //            if (playerIsAccessible)
            //            {
            //                robot.StartFollow();
            //            }
            //            else
            //            {
            //                robot.StartPatrol();
            //            }
            //        }
            //        else
            //        {
            //            robot.TurnOff();
            //        }
            //    }
            //}
        }
    }

    public Node getNodeAtPos(Vector3 pos)
    {
        float x = (pos.x - nodeRadius - startingCorner.x) / (nodeRadius * 2);
        float y = (pos.y - nodeRadius - startingCorner.y) / (nodeRadius * 2);

        var i = (int) Mathf.Round(x);
        var j = (int) Mathf.Round(y);

        if (areaOfNodes == null || i < 0 || i >= areaOfNodes.GetLength(0) || j < 0 || j >= areaOfNodes.GetLongLength(1))
        {
            return null;
        } 

        return areaOfNodes[i, j];
    }



    public IList<Node> getNodesNearPos(Vector3 pos, float radius, Predicate<Node> conditionPredicate = null)
    {
        return getNodesNearPos(pos, radius, 0, conditionPredicate);
    }

    public IList<Node> getNodesNearPos(Vector3 pos, float radius, float minRadius, Predicate<Node> conditionPredicate = null)
    {
        pos.z = 0;

        var nodes = new List<Node>();

        minRadius = Mathf.Clamp(minRadius, 0, radius - 1);

        var minPos = ClampPosition(pos - Vector3.one * radius);
        var maxPos = ClampPosition(pos + Vector3.one * radius);
        
        var minNode = getNodeAtPos(minPos);
        var maxNode = getNodeAtPos(maxPos);
        var sqrRadius = radius * radius;
        var sqrMinRadius = minRadius * minRadius;

        var minx = (int)minNode.coord.x;
        var miny = (int)minNode.coord.y;
        var maxx = (int)maxNode.coord.x + 1;
        var maxy = (int)maxNode.coord.y + 1;

        for (int x = minx; x < maxx; x++)
        {
            for (int y = miny; y < maxy; y++)
            {
                var node = areaOfNodes[x, y];
                var sqrMag = (node.position - pos).sqrMagnitude;
                if (sqrMag > sqrRadius || sqrMag < sqrMinRadius || conditionPredicate == null || !conditionPredicate(node))
                    continue;

                nodes.Add(node);
            }
        }

        return nodes;
    }

    private Vector3 ClampPosition(Vector3 position)
    {
        var rad = (Vector3.one * nodeRadius);
        var uv = (position - rad - startingCorner) / (nodeRadius * 2);
        uv.x = Mathf.Clamp(uv.x, 0, numSpheresX - 1);
        uv.y = Mathf.Clamp(uv.y, 0, numSpheresY - 1);
        uv.z = 0;

        return (uv * nodeRadius * 2) + rad + startingCorner;
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

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (!ShowDebugLines)
        {
            return;
        }

        float size = nodeRadius * .95f;

        if (areaOfNodes != null)
        {
            foreach (Node node in areaOfNodes)
            {
                if (!DebugArea.Contains(node.position))
                {
                    continue;
                }

                Handles.color = new Color(1, 1, 1, 1);
                Handles.DrawSolidRectangleWithOutline(new[] {
                    node.position + new Vector3(size, size, 0f),
                    node.position + new Vector3(size, -size, 0f),
                    node.position + new Vector3(-size, -size, 0f),
                    node.position + new Vector3(-size, size, 0f) },
                    node.hasLight ? new Color(1, 1, 0, 0.05f) : new Color(0.5f, 0.5f, 0.5f, 0.05f),
                    node.hasLight ? new Color(1, 1, 0, 0.4f) : new Color(0.5f, 0.5f, 0.5f, 0.4f));

                if (!node.canWalk)
                {
                    Handles.color = node.hasLight ? new Color(1, 1, 0, .4f) : new Color(0.5f, 0.5f, 0.5f, .4f);
                    //Handles.DrawSolidDisc(node.position, new Vector3(0, 0, 1), nodeRadius * .4f);
                    Handles.DrawLine(node.position + new Vector3(size, size, 0f),
                        node.position + new Vector3(-size, -size, 0f));
                    Handles.DrawLine(node.position + new Vector3(-size, size, 0f),
                        node.position + new Vector3(size, -size, 0f));
                }

                //Handles.Label(node.position, node.Weight == float.MaxValue ? "max" : node.Weight.ToString("F2"));
                //Handles.Label(node.position, (node.coord).ToString());
            }
        }
    }
#endif

    public int GetMaxSize
    {
        get { return numSpheresX * numSpheresY; }
    }


    public static float Heuristic(INode nodeA, INode nodeB)
    {
        var a = (Node)nodeA;
        var b = (Node)nodeB;
        
        var xDist = Mathf.Abs(a.coord.x - b.coord.x);
        var yDist = Mathf.Abs(a.coord.y - b.coord.y);
        if(xDist > yDist)
            return 1.4f*yDist + (xDist-yDist);
        return 1.4f*xDist + (yDist-xDist);
    }

    
    public IEnumerable<INode> GetFringePath(Vector3 startPosition, Vector3 endPosition, int maxSteps = int.MaxValue)
    {
        var fringe = new Fringe(Heuristic);

        var startNode = getNodeAtPos(startPosition);
        var endNode = getNodeAtPos(endPosition);

        if (startNode == null || endNode == null) return new List<INode>();

        startNode.isStartEnd = true;
        endNode.isStartEnd = true;
        
        var path = fringe.FindPath(startNode, endNode, maxSteps);
        if (fringe.PathCost > 2000)
        {
            return new List<INode>();
        }

        startNode.isStartEnd = false;
        endNode.isStartEnd = false;
        
        return path;
    }

}
