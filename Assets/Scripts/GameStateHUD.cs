using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameStateHUD : MonoBehaviour {

    Text msg;
    Image img;

	// Use this for initialization
	void Start () {
        img = GetComponent<Image>();
        msg = gameObject.GetComponentInChildren<Text>();

        img.enabled = false;
        msg.enabled = false;
        msg.text = null;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetMsg(string text)
    {
        img.enabled = true;
        msg.enabled = true;
        msg.text = text;
    }
}
