using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
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

    [Command]
    public void CmdPropagateRdy(PlyrNum PlyrId)
    {
        var StartScreenBhvr = transform.Find("StartUpWindow").GetComponent<StartUpScrBhvr>();
    }

    [Command]
    public void CmdPropagateRdy(PlyrNum PlyrId)
    {

    }

}
