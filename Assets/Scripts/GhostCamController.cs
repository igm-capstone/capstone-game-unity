using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Script that contols the Ghost Sliding windonw. The move speed depends on the FrameRate.
public class GhostCamController : MonoBehaviour
{
    public GameObject OuterWallsArray;
    Transform GhostTransform;
    Camera GhostCam;
    Rect LvlBounds;

    Vector2 LastWorldPosition;
    Vector2 cameraTarget;

    public float keyMoveSpeed = 80.0f;


    // Use this for initialization
    void Start ()
    {
        // Object Getters
        GhostTransform = transform.parent.transform;
        GhostCam = GetComponent<Camera>();
        OuterWallsArray = GameObject.Find("Walls");
        LvlBounds = CreateLevelBounds(OuterWallsArray);
        GhostCam.ScreenToWorldPoint(Input.mousePosition);
    }

    void Update()
    {
        //if (worldGraph != null)
        //{
        //    DrawHistory();
        //}

        MoveCamWithMouseDrag();

        GhostTransform.position = Vector2.Lerp(GhostTransform.position, cameraTarget, Time.deltaTime * 50);
    }

    List<Vector2> worldGraph;
    List<Vector2> screenGraph;
    int index = 0;

    void MoveCamWithMouseDrag()
    {
        if (worldGraph == null)
        {
            worldGraph = new List<Vector2>();
            screenGraph = new List<Vector2>();
            for (int i = 0; i < 100; i++)
            {
                worldGraph.Add(new Vector2());
                screenGraph.Add(new Vector2());
            }
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 dskeyboard = new Vector2(horizontal, vertical);

        // calculate ds (delta space) for the mouse
        var mousePos = Input.mousePosition;
        var nearClip = GhostCam.nearClipPlane;
        Vector2 currWorldPos = mousePos * 0.1f; // GhostCam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, nearClip)); //camManager->Screen2WorldAtZ0(currScreenPos);
        Vector2 ds = (LastWorldPosition - currWorldPos) + dskeyboard;

        worldGraph[index] = currWorldPos;
        screenGraph[index] = mousePos;
        index = (index + 1) % worldGraph.Count;

        if (!Input.GetMouseButton(1) && dskeyboard.magnitude == 0)
        {
            LastWorldPosition = currWorldPos;
            return;
        }

        // get camera projected bounds (only valid for orthogonal camera)
        Vector2 min = GhostCam.ScreenToWorldPoint(new Vector3(100, 100, nearClip));
        Vector2 max = GhostCam.ScreenToWorldPoint(new Vector3(Screen.width - 100, Screen.height - 100, nearClip));

        // we want to test if scene bounds will contain the camera frame 
        // after the movement. If not, we need to remove the excess offset
        // from ds.
        Rect targetFrame = Rect.MinMaxRect(min.x + ds.x, min.y + ds.y, max.x + ds.x, max.y + ds.y);
        if (!LvlBounds.Contains(min) || !LvlBounds.Contains(max))
        {
            Vector2 targetOrigin = targetFrame.center;

            // move frame bounds inside scene bounds
            targetFrame = Fit(targetFrame, LvlBounds);

            Vector2 overflow = targetOrigin - targetFrame.center;
            ds -= overflow;
        }

        DrawRect(min, max, Color.blue);
        DrawRect(targetFrame.min, targetFrame.max, Color.red);
        DrawRect(LvlBounds.min, LvlBounds.max, Color.green);

        cameraTarget += ds;
        LastWorldPosition = currWorldPos;
    }

    void DrawHistory()
    {
        float width = Screen.width;
        float height = Screen.height;

        float wFactor = width / 1000.0f;
        float hxFactor = (height / width);
        float hyFactor = (width / height);

        float sxFactor = 50 / width;
        float syFactor = 50 / height;

        var u1 = worldGraph[0];
        var v1 = screenGraph[0];

        var px1 = new Vector2(0 * wFactor, u1.x * hxFactor);
        var py1 = new Vector2(0 * wFactor, u1.y * hyFactor);

        var sx1 = new Vector2(0 * wFactor, v1.x * hxFactor);
        var sy1 = new Vector2(0 * wFactor, v1.y * hyFactor);
        for (int i = 1; i < 100; i++)
        {
            var u2 = worldGraph[i];
            var px2 = new Vector2(i * wFactor, u2.x * hxFactor);
            var py2 = new Vector2(i * wFactor, u2.y * hyFactor);

            var v2 = screenGraph[i];
            var sx2 = new Vector2(i * wFactor, v2.x * sxFactor);
            var sy2 = new Vector2(i * wFactor, v2.y * syFactor);


            Debug.DrawLine(px1, px2, Color.cyan);
            Debug.DrawLine(py1, py2, Color.magenta);

            Debug.DrawLine(sx1, sx2, Color.blue);
            Debug.DrawLine(sy1, sy2, Color.red);

            px1 = px2;
            py1 = py2;

            sx1 = sx2;
            sy1 = sy2;
        }
    }

    void DrawRect(Vector2 min, Vector2 max, Color color)
    {
        Debug.DrawLine(new Vector2(min.x, min.y), new Vector2(min.x, max.y), color);
        Debug.DrawLine(new Vector2(min.x, max.y), new Vector2(max.x, max.y), color);
        Debug.DrawLine(new Vector2(max.x, max.y), new Vector2(max.x, min.y), color);
        Debug.DrawLine(new Vector2(max.x, min.y), new Vector2(min.x, min.y), color);
    }


    static Rect Fit(Rect inner, Rect outer)
    {
        Vector2 extentsOffset = (outer.size - inner.size) * .5f;
        Vector2 centerOffset = outer.center - inner.center;
        
        Vector2 size = Vector2.Min(inner.size, outer.size);
        Vector2 center = outer.center;

        if (extentsOffset.x >= 0)
        {
            center.x -= Mathf.Clamp(centerOffset.x, -extentsOffset.x, extentsOffset.x);
        }

        if (extentsOffset.y >= 0)
        {
            center.y -= Mathf.Clamp(centerOffset.y, -extentsOffset.y, extentsOffset.y);
        }


        return new Rect (center - size * .5f, size);
    }


    // Searches for the outer Walls
    Rect CreateLevelBounds(GameObject OuterWalls)
    {
        // Gets all renderer bounds from the outer walls
        var WallRends = OuterWalls.GetComponentsInChildren<Renderer>();
        Vector2 min = Vector2.one * float.MaxValue;
        Vector2 max = Vector2.one * float.MinValue;

        // Starting Bounds to encapsulate others.
        foreach (Renderer curRend in WallRends)
        {
            min = Vector2.Min(min, curRend.bounds.min);
            max = Vector2.Max(max, curRend.bounds.max);
        }

        return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
    }
}
