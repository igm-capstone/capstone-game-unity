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

        if (MyPlyrId == PlyrNum.Ghost)
        {
            windowObj.GetComponent<StartUpScrBhvr>().ClassName.text = "Ghost";
            windowScrpt.BaseNetBhvr = gameObject.GetComponent<GhostNetworkBehavior>();
        }
        else 
        {
            windowObj.GetComponent<StartUpScrBhvr>().ClassName.text = "Explorer";
            windowScrpt.BaseNetBhvr = gameObject.GetComponent<AvatarNetworkBehavior>();
        }
    }

    // ReadyBtnfunction

    // StartTheGame function
    public void GameStart()
    {

        if (windowObj)
        {
            // DeletesStart up window instance
            Destroy(windowObj);
        }

        // Enable movement
        switch (MyPlyrId)
        {
            case PlyrNum.Ghost:
                if (PlayerObjRef.GetComponent<GhostController>().enabled == false)
                {
                    PlayerObjRef.GetComponent<GhostController>().enabled = true;
                }
                break;

            default:
                if (PlayerObjRef.GetComponent<AvatarController>().enabled == false)
                {
                    PlayerObjRef.GetComponent<AvatarController>().enabled = true;
                }
                break;
        }
    }
}
