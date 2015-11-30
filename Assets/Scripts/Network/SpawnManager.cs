using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class SpawnManager : NetworkBehaviour
{

    public GameObject EnemyPrefab;
    private Transform pathContainer;
    private Transform minionContainer;
    private List<WaypointPath> paths; 

    public static SpawnManager Instance { get; private set; }

    public override void OnStartServer ()
    {
        Instance = this;
        paths = new List<WaypointPath>();

        pathContainer = GameObject.Find("PathCollector").transform;
        minionContainer = GameObject.Find("MinionCollector").transform;
        foreach (Transform child in pathContainer)
        {
            paths.Add(child.GetComponentInChildren<WaypointPath>());
        }
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
    public void CmdSpawn(Vector3 position)
    {
        var minDist = float.MaxValue;
        WaypointPath path = null;
        Vector3 spawnPos = Vector3.zero;
        int waypointIndex = 0;

        foreach (var waypointPath in paths)
        {
            for (int i = 0; i < waypointPath.transform.childCount; i++)
            {
                Transform waypoint = waypointPath.transform.GetChild(i);
                var dist = (waypoint.position - position).sqrMagnitude;
                if (dist < minDist)
                {
                    path = waypointPath;
                    minDist = dist;
                    spawnPos = waypoint.position;
                    waypointIndex = i;
                }
            }
        }


        Transform spawn = minionContainer;
        GameObject robot = Instantiate(EnemyPrefab, position, Quaternion.identity) as GameObject;
        robot.transform.SetParent(spawn);
        robot.GetComponent<MinionController>().enabled = true;
        robot.GetComponent<PatrolWaypoints>().path = path;
        robot.GetComponent<PatrolWaypoints>().nextStop = waypointIndex;
        NetworkServer.Spawn(robot);

        TextureRandomizer rnd = robot.GetComponent<TextureRandomizer>();
        rnd.RandomizeTexture();

        StartCoroutine(SetGridDirty());
    }

    IEnumerator SetGridDirty()
    {
        yield return new WaitForSeconds(.5f);
        FindObjectOfType<GridBehavior>().SetAIDirty();
    }

}