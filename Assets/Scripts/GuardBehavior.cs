using UnityEngine;
using System.Collections;

public class GuardBehavior : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	
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
            GameObject[] lights = GameObject.FindGameObjectsWithTag("Light");
            foreach (GameObject light in lights) {
                if (light.GetComponent<CircleCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition))) {
                    light.GetComponent<Light2D>().Toggle();
                }
            }
        }
    }
}
