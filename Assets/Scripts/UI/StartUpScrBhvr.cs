using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StartUpScrBhvr : MonoBehaviour {

    public Button RdyBtn;
    public Text ClassName;
    
    // Use this for initialization
	void Start()
    {
        RdyBtn = transform.Find("ReadyButton").GetComponent<Button>();
        ClassName = transform.Find("ClassName").GetComponent<Text>();
	}
}
