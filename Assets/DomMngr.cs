using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class DomMngr : MonoBehaviour
{
    Domination[] DomPntList;

        // Use this for initialization
	void Awake()
    {
        DomPntList = GetComponentsInChildren<Domination>();

        foreach (var DomPnt in DomPntList)
        {
            DomPnt.WasCaptured += DomPnt_WasCaptured;
            if (DomPnt.DomPntTier == 0)
            {
                DomPnt.canBeDominated = true;
            }
            else
            {
                DomPnt.canBeDominated = false;
            }
        }

	}

    private void DomPnt_WasCaptured(Domination CapturedDomPnt)
    {
        bool EnableNextTier = false;
        // Check to see if all lower tier domination points were captured
        foreach (var DomPnt in DomPntList)
        {
            if ((DomPnt.DomPntTier <= CapturedDomPnt.DomPntTier) && DomPnt.captured == false)
            {
                // If one Domination Point is lower/same tier and is not captured, exit from event
                EnableNextTier = false;
                return;
            }
            else
            {
                EnableNextTier = true;
            }
        }
        // If all were Captured
        if (EnableNextTier)
        {
            foreach (var DomPnt in DomPntList)
            {
                if (DomPnt.DomPntTier == CapturedDomPnt.DomPntTier + 1)
                {
                    DomPnt.canBeDominated = true;
                }
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
	
	}
}
