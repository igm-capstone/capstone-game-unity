﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AoE : ISkill
{
    RpcNetworkAnimator animator;
    AvatarNetworkBehavior avatarNetwork;
    AvatarController avatarController;
    public Transform FX;

    public LayerMask HitLayers = 1 << LayerMask.NameToLayer("Minion") | 1 << LayerMask.NameToLayer("Player");

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

        key = KeyCode.O;
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

        var Targets = Physics2D.OverlapCircleAll(transform.position, AreaRadius, HitLayers);

        foreach (var trgt in Targets)
        {
            // Check if I am hitting myself
            if (trgt.gameObject == this.gameObject)
            {
                return null;
            }
            // Check if KnockBack is Enabled
            if (hasKnockBack)
            {
                avatarNetwork.CmdAssignDamageWithForce(trgt.gameObject, Damage, KnockBackMag);
            }
            else
            {
                avatarNetwork.CmdAssignDamage(trgt.gameObject, Damage);
            }
        }
        return null;
    }

}
