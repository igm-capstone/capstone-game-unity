﻿using UnityEngine;
using System.Linq;

public class SetTrapGlue : ISkill
{
    TrapType SlctdTrap;

    public float spawnDistance;
    Vector3 spawnPosition;

    AvatarController avatarController;
    public void Awake()
    {
        Name = "SetTrapGlue";
        canDrop = false;

        SlctdTrap = TrapType.Glue;

        spawnDistance = 1.0f;

        // Set key code:
        key = KeyCode.Alpha2;

        avatarController = GetComponent<AvatarController>();
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
        if (avatarController.isHidden) return "You are hiding";
        // Face mouse direction
        TurnToMousePos();

        spawnPosition = GetPosForwardFromAvatar(spawnDistance);

        // Use skill
        GetComponent<AvatarNetworkBehavior>().CmdSetTrapExplorer(spawnPosition, SlctdTrap);
        return null;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(spawnPosition, 0.5f);
    }
}

