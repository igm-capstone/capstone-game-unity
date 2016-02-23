using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EquippedSkill : MonoBehaviour
{
    GameObject curSkillObj = null;
    ISkill curSkillScrpt = null;

    SkillBar skillBarScr = null;

    KeyCode UseEquipped;
    KeyCode ChangeSkill;

    public void Start()
    {

        // Right Mouse Button
        UseEquipped = KeyCode.Mouse1;
        ChangeSkill = KeyCode.LeftShift;

        skillBarScr = GetComponent<SkillBar>();

        skillBarScr.SetSkillCapacity(skillBarScr.GetSkillCapacity()+1);

        // Updates Script
        curSkillScrpt = curSkillObj.GetComponent<ISkill>();
        // Updates Skill key
        curSkillScrpt.key = UseEquipped;
        curSkillScrpt.canDrop = true;

        Debug.Log(curSkillScrpt);

        // Adds new skill to the skillbar
        //skillBarScr.AddSkill(curSkillScrpt);
        curSkillScrpt.enabled = true;
    }

    void Update()
    {
        // Changes equipped skill
        if (Input.GetKeyDown(ChangeSkill))
        {
            switchSkill();
        }

        // Uses equiped skill
        if (Input.GetKeyDown(UseEquipped))
        {
            curSkillScrpt.Use();
        }

        if (curSkillScrpt.IsReady())
        {
            GetComponent<AvatarController>().isAttacking = false;
        }

    }

    // Changes equipped skill
    void switchSkill()
    {
        Debug.Log("Trying to change skill");

        // Updates Script
        curSkillScrpt = curSkillObj.GetComponent<ISkill>();
        // Updates Skill key
        curSkillScrpt.key = UseEquipped;

        // Adds new skill to the skillbar
        //skillBarScr.AddSkill(curSkillScrpt);
        curSkillScrpt.enabled = true;

        curSkillScrpt.canDrop = true;

        Debug.Log("Current Skill script equipped: " + curSkillScrpt);

    }
}
