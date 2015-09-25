using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SpawnManager : NetworkBehaviour {

    public GameObject EnemyPrefab;

	public override void OnStartServer ()
	{
        var spawnPointCollector = GameObject.Find("EnemyCollector").transform;
        for (int i = 0; i < spawnPointCollector.childCount; ++i)
        {
            Transform spawn = spawnPointCollector.GetChild(i);
            GameObject robot = GameObject.Instantiate(EnemyPrefab, spawn.transform.position, Quaternion.identity) as GameObject;
            robot.transform.SetParent(spawn);
            robot.GetComponent<AIController>().enabled = true;
            robot.GetComponent<PatrolWaypoints>().path = spawn.GetComponentInChildren<WaypointPath>();
            NetworkServer.Spawn(robot); 
            
        }
	}
	

}