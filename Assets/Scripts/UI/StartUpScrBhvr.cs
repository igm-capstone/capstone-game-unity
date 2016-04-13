using UnityEngine;
using UnityEngine.UI;
using System;
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
    // Chaltein do futuro: Usa base net behavior para pegar ambos behaviors e chamar as funcopes de propagar o rdy. Vai ser um Command  que chama um RPC.

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

    // Ready button was pressed
    public void RdyBtnClick()
    {
        // Propagates the Ready button click to the server.
        BaseNetBhvr.CmdPropagateRdy(MyId);
    }
}
