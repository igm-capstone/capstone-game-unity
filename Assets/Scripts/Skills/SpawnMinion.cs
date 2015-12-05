using UnityEngine;
using System.Collections;

public class SpawnMinion : ISkill
{
    protected override bool Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (target.layer != LayerMask.NameToLayer("Floor")) return false;

        var z = FindObjectOfType<GridBehavior>().transform.position.z;
        clickWorldPos.z = z;
        MinionSpawnManager.Instance.CmdSpawn(clickWorldPos);

        return true;
    }
}
