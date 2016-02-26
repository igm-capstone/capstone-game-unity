using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;

public enum LanternType
{
    Regular,
    Red,
    Blue,
    Orange
}

public class LanternSpawnManager : NetworkBehaviour 
{
    public GameObject[] LanternsPrefab;
    private Transform LanternContainer;

    public static LanternSpawnManager Instance { get; private set; }

    public void Awake()
    {
        // Get Manager Instance
        Instance = this;
        // Get objt transform to serve as the traps parent.
        LanternContainer = transform;
    }

    [Command]
    public void CmdSetLantern(Vector3 position, LanternType _SlctdLantern)
    {
        GameObject spawnedLantern;

        switch (_SlctdLantern)
        {
            case LanternType.Regular:

                if (LanternsPrefab[0] != null)
                {
                    spawnedLantern = Instantiate(LanternsPrefab[0], position, Quaternion.identity) as GameObject;
                    spawnedLantern.transform.SetParent(LanternContainer);

                    if (isServer)
                    {
                        GridBehavior.Instance.SetAIDirty();
                        GridBehavior.Instance.SetGridDirty();
                    }

                    NetworkServer.Spawn(spawnedLantern);
                }
                else
                {
                    Debug.Log("LanternsPrefab[0] == null. Did you forget to assign a prefab?");
                }
                break;

            case LanternType.Red:

                if (LanternsPrefab[1] != null)
                {
                    /*
                    spawnedLantern = Instantiate(LanternsPrefab[0], position, Quaternion.identity) as GameObject;
                    spawnedLantern.transform.SetParent(LanternContainer);
                    NetworkServer.Spawn(spawnedLantern);
                    */
                }
                else
                {
                    Debug.Log("LanternsPrefab[1] == null. Did you forget to assign a prefab?");
                }
                break;

            case LanternType.Blue:

                if (LanternsPrefab[2] != null)
                {
                    /*
                    spawnedLantern = Instantiate(LanternsPrefab[0], position, Quaternion.identity) as GameObject;
                    spawnedLantern.transform.SetParent(LanternContainer);
                    NetworkServer.Spawn(spawnedLantern);
                    */
                }
                else
                {
                    Debug.Log("LanternsPrefab[] == null. Did you forget to assign a prefab?");
                }
                break;

            case LanternType.Orange:
                Debug.Log("Orange Lantern not implemented yet");
                if (LanternsPrefab[3] != null)
                {
                    /*
                    spawnedTrap = Instantiate(TrapsPrefab[2], position, Quaternion.identity) as GameObject;
                    spawnedTrap.transform.SetParent(TrapContainer);
                    NetworkServer.Spawn(spawnedTrap);
                    */
                }
                else
                {
                    Debug.Log("LanternsPrefab[3] == null. Did you forget to assign a prefab?");
                }
                break;

            default:
                Debug.Log("Lantern Type not found.");
                break;
        }
    }

}
 