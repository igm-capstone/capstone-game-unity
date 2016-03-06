using UnityEngine;
using System.Linq;

public class SetRegLantern : ISkill
{
    LanternType SlctdLant;
    AvatarController avatarController;
    float spawnDistance;

    [SerializeField]
    bool hasLimitedUses = false;

    public void Awake()
    {
        // Class SetUps
        Name = "SetRegLantern";
        canDrop = false;

        SlctdLant = LanternType.Regular;

        // Set key code:
        key = KeyCode.Mouse1;

        UseCount = 3;
        
        // Component Getters
        avatarController = GetComponent<AvatarController>();
        
        // Initialization
        SlctdLant = LanternType.Regular;
        spawnDistance = 1.0f;

    }    

    void Start()
    {
        if (!hasLimitedUses)
        {
            // Make it zero to remove number of uses from the UI.
            UseCount = 0;
            SkillBtnScript.UpdateUseAmount();
        }

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

        if (UseCount <= 0 && hasLimitedUses)
        {
            return "You have no more Lanterns of this type";
        }

        if (hasLimitedUses)
        {
            // Update amount.
            UseCount--;
            SkillBtnScript.UpdateUseAmount();
        }

        TurnToMousePos();

        Vector3 spawnLocation = GetPosForwardFromAvatar(spawnDistance);

        GetComponent<AvatarNetworkBehavior>().CmdSetLantExplorer(spawnLocation, SlctdLant, Cooldown);
        return null;
    }
}

