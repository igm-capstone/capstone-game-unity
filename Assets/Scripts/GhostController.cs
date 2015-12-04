using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class GhostController : NetworkBehaviour {

    public int MaxActiveLights = 3;
    public bool InteractiveLights;
    private int ActiveLights = 0;
    LightController[] lights;
    [SerializeField]
    GameObject ghostUIPrefab;
    GhostHUD ghostHUD;
            
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

        GameObject ghostUI = Instantiate(ghostUIPrefab);
        ghostUI.transform.SetParent(GameObject.Find("MainCanvas").transform, false);
        ghostHUD = ghostUI.GetComponent<GhostHUD>();
        ghostHUD.SetMaxLightLevel(MaxActiveLights);
        ghostHUD.SetLightLevel(ActiveLights);
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
            if (InteractiveLights)
            {
                ToggleLightsAtWorldPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            }
        }
    }

    void ToggleLightsAtWorldPoint(Vector3 worldPoint)
    {
        foreach (LightController light in lights)
        {
            if (light.GetComponent<CircleCollider2D>().OverlapPoint(worldPoint))
            {
                bool shouldToggle = false;

                if ((light.CurrentStatus == LightController.Status.Off && ActiveLights < MaxActiveLights))
                {
                    ActiveLights++;
                    shouldToggle = true;

                }
                else if (light.CurrentStatus == LightController.Status.On)
                {
                    ActiveLights--;
                    shouldToggle = true;
                }

                if (shouldToggle)
                {
                    //Debug.Log("Client: toggling light");
                    //light.ToggleStatus();
                    ghostHUD.SetLightLevel(ActiveLights);
                    CmdLightHasBeenClicked(light.gameObject.name); //Toggle on server
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
