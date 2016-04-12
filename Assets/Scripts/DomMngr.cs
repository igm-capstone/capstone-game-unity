using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.Networking;

public class DomMngr : MonoBehaviour
{
    GhostNetworkBehavior GhostPlyer;

    Domination[] DomPntList;
    int CurrentTier = 0;
    int MaxTier;
    bool gameOver = false;

    private BasePlayerNetworkBehavior netBehavior;
    private GameObject objectiveCanvas;
    private bool EnableNextTier = false;
    private int pointsCaptured = 0;

    public static DomMngr Instance { get; private set; }

    //callback signature 
    public delegate void DomPoint(int domID);
    public event System.Action PointDominated;

    // Use this for initialization
    void Awake()
    {
        Instance = this;
        StartCoroutine(DelayedStart());
        objectiveCanvas = transform.FindChild("ObjectiveCanvas").gameObject;
        objectiveCanvas.transform.FindChild("3").gameObject.SetActive(false);
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(2);

        // Get all Dom Pnts
        DomPntList = GetComponentsInChildren<Domination>();

        // Reset Current Tier
        CurrentTier = 0;
        foreach (var DomPnt in DomPntList)
        {
            //Update MaxTier
            if (MaxTier < DomPnt.TierCapture)
                MaxTier = DomPnt.TierCapture;

            // Set all first Tier Dom points to be capturable
            if (DomPnt.TierCapture == 0)
                DomPnt.canBeCaptured = true;
            // Set all other Dom Pnt to not be capturable
            else
                DomPnt.canBeCaptured = false;
        }
    }

    void Update()
    {

        if (gameOver)
        {
            return;
        }

        if (netBehavior == null)
        {
            var me = GameObject.Find("Me");
            if (me)
                netBehavior = me.GetComponent<BasePlayerNetworkBehavior>();
        }

        var players = FindObjectsOfType<AvatarNetworkBehavior>();
        bool AreAllDead = (players.Length > 0);
        foreach (var player in players)
        {
            if (player.GetComponent<Health>().CurrentHealth > 0)
            {
                AreAllDead = false;
                break;
            }
        }

        if (AreAllDead)
        {
            gameOver = true;
            string WinMsg = "Ghost Wins!";
            netBehavior.CmdEndGame(WinMsg);
        }
    }

    // Treats DomPnt wasCaptured Event
    public void DomPnt_WasCaptured(int domID)
    {
        if (gameOver)
        {
            return;
        }

        EnableNextTier = false;

        Debug.Log(domID);

        List<Domination> currentTierDomPoints = DomPntList.Where(a => a.TierCapture == CurrentTier).ToList();
        // Check to see if the current tier domination points were captured
        foreach (var DomPnt in currentTierDomPoints )
        {            
            if (DomPnt.captured == true)
            {
                pointsCaptured++;
                //If all current tier points are captured
                if (pointsCaptured == currentTierDomPoints.Count)
                {
                    EnableNextTier = true;

                    
                    bool enableMainHall = (EnableNextTier && CurrentTier != MaxTier);
                    objectiveCanvas.GetComponent<ObjectiveUI>().CmdUpdateObjectiveUI(domID, enableMainHall);
                }
            }
            else
            {
                EnableNextTier = false;
                pointsCaptured = 0;

                bool enableMainHall = (EnableNextTier && CurrentTier != MaxTier);
                objectiveCanvas.GetComponent<ObjectiveUI>().CmdUpdateObjectiveUI(domID, enableMainHall);

                if (PointDominated != null) PointDominated();
                return;
            }
        }

        // Check to see if the Game is Over
        if (CurrentTier + 1 > MaxTier)
        {

            gameOver = true;
            // Game is over - Avatars Win.
            string WinMsg = "Explorers Win!";

            // display win msg and exit 
            netBehavior.CmdEndGame(WinMsg);

            return;

        }

        // If all were Captured
        if (EnableNextTier)
        {
            // Increase Current Tier
            CurrentTier++;
            foreach (var DomPnt in DomPntList)
                if (DomPnt.TierCapture == CurrentTier)
                    DomPnt.canBeCaptured = true;
        }

        if (PointDominated != null) PointDominated();
    }
}