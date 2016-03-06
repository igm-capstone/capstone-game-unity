using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LobbyHUD : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        var ip = "localhost";
        var autoConnect = false;
        var autoHost = false;
        var args = System.Environment.GetCommandLineArgs();

        foreach (var item in args)
        {
            if (item.StartsWith("--host"))
            {
                var split = item.Split('=');
                if (split.Length == 2)
                {
                    ip = split[1];
                    Debug.Log("host: " + ip);
                }
            }

            if (item.StartsWith("--start-host"))
            {
                autoHost = true;
            }


            if (item.StartsWith("--start-client"))
            {
                autoConnect = true;
            }
        }

        if (NetworkManager.singleton == null)
        {
            StartCoroutine(WaitForNetworkLoad(SetButtons, ip, autoConnect, autoHost));
        }
        else
        {
            SetButtons(ip, autoConnect, autoHost);
        }
    }

    IEnumerator WaitForNetworkLoad(System.Action<string, bool, bool> action, string ip, bool autoConnect, bool autoHost)
    {
        while (CustomNetworkManager.singleton == null) yield return 0;
        if (action != null)
            action.Invoke(ip, autoConnect, autoHost);
    }

    void SetButtons(string ip, bool autoConnect, bool autoHost)
    {
        ((CustomNetworkManager)NetworkManager.singleton).SetButtons(ip, autoConnect, autoHost);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
