using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Networking.NetworkSystem;

public class CustomNetworkManager : NetworkManager
{
    public GameObject ghostPrefab;
    public GameObject avatarPrefab;
    
    [NonSerialized]
    public bool sceneLoaded = false;

    void GetIPAddress()
    {
        string ip = GameObject.Find("InputFieldIP").transform.FindChild("Text").GetComponent<Text>().text;
        networkAddress = ip;
    }

    public void SetButtons()
    {
        var btnGO = GameObject.Find("ButtonHost");
        if (btnGO != null)
        {
            var btn = btnGO.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { StartHost(); });
        }

        btnGO = GameObject.Find("ButtonJoin");
        if (btnGO != null)
        {
            var btn = btnGO.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { GetIPAddress(); StartClient(); });
        }

        btnGO = GameObject.Find("ButtonQuit");
        if (btnGO != null)
        {
            var btn = btnGO.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { Application.Quit(); });
        }

        btnGO = GameObject.Find("ButtonExit");
        if (btnGO != null)
        {
            var btn = btnGO.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { StopHost(); });
        }
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