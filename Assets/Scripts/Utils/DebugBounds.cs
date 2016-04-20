using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class DebugBounds : MonoBehaviour
{

    public Bounds bounds;

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
