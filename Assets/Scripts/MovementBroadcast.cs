using UnityEngine;
using System.Collections;

public class MovementBroadcast : MonoBehaviour {

    private GridBehavior grid;
    private LightController[] lights;
    Vector3 lastPos;

    // Use this for initialization
    void Awake()
    {
        grid = FindObjectOfType<GridBehavior>();
        lights = FindObjectsOfType<LightController>();
        lastPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != lastPos)
        {
            grid.dirty = true;
            foreach (LightController light in lights)
            {
                light.dirty = true;
            }
            lastPos = transform.position;
        }
    }
}
