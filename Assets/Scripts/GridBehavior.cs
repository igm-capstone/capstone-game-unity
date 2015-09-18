using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using PathFinder;

public class GridBehavior : MonoBehaviour, ISearchSpace
{
    private Vector2 maxSize;
    public float nodeRadius;

    private int numSpheresX, numSpheresY;
    private Vector3 startingCorner;

    public LayerMask obstacleLayer = 1 << 9;

    Node[,] areaOfNodes;

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
        maxSize = new Vector2(gameObject.GetComponent<Collider>().bounds.size.x, gameObject.GetComponent<Collider>().bounds.size.y);
        numSpheresX = Mathf.RoundToInt(maxSize.x / nodeRadius) / 2;
        numSpheresY = Mathf.RoundToInt(maxSize.y / nodeRadius) / 2;

        startingCorner = new Vector3(gameObject.GetComponent<Collider>().bounds.min.x, gameObject.GetComponent<Collider>().bounds.min.y, gameObject.transform.position.z);

        createGrid();
    }

    public void createGrid()
    {
        areaOfNodes = new Node[numSpheresX,numSpheresY];
        for (int x = 0; x < numSpheresX; x++)
        {
            for (int y = 0; y < numSpheresY; y++)
            {
                Vector3 nodePos = startingCorner + new Vector3((nodeRadius * 2) * x + nodeRadius, (nodeRadius * 2) * y + nodeRadius, 0.1f);
                //Init nodes
                areaOfNodes[x, y] = new Node(this, nodePos, new Vector2(x, y), 1);
            }
        }
        UpdateCosts();
    }

    public void UpdateCosts()
    {
        Debug.Log("Updating costs");
        Light2D[] sceneLights = FindObjectsOfType(typeof(Light2D)) as Light2D[];
        
        for (int x = 0; x < numSpheresX; x++)
        {
            for (int y = 0; y < numSpheresY; y++)
            {
                Vector3 nodePos = startingCorner + new Vector3((nodeRadius * 2) * x + nodeRadius, (nodeRadius * 2) * y + nodeRadius, 0.1f);
                //Check for obstacles
                areaOfNodes[x, y].canWalk = !Physics.CheckSphere(nodePos, nodeRadius, obstacleLayer);

                //Check for lights
                areaOfNodes[x, y].OnLightUpdate(sceneLights);
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

        //if (notLeftEdge) AddNodeIfValid(connections, nodes[x - 1, y]);
        //if (notRightEdge) AddNodeIfValid(connections, nodes[x + 1, y]);
        //if (notBottomEdge) AddNodeIfValid(connections, nodes[x, y - 1]);
        //if (notTopEdge) AddNodeIfValid(connections, nodes[x, y + 1]);

        //if (notLeftEdge && notBottomEdge) AddNodeIfValid(connections, nodes[x - 1, y - 1]);
        //if (notLeftEdge && notTopEdge) AddNodeIfValid(connections, nodes[x - 1, y + 1]);
        //if (notRightEdge && notBottomEdge) AddNodeIfValid(connections, nodes[x + 1, y - 1]);
        //if (notRightEdge && notTopEdge) AddNodeIfValid(connections, nodes[x + 1, y + 1]);

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

    public List<Node> GetNeighbors(Vector2 coord)
    {
        List<Node> neighborNodes = new List<Node>();
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int xLimit = x + Mathf.RoundToInt(coord.x);
                int zLimit = y + Mathf.RoundToInt(coord.y);

                if (xLimit > -1 && zLimit > -1 && xLimit < numSpheresX && zLimit < numSpheresY)
                {
                    neighborNodes.Add(areaOfNodes[xLimit, zLimit]);
                    if (x == y)
                        areaOfNodes[xLimit, zLimit].Weight = 1.4f;
                }
            }
        }
        return neighborNodes;
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
                    Handles.color = new Color(0, ((11.0f - node.Weight) / 10.0f)*0.6f+0.4f, 0);
                }
                if (!node.hasLight)
                {
                    Handles.color = Color.gray;
                }
                if (!node.canWalk)
                {
                    Handles.color = Color.black;
                }
                

                Handles.DrawWireDisc(node.position, Vector3.back, nodeRadius);

                if (node.gCost != -1)
                {
                    Handles.color = Color.blue;
                    Handles.DrawWireDisc(node.position, Vector3.back, nodeRadius / 2.0f);
                }
                //Handles.Label(node.position, node.weight > 10 ? "X" : node.weight.ToString());
                
                /*if (node == playerNode)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawSphere(node.myPos, nodeRadius);
                }*/
            }

            if (path != null)
            {
                Handles.color = Color.red;
                foreach (Node node in path)
                {
                    Handles.DrawWireDisc(node.position, Vector3.back, nodeRadius/2.0f);
                }
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

    IEnumerable<INode> path;
    public IEnumerable<INode> GetFringePath(GameObject Start, GameObject End)
    {
        Debug.Log("Fringe start");
        PathFinder.Fringe fringe = new PathFinder.Fringe(Heuristic);

        Node startNode = getNodeAtPos(Start.transform.position);
        Node endNode = getNodeAtPos(End.transform.position);

        path = fringe.FindPath((INode)startNode, (INode)endNode);
        
        /*foreach (Node n in areaOfNodes) n.Reset();

        Node startNode = getNodeAtPos(Start.transform.position);
        Node endNode = getNodeAtPos(End.transform.position);

        if (!endNode.hasLight) { Debug.Log("Fringe aborted"); return path; }

        float threshold = Heuristic(startNode, endNode);
        startNode.gCost = 0;
    
        Queue<Node> now = new Queue<Node>();
        Queue<Node> later = new Queue<Node>();
        
        now.Enqueue(startNode);
        
        Node current = null;
        while (threshold < 10 * numSpheresX * numSpheresX) //too far...
        {
            while (now.Count > 0)
            {
                current = now.Dequeue();
                if (current == endNode) break;

                float f = current.gCost + Heuristic(current, endNode);
                if (f <= threshold)
                {
                    List<Node> nearby = GetNeighbors(current);
                    foreach (Node n in nearby)
                    {
                        if (n.gCost == -1)
                        {
                            now.Enqueue(n);
                            n.parent = current;
                            n.gCost = current.gCost + n.Weight;
                        }
                    }
                }
                else
                {
                    later.Enqueue(current);
                }
            }

            if (current != endNode)
            {
                threshold++;
                now = later;
                later = new Queue<Node>();
            }
            else break;
        }

        //Reconstruct if found endNode
        if (current == endNode)
        {
            while (current != startNode)
            {
                path.Push(current.parent);
                current = current.parent;
            }
        }
        Debug.Log("Fringe end");*/
        return path;
    }

}
