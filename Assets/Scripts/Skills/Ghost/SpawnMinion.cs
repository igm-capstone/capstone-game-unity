using UnityEngine;
using System.Linq;

public class SpawnMinion : ISkill
{
    SkillBar mySkillBar;

    public void Awake()
    {
        Name = "SpawnMinion";
        canDrop = false;
        cost = 10;

        IsSpawnSkill = true;
        MinSpawnDist = 10f;
        MaxSpawnDist = 50f;

        key = KeyCode.Alpha2;

        mySkillBar = GetComponent<SkillBar>();
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
        if (node == null || !node.canWalk || node.hasLight)
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
            return Name + " skill cannot be used ouside of spawn circle.";
        }

        MinionSpawnManager.Instance.CmdSingleSpawn(clickWorldPos, MinionType.Meelee);

        // DebugCode
        foreach (var trgt in avatars)
        {
            Debug.DrawLine(trgt.transform.position, trgt.transform.position + transform.right * MinSpawnDist);
            Debug.DrawLine(trgt.transform.position, trgt.transform.position + transform.up * MaxSpawnDist);
        }

        return null;
    }
}
