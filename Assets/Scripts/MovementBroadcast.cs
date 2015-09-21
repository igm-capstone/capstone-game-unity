using UnityEngine;
using System.Collections;

public class MovementBroadcast : MonoBehaviour {

    private GridBehavior grid;
    private LightController[] lights;
    Vector3 lastPos;
    Node lastNode = null;

    public bool AffectsGrid = true;
    public bool AffectsAI = true;

    // Use this for initialization
    void Start()
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
            if (AffectsGrid)
            {
                grid.dirty = true;
                foreach (LightController light in lights)
                {
                    light.dirty = true;
                }
            }
            if (AffectsAI)
            {
                grid.checkAI = true;
            }
            lastPos = transform.position;
        }
    }
}
