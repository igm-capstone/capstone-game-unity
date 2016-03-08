using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameStateHUD : MonoBehaviour {
    
    public GameObject gameStateHUD;
    Text msg;
    Image img;
    Button btn;

    private bool isOver = false;

	// Use this for initialization
	void Start () {
	    /*if (GameObject.FindObjectOfType<CustomNetworkManager>() == null)
	    {
	        foreach (var obj in FindObjectsOfType<Component>())
	        {
	            Destroy(obj.gameObject);
	        }
            Application.LoadLevel("Menu");
	    }*/
        gameStateHUD.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetMsg(string text)
    {
        if (isOver) return;

        isOver = true;
        gameStateHUD.SetActive(true);

        msg = gameStateHUD.GetComponentInChildren<Text>();
        msg.text = text;

        btn = gameStateHUD.GetComponentInChildren<Button>();
        if (GameObject.Find("Me").GetComponent<GhostController>())
        {
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(RestartGame);
            btn.GetComponentInChildren<Text>().text = "Restart";
            btn.gameObject.SetActive(true);
        }
        else
        {
            btn.enabled = false;
            btn.interactable = false;
            btn.gameObject.SetActive(false);
        }

    }

    public void SetRestarting()
    {
        btn = gameStateHUD.GetComponentInChildren<Button>();
        if(btn && btn.gameObject.activeSelf == true)
        {
            btn.onClick.RemoveAllListeners();
            btn.GetComponentInChildren<Text>().text = "Restarting...";
        }
    }

    void RestartGame()
    {
        GameObject.Find("Me").GetComponent<BasePlayerNetworkBehavior>().CmdRestartGame();
    }
}
