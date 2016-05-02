using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Networking.NetworkSystem;
using System.Linq;

public class CustomNetworkManager : NetworkManager
{
    public GameObject ghostPrefab;
    public GameObject[] avatarPrefab;

    public bool exposeConsole = true;
    public GameObject console;

    private List<GameObject> explorerHealth = new List<GameObject>();

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
        string ip = GameObject.Find("InputFieldIP").GetComponent<InputField>().text;
        networkAddress = ip;
    }

    void SetIPAddress(string ip)
    {
        if (!GameObject.Find("InputFieldIP"))
        {
            return;
        }

        var input = GameObject.Find("InputFieldIP").GetComponent<InputField>();
        input.text = ip;
        networkAddress = ip;
    }


    public void SetButtons(string ip, bool autoConnect, bool autoHost)
    {
        EnableBtn("ButtonHost", "Host Game", TryHost);
        EnableBtn("ButtonJoin", "Join Game", TryJoin);
        EnableBtn("ButtonQuit", "Quit Game", () => { Application.Quit(); });
        EnableBtn("ButtonExit", "Exit", () => { DisableBtn("ButtonExit", "Exiting..."); StopHost(); });

        SetIPAddress(ip);

        if (autoConnect)
        {
            TryJoin();
        }
        else if (autoHost)
        {
            TryHost();
        }
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
            GameObject radarCam = GameObject.Find("RadarCamera");
            radarCam.SetActive(false);
            FindObjectOfType<ObjectiveUI>().transform.FindChild("MainText").GetComponent<Text>().text = "Areas Exorcised";            

            var sp = FindObjectsOfType<AvatarSpawnPoint>().FirstOrDefault(s => s.PlayerID == 0);
            var pos = sp ? sp.transform.position : Vector3.zero;
            pos.z = 0;

            //Debug.Log("Spawning ghost");
            player = (GameObject)Instantiate(ghostPrefab, pos, Quaternion.identity);
            player.GetComponent<GhostNetworkBehavior>().conn = conn;
            explorerHealth.Clear();
        }
        else
        {
            //Debug.Log("Spawning avatar");
            var spawnPoints = FindObjectsOfType<AvatarSpawnPoint>();
            var sp = spawnPoints.FirstOrDefault(s => s.PlayerID == conn.connectionId) ?? spawnPoints.FirstOrDefault(s => s.PlayerID == 1);

            player = (GameObject)Instantiate(avatarPrefab[sp.PlayerID - 1], sp.transform.position, Quaternion.identity);

            player.GetComponent<AvatarNetworkBehavior>().conn = conn;

            
        }

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("Client Disconnected");
        base.OnServerDisconnect(conn);
        GameObject.FindObjectOfType<GhostNetworkBehavior>().RpcDestroyHealthBar();
    }
    IEnumerator RemoveExplorerHealth(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);


        List<GameObject> avatarCtrl = FindObjectsOfType<AvatarController>().Select(a => a.GetComponent<Health>().canvas).ToList();

        List<GameObject> allExplorersHealth = GameObject.FindGameObjectsWithTag("ExplorerHealth").ToList();

        List<GameObject> healthRemaining = new List<GameObject>();

        for (int i = allExplorersHealth.Count - 1; i >= 0; i--)
        {
            if(avatarCtrl.Contains(allExplorersHealth[i]))
            {
                allExplorersHealth.RemoveAt(i);
            }

        }

        foreach (var item in allExplorersHealth)
        {
            Destroy(item);
        }
        
    }
}

