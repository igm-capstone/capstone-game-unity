using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Networking.NetworkSystem;

public class CustomNetworkManager : NetworkManager
{
    public GameObject ghostPrefab;
    public GameObject avatarPrefab;

    public void Host()
    {
        StartHost();
    }

    public void Join()
    {
        GetIPAddress();
        StartClient();
    }

    void GetIPAddress()
    {
        string ip = GameObject.Find("InputFieldIP").transform.FindChild("Text").GetComponent<Text>().text;
        networkAddress = ip;
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
            btnExit.onClick.AddListener(StopHost);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
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
                if (sp.PlayerID == numPlayers)
                {
                    player = (GameObject)Instantiate(avatarPrefab, sp.transform.position, Quaternion.identity);
                    break;
                }
            }
            if (player == null) // didnt match to a spawn point
            {
                player = (GameObject)Instantiate(avatarPrefab, spawnPoints[0].transform.position, Quaternion.identity);
            }
            player.GetComponent<AvatarNetworkBehavior>().conn = conn;
        }

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

}