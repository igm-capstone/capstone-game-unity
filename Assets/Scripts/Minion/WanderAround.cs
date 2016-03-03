using UnityEngine;
using System.Collections;

public class WanderAround : MinionBehaviour
{
    private Node target;
    private GridBehavior grid;
    private float waitTime;

    public override void ActivateBehaviour()
    {
        grid = GridBehavior.Instance;
        GetNewTarget();
        var pos = transform.position;
        pos.z = grid.getNodeAtPos(pos).ZIndex;
        transform.position = pos;
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

        if (waitTime > 0)
        {
            GetComponent<RpcNetworkAnimator>().SetFloat("Speed", 0);
            waitTime -= Time.deltaTime;
            return;
        }

        var currNode = grid.getNodeAtPos(transform.position);
        if (currNode.position == target.position)
        {
            GetNewTarget();
        }

        if (!Controller.Follower.MoveTowards(target.position,20))
        {
            GetNewTarget();
        }
    }

    private void GetNewTarget()
    {
        waitTime = Random.Range(.5f, 4.5f);
        var possiblePoints = grid.getNodesNearPos(transform.position, 10, 5, p => p.canWalk && !p.hasLight);
        target = possiblePoints[Random.Range(0, possiblePoints.Count)];
    }
}
