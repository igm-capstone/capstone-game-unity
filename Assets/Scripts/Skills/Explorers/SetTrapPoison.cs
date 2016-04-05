﻿using UnityEngine;
using System.Linq;

public class SetTrapPoison : ISkill
{
    TrapType SlctdTrap;

    float spawnDistance;
    Vector3 spawnPosition;

    AvatarController avatarController; 
    public void Awake()
    {
        Name = "Poison Trap";
        canDrop = false;

        SlctdTrap = TrapType.Poison;
        spawnDistance = 1.0f;

        // Set key code:
        key = KeyCode.LeftShift;
        avatarController = GetComponent<AvatarController>();

        // ToolTip text
        ToolTip.Description = "Places a Poison trap.\nDamages enemies.";
        ToolTip.AtkAttrbt=      "9 per second";
        ToolTip.DefAttrbt =     "N/A";
        ToolTip.SpdAttrbt =     "N/A";

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

