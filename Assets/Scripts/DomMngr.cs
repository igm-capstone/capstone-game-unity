using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class DomMngr : MonoBehaviour
{
    GhostNetworkBehavior GhostPlyer;

    Domination[] DomPntList;
    int CurrentTier = 0;
    int MaxTier;


    // Use this for initialization
    void Awake()
    {
        // Get all Dom Pnts
        DomPntList = GetComponentsInChildren<Domination>();

        // Reset Current Tier
        CurrentTier = 0;
        foreach (var DomPnt in DomPntList)
        {
            // Start listening to DmtPnt WasCaptured Event
            DomPnt.WasCaptured += DomPnt_WasCaptured;

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

    // Treats DomPnt wasCaptured Event
    private void DomPnt_WasCaptured()
    {
        bool EnableNextTier = false;

        // Check to see if all lower tier domination points were captured
        foreach (var DomPnt in DomPntList)
        {
            if ((DomPnt.TierCapture <= CurrentTier) && DomPnt.captured == false)
            {
                // If one Domination Point is lower/same tier as CurrentTier and is not captured, exit from event
                EnableNextTier = false;
                return;
            }
            else
            {
                EnableNextTier = true;
            }
        }
 
        // Check to see if the Game is Over
        if (CurrentTier+1 > MaxTier)
        {
            // Game is over - Avatars Win.
            string WinMsg = "Avatars Win!";

            // Gets reference to the Ghost Player, the Host
            GhostPlyer = FindObjectOfType<GhostNetworkBehavior>();
            // display win msg and exit 
            GhostPlyer.CmdEndGame(WinMsg);

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
    }
}
