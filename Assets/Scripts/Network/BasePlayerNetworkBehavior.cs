using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BasePlayerNetworkBehavior : NetworkBehaviour
{
    public NetworkConnection conn;
    static GameStateHUD gameStateHud;

    void Start()
    {
        gameStateHud = FindObjectOfType<GameStateHUD>();
    }

    public override void OnStartLocalPlayer()
    {
        GetComponentInChildren<Camera>().enabled = true;
        name = "Me";

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
    public void CmdRestartGame()
    {
        RpcRestartingGame();
        NetworkManager.singleton.ServerChangeScene(Application.loadedLevelName);
    }

    [ClientRpc]
    protected void RpcEndGame(string msg)
    {
        gameStateHud.SetMsg(msg);
    }

    [ClientRpc]
    protected void RpcRestartingGame()
    {
        gameStateHud.SetRestarting();
    }


}
