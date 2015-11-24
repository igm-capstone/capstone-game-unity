using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GhostBehavior : NetworkBehaviour {

    public int MaxActiveLights = 3;
    private int ActiveLights = 0;
    LightController[] lights;
    [SerializeField]
    GameObject guardUIPrefab;
    GhostHUD guardHUD;
            
	// Use this for initialization
	void Start ()
    {
        lights = FindObjectsOfType<LightController>();
        foreach (LightController light in lights)
        {
            if (light.CurrentStatus == LightController.Status.On)
            {
                ActiveLights++;
            }
        }

        GameObject guardUI = Instantiate(guardUIPrefab);
        guardUI.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
        guardHUD = guardUI.GetComponent<GhostHUD>();
        guardHUD.SetMaxLightLevel(MaxActiveLights);
        guardHUD.SetLightLevel(ActiveLights);
	}
	
	// Update is called once per frame
	void Update ()
    {
        HandleInput();
	}

    void HandleInput()
    {
        // Using mouse over instead of ray cast due to 2D collider. Physics does not interact with Physics2D.
        if (Input.GetMouseButtonUp(0))
        {
            foreach (LightController light in lights)
            {
                if (light.GetComponent<CircleCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
                {
                    bool shouldToggle = false;

                    if ((light.CurrentStatus == LightController.Status.Off && ActiveLights < MaxActiveLights)) {
                        ActiveLights++;
                        shouldToggle = true;
                        
                    }
                    else if (light.CurrentStatus == LightController.Status.On)
                    {
                        ActiveLights--;
                        shouldToggle = true;
                    }

                    if (shouldToggle) {
                        //Debug.Log("Client: toggling light");
                        //light.ToggleStatus();
                        guardHUD.SetLightLevel(ActiveLights);
                        CmdLightHasBeenClicked(light.gameObject.name); //Toggle on server
                    }
                }
            }
        }
    }

    [Command]
    void CmdLightHasBeenClicked(string lightName)
    {
        //Debug.Log("Client: sending CMD!");
        LightController light = GameObject.Find(lightName).GetComponent<LightController>();
        light.ToggleStatus();
    }

}
