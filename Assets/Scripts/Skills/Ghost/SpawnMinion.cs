using UnityEngine;
using System.Linq;

public class SpawnMinion : ISkill
{
    public float distanceFromPlayers = 5f;

    public void Awake()
    {
        Name = "SpawnMinion";
        canDrop = false;
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
        var sqrDist = distanceFromPlayers*distanceFromPlayers;
        var minDistance = avatars.Select(a => (a.transform.position - clickWorldPos).sqrMagnitude).OrderBy(d => d).FirstOrDefault();

        if (avatars.Any() && minDistance < sqrDist)
        {
            //Debug.Log("Near player");
            return Name + " skill cannot be used so close to a player!";
        }

        MinionSpawnManager.Instance.CmdSingleSpawn(clickWorldPos, MinionType.Meelee);

        return null;
    }
}
