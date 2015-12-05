using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Hallucinate : ISkill
{
    private static GhostController ghost;
    public float Duration = 10;

    protected override bool Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (target.layer != LayerMask.NameToLayer("Player")) return false;

        var z = FindObjectOfType<GridBehavior>().transform.position.z;
        clickWorldPos.z = z;

        MinionSpawnManager.Instance.CmdCoolerSpawn(target, 3.0f, 3, 
            (GameObject[] minions, GameObject player) =>
            {
                IEnumerable<GameObject> players = GameObject.FindGameObjectsWithTag("Player").Where(p => p != player);
                foreach (GameObject p in players)
                {
                    AvatarNetworkBehavior ac = p.GetComponentInParent<AvatarNetworkBehavior>();
                    foreach (GameObject minion in minions)
                    {
                        ac.RpcDisableMinion(minion.name);
                    }
                }
            });

        return true;
    }
}
