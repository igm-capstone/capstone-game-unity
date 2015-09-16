using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour {

    private GridBehavior grid;
    Vector3 lastPos;

    // Use this for initialization
    void Start()
    {
        grid = FindObjectOfType<GridBehavior>();
        lastPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != lastPos)
        {
            grid.SendMessage("UpdateCosts");
            lastPos = transform.position;
        }
    }
}
