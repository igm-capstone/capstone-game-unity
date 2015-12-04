using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class LobbyHUD : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        if (NetworkManager.singleton == null)
        {
            StartCoroutine(WaitForNetworkLoad(SetButtons));
        }
        else
        {
            SetButtons();
        }
    }

    IEnumerator WaitForNetworkLoad(System.Action action)
    {
        while (CustomNetworkManager.singleton == null) yield return 0;
        if (action != null)
            action.Invoke();
    }

    void SetButtons()
    {
        ((CustomNetworkManager)NetworkManager.singleton).SetButtons();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
