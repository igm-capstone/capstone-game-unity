using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TestRobotPath : MonoBehaviour
{

    public Transform target;
    public GridBehavior grid;

    void Update()
    {

        var path = GetTargetPath();

        var colors = new[] { Color.magenta, Color.yellow };


        for (int i = 1; i < path.Count; i++)
        {
            Debug.DrawLine(path[i - 1], path[i], colors[i%colors.Length]);
        }

        Vector2 targetPosition = transform.position;
        Vector2 position = transform.position;
        foreach (var nodePosition in path.Reverse())
        {
            var distance = nodePosition - position;
            var direction = distance.normalized;

            var hit = Physics2D.Raycast(position, direction, distance.magnitude);
            if (!hit)
            {
                targetPosition = nodePosition;
                break;
            }
        }

        Debug.DrawLine(position, targetPosition, Color.cyan);

        var targetAngle = Vector2.Angle(position, targetPosition);
        var targetRotation = Vector3.Cross(position, targetPosition).normalized * targetAngle;
        var newAngle = Vector3.RotateTowards(transform.eulerAngles, targetRotation, Time.deltaTime, 0);
        transform.eulerAngles = newAngle;
    }

    IList<Vector2> GetTargetPath()
    {
        var nodePath = grid.GetFringePath(gameObject, target.gameObject).Cast<Node>();

        // compress path
        var path = new List<Vector2>();

        if (nodePath.Count() < 2)
        {
            return path;
        }

        var previousNode = nodePath.First();
        var previousAngle = float.MaxValue;

        foreach (var node in nodePath.Skip(1))
        {
            var distance = node.position - previousNode.position;
            var pseudoAngle = FastMath.PseudoAtan2(distance.y, distance.x);

            if (FastMath.Approximately(previousAngle, pseudoAngle))
            {
                previousNode = node;
                continue;
            }

            path.Add(previousNode.position);

            previousNode = node;
            previousAngle = pseudoAngle;
        }

        path.Add(nodePath.Last().position);

        return path;
    } 

}
