using UnityEngine;
using System.Linq;

public class SetRegLantern : ISkill
{
    LanternType SlctdLant;
    AvatarController avatarController;

    float spawnDistance;
    public void Awake()
    {
        Name = "SetRegLantern";
        canDrop = false;

        SlctdLant = LanternType.Regular;

        // Set key code:
        key = KeyCode.Mouse1;

        UseCount = 3;
        avatarController = GetComponent<AvatarController>();

        spawnDistance = 1.0f;
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

        if (UseCount <= 0)
        {
            return "You have no more Lanterns of this type";
        }

        // Update amount.
        UseCount--;
        SkillBtnScript.UpdateUseAmount();

        TurnToMousePos();

        Vector3 spawnLocation = GetPosForwardFromAvatar(spawnDistance);

        GetComponent<AvatarNetworkBehavior>().CmdSetLantExplorer(spawnLocation, SlctdLant, Cooldown);
        return null;
    }
}

