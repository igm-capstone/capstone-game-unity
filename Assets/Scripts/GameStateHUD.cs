using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameStateHUD : MonoBehaviour {

    Text msg;
    Image img;
    Button btn;

	// Use this for initialization
	void Start () {
        img = GetComponent<Image>();
        msg = gameObject.GetComponentInChildren<Text>();
        btn = gameObject.GetComponentInChildren<Button>();

        img.enabled = false;
        msg.enabled = false;
        btn.gameObject.SetActive(false);
        msg.text = null;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetMsg(string text)
    {
        img.enabled = true;
        msg.enabled = true;
        btn.gameObject.SetActive(true);

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(RestartGame);
        msg.text = text;
    }

    void RestartGame()
    {
        CustomNetworkManager.singleton.ServerChangeScene(Application.loadedLevelName);
    }
}
