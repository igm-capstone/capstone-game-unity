using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class GridBehavior : MonoBehaviour 
{
    private Vector2 maxSize;
    public float nodeRadius;

    private int numSpheresX, numSpheresY;
    private Vector3 startingCorner;

    public LayerMask obstacleLayer = 1 << 9;

    Node[,] areaOfNodes;

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
                areaOfNodes[x, y] = new Node(nodePos, true, new Vector2(x, y), 1);
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

    public List<Node> GetNeighbors(Node centerNode)
    {
        List<Node> neighborNodes = new List<Node>();
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                
                int xLimit = x + Mathf.RoundToInt(centerNode.coord.x);
                int zLimit = y + Mathf.RoundToInt(centerNode.coord.y);

                if (xLimit > -1 && zLimit > -1 && xLimit < numSpheresX && zLimit < numSpheresY)
                {
                    neighborNodes.Add(areaOfNodes[xLimit, zLimit]);
                    if (x == y)
                        areaOfNodes[xLimit, zLimit].weight = 1.4f;
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
                    Handles.color = new Color(0, ((11.0f - node.weight) / 10.0f)*0.6f+0.4f, 0);
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

            if (path.Count > 0)
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


    private float Heuristic(Node A, Node B)
    {
        //return Mathf.Max(Mathf.Abs(A.coord.x - B.coord.x), Mathf.Abs(A.coord.y - B.coord.y));
        float xDist = Mathf.Abs(A.coord.x - B.coord.x);
        float yDist = Mathf.Abs(A.coord.y - B.coord.y);
        if(xDist > yDist)
            return 1.4f*yDist + (xDist-yDist);
        return 1.4f*xDist + (yDist-xDist);
    }

    Stack<Node> path = new Stack<Node>();
    public Stack<Node> GetAStartPath(GameObject Start, GameObject End)
    {
        Debug.Log("Fringe start");
        path.Clear();
        foreach (Node n in areaOfNodes) n.Reset();

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
                            n.gCost = current.gCost + n.weight;
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
        Debug.Log("Fringe end");
        return path;
    }

}
