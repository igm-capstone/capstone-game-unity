using UnityEngine;
using System.Collections;
using System;

public class ConeAttack : ISkill
{
    RpcNetworkAnimator animator;
    AvatarNetworkBehavior avatarNetwork;
    AvatarController avatarController;
    Transform avatarModelTransform;
    GameObject hb;

    // Class Variable
    public int Damage = 2;
    public float radius = 2;
    public bool hasKnockBack = false;

    [SerializeField]
    [Range(0.0f, 10.0f)]
    float KnockBackMag = 5.0f;


    public void Awake()
    {
        Name = "ConeAttack";
        canDrop = false;

        avatarController = GetComponent<AvatarController>();
        avatarNetwork = GetComponent<AvatarNetworkBehavior>();
        animator = GetComponent<RpcNetworkAnimator>();

        avatarModelTransform = transform.FindChild("AvatarRotation").FindChild("AllAnimsInOne");
        hb = transform.FindChild("AvatarRotation").FindChild("ConeAttackHitBox").gameObject;
        hb.SetActive(false);

        key = KeyCode.Mouse0;
    }

    void Update()
    {
        if(Input.GetKeyDown(key))
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
        if (avatarController.Disabled) return "You are incapacitated. Seek Help!";

        //show hitbox 
        hb.SetActive(true);

        animator.SetTrigger("ConeAttack");
        GetComponent<AvatarController>().isAttacking = true;

        var minions = Physics2D.OverlapCircleAll(transform.position, radius, 1 << LayerMask.NameToLayer("Minion"));

        foreach (Collider2D m in minions)
        {
            var minionModel = m.gameObject.transform.FindChild("Rotation").FindChild("Model");
            if(Vector2.Dot(minionModel.transform.forward, avatarModelTransform.forward) < 0)
            {
                if (hasKnockBack)
                {
                    // Assign Damage with Force
                    avatarNetwork.CmdAssignDamageWithForce(m.gameObject, Damage, KnockBackMag);
                }
                else
                {
                    // Assign Damage
                    avatarNetwork.CmdAssignDamage(m.gameObject, Damage);
                }
            }
        }
        return null;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void ConeAttackAnimationComplete()
    {
        hb.SetActive(false);
    }
}
