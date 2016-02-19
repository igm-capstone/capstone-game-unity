using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;

public enum TrapType
{
    Poison,
    Glue,
    Spring
}

public class TrapSpawnManager : NetworkBehaviour 
{
    public GameObject[] TrapsPrefab;
    private Transform TrapContainer;

    public TrapType SlctdTrap;

    public static TrapSpawnManager Instance { get; private set; }

    public void Awake()
    {
        // Get Manager Instance
        Instance = this;
        // Get objt transform to serve as the traps parent.
        TrapContainer = transform;
    }

    [Command]
    public void CmdSetTrap(Vector3 position, TrapType _SlctdTrap)
    {
        GameObject spawnedTrap;

        Debug.Log("About to select trap!");

        spawnedTrap = Instantiate(TrapsPrefab[0], position, Quaternion.identity) as GameObject;
        spawnedTrap.transform.SetParent(TrapContainer);
        NetworkServer.Spawn(spawnedTrap);

        /*
        switch (_SlctdTrap)
        {
            case TrapType.Poison:
                if (TrapsPrefab[0] != null)
                {
                    spawnedTrap = Instantiate(TrapsPrefab[0], position, Quaternion.identity) as GameObject;
                    spawnedTrap.transform.SetParent(TrapContainer);
                    NetworkServer.Spawn(spawnedTrap);
                }
                else
                {
                    Debug.Log("TrapsPrefab[0] == null. Did you forget to assign a prefab?");
                }
                break;

            case TrapType.Glue:
                if (TrapsPrefab[1] != null)
                {
                    spawnedTrap = Instantiate(TrapsPrefab[1], position, Quaternion.identity) as GameObject;
                    spawnedTrap.transform.SetParent(TrapContainer);
                    NetworkServer.Spawn(spawnedTrap);
                }
                else
                {
                    Debug.Log("TrapsPrefab[1] == null. Did you forget to assign a prefab?");
                }
                break;

            case TrapType.Spring:
                if (TrapsPrefab[2] != null)
                {
                    spawnedTrap = Instantiate(TrapsPrefab[2], position, Quaternion.identity) as GameObject;
                    spawnedTrap.transform.SetParent(TrapContainer);
                    NetworkServer.Spawn(spawnedTrap);
                }
                else
                {
                    Debug.Log("TrapsPrefab[2] == null. Did you forget to assign a prefab?");
                }
                break;

            default:
                Debug.Log("Trap Type not found.");
                break;
        }//switch
        */
    } 
}
 