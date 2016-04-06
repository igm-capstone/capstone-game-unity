using UnityEngine;
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
        ToolTip.Description =       "Places a Poison trap.\nDamages enemies.";

        ToolTip.FirstLabel =        "Damage:";
        ToolTip.FirstAttribute =    "9 per sec";

        ToolTip.SecondLabel =       "Duration:";
        ToolTip.SecondAttribute =   "5 sec";

        ToolTip.ThirdLabel =        "";
        ToolTip.ThirdAttribute =    "";

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

