using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;

public class Haunt_ExpToMinion : ISkill {

    public GameObject HauntMeleeMinion;

    public void Awake()
    {
        Name = "ExplorerToMinion";
        canDrop = false;
        cost = 15;
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (target.tag != "Player") return Name + " skill needs to target an explorer.";                
        
        var selfAc = target.GetComponent<AvatarNetworkBehavior>();
        MinionSpawnManager.Instance.CmdHauntSpawn(target.transform.position, MinionType.HauntMelee, target.GetComponentInParent<NetworkIdentity>().gameObject);
        //target.GetComponentInParent<NetworkIdentity>().gameObject.GetComponent<AvatarController>().hauntEtoM = true;
        return null;
    }    
}
