using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AoE : ISkill
{
    RpcNetworkAnimator animator;
    AvatarNetworkBehavior avatarNetwork;
    AvatarController avatarController;
    public Transform FX;

    public int Damage = 2;
    public float AreaRadius = 3;
    public bool hasKnockBack = false;

    [SerializeField]
    [Range(0.0f, 10.0f)]
    float KnockBackMag = 5.0f;

    public void Awake()
    {
        Name = "AoE";
        canDrop = false;

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

        key = KeyCode.Mouse0;
    }

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            Use();
        }

        if (IsReady())
        {
            GetComponent<AvatarController>().isAttacking = false;
        }
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (avatarController.Disabled) return "You are incapacitated. Seek help!";

        animator.SetTrigger("AoE");
        GetComponent<AvatarController>().isAttacking = true;

        var minions = Physics2D.OverlapCircleAll(transform.position, AreaRadius, 1 << LayerMask.NameToLayer("Minion"));

        foreach (var m in minions)
        {
            // Check if KnockBack is Enabled
            if (hasKnockBack)
            {
                avatarNetwork.CmdAssignDamageWithForce(m.gameObject, Damage, KnockBackMag);
            }
            else
            {
                avatarNetwork.CmdAssignDamage(m.gameObject, Damage);
            }
        }
        return null;
    }

}
