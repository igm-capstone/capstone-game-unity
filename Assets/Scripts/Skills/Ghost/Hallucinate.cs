using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class Hallucinate : ISkill
{
    private static GhostController ghost;
    public float Duration = 10;
    public float Radius = 10.0f;
    public int Minions = 3;

    SkillBar mySkillBar;

    public void Awake()
    {
        Name = "Fake Imps";
        canDrop = false;
        cost = 10;

        key = KeyCode.Alpha5;

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
        if (target.tag != "Player") return Name + " skill needs to target an explorer.";

        MinionSpawnManager.Instance.CmdHallucinateSpawn(target, Radius, Minions,
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

        return null;
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
