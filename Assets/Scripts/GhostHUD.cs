using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GhostHUD : MonoBehaviour {
    public Sprite AvailableEnergySprite;
    public Sprite UsedEnergySprite;
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SetMaxLightLevel(int MaxActiveLights)
    {
        /*var panel = transform.FindChild("Panel");
        if (panel.transform.childCount != MaxActiveLights)
        {
            var icon = panel.transform.GetChild(0);
            for (var i = 1; i < MaxActiveLights; i++)
            {
                var newIcon = Instantiate(icon);
                newIcon.transform.SetParent(panel, false);
            }
        }*/
    }

    public void SetLightLevel(int ActiveLights)
    {
        /*var panel = transform.FindChild("Panel");
        
        for (var i = 0; i < ActiveLights && i < panel.transform.childCount; i++)
        {
            Image icon = panel.transform.GetChild(i).GetComponent<Image>();
            icon.sprite = UsedEnergySprite;
        }
        
        for (var i = ActiveLights; i < panel.transform.childCount; i++)
        {
            Image icon = panel.transform.GetChild(i).GetComponent<Image>();
            icon.sprite = AvailableEnergySprite;
        }*/
    }
}
