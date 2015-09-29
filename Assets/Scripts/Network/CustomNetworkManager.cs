using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Networking.NetworkSystem;

public class CustomNetworkManager : NetworkManager
{
    public GameObject prisonerPrefab;
    public GameObject guardPrefab;

    public void Host()
    {
        NetworkManager.singleton.StartHost();
    }

    public void Join()
    {
        GetIPAddress();
        NetworkManager.singleton.StartClient();
    }

    void GetIPAddress()
    {
        string ip = GameObject.Find("InputFieldIP").transform.FindChild("Text").GetComponent<Text>().text;
        NetworkManager.singleton.networkAddress = ip;
    }

    void OnLevelWasLoaded(int level)
    {
        if (level == 0)
        {
            var btnHost = GameObject.Find("ButtonHost").GetComponent<Button>();
            btnHost.onClick.RemoveAllListeners();
            btnHost.onClick.AddListener(Host);

            var btnJoin = GameObject.Find("ButtonJoin").GetComponent<Button>();
            btnJoin.onClick.RemoveAllListeners();
            btnJoin.onClick.AddListener(Join);

            var btnQuit = GameObject.Find("ButtonQuit").GetComponent<Button>();
            btnQuit.onClick.RemoveAllListeners();
            btnQuit.onClick.AddListener(Quit);
        }
        else
        {
            var btnExit = GameObject.Find("ButtonExit").GetComponent<Button>();
            btnExit.onClick.RemoveAllListeners();
            btnExit.onClick.AddListener(NetworkManager.singleton.StopHost);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player;
        if (conn.hostId == -1) //Local client
        {
            //Debug.Log("Spawning prisioner");
            GameObject spawnPoint = GameObject.Find("PrisonerSpawnPoint");
            player = (GameObject)GameObject.Instantiate(prisonerPrefab, spawnPoint.transform.position, Quaternion.identity);
        }
        else
        {
            //Debug.Log("Spawning guard");
            player = (GameObject)GameObject.Instantiate(guardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

}