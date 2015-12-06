using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//[RequireComponent(typeof(MeleeWeaponBehavior))]
public class AvatarNetworkBehavior : BasePlayerNetworkBehavior {
    private GameObject blockCollector;
    //private MeleeWeaponBehavior meleeBehaviour;
    
    void Start()
    {
        blockCollector = GameObject.Find("BlocksCollector");
    }

	public override void OnStartLocalPlayer () {
        GetComponentInChildren<AvatarController>().enabled = true;
        GetComponentInChildren<MovementBroadcast>().enabled = true;
        base.OnStartLocalPlayer();
    }

    [Command]
    public void CmdTakeBlockOver(string block, bool status)
    {
        RpcTakeBlockOver(block, status);
    }
    
    [ClientRpc]
    void RpcTakeBlockOver(string block, bool status)
    {
        GetComponentInChildren<MoveBlock>().TakeBlockOver(block, status);
    }

    [Command]
    public void CmdEnableSlash(bool status)
    {
        RpcEnableSlash(status);
    }

    [ClientRpc]
    public void RpcEnableSlash(bool status)
    {
        GetComponentInChildren<MeleeWeapon>().EnableSlash(status);
    }


    [ClientRpc]
    public void RpcDisableMinion(GameObject minion)
    {
        if (hasAuthority) minion.SetActive(false);
    }

    [Command]
    public void CmdAssignDamage(GameObject obj, int damage)
    {
        obj.GetComponent<Health>().TakeDamage(damage);
    }
}
