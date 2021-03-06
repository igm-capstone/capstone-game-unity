﻿using UnityEngine;
using System.Linq;

public class SpawnAOE : ISkill
{
    SkillBar mySkillBar;

    public override string Name {  get { return "Abomination"; } }

    public void Awake()
    {
        canDrop = false;

        IsSpawnSkill = true;
        MinSpawnDist = 13f;
        MaxSpawnDist = 60f;

        key = KeyCode.Alpha2;

        mySkillBar = GetComponent<SkillBar>();

        // ToolTip text
        ToolTip.Description =       "Spawns Abomination.\nExplodes on contact.";
        ToolTip.FirstAttribute =    "High";
        ToolTip.SecondAttribute =   "High";
        ToolTip.ThirdAttribute =    "Low";

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
        if (target.layer != LayerMask.NameToLayer("Floor")) return Name + " skill needs to hit the floor. Layer found: " + target.layer.ToString();

        var grid = GridBehavior.Instance;
        var z = grid.transform.position.z;
        clickWorldPos.z = z;

        var node = grid.getNodeAtPos(clickWorldPos);
        if (!node.canWalk || node.hasLight)
        {
            //Debug.LogFormat("Lit area {0}, unwalkable area {0}", node.hasLight, !node.canWalk);
            return Name + " skill can only be used in the darkness!";
        }

        var avatars = FindObjectsOfType<AvatarController>();
        var minSqrDist = MinSpawnDist * MinSpawnDist;
        var maxSqrDist = MaxSpawnDist * MaxSpawnDist;
        var plyrDistance = avatars.Select(a => (a.transform.position - clickWorldPos).sqrMagnitude).OrderBy(d => d).FirstOrDefault();


        if (avatars.Any() && ((plyrDistance < minSqrDist) || (plyrDistance > maxSqrDist)))
        {
            //Debug.Log("Near player");
            return Name + " skill must be used inside spawn area.";
        }

        MinionSpawnManager.Instance.CmdSingleSpawn(clickWorldPos, MinionType.AOEBomber);

        // DebugCode
        foreach (var trgt in avatars)
        {
            Debug.DrawLine(trgt.transform.position, trgt.transform.position + transform.right * MinSpawnDist);
            Debug.DrawLine(trgt.transform.position, trgt.transform.position + transform.up * MaxSpawnDist);
        }

        return null;
    }
}
