using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;

public class Haunt_ExpToMinion : ISkill {

    public GameObject HauntMeleeMinion;
    public float HauntDuration = 15;

    SkillBar mySkillBar;

    public void Awake()
    {
        Name = "Imp Illusion";
        canDrop = false;

        key = KeyCode.Alpha4;

        mySkillBar = GetComponent<SkillBar>();

        // ToolTip text
        ToolTip.Description =       "Disguise an Explorer as an Imp.";

        ToolTip.FirstLabel =        "Duration:";
        ToolTip.FirstAttribute =    HauntDuration.ToString() + "sec";

        ToolTip.SecondLabel =       "Cooldown:";
        ToolTip.SecondAttribute =   Cooldown.ToString() + "sec";

        ToolTip.ThirdLabel =        "Cost:";
        ToolTip.ThirdAttribute =    cost.ToString() + " MP";

    }

    public void Update()
    {
        if (Input.GetKeyDown(key))
        {
            mySkillBar.SetActiveSkill(this);
        }
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (target.tag != "Player") return Name + " skill needs to target an explorer.";                
        
        var selfAc = target.GetComponent<AvatarNetworkBehavior>();

        MinionSpawnManager.Instance.CmdHauntSpawn(HauntDuration, target.transform.position, MinionType.HauntMelee, target.GetComponentInParent<NetworkIdentity>().gameObject);

        return null;
    }    
}
