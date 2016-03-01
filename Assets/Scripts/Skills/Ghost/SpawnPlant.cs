using UnityEngine;
using System.Linq;
using System.Collections;

public class SpawnPlant : ISkill
{
    public float minSpawnDist = 5f;
    public float maxSpawnDist = 150f;

    public void Awake()
    {
        Name = "SpawnPlant";
        canDrop = false;
        cost = 10;
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
        var minSqrDist = minSpawnDist * minSpawnDist;
        var maxSqrDist = maxSpawnDist * maxSpawnDist;
        var plyrDistance = avatars.Select(a => (a.transform.position - clickWorldPos).sqrMagnitude).OrderBy(d => d).FirstOrDefault();


        if (avatars.Any() && ((plyrDistance < minSqrDist) || (plyrDistance > maxSqrDist)))
        {
            //Debug.Log("Near player");
            return Name + " skill cannot be used ouside of spawn circle.";
        }

        MinionSpawnManager.Instance.CmdSingleSpawn(clickWorldPos, MinionType.Plant);

        return null;
    }
}
