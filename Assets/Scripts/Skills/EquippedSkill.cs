using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EquippedSkill : MonoBehaviour
{
    List<ISkill> skillList = null;
    ISkill curEquipSkill = null;
    bool isFirstFrame = true;

    KeyCode UseKey;
    KeyCode ChangeSkill;

    public void Start()
    {
        // Right Mouse Button
        UseKey = KeyCode.Mouse1;

        ChangeSkill = KeyCode.Space;

        isFirstFrame = true;

        skillList = new List<ISkill>();

        var allSkillScrpts = GetComponents<ISkill>();

        foreach (var skill in allSkillScrpts)
        {
            Debug.Log("Skills found: " + skill);
            // If skill is not static, adds to skill list
            if (!skill.isStaticSkill)
            {
                skillList.Add(skill);
                skill.enabled = false;
            }// if
        }// foreach

        // Only one non-static skill found, enable it and disable this script
        if (skillList.Count <= 1)
        {
            skillList[0].enabled = true;
            this.enabled = false;
        }

        // Equip first skill
        curEquipSkill = skillList[0];
        curEquipSkill.key = UseKey;

        // Remove equipped skill from top of list
        skillList.Remove(curEquipSkill);

    }

    void Update()
    {
        // Equip first skill and make sure it is at the correct position.
        if (isFirstFrame)
        {
            switchSkill();
            switchSkill();
            isFirstFrame = false;
        }

        // Changes equipped skill
        if (Input.GetKeyDown(ChangeSkill))
        {
            switchSkill();
        }
    }

    // Changes equipped skill
    void switchSkill()
    {
        //Unequip currently equipped
        curEquipSkill.enabled = false;
        
        // Add skill to bottom of list
        skillList.Add(curEquipSkill);

        // Equip skill
        curEquipSkill = skillList[0];
        curEquipSkill.enabled = true;
        curEquipSkill.key = UseKey;

        // Remove equipped skill from top of list
        skillList.Remove(curEquipSkill);
    }
}
