using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;


public class MinionSpawnManager : NetworkBehaviour
{
    public GameObject[] EnemyPrefab;
    public GameObject[] EnemyHauntPrefab;
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

    [Command]
    public void CmdHauntSpawn(Vector3 position, MinionType minType, GameObject explorer)
    {
        GameObject minion;
        switch (minType)
        {
            case MinionType.HauntMelee:
                minion = Instantiate(EnemyHauntPrefab[0], position, Quaternion.identity) as GameObject;                
                break;

            //case MinionType.AOEBomber:
            //    robot = Instantiate(EnemyPrefab[1], position, Quaternion.identity) as GameObject;
            //    break;

            default:
                minion = Instantiate(EnemyPrefab[0], position, Quaternion.identity) as GameObject;
                break;
        }

        
        NetworkServer.Spawn(minion);
        RpcSetParent(minion, explorer);
        DisableMinion(minion);
        StartCoroutine(FinishHauntEtoM(15,minion,explorer));

    }

    [ClientRpc]
    void RpcSetParent(GameObject minion, GameObject explorer)
    {
        minion.transform.SetParent(explorer.transform.FindChild("AvatarRotation").gameObject.transform);
        minion.transform.localRotation = Quaternion.Euler(0,0,-90);

        if (GameObject.Find("Me") == explorer || isServer)
        {
            DisableMinion(minion);
        }
        else
        {
            //disable exlorer model
            explorer.transform.FindChild("AvatarRotation").transform.FindChild("AllAnimsInOne").gameObject.SetActive(false);
            explorer.transform.FindChild("ClassAura").gameObject.SetActive(false);
            explorer.GetComponent<AvatarController>().SetMinionToControl(minion,true);
        }

    }

    public void DisableMinion(GameObject minion)
    {
        foreach (var s in minion.GetComponentsInChildren<Renderer>())
        {
            s.enabled = false;
        }
        foreach (var s in minion.GetComponentsInChildren<Collider2D>())
        {
            s.enabled = false;
        }
        foreach (var s in minion.GetComponentsInChildren<Canvas>())
        {
            s.gameObject.SetActive(false);
        }
    }
    
    public IEnumerator FinishHauntEtoM(float skillTime, GameObject minion, GameObject explorer)
    {
        yield return new WaitForSeconds(skillTime);
        //Destroy minion
        NetworkServer.Destroy(minion);
        //Enable explorer again for client
        RpcEnableExplorer(explorer);

    }

    [ClientRpc]
    public void RpcEnableExplorer(GameObject explorer)
    {
        if (!(GameObject.Find("Me") == explorer || isServer))
        {
            //enable exlorer model
            explorer.transform.FindChild("AvatarRotation").transform.FindChild("AllAnimsInOne").gameObject.SetActive(true);
            explorer.transform.FindChild("ClassAura").gameObject.SetActive(true);
            explorer.GetComponent<AvatarController>().SetMinionToControl(null, false);
        }
    }
}
