using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridBehavior : MonoBehaviour 
{
    private Vector2 maxSize;
    public float nodeRadius;

    private int nodeCost, numSpheresX, numSpheresY;
    private LayerMask obstacleLayer = 1 << 9;
    private Vector3 startingCorner;
    
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
                areaOfNodes[x, y] = new Node(nodePos, true, new Vector2(x, y), 0);
            }
        }
        updateWeights();
    }

    public void updateWeights()
    {
        for (int x = 0; x < numSpheresX; x++)
        {
            for (int y = 0; y < numSpheresY; y++)
            {
                Vector3 nodePos = startingCorner + new Vector3((nodeRadius * 2) * x + nodeRadius, (nodeRadius * 2) * y + nodeRadius, 0.1f);
                //Check for obstacles
                areaOfNodes[x, y].canWalk = !Physics.CheckSphere(nodePos, nodeRadius, obstacleLayer);
                if (!areaOfNodes[x, y].canWalk)
                    areaOfNodes[x, y].cost = 100000;

                //Check for lights
                //areaOfNodes[x, y].OnLightUpdate(new GameObject[]); //TODO: What will be a moveable light GO??
            }
        }
    }

    public Node getNodeAtPos(Vector3 pos)
    {
        Vector2 percent = new Vector2(Mathf.Clamp01((pos.x + maxSize.x / 2) / maxSize.x), Mathf.Clamp01((pos.z + maxSize.y / 2) / maxSize.y));

        int gridCoordX = Mathf.RoundToInt((numSpheresX - 1) * percent.x);
        int gridCoordY = Mathf.RoundToInt((numSpheresY - 1) * percent.y);

        return areaOfNodes[gridCoordX, gridCoordY];
    }

    public List<Node> neighbors(Node centerNode)
    {
        List<Node> neighborNodes = new List<Node>();
        for (int x = -1; x < 2; x++)
        {
            for (int z = -1; z < 2; z++)
            {
                if (x == 0 && z == 0)
                    continue;

                int xLimit = x + Mathf.RoundToInt(centerNode.coord.x);
                int zLimit = z + Mathf.RoundToInt(centerNode.coord.y);

                if (xLimit > -1 && zLimit > -1 && xLimit < numSpheresX && zLimit < numSpheresY)
                    neighborNodes.Add(areaOfNodes[xLimit, zLimit]);
            }
        }
        return neighborNodes;
    }

    public List<Node> aStarPath = new List<Node>();
    public LinkedList<Node> fringePath = new LinkedList<Node>();
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if (areaOfNodes != null)
        {
            //Node playerNode = getNodeAtPos(player.position);
            foreach (Node node in areaOfNodes)
            {
                if (node.canWalk)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.white;
                }
                UnityEditor.Handles.DrawWireDisc(node.position, Vector3.back, nodeRadius*0.8f);
                UnityEditor.Handles.Label(node.position, node.cost > 99 ? "X" : node.cost.ToString());

                /*if (node == playerNode)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawSphere(node.myPos, nodeRadius);
                }*/
            }

            if (fringePath.Count > 0)
            {
                Gizmos.color = Color.black;
                foreach (Node n in fringePath)
                {
                    //Gizmos.DrawSphere(n.myPos, nodeRadius);
                }
            }

            if (aStarPath.Count > 0)
            {
                Gizmos.color = Color.black;
                foreach (Node n in aStarPath)
                    Gizmos.DrawSphere(n.position, nodeRadius);
            }
        }
    }

    public int GetMaxSize
    {
        get { return numSpheresX * numSpheresY; }
    }
}
