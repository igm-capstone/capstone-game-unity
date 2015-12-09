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
    private AvatarController avatarController;
    private string currentBlock = null;
    
    public void Awake()
    {
        Name = "MoveBlock";
        canDrop = false;
        blockCollector = GameObject.Find("BlocksCollector");
        avatarController = GetComponent<AvatarController>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Use();
        }

        // Drop if dead
        if (IsActive && avatarController.Disabled)
        {
            GetComponent<AvatarNetworkBehavior>().CmdTakeBlockOver(currentBlock, false);
            currentBlock = null;
            IsActive = false;
        }
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (avatarController.Disabled) return "You are incapacitated. Seek help!";

        if (currentBlock != null)
        {
            GetComponent<AvatarNetworkBehavior>().CmdTakeBlockOver(currentBlock, false);
            currentBlock = null;
            IsActive = false;
            return null;
        }
        else
        {
            var moveableObstacle = Physics2D.OverlapCircleAll(transform.position, collisionRadius, moveableObjectLayer);
            if (moveableObstacle.Length > 0 && moveableObstacle[0].gameObject != null)
            {
                GetComponent<AvatarNetworkBehavior>().CmdTakeBlockOver(moveableObstacle[0].name, true);
                currentBlock = moveableObstacle[0].name;
                IsActive = true;
                return null;
            }
            else
            {
                return "You need to be close to a block to pick it up!";
            }
        }
    }

    public void TakeBlockOver(string block, bool status)
    {
        GameObject blockNetID = GameObject.Find(block);
        
        if (status && blockNetID.transform.parent == blockCollector.transform)
        {
            blockNetID.transform.SetParent(transform);
        }
        else if (!status && blockNetID.transform.parent == transform)
        {
            blockNetID.transform.SetParent(blockCollector.transform);
        }
    }
}
