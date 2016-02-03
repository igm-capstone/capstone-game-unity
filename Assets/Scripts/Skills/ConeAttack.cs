using UnityEngine;
using System.Collections;
using System;

public class ConeAttack : ISkill
{
    public int damage = 2;
    public float radius = 2;

    RpcNetworkAnimator animator;
    AvatarNetworkBehavior avatarNetwork;
    AvatarController avatarController;
    Transform avatarModelTransform;
    GameObject hb;
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

        key = KeyCode.J;
    }

    void Update()
    {
        if(Input.GetKeyDown(key))
        {
            Use();
        }
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (avatarController.Disabled) return "You are incapacitated. Seek Help!";

        //show hitbox 
        hb.SetActive(true);

        animator.SetTrigger("ConeAttack");
        var minions = Physics2D.OverlapCircleAll(transform.position, radius, 1 << LayerMask.NameToLayer("Minion"));

        foreach (Collider2D m in minions)
        {
            var minionModel = m.gameObject.transform.FindChild("Rotation").FindChild("Model");
            if(Vector2.Dot(minionModel.transform.forward, avatarModelTransform.forward) < 0)
            {
                avatarNetwork.CmdAssignDamage(m.gameObject, damage);
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
