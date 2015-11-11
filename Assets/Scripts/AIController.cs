using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ChaseTarget))]
[RequireComponent(typeof(PatrolWaypoints))]
public class AIController : MonoBehaviour {

    private ChaseTarget chase;
    private PatrolWaypoints patrol;

	// Use this for initialization
	void Awake () {
	    chase = GetComponent<ChaseTarget>();
	    patrol = GetComponent<PatrolWaypoints>();
	}

    void Start()
    {
        chase.enabled = false;
	    patrol.enabled = false;
    }

    public void StartFollow() {
        chase.enabled = true;
        patrol.enabled = false;
    }

    public void StartPatrol() {
        chase.enabled = false;
        patrol.enabled = true;
    }

    public void TurnOff() {
        chase.enabled = false;
        patrol.enabled = false;
    }
}
