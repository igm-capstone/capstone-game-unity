using UnityEngine;
using System.Linq;

public class SetRegLantern : ISkill
{
    LanternType SlctdLant;
    AvatarController avatarController;
    public void Awake()
    {
        Name = "SetRegLantern";
        canDrop = false;

        SlctdLant = LanternType.Regular;

        // Set key code:
        key = KeyCode.Q;

        UseCount = 3;
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

        if (UseCount <= 0)
        {
            return "You have no more Lanterns of this type";
        }

        // Update amount.
        UseCount--;
        SkillBtnScript.UpdateUseAmount();

        TurnToMousePos();

        GetComponent<AvatarNetworkBehavior>().CmdSetLantExplorer(transform.position, SlctdLant);
        return null;
    }
}

