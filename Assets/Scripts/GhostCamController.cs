using UnityEngine;
using System.Collections;

// Script that contols the Ghost Sliding windonw. The move speed depends on the FrameRate.
public class GhostCamController : MonoBehaviour
{
    public GameObject OuterWallsArray;
    float SpdXAxis, SpdYAxis;
    float ScrRightBorder, ScrLeftBorder, ScrUpBorder, ScrDownBorder;

    Transform GhostTransform;

    Camera GhostCam;

    Bounds LvlBounds;

    Vector3 LeftStep;
    Vector3 RightStep;
    Vector3 DownStep;
    Vector3 UpStep;

    public float keyMoveSpeed = 80.0f;


    // Use this for initialization
    void Start ()
    {
        // Object Getters
        GhostTransform = transform.parent.transform;
        GhostCam = GetComponent<Camera>();
        //GhostCam.orthographicSize = 20.0f;

        // Get Lvl bounds
        OuterWallsArray = GameObject.Find("Walls");
        LvlBounds = CreateLevelBounds(OuterWallsArray);

        // Starting Values
        // Speed
        SpdXAxis = 1.2f;
        SpdYAxis = 1.2f;

        // Position Steps
        LeftStep = new Vector3(-SpdXAxis, 0);
        RightStep = new Vector3(SpdXAxis, 0);
        DownStep = new Vector3(0, -SpdYAxis);
        UpStep = new Vector3(0, SpdYAxis);
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamWithKeyboard();
    }

    void MoveCamWithKeyboard()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Applies movement
        if (Mathf.Abs(vertical) > Mathf.Epsilon || Mathf.Abs(horizontal) > Mathf.Epsilon)
        {
            Vector3 moveDir = new Vector3(horizontal, vertical).normalized;
            GhostTransform.Translate(moveDir * keyMoveSpeed  * Time.deltaTime, Space.Self);
        }
    }

    void MoveCamWithMouse()
    {
        // This calculation is done here to account for the window changing sizes
        // These numbers are in pixels (screen space)
        ScrRightBorder = Screen.width - 10.0f;
        ScrLeftBorder = 10.0f;
        ScrUpBorder = Screen.height - 10.0f;
        ScrDownBorder = 10.0f;


        // This needs to be revamped
        // Mouse movement on X-axis of the screen
        // Left
        if (Input.mousePosition.x < ScrLeftBorder)
        {
            // If Screen can move, move!
            if (LvlBounds.Contains(GhostTransform.position + 10 * LeftStep))
                GhostTransform.position += LeftStep;
        }
        // Right
        else if (Input.mousePosition.x > ScrRightBorder)
        {
            if (LvlBounds.Contains(GhostTransform.position + 10 * RightStep))
                GhostTransform.position += RightStep;
        }

        // Mouse movement on Y-axis of the screen

        // Down
        if (Input.mousePosition.y < ScrDownBorder)
        {
            if (LvlBounds.Contains(GhostTransform.position + 10 * DownStep))
                GhostTransform.position += DownStep;
        }
        // Up
        else if (Input.mousePosition.y > ScrUpBorder)
        {
            if (LvlBounds.Contains(GhostTransform.position + 10 * UpStep))
                GhostTransform.position += UpStep;
        }
    }

    // Searches for the outer Walls
    Bounds CreateLevelBounds (GameObject OuterWalls)
    {
        Bounds WallBounds;
        Renderer StartRend;
        // Gets all renderer bounds from the outer walls
        var WallRends = OuterWalls.GetComponentsInChildren<Renderer>();
        WallBounds = WallRends[0].bounds;
        
        // Starting Bounds to encapsulate others.
        StartRend = WallRends[0];
        foreach (Renderer curRend in WallRends)
        {
            if (curRend.bounds != StartRend.bounds)
                WallBounds.Encapsulate(curRend.bounds);
        }

        return WallBounds;
    }
}
