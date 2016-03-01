using UnityEngine;
using System.Linq;

public class SpawnAOE : ISkill
{
    public float minSpawnDist = 5f;
    public float maxSpawnDist = 150f;


    public void Awake()
    {
        Name = "SpawnAOE";
        canDrop = false;
        cost = 30;
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
        var minSqrDist = minSpawnDist * minSpawnDist;
        var maxSqrDist = maxSpawnDist * maxSpawnDist;
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
            Debug.DrawLine(trgt.transform.position, trgt.transform.position + transform.right * minSpawnDist);
            Debug.DrawLine(trgt.transform.position, trgt.transform.position + transform.up * maxSpawnDist);
        }

        return null;
    }
}
