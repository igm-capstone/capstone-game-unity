using UnityEngine;
using System.Collections;

public class PatrolWaypoints : MinionBehaviour
{
    public WaypointPath path;
    public int nextStop;
    
    private Transform[] waypoints;

    void Awake()
    {
        if (path == null) return;

        // update waypoints
	    waypoints = path.GetWaypoints();
    }

    // Update is called once per frame
	public override void UpdateBehaviour ()
	{
        if (waypoints == null) return;

	    var nextWaypoint = waypoints[Mathf.Abs(nextStop)];
	    var direction = nextWaypoint.position - transform.position;
        var node = GridBehavior.Instance.getNodeAtPos(nextWaypoint.position);
        var canWalk = node != null && node.canWalk;

	    if (direction.sqrMagnitude < 1 || !canWalk)
	    {
	        nextStop += 1;

            // the sign represents the direction of the movement (+ -> towards last wp, 
            // - -> towards first wp) for non looping paths.
            //
            //            ->      1     ->     n-1     ->    
            //   0   x------------x-------------x------------x   n
            //            <-     -1     <-   -(n-1)    <-
	        if (nextStop >= waypoints.Length)
	        {
	            nextStop = path.loop ? 0 : 2 - waypoints.Length;
	        }

            return;
            //nextWaypoint = waypoints[Mathf.Abs(nextStop)];
        }

        Controller.Follower.MoveTowards(nextWaypoint.position);
	}
}
