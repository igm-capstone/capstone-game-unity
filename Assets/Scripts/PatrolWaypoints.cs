using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TargetFollower))]
public class PatrolWaypoints : MonoBehaviour
{
    public WaypointPath path;
    public int nextStop;
    
    private TargetFollower follower;

    private GridBehavior grid;
    private Transform[] waypoints;

    void Awake()
    {
        grid = FindObjectOfType<GridBehavior>();
        follower = GetComponent<TargetFollower>();
    }

    void Start()
    {
        if (path == null) return;

        // update waypoints
	    waypoints = path.GetWaypoints();
    }

    // Update is called once per frame
	void Update ()
	{
        if (waypoints == null) return;

	    var nextWaypoint = waypoints[Mathf.Abs(nextStop)];
	    var direction = nextWaypoint.position - transform.position;
        var node = grid.getNodeAtPos(nextWaypoint.position);
        var canWalk = node.hasLight && node.canWalk;

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
            nextWaypoint = waypoints[Mathf.Abs(nextStop)];
        }

        follower.MoveTowards(nextWaypoint);
	}


    void TargetNextWaypoint()
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

    }
}
