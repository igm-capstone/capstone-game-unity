using UnityEngine;
using System.Collections;

public enum PlyrNum
{
    Ghost,
    Sprinter,
    TrapMaster,
    Support
}

public class StartUpScreenMngr : MonoBehaviour
{
    int MyPlayerInt;

    GameObject MainCanvasRef;
    GameObject PlayerObjRef;

    [SerializeField]
    GameObject StrtWndwPreFab;

    PlyrNum MyPlyrId;

    // Use this for initialization
    void Start()
    {
        PlayerObjRef = this.gameObject;

        // Disable this script for all player classes that are not "Me" and exits.
        if (PlayerObjRef.name != "Me")
        {
            this.enabled = false;
            return;
        }

        // Gets a reference for the main canvas
        MainCanvasRef = GameObject.Find("MainCanvas");

        // Find out what class the player is playing.
        if (PlayerObjRef.GetComponent<GhostController>())
        {
            // Disable movement
            PlayerObjRef.GetComponent<GhostController>().enabled = false;
            MyPlyrId = PlyrNum.Ghost;
        }
        else
        {
            // Disable movement
            PlayerObjRef.GetComponent<AvatarController>().enabled = false;
            if (PlayerObjRef.GetComponent<Sprint>())
            {
                MyPlyrId = PlyrNum.Sprinter;
            }
            else if (PlayerObjRef.GetComponent<Heal>())
            {
                MyPlyrId = PlyrNum.Support;
            }
            else if (PlayerObjRef.GetComponent<SetTrapPoison>())
            {
                MyPlyrId = PlyrNum.TrapMaster;
            }
        }

        MyPlayerInt = (int)MyPlyrId;

        // Instance the Start Up window.

        GameObject window = (GameObject)Instantiate(StrtWndwPreFab,Vector3.zero,Quaternion.identity);

        var windowScr = window.GetComponent<StartUpScrBhvr>();
        windowScr.ScrnMngrRef = this;
        windowScr.MyId = MyPlyrId;

        // Set window parent and center it.
        window.transform.SetParent(MainCanvasRef.transform);
        window.transform.localPosition = Vector3.zero;
        window.transform.localScale= Vector3.one;

        switch (MyPlyrId)
        {
            case PlyrNum.Ghost:
                window.GetComponent<StartUpScrBhvr>().ClassName.text = "Ghost";
                windowScr.GhostNetBhvr = gameObject.GetComponent<GhostNetworkBehavior>();
                break;

            case PlyrNum.Sprinter:
                window.GetComponent<StartUpScrBhvr>().ClassName.text = "Sprinter";
                windowScr.AvNetBhvr = gameObject.GetComponent<AvatarNetworkBehavior>();
                break;

            case PlyrNum.TrapMaster:
                window.GetComponent<StartUpScrBhvr>().ClassName.text = "TrapMaster";
                windowScr.AvNetBhvr = gameObject.GetComponent<AvatarNetworkBehavior>();
                break;

            case PlyrNum.Support:
                window.GetComponent<StartUpScrBhvr>().ClassName.text = "Support";
                windowScr.AvNetBhvr = gameObject.GetComponent<AvatarNetworkBehavior>();
                break;

            default:
                break;
        }



    }

    // TEST CODE.
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameStart();
        }
    }
    // END OF TEST CODE

    // ReadyBtnfunction

    // StartTheGame function
    public void GameStart()
    {
        // DeletesStart up window instance

        // Enable movement
        switch (MyPlyrId)
        {
            case PlyrNum.Ghost:
                PlayerObjRef.GetComponent<GhostController>().enabled = true;
                break;

            default:
                PlayerObjRef.GetComponent<AvatarController>().enabled = true;
                break;
        }
    }
}
