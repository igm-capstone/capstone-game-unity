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

                var distance = transform.position - lastPos;
                var direction = distance.normalized;
                var angle = FastMath.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                var size = GetComponent<PolygonCollider2D>().bounds.size * 1.25f;
                var layerMask = 1 << LayerMask.NameToLayer("Lights");

                DebugBox(lastPos, size, Color.blue);
                DebugBox(transform.position, size, Color.red);

                var lights = Physics2D.BoxCastAll(lastPos, size, angle, direction, distance.magnitude, layerMask);
                foreach (var hit in lights)
                {
                    var light = hit.transform.GetComponentInParent<LightController>();
                    if (light) light.dirty = true;
                }

                //foreach (LightController light in lights)
                //{
                //    light.dirty = true;
                //}
            }
            if (AffectsAI)
            {
                grid.checkAI = true;
            }
            lastPos = transform.position;
        }

    }
    void DebugBox(Vector2 pos, Vector2 size, Color color) {

        Debug.DrawLine(pos + new Vector2(+size.x * .5f, +size.y * .5f), pos + new Vector2(+size.x * .5f, -size.y * .5f), color);
        Debug.DrawLine(pos + new Vector2(+size.x * .5f, -size.y * .5f), pos + new Vector2(-size.x * .5f, -size.y * .5f), color);
        Debug.DrawLine(pos + new Vector2(-size.x * .5f, -size.y * .5f), pos + new Vector2(-size.x * .5f, +size.y * .5f), color);
        Debug.DrawLine(pos + new Vector2(-size.x * .5f, +size.y * .5f), pos + new Vector2(+size.x * .5f, +size.y * .5f), color);
    }
}
