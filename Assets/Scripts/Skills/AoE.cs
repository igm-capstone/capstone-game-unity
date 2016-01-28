using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AoE : ISkill
{
    public int Damage = 2;
    public float AreaRadius = 3;

    RpcNetworkAnimator animator;
    AvatarNetworkBehavior avatarNetwork;
    AvatarController avatarController;
    public Transform FX;

    public void Awake()
    {
        Name = "AoE";
        canDrop = true;

        animator = GetComponentInParent<RpcNetworkAnimator>();
        avatarNetwork = GetComponent<AvatarNetworkBehavior>();
        avatarController = GetComponent<AvatarController>();

        if (FX)
        {
            Transform oldParent = FX.parent;
            FX.parent = null;
            FX.localScale = new Vector3(AreaRadius, AreaRadius, 1);
            ;
            FX.parent = oldParent;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Use();
        }
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (avatarController.Disabled) return "You are incapacitated. Seek help!";

        animator.SetTrigger("AoE");

        var minions = Physics2D.OverlapCircleAll(transform.position, AreaRadius, 1 << LayerMask.NameToLayer("Minion"));

        foreach (var m in minions)
        {
            avatarNetwork.CmdAssignDamage(m.gameObject, Damage);
        }
        return null;
    }

}
