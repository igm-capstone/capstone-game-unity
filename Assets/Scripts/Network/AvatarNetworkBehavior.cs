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

    [ClientRpc]
    public void RpcDisableMinion(GameObject minion)
    {
        if (hasAuthority)
        {
            foreach (var s in minion.GetComponentsInChildren<Renderer>())
            {
                s.enabled = false;
            }
            foreach (var s in minion.GetComponentsInChildren<Collider2D>())
            {
                s.enabled = false;
            }
            foreach (var s in minion.GetComponentsInChildren<Canvas>())
            {
                s.gameObject.SetActive(false);
            }
        }
    }

    [Command]
    public void CmdAssignDamage(GameObject obj, int damage)
    {
        // Assign Damage
        obj.GetComponent<Health>().TakeDamage(damage);
    }

    [Command]
    public void CmdAssignDamageWithForce(GameObject obj, int damage, float KnockBackAmount)
    {
        // Assign Damage
        obj.GetComponent<Health>().TakeDamage(damage);
        
        // Calculate KnockBackForce
        Vector3 TransForce = (obj.transform.position - transform.position).normalized* KnockBackAmount;
        // Apply Knock back and start stunn timer.
        obj.transform.Translate(TransForce, Space.Self);
        obj.GetComponent<TargetFollower>().StartStunTimer();
    }

    [Command]
    public void CmdPickup(GameObject obj)
    {
        RpcPickup(obj);
        NetworkServer.Destroy(obj);
    }

    [ClientRpc]
    public void RpcPickup(GameObject obj)
    {
        Destroy(obj);
    }

    [Command]
    public void CmdSetTrapExplorer(Vector3 position, TrapType _SlctdTrap)
    {
        TrapSpawnManager.Instance.CmdSetTrap(transform.position, _SlctdTrap);
    }

}
