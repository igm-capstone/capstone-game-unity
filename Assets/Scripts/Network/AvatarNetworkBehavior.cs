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
        //meleeBehaviour.EnableSlash(status);
        RpcEnableSlash(status);
    }

    [ClientRpc]
    public void RpcEnableSlash(bool status)
    {
        //meleeBehaviour.EnableSlash(status);
    }


    [ClientRpc]
    public void RpcEnableMinion(string minionID)
    {
        GameObject minion = GameObject.Find(minionID);
        minion.SetActive(true);
    }

    [ClientRpc]
    public void RpcDisableMinion(string minionID)
    {
        if (gameObject.name == "Me")
        {
            GameObject minion = GameObject.Find(minionID);
            minion.SetActive(false);
        }
    }
}
