using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class StartUpScrBhvr : MonoBehaviour {

    public Text ClassName;
    [NonSerialized]
    public StartUpScreenMngr ScrnMngrRef;
    [NonSerialized]
    public PlyrNum MyId;
    [NonSerialized]
    public AvatarNetworkBehavior AvNetBhvr;
    [NonSerialized]
    public GhostNetworkBehavior GhostNetBhvr;

    // Use this for initialization
    void Start()
    {
        ClassName = transform.Find("ClassName").GetComponent<Text>();
	}

    // Ready button was pressed
    void RdyBtnClick()
    {
        // Signal
    }
}
