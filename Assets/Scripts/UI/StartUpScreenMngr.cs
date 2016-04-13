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

    GameObject windowObj;

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

        windowObj = (GameObject)Instantiate(StrtWndwPreFab,Vector3.zero,Quaternion.identity);

        var windowScrpt = windowObj.GetComponent<StartUpScrBhvr>();
        windowScrpt.ScrnMngrRef = this;
        windowScrpt.MyId = MyPlyrId;

        // Set window parent and center it.
        windowObj.transform.SetParent(MainCanvasRef.transform);
        windowObj.transform.localPosition = Vector3.zero;
        windowObj.transform.localScale= Vector3.one;

        switch (MyPlyrId)
        {
            case PlyrNum.Ghost:
                windowObj.GetComponent<StartUpScrBhvr>().ClassName.text = "Ghost";
                windowScrpt.GhostNetBhvr = gameObject.GetComponent<GhostNetworkBehavior>();
                break;

            case PlyrNum.Sprinter:
                windowObj.GetComponent<StartUpScrBhvr>().ClassName.text = "Sprinter";
                windowScrpt.AvNetBhvr = gameObject.GetComponent<AvatarNetworkBehavior>();
                break;

            case PlyrNum.TrapMaster:
                windowObj.GetComponent<StartUpScrBhvr>().ClassName.text = "TrapMaster";
                windowScrpt.AvNetBhvr = gameObject.GetComponent<AvatarNetworkBehavior>();
                break;

            case PlyrNum.Support:
                windowObj.GetComponent<StartUpScrBhvr>().ClassName.text = "Support";
                windowScrpt.AvNetBhvr = gameObject.GetComponent<AvatarNetworkBehavior>();
                break;

            default:
                Debug.Log("Player ID not found!");
                break;
        }
    }

    // ReadyBtnfunction

    // StartTheGame function
    public void GameStart()
    {
        // DeletesStart up window instance
        Destroy(windowObj);

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
