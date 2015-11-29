using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AvatarNetworkBehavior : NetworkBehaviour {
    public NetworkConnection conn;

    private Transform blockCollector;
    
    void Start()
    {
        blockCollector = GameObject.Find("BlocksCollector").transform;
    }

	public override void OnStartLocalPlayer () {
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
        RpcEndGame(msg);
    }

    [Command]
    public void CmdTakeBlockOver(string block, bool status)
    {
        NetworkIdentity blockNetID = GameObject.Find(block).GetComponent<NetworkIdentity>();

        if (status && blockNetID.transform.parent == blockCollector)
        {
            blockNetID.transform.parent = transform;
            RpcTakeBlockOver(block, status, this.netId);
        }
        else if (!status)
        {
            blockNetID.transform.parent = blockCollector;
            RpcTakeBlockOver(block, status, this.netId);
        }
    }

    [ClientRpc]
    void RpcEndGame(string msg)
    {
        GameStateHUD hud = FindObjectOfType<GameStateHUD>();
        hud.SetMsg(msg);
    }

    [ClientRpc]
    void RpcTakeBlockOver(string block, bool status, NetworkInstanceId playerID)
    {
        NetworkIdentity blockNetID = GameObject.Find(block).GetComponent<NetworkIdentity>();

        AvatarNetworkBehavior[] players = FindObjectsOfType<AvatarNetworkBehavior>();

        foreach (var player in players)
        {
            if (player.netId == playerID)
            {
                if (status && blockNetID.transform.parent == blockCollector)
                {
                    blockNetID.transform.parent = player.transform;
                }
                else if (!status)
                {
                    blockNetID.transform.parent = blockCollector;

                }
            }
        }
    }
}
