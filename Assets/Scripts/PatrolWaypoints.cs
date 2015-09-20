using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TargetFollower))]
public class PatrolWaypoints : MonoBehaviour
{
    public WaypointPath path;

    private int nextStop;
    private TargetFollower follower;

    private void Awake()
    {
        follower = GetComponent<TargetFollower>();
    }

    // Update is called once per frame
	void Update ()
	{
        if (path == null) return;
	    var waypoints = path.GetWaypoints();

	    var nextWaypoint = waypoints[Mathf.Abs(nextStop)];
	    var direction = nextWaypoint.position - transform.position;

	    if (direction.sqrMagnitude < 1)
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

            nextWaypoint = waypoints[Mathf.Abs(nextStop)];
        }

        follower.MoveTowards(nextWaypoint);
	}
}
