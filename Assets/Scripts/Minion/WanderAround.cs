using UnityEngine;
using System.Collections;

public class WanderAround : MinionBehaviour
{
    private Node target;
    private GridBehavior grid;

    public override void ActivateBehaviour()
    {
        grid = GridBehavior.Instance;
        GetNewTarget();
    }

    public override void DeactivateBehaviour()
    {
    }

    public override void UpdateBehaviour()
    {
        if (Controller.ClosestAvatar != null)
        {
            Debug.Log("chase");
            Controller.ActivateBehaviour<ChaseTarget>();
            return;
        }

        var currNode = grid.getNodeAtPos(transform.position);
        if (currNode.position == target.position)
        {
            GetNewTarget();
        }

        if (!Controller.Follower.MoveTowards(target.position))
        {
            GetNewTarget();
        }
    }

    private void GetNewTarget()
    {
        var possiblePoints = grid.getNodesNearPos(transform.position, 10, 5, p => p.canWalk && !p.hasLight);
        target = possiblePoints[Random.Range(0, possiblePoints.Count)];
    }
}
