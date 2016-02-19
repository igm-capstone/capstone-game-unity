using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;


public class MinionSpawnManager : NetworkBehaviour
{
    public GameObject[] EnemyPrefab;
    private Transform minionContainer;

    public static MinionSpawnManager Instance { get; private set; }

    public override void OnStartServer ()
    {
        Instance = this;

        minionContainer = GameObject.Find("MinionCollector").transform;
        //for (int i = 0; i < spawnPointCollector.childCount; ++i)
        //{
        //    Transform spawn = spawnPointCollector.GetChild(i);
        //    GameObject robot = GameObject.Instantiate(EnemyPrefab, spawn.transform.position, Quaternion.identity) as GameObject;
        //    robot.transform.SetParent(spawn);
        //    robot.GetComponent<MinionController>().enabled = true;
        //    robot.GetComponent<PatrolWaypoints>().path = spawn.GetComponentInChildren<WaypointPath>();
        //    NetworkServer.Spawn(robot); 
        //}
    }

    [Command]
    public void CmdSingleSpawn(Vector3 position, MinionType minType)
    {
        Transform spawn = minionContainer;
        GameObject robot;
        switch (minType)
        {
            case MinionType.Meelee:
                robot = Instantiate(EnemyPrefab[0], position, Quaternion.identity) as GameObject;
                break;

            case MinionType.AOEBomber:
                robot = Instantiate(EnemyPrefab[1], position, Quaternion.identity) as GameObject;
                break;

            default:
                robot = Instantiate(EnemyPrefab[0], position, Quaternion.identity) as GameObject;
                break;
        }

        robot.transform.SetParent(spawn);
        robot.GetComponent<MinionController>().enabled = true;
        NetworkServer.Spawn(robot);

        TextureRandomizer rnd = robot.GetComponent<TextureRandomizer>();
        rnd.RandomizeTexture();

        StartCoroutine(SetGridDirty());
    }

    [Command]
    public void CmdMultipleSpawn(GameObject target, float radius, int numMinions, Action<GameObject[], GameObject> completion)
    {
        List<Node> nodes = GridBehavior.Instance.getNodesNearPos(target.transform.position, radius, node => !node.hasLight && node.canWalk) as List<Node>;
        nodes.Sort((a, b) => UnityEngine.Random.Range(-1, 2));

        Vector3[] positions = new Vector3[numMinions];
        for (int n = 0; n < numMinions; n++)
        {
            positions[n] = nodes[n].position;
        }

        GameObject[] minions = new GameObject[numMinions];
        for (int n = 0; n < numMinions; n++)
        {
            var avatar = target.GetComponentInParent<AvatarController>();
            
            Transform spawn = minionContainer;
            minions[n] = Instantiate(EnemyPrefab[0], positions[n], Quaternion.identity) as GameObject;
            minions[n].transform.SetParent(spawn);
            minions[n].GetComponent<MinionController>().enabled = true;
            minions[n].GetComponent<MinionController>().SetVisibility(avatar.gameObject);
            NetworkServer.Spawn(minions[n]);

            TextureRandomizer rnd = minions[n].GetComponent<TextureRandomizer>();
            rnd.RandomizeTexture();
        }

        completion(minions, target);

        StartCoroutine(SetGridDirty());
    }

    IEnumerator SetGridDirty()
    {
        yield return new WaitForSeconds(.5f);
        GridBehavior.Instance.SetAIDirty();
    }
}