using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class MeleeWeapon : ISkill
{
    public int Damage = 1;

    RpcNetworkAnimator animator;
    AvatarNetworkBehavior avatarNetwork;
    AvatarController avatarController;
    
    public void Awake()
    {
        Name = "Melee";
        canDrop = false;

        avatarController = GetComponent<AvatarController>();
        avatarNetwork = GetComponent<AvatarNetworkBehavior>();
        animator = GetComponent<RpcNetworkAnimator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Use();
        }
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (avatarController.Disabled) return "You are incapacitated. Seek help!";

        //TODO: This is a circle, but it should be a cone in front of the player!
        var minions = Physics2D.OverlapCircleAll(transform.position, 1.0f, 1 << LayerMask.NameToLayer("Minion"));

        foreach (var m in minions)
        {
            avatarNetwork.CmdAssignDamage(m.gameObject, Damage);
        }

        animator.SetTrigger("Attack"); //StartCoroutine(Slash());
        return null;
    }
}
