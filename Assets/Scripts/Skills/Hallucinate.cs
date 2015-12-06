using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class Hallucinate : ISkill
{
    private static GhostController ghost;
    public float Duration = 10;

    protected override bool Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (target.layer != LayerMask.NameToLayer("Player")) return false;

        MinionSpawnManager.Instance.CmdCoolerSpawn(target, 3.0f, 3,
            (GameObject[] minions, GameObject player) =>
            {
                var selfAc = player.GetComponentInParent<AvatarNetworkBehavior>();
                foreach (var ac in FindObjectsOfType<AvatarNetworkBehavior>())
                {
                    if (ac != selfAc)
                        foreach (GameObject minion in minions)
                        {
                            ac.RpcDisableMinion(minion);
                        }
                }

                foreach (GameObject minion in minions)
                {
                    StartCoroutine(DisposeMinion(minion));
                }
            });

        return true;
    }

    IEnumerator DisposeMinion(GameObject minion)
    {
        yield return new WaitForSeconds(Duration);

        if (minion != null)
        {
            NetworkServer.Destroy(minion);
        }
    }
}
