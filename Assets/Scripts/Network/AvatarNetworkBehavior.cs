using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

//[RequireComponent(typeof(MeleeWeaponBehavior))]
public class AvatarNetworkBehavior : BasePlayerNetworkBehavior
{
    private GameObject blockCollector;
    //private MeleeWeaponBehavior meleeBehaviour;

    void Start()
    {
        blockCollector = GameObject.Find("BlocksCollector");
    }

    public override void OnStartLocalPlayer()
    {
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
        obj.GetComponent<Health>().TakeDamage(damage);
    }

    [Command]
    public void CmdPickup(GameObject obj)
    {
        RpcPickup(obj);
        NetworkServer.Destroy(obj);
    }    

    [Command]
    public void CmdDoor(GameObject obj)
    {
        RpcDoor(obj);
    }

    [ClientRpc]
    public void RpcDoor(GameObject obj)
    {
        obj.GetComponentInParent<Door>().SwingDoor();
    }

    [ClientRpc]
    public void RpcPickup(GameObject obj)
    {
        Destroy(obj);
    }

    GameObject doorToOpen;
    public void OnTriggerStay2D(Collider2D other)
    {
        var door = other.GetComponentInParent<Door>();
        if (isClient && door)
        {
            doorToOpen = door.gameObject;
        }
    }
    
    public void Update()
    {
        if (doorToOpen && Input.GetKeyDown(KeyCode.M))
        {
            CmdDoor(doorToOpen);
            doorToOpen = null;
        }
    }
}
