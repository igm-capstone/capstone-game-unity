using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AvatarNetworkBehavior : NetworkBehaviour {
    public NetworkConnection conn;

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

        if (status)
        {
            blockNetID.RemoveClientAuthority(FindObjectOfType<GhostNetworkBehavior>().conn);
            blockNetID.AssignClientAuthority(conn);
        }
        else
        {
            blockNetID.RemoveClientAuthority(conn);
            blockNetID.AssignClientAuthority(FindObjectOfType<GhostNetworkBehavior>().conn);
        }
    }

    [ClientRpc]
    void RpcEndGame(string msg)
    {
        GameStateHUD hud = FindObjectOfType<GameStateHUD>();
        hud.SetMsg(msg);
    }
}
