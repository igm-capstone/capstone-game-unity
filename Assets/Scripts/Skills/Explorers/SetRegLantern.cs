using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

public class SetRegLantern : ISkill
{
    LanternType SlctdLant;
    AvatarController avatarController;
    float spawnDistance;

    [SerializeField]
    bool hasLimitedUses = false;

    int maxLanternCount;
    float refillTime;

    public override string Name { get { return "Drop Lantern"; } }


    public void Awake()
    {
        // Class SetUps
        canDrop = false;

        SlctdLant = LanternType.Regular;

        // Set key code:
        key = KeyCode.Mouse1;
        
        // Component Getters
        avatarController = GetComponent<AvatarController>();
        
        // Initialization
        SlctdLant = LanternType.Regular;
        spawnDistance = 1.0f;
        maxLanternCount = UseCount;


        // ToolTip text
        ToolTip.Description = "Places a Latern down.\nLimited amount.";

        ToolTip.FirstLabel =        "Duration:";
        ToolTip.FirstAttribute =    "8 sec";

        ToolTip.SecondLabel =       "Cooldown:";
        ToolTip.SecondAttribute =   Cooldown.ToString() + " sec";

        ToolTip.ThirdLabel =        "";
        ToolTip.ThirdAttribute =    "";


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
            //Auto refill of lantern
            //if (UseCount < maxLanternCount && UseCount>= 0)
            //{
            //    StopCoroutine("RefillLantern");
            //    StartCoroutine("RefillLantern");
            //}
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

    //Auto refill of lantern
    IEnumerator RefillLantern()
    {
        //(5, 36)
        //(4, 25)
        //(3, 16)
        //(2, 10);
        //(1, 7)
        //(0, 6);

        refillTime = (1.32f * (Mathf.Pow(UseCount, 2))) - (0.6f * UseCount) + 6f;
        yield return new WaitForSeconds(refillTime);

        UseCount++;
        SkillBtnScript.UpdateUseAmount();

        if (UseCount < maxLanternCount)
            StartCoroutine("RefillLantern");
    }    


    //To pick up lanterns
    public void PickUpLantern()
    {
        UseCount++;
        SkillBtnScript.UpdateUseAmount();
    }

    public bool CanPickUpLantern()
    {
        return UseCount < maxLanternCount;
    }
}

