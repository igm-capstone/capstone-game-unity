using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MoveBlock : ISkill
{
    [SerializeField]
    private LayerMask moveableObjectLayer = 1 << 12;
    [SerializeField]
    private float collisionRadius = 1.0f;

    private GameObject blockCollector;
    
    public void Awake()
    {
        blockCollector = GameObject.Find("BlocksCollector");
    }

    public void Update()
    {
        var moveableObstacle = Physics2D.OverlapCircleAll(transform.position, collisionRadius, moveableObjectLayer);
        if (moveableObstacle.Length > 0 && moveableObstacle[0].gameObject != null)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                transform.parent.GetComponent<AvatarNetworkBehavior>().CmdTakeBlockOver(moveableObstacle[0].name, true);
            }
            else
            {
                transform.parent.GetComponent<AvatarNetworkBehavior>().CmdTakeBlockOver(moveableObstacle[0].name, false);
            }
        }
    }

    protected override bool Usage(GameObject target, Vector3 clickWorldPos)
    {
        return true;
    }

    public void TakeBlockOver(string block, bool status)
    {
        GameObject blockNetID = GameObject.Find(block);
        
        if (status && blockNetID.transform.parent == blockCollector.transform)
        {
            blockNetID.transform.SetParent(transform.parent.transform);
        }
        else if (!status && blockNetID.transform.parent == transform.parent.transform)
        {
            blockNetID.transform.SetParent(blockCollector.transform);
        }
    }
}
