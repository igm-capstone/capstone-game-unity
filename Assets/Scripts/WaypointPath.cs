using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[ExecuteInEditMode]
public class WaypointPath : MonoBehaviour
{
    public bool loop;

    public Transform[] GetWaypoints()
    {
        return transform.Cast<Transform>().ToArray();
    }

    public void OnDrawGizmos()
    {
        var children = GetWaypoints();

        Gizmos.color = Color.yellow;
        for (int i = 1; i < children.Length; i++)
        {
            Gizmos.DrawLine(children[i-1].position, children[i].position);
        }

        if (loop)
        {
            Gizmos.DrawLine(children[children.Length - 1].position, children[0].position);
        }
    }
}
