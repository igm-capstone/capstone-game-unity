﻿using UnityEngine;
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
            //Debug.Log("Spawning ghost");
            player = (GameObject)GameObject.Instantiate(ghostPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            player.GetComponent<GhostNetworkBehavior>().conn = conn;
        }
        else
        {
            //Debug.Log("Spawning avatar");
            GameObject spawnPoint = GameObject.Find("AvatarSpawnPoint");
            player = (GameObject)GameObject.Instantiate(avatarPrefab, spawnPoint.transform.position, Quaternion.identity);
            player.GetComponent<AvatarNetworkBehavior>().conn = conn;
        }

        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
    }

}