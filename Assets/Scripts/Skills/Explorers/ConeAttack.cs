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

    public LayerMask HitLayers = 1 << LayerMask.NameToLayer("Minion") | 1 << LayerMask.NameToLayer("Player");

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

        key = KeyCode.O;
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

        var Targets = Physics2D.OverlapCircleAll(transform.position, radius, HitLayers);

        foreach (Collider2D trgt in Targets)
        {
            // Check if I am hitting myself
            if (trgt.gameObject == this.gameObject)
            {
                return null;
            }
            var minionModel = trgt.gameObject.transform.FindChild("Rotation").FindChild("Model");
            if(Vector2.Dot(minionModel.transform.forward, avatarModelTransform.forward) < 0)
            {
                if (hasKnockBack)
                {
                    // Assign Damage with Force
                    avatarNetwork.CmdAssignDamageWithForce(trgt.gameObject, Damage, KnockBackMag);
                }
                else
                {
                    // Assign Damage
                    avatarNetwork.CmdAssignDamage(trgt.gameObject, Damage);
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
