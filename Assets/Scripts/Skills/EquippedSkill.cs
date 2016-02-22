using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EquippedSkill : ISkill
{
    public List<ISkill> SkillList = new List<ISkill>();
    ISkill curSkill = null;

    float clkdtimer = 0.0f;
    float waitTime = 0.0f;

    public void Awake()
    {
        Name = "Equipped skill";
        canDrop = false;

        // Right Mouse Button
        key = KeyCode.Mouse1;
    }


    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            Use();
        }

        if (IsReady())
        {
            GetComponent<AvatarController>().isAttacking = false;
        }

    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (curSkill == null)
        {
            return "Error, there are no skills equipped";
        }
        curSkill.Use(target, clickWorldPos);
        return null;
    }
}
