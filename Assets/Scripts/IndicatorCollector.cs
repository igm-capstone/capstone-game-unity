using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class IndicatorCollector : MonoBehaviour {

    public GameObject ExplorerIndicatorPrefab;
    public GameObject DomPointIndicatorPrefab;

    public void DomPointIndicators()
    {
        GameObject radarCam = GameObject.Find("RadarCamera");
        if (IsSelfClient())
        {
            List<Domination> tier0DomPoints = FindObjectsOfType<Domination>().Where(a => a.GetComponent<Domination>().TierCapture == 0).ToList();
            foreach (Domination dom in tier0DomPoints)
            {
                GameObject indicator = GameObject.Instantiate(DomPointIndicatorPrefab);
                indicator.GetComponent<Canvas>().worldCamera = radarCam.GetComponent<Camera>();
                IndicatorBehavior ib = indicator.GetComponent<IndicatorBehavior>();
                ib.domPoint = dom.gameObject;
                indicator.transform.FindChild("UIImageGrey").GetComponent<Image>().sprite = dom.indicator;
                indicator.transform.FindChild("UIImageColor").GetComponent<Image>().sprite = dom.indicator;
            }
        }
    }

    public void ChangeIndicators()
    {
        GameObject radarCam = GameObject.Find("RadarCamera");
        Debug.Log("Update Indicator UI");

        if (IsSelfClient())
        {
            //Get all old dom point indicators
            List<IndicatorBehavior> oldIndicators = FindObjectsOfType<IndicatorBehavior>().Where(I => I.domPoint != null).ToList();
            foreach (IndicatorBehavior I in oldIndicators)
            {
                Destroy(I.gameObject);
            }

            List<Domination> tier1DomPoints = FindObjectsOfType<Domination>().Where(a => a.GetComponent<Domination>().TierCapture == 1).ToList();

            foreach (Domination dom in tier1DomPoints)
            {
                GameObject indicator = GameObject.Instantiate(DomPointIndicatorPrefab);
                indicator.GetComponent<Canvas>().worldCamera = radarCam.GetComponent<Camera>();
                IndicatorBehavior ib = indicator.GetComponent<IndicatorBehavior>();
                ib.domPoint = dom.gameObject;
                indicator.transform.FindChild("UIImageGrey").GetComponent<Image>().sprite = dom.indicator;
                indicator.transform.FindChild("UIImageColor").GetComponent<Image>().sprite = dom.indicator;
            }
        }
    }


    public void RefreshIndicators ()
    {
        GameObject radarCam = GameObject.Find("RadarCamera");
        if (IsSelfClient())
        {
            foreach(IndicatorBehavior i in GameObject.FindObjectsOfType<IndicatorBehavior>())
            {
                GameObject.Destroy(i.gameObject);
            }


            AvatarController[] tagArray = GameObject.FindObjectsOfType<AvatarController>();

            AvatarController myController = GetComponent<AvatarController>();


            foreach (AvatarController ac in tagArray)
            {
                if (ac == myController)
                {
                    continue;
                }

                // Create Indicator
                GameObject indicator = GameObject.Instantiate(ExplorerIndicatorPrefab);
                indicator.GetComponent<Canvas>().worldCamera = radarCam.GetComponent<Camera>();

                // Tie indicator to a given instance of explorer
                IndicatorBehavior ib = indicator.GetComponent<IndicatorBehavior>();
                ib.avatar = ac.gameObject;

                if(ac.GetComponent<Sprint>() != null)//Sprinter
                {
                    GameObject uiimage = ib.gameObject.transform.FindChild("UIImage").gameObject;
                    uiimage.GetComponent<Image>().sprite = ib.Sprinter;
                }
                if(ac.GetComponent<Heal>() != null)//Prof
                {
                    GameObject uiimage = ib.gameObject.transform.FindChild("UIImage").gameObject;
                    uiimage.GetComponent<Image>().sprite = ib.Support;
                }   
                if(ac.GetComponent<SetTrapGlue>() != null)//Trapmaster 
                {
                    GameObject uiimage = ib.gameObject.transform.FindChild("UIImage").gameObject;
                    uiimage.GetComponent<Image>().sprite = ib.Trapper;
                }
            }
        }
    }

    bool IsSelfClient()
    {
        var avatar = GameObject.Find("Me");
        if (avatar !=null && avatar.GetComponent<AvatarController>() != null && avatar == this.gameObject)
        {
            return true;
        }

        return false;
    }
}
