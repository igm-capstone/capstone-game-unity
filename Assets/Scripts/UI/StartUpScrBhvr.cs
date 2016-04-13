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
    public AvatarNetworkBehavior AvNetBhvr;
    [NonSerialized]
    public GhostNetworkBehavior GhostNetBhvr;
    bool[] RdyArray = new bool[4];

    [NonSerialized]
    public BasePlayerNetworkBehavior BaseNetBhvr;
    // Cha;tein de futuro: Usa base net behavior para pegar ambos behaviors e chamar as funcopes de propagar o rdy. Vai ser um Command  que chama um RPC.

    // Use this for initialization
    void Start()
    {
        ClassName = transform.Find("ClassName").GetComponent<Text>();
        ClassImg = transform.Find("ClassInfoImg").GetComponent<Image>();

        // All enums are an INT.
        ClassImg.sprite = ClassInfoSprt[(int)MyId];
    }

    // Ready button was pressed
    public void RdyBtnClick()
    {
        // Signal
        RdyArray[(int)MyId] = true;
        Debug.Log(MyId + " is ready!");

        // Calls Command/Client RPC
    }
}
