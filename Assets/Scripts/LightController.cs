using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Light2D))]
public class LightController : MonoBehaviour {

    Light2D light2d;

	// Use this for initialization
	void Awake () {
        light2d = GetComponent<Light2D>();    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void UpdateLightFX()
    {
        light2d.UpdateLightFX();
    }


}
