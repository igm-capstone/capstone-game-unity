using UnityEngine;
using System.Collections;

public class GuardBehavior : MonoBehaviour {

    public GameObject mLightPrefab;

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
            foreach (GameObject light in lights)
            {
                if (light.GetComponent<CircleCollider2D>().OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition)))
                {
                    light.GetComponent<Light2D>().Toggle();
                }
            }
        }
        //else if (Input.GetMouseButtonUp(1))
        //{
        //    Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //    position.z = 0.0f;
        //    GameObject.Instantiate(mLightPrefab, position, Quaternion.identity);
        //}
    }
}
