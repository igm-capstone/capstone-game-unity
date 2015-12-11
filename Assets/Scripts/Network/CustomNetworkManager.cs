using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Networking.NetworkSystem;

public class CustomNetworkManager : NetworkManager
{
    public GameObject ghostPrefab;
    public GameObject avatarPrefab;

    public bool exposeConsole = true;
    public GameObject console;

    [NonSerialized]
    public bool sceneLoaded = false;

    void Start()
    {
        if (exposeConsole && console)
        {
            var c = Instantiate(console);
            c.transform.SetParent(this.transform);
        }
    }

    public void EnableBtn(string buttonName, string msg, UnityAction action)
    {
        var btnGO = GameObject.Find(buttonName);
        if (btnGO == null) return;

        var btn = btnGO.GetComponent<Button>();
        var text = btnGO.GetComponentInChildren<Text>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(action);
        text.text = msg;
    }

    public void DisableBtn(string buttonName, string msg)
    {
        var btnGO = GameObject.Find(buttonName);
        if (btnGO == null) return;

        var btn = btnGO.GetComponent<Button>();
        var text = btnGO.GetComponentInChildren<Text>();
        btn.onClick.RemoveAllListeners();
        text.text = msg;
    }

    void GetIPAddress()
    {
        string ip = GameObject.Find("InputFieldIP").transform.FindChild("Text").GetComponent<Text>().text;
        networkAddress = ip;
    }

    public void SetButtons()
    {
        EnableBtn("ButtonHost", "Host Game", TryHost);
        EnableBtn("ButtonJoin", "Join Game", TryJoin);
        EnableBtn("ButtonQuit", "Quit Game", () => { Application.Quit(); });
        EnableBtn("ButtonExit", "Exit", () => { DisableBtn("ButtonExit", "Exiting..."); StopHost(); });
    }

    public void TryHost()
    {
        DisableBtn("ButtonHost", "Hosting...");
        if (StartHost() == null) EnableBtn("ButtonHost", "Error - Host Again?", TryHost);
    }

    public void TryJoin()
    {
        DisableBtn("ButtonJoin", "Joining...");
        GetIPAddress();
        StartClient();
    }

    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        EnableBtn("ButtonJoin", "Error - Join Again?", TryJoin);
        base.OnClientError(conn, errorCode);
    }

    public override void ServerChangeScene(string sceneName)
    {
        sceneLoaded = false;
        base.ServerChangeScene(sceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        sceneLoaded = true;
        base.OnServerSceneChanged(sceneName);
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        if (!sceneLoaded)
        {
            StartCoroutine(WaitForLevelLoad(delegate { AddPlayer(conn, playerControllerId); }));
        }
        else
        {
            AddPlayer(conn, playerControllerId);
        }
    }

    IEnumerator WaitForLevelLoad(System.Action action)
    {
        while (!sceneLoaded) yield return 0;
        if (action != null)
            action.Invoke();
    }

    private void AddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = null;
        if (conn.hostId == -1) //Local client
        {
            //Debug.Log("Spawning ghost");
            player = (GameObject)GameObject.Instantiate(ghostPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            player.GetComponent<GhostNetworkBehavior>().conn = conn;
        }
        else
        {
            //Debug.Log("Spawning avatar");
            AvatarSpawnPoint[] spawnPoints = FindObjectsOfType<AvatarSpawnPoint>();
            foreach (AvatarSpawnPoint sp in spawnPoints)
            {
                if (sp.PlayerID == conn.connectionId)
                {
                    player = (GameObject)Instantiate(avatarPrefab, sp.transform.position, Quaternion.identity);
                    break;
                }
            }
            if (player == null) // didnt match to a spawn point
            {
                player =
                    (GameObject)Instantiate(avatarPrefab, spawnPoints[0].transform.position, Quaternion.identity);
            }
            player.GetComponent<AvatarNetworkBehavior>().conn = conn;
        }

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

}