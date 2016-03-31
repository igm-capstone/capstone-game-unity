using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class IndicatorCollector : MonoBehaviour {

    public GameObject ExplorerIndicatorPrefab;
    public GameObject DomPointIndicatorPrefab;

    public void DomPointIndicators()
    {
        GameObject radarCam = GameObject.Find("RadarCamera");
        if (IsSelfClient())
        {
            Domination[] dominationPoints = GameObject.FindObjectsOfType<Domination>();
            foreach (Domination dom in dominationPoints)
            {
                GameObject indicator = GameObject.Instantiate(DomPointIndicatorPrefab);
                indicator.GetComponent<Canvas>().worldCamera = radarCam.GetComponent<Camera>();
                IndicatorBehavior ib = indicator.GetComponent<IndicatorBehavior>();
                ib.domPoint = dom.gameObject;
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

                if(ac.name == "ConeSprinter(Clone)")
                {
                    GameObject uiimage = ib.gameObject.transform.FindChild("UIImage").gameObject;
                    uiimage.GetComponent<Image>().sprite = ib.Sprinter;
                }
                if(ac.name == "GrenadeSupport(Clone)")
                {
                    GameObject uiimage = ib.gameObject.transform.FindChild("UIImage").gameObject;
                    uiimage.GetComponent<Image>().sprite = ib.Support;
                }   
                if(ac.name == "LongAtckTrapper(Clone)")
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
