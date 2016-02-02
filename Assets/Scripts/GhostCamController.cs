using UnityEngine;
using System.Collections;

public class GhostCamController : MonoBehaviour {


    public bool isMouseOnScreen;
    float SpdXAxis, SpdYAxis;
    float ScrRightBorder, ScrLeftBorder, ScrUpBorder, ScrDownBorder;

    Transform GhostTransform;

    // Use this for initialization
    void Start ()
    {
        // Object Getters
        GhostTransform = transform.parent.transform;

        // Starting Values
        SpdXAxis = 1.2f;
        SpdYAxis = 1.2f;
    }

    // Update is called once per frame
    void Update ()
    {
        // This calculation is done here to account for the window changing sizes
        ScrRightBorder = Screen.width - 10.0f;
        ScrLeftBorder = 10.0f;
        ScrUpBorder = Screen.height - 10.0f;
        ScrDownBorder = 10.0f;


        Debug.Log(Input.mousePosition);
        // Mouse movement on X-axis of the screen
        // Left
        if (Input.mousePosition.x < ScrLeftBorder)
            GhostTransform.position += new Vector3(-SpdXAxis, 0);
        // Right
        else if(Input.mousePosition.x > ScrRightBorder)
            GhostTransform.position += new Vector3(SpdXAxis, 0);
        
        // Mouse movement on Y-axis of the screen
        // Down
        if (Input.mousePosition.y < ScrDownBorder)
            GhostTransform.position += new Vector3(0, -SpdXAxis);
        // Up
        else if (Input.mousePosition.y > ScrUpBorder)
            GhostTransform.position += new Vector3(0, SpdXAxis);

        /*
                if (GhostTransform.position.x > RightLimit)
                {
                    GhostTransform.position = new Vector3(RightLimit, GhostTransform.position.y);
                }
                if (GhostTransform.position.x < LeftLimit)
                {
                    GhostTransform.position = new Vector3(LeftLimit, GhostTransform.position.y);
                }
                if (GhostTransform.position.x > UpLimit)
                {
                    GhostTransform.position = new Vector3(GhostTransform.position.x, UpLimit);
                }
                if (GhostTransform.position.x < DownLimit)
                {
                    GhostTransform.position = new Vector3(GhostTransform.position.x, DownLimit);
                }
                */
    }
}
