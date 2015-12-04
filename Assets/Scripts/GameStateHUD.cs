using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameStateHUD : MonoBehaviour {
    
    public GameObject gameStateHUD;
    Text msg;
    Image img;
    Button btn;

	// Use this for initialization
	void Start () {
        if (GameObject.FindObjectOfType<CustomNetworkManager>() == null) Application.LoadLevel("Menu");
        gameStateHUD.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetMsg(string text)
    {
        gameStateHUD.SetActive(true);

        msg = gameStateHUD.GetComponentInChildren<Text>();
        msg.text = text;

        btn = gameStateHUD.GetComponentInChildren<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(RestartGame);
        btn.GetComponentInChildren<Text>().text = "Restart";
    }

    public void SetRestarting()
    {
        btn = gameStateHUD.GetComponentInChildren<Button>();
        btn.onClick.RemoveAllListeners();
        btn.GetComponentInChildren<Text>().text = "Restarting...";
    }

    void RestartGame()
    {
        GameObject.Find("Me").GetComponent<BasePlayerNetworkBehavior>().CmdRestartGame();
    }
}
