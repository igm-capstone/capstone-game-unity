using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AvatarNetworkBehavior : NetworkBehaviour {
    public NetworkConnection conn;

    private Transform blockCollector;
    private MeleeWeaponBehavior meleeBehaviour;
    
    void Start()
    {
        blockCollector = GameObject.Find("BlocksCollector").transform;
        meleeBehaviour = GetComponentInChildren<MeleeWeaponBehavior>();
    }

	public override void OnStartLocalPlayer () {
        GetComponentInChildren<Camera>().enabled = true;
        GetComponentInChildren<AvatarController>().enabled = true;
        GetComponentInChildren<MovementBroadcast>().enabled = true;
        transform.GetChild(0).FindChild("Fog").GetComponent<SpriteRenderer>().color = Color.white;

        foreach (var light in FindObjectsOfType<LightController>())
        {
            light.dirty = true;
        }

        foreach (var grid in FindObjectsOfType<GridBehavior>())
        {
            grid.SetGridDirty();
            grid.SetAIDirty();
        }
	}

    [Command]
    public void CmdEndGame(string msg)
    {
        AvatarController[] players = FindObjectsOfType<AvatarController>();
        foreach (var p in players)
        {
            if (p.disable == false)
            {
                return;
            }
        }
        RpcEndGame(msg);
    }

    [Command]
    public void CmdTakeBlockOver(string block, bool status)
    {
        NetworkIdentity blockNetID = GameObject.Find(block).GetComponent<NetworkIdentity>();

        if (status && blockNetID.transform.parent == blockCollector)
        {
            blockNetID.transform.parent = transform;
            RpcTakeBlockOver(block, status);
        }
        else if (!status)
        {
            blockNetID.transform.parent = blockCollector;
            RpcTakeBlockOver(block, status);
        }
    }

    [ClientRpc]
    void RpcEndGame(string msg)
    {
        GameStateHUD hud = FindObjectOfType<GameStateHUD>();
        hud.SetMsg(msg);
    }

    [ClientRpc]
    void RpcTakeBlockOver(string block, bool status)
    {
        NetworkIdentity blockNetID = GameObject.Find(block).GetComponent<NetworkIdentity>();

        if (status && blockNetID.transform.parent == blockCollector)
        {
            blockNetID.transform.parent = transform;
        }
        else if (!status)
        {
            blockNetID.transform.parent = blockCollector;

        }
    }

    [Command]
    public void CmdEnableSlash(bool status)
    {
        meleeBehaviour.EnableSlash(status);
        RpcEnableSlash(status);
    }

    [ClientRpc]
    public void RpcEnableSlash(bool status)
    {
        meleeBehaviour.EnableSlash(status);
    }
}
