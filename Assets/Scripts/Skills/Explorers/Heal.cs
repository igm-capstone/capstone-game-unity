﻿using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Heal : ISkill
{
    public int HealAmount = 1;
    public float AreaRadius = 3;

    RpcNetworkAnimator animator;
    AvatarNetworkBehavior avatarNetwork;
    AvatarController avatarController;
    public Transform FX;

    public void Awake()
    {
        Name = "Heal";
        canDrop = false;

        animator = GetComponentInParent<RpcNetworkAnimator>();
        avatarNetwork = GetComponent<AvatarNetworkBehavior>();
        avatarController = GetComponent<AvatarController>();

        if (FX == null)
        {
           FX =  transform.FindChild("AvatarRotation").FindChild("AllAnimsInOne").FindChild("HealFX");
        }

        if (FX)
        {
            Transform oldParent = FX.parent;
            FX.parent = null;
            FX.localScale = new Vector3(AreaRadius, AreaRadius, 1);
            FX.parent = oldParent;
        }

        // Set key code:
        key = KeyCode.Alpha1;
    }

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            Use();
        }
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (avatarController.Disabled) return "You are incapacitated. Seek help!";
        
        animator.SetTrigger("Heal");

        var players = Physics2D.OverlapCircleAll(transform.position, AreaRadius, 1 << LayerMask.NameToLayer("Player"));

        foreach (var p in players)
        {
            // NEgative damage means healing.
            avatarNetwork.CmdAssignDamage(p.gameObject, -HealAmount);
        }
        return null;
    }

}
