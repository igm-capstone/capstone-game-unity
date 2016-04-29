using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using Wenzil.Console;

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
    }

    [ConsoleCommand("use")]
    public static void UseCommand(params string[] args)
    {
        var me = GameObject.Find("Me");
        ISkill skill = null;
        switch (args[0])
        {
            case "minion":
                skill = me.GetComponent<SpawnMinion>();
                break;
        }

        skill.Use();
    }

    [Command]
    public void CmdEndGame(string msg)
    {
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

    // Sets PlayerId player as ready on the Start Up screen. Propagates to all clients
    [Command]
    public void CmdPropagateRdy(PlyrNum _PlyrId)
    {
        var StartScreenBhvr = GameObject.Find("StartUpWindow(Clone)").GetComponent<StartUpScrBhvr>();

        // Sets PlyerId as ready on server.
        StartScreenBhvr.RdyArray[(int)_PlyrId] = true;
        StartScreenBhvr.RdyCheckMark[(int)_PlyrId].gameObject.SetActive(true);

        // Propagates to clients
        RpcPropagateRdy(_PlyrId, StartScreenBhvr.RdyArray);
    }

    // Sets PlayerId player as ready on the Start Up screen. Propagates to all clients
    [ClientRpc]
    public void RpcPropagateRdy(PlyrNum _PlyrId, bool[] _RdyArray)
    {
        var StartScreenBhvr = GameObject.Find("StartUpWindow(Clone)").GetComponent<StartUpScrBhvr>();       

        // Get Rdy state array from server.
        StartScreenBhvr.RdyArray = _RdyArray;

        int i = 0;
        // Propagates all Rdy states to all clients
        foreach (var RdyState in StartScreenBhvr.RdyArray)
        {
            if (RdyState)
            {
                StartScreenBhvr.RdyCheckMark[i].gameObject.SetActive(true);
            }
            i++;
        }

        // If all players are ready start the game.
        if (StartScreenBhvr.RdyArray.All(b => b))
        {
            GameObject.Find("Me").GetComponent<StartUpScreenMngr>().GameStart();
        }
    }

}
