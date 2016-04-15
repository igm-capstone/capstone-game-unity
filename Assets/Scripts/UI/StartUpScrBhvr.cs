using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections;

public class StartUpScrBhvr : MonoBehaviour
{
    public Text ClassName;
    public Sprite[] ClassInfoSprt;
    Image ClassImg;

    [NonSerialized]
    public StartUpScreenMngr ScrnMngrRef;
    [NonSerialized]
    public PlyrNum MyId;
    [NonSerialized]
    public Image[] RdyCheckMark = new Image[4];
    [NonSerialized]
    public bool[] RdyArray = new bool[4];

    [NonSerialized]
    public BasePlayerNetworkBehavior BaseNetBhvr;

    public bool DebugEnable;

    // Use this for initialization
    void Start()
    {
        ClassName = transform.Find("ClassName").GetComponent<Text>();
        ClassImg = transform.Find("ClassInfoImg").GetComponent<Image>();

        // All enums are an INT.
        ClassImg.sprite = ClassInfoSprt[(int)MyId];

        int i = 0;
        // Get all checkmarks and disable them.
        foreach (var chkMark in RdyCheckMark)
        {
            RdyCheckMark[i] = transform.Find("RdyBox-" + (i.ToString()+"/Checkmark")).GetComponent<Image>();
            RdyCheckMark[i].gameObject.SetActive(false);
            i++;
        }
    }

    // Debug code for easier testing.
    // Set everyone as ready and signal to start the game.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && DebugEnable)
        {
            RdyArray.All(b => { b = true; return true; });
            DebugStart();
        }
    }

    // Ready button was pressed
    public void RdyBtnClick()
    {
        // Propagates the Ready button click to the server.
        BaseNetBhvr.CmdPropagateRdy(MyId);
    }

    // DebugCode: Send all PlyrID Rdy states to server. Used to start the game with varying amounts of players.
    void DebugStart()
    {
        // Propagates the Ready button click to the server.
        BaseNetBhvr.CmdPropagateRdy(PlyrNum.Ghost);
        BaseNetBhvr.CmdPropagateRdy(PlyrNum.Sprinter);
        BaseNetBhvr.CmdPropagateRdy(PlyrNum.Support);
        BaseNetBhvr.CmdPropagateRdy(PlyrNum.TrapMaster);

        // Calls game start to fix players joining at a later date.
        BaseNetBhvr.gameObject.GetComponent<StartUpScreenMngr>().GameStart();
    }
}
