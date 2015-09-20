using UnityEngine;
using System.Collections;

public class GuardBehavior : MonoBehaviour {

    public int MaxActiveLights = 3;
    private int ActiveLights = 0;
    LightController[] lights;
            
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
                    if ((light.CurrentStatus == LightController.Status.Off && ActiveLights < MaxActiveLights)) {
                        light.ToggleStatus();
                        ActiveLights++;
                    }
                    else if (light.CurrentStatus == LightController.Status.On)
                    {
                        light.ToggleStatus();
                        ActiveLights--;
                    }
                }
            }
        }
    }
}
