using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridBehavior : MonoBehaviour 
{
    private Vector2 maxSize;
    public float nodeRadius;

    private int nodeCost, numSpheresX, numSpheresZ;
    private LayerMask obstacle = 1 << 9;
    private LayerMask playerMask = 1 << 10;
    
    NodeBehavior[,] areaOfNodes;
    //public Transform player;

    void Start()
    {
        maxSize = new Vector2(gameObject.GetComponent<Collider>().bounds.size.x, gameObject.GetComponent<Collider>().bounds.size.z);
        numSpheresX = Mathf.RoundToInt(maxSize.x / nodeRadius) / 2;
        numSpheresZ = Mathf.RoundToInt(maxSize.y / nodeRadius) / 2;
        createGrid();
    }

    public void createGrid()
    {
        areaOfNodes = new NodeBehavior[numSpheresX,numSpheresZ];
        Vector3 startingCorner = new Vector3(gameObject.GetComponent<Collider>().bounds.min.x,gameObject.transform.position.y, gameObject.GetComponent<Collider>().bounds.min.z);

        for (int x = 0; x < numSpheresX; x++)
        {
            for (int z = 0; z < numSpheresZ; z++)
            {
                Vector3 nodePos = startingCorner + new Vector3((nodeRadius * 2) * x + nodeRadius, 0.1f, (nodeRadius * 2) * z + nodeRadius);
                bool canWalk = !Physics.CheckSphere(nodePos, nodeRadius, obstacle);
                if(canWalk) {
                    areaOfNodes[x,z] = new NodeBehavior();
                    areaOfNodes[x,z].SetNodeProperties(nodePos, canWalk, new Vector2(x,z),1);
                } else {
                    areaOfNodes[x, z] = new NodeBehavior();
                    areaOfNodes[x, z].SetNodeProperties(nodePos, canWalk, new Vector2(x, z), 1);
                }
            }
        }
    }

    public NodeBehavior getNodeAtPos(Vector3 pos)
    {

        Vector2 percent = new Vector2(Mathf.Clamp01((pos.x + maxSize.x / 2) / maxSize.x), Mathf.Clamp01((pos.z + maxSize.y / 2) / maxSize.y));

        int gridCoordX = Mathf.RoundToInt((numSpheresX - 1) * percent.x);
        int gridCoordY = Mathf.RoundToInt((numSpheresZ - 1) * percent.y);

        //if (Physics.CheckSphere(areaOfNodes[gridCoordX, gridCoordY].myPos, nodeRadius, objLayer))
        return areaOfNodes[gridCoordX, gridCoordY];
//        return null;
    }

    public List<NodeBehavior> neighbors(NodeBehavior centerNode)
    {
        List<NodeBehavior> neighborNodes = new List<NodeBehavior>();
        for (int x = -1; x < 2; x++)
        {
            for (int z = -1; z < 2; z++)
            {
                if (x == 0 && z == 0)
                    continue;

                int xLimit = x + Mathf.RoundToInt(centerNode.myCoord.x);
                int zLimit = z + Mathf.RoundToInt(centerNode.myCoord.y);

                if (xLimit > -1 && zLimit > -1 && xLimit < numSpheresX && zLimit < numSpheresZ)
                    neighborNodes.Add(areaOfNodes[xLimit, zLimit]);
            }
        }
        return neighborNodes;
    }

    public List<NodeBehavior> aStarPath = new List<NodeBehavior>();
    public LinkedList<NodeBehavior> fringePath = new LinkedList<NodeBehavior>();
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        if(maxSize != null)
            Gizmos.DrawWireCube(gameObject.transform.position, new Vector3(maxSize.x,1,maxSize.y));
        
        if (areaOfNodes != null)
        {
            //NodeBehavior playerNode = getNodeAtPos(player.position);
            foreach (NodeBehavior node in areaOfNodes)
            {
                if (node.walkable)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawSphere(node.myPos, nodeRadius);
                }
                else
                {
                    Gizmos.color = Color.white;
                    Gizmos.DrawSphere(node.myPos, nodeRadius);
                }

                /*if (node == playerNode)
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawSphere(node.myPos, nodeRadius);
                }*/
            }

            if (fringePath.Count > 0)
            {
                Gizmos.color = Color.black;
                foreach (NodeBehavior n in fringePath)
                {
                    //Gizmos.DrawSphere(n.myPos, nodeRadius);
                }
            }

            if (aStarPath.Count > 0)
            {
                Gizmos.color = Color.black;
                foreach (NodeBehavior n in aStarPath)
                    Gizmos.DrawSphere(n.myPos, nodeRadius);
            }
        }
    }

    public int GetMaxSize
    {
        get { return numSpheresX * numSpheresZ; }
    }
}
