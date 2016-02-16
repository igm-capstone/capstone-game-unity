using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public enum TrapType
{
    Poison,
    Glue,
    Spring
}

public class SpawnTrap: ISkill
{
    /* Commented for now, will be useful for animations later
    RpcNetworkAnimator animator;
    AvatarNetworkBehavior avatarNetwork;
    */
    AvatarController avatarController;
    

    TrapType SlctdTrap;

    public GameObject SlctdTrapObj;

    public void Awake()
    {
        Name = "SetTrap";
        canDrop = false;

        /* Commented for now, will be useful for animations later
        animator = GetComponentInParent<RpcNetworkAnimator>();
        avatarNetwork = GetComponent<AvatarNetworkBehavior>();
        */

        avatarController = GetComponent<AvatarController>();
        

        SlctdTrap = TrapType.Poison;

        key = KeyCode.Mouse1;
    }

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            Use();
        }
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (avatarController.Disabled) return "You are incapacitated. Seek help!";

        switch (SlctdTrap)
        {
            case TrapType.Poison:
                Debug.Log(TrapType.Poison.ToString());
                break;

            case TrapType.Glue:
                Debug.Log(TrapType.Glue.ToString());
                break;

            case TrapType.Spring:
                Debug.Log(TrapType.Spring.ToString());
                break;

            default:
                Debug.Log(TrapType.Poison.ToString());
                break;
        }

        Instantiate(SlctdTrapObj, transform.position, transform.rotation);

        return null;
    }

    [RcpServer]

}
