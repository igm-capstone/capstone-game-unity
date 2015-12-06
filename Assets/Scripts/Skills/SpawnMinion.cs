using UnityEngine;
using System.Linq;

public class SpawnMinion : ISkill
{
    public float distanceFromPlayers = 5f;

    protected override bool Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (target.layer != LayerMask.NameToLayer("Floor")) return false;

        var grid = GridBehavior.Instance;
        var z = grid.transform.position.z;
        clickWorldPos.z = z;

        var node = grid.getNodeAtPos(clickWorldPos);
        if (!node.canWalk || node.hasLight)
        {
            Debug.LogFormat("Lit area {0}, unwalkable area {0}", node.hasLight, !node.canWalk);
            return false;
        }

        var avatars = FindObjectsOfType<AvatarController>();
        var sqrDist = distanceFromPlayers*distanceFromPlayers;
        var minDistance = avatars.Select(a => (a.transform.position - clickWorldPos).sqrMagnitude).OrderBy(d => d).FirstOrDefault();

        if (avatars.Any() && minDistance < sqrDist)
        {
            Debug.Log("Near player");
            return false;
        }

        MinionSpawnManager.Instance.CmdSpawn(clickWorldPos);

        return true;
    }
}
