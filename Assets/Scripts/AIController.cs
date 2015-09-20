using UnityEngine;
using System.Collections;

public class AIController : MonoBehaviour {

    TestRobotPath follower;

	// Use this for initialization
	void Start () {
	    follower = GetComponent<TestRobotPath>();
        follower.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void StartFollow() {
        follower.enabled = true;
    }

    public void StartPatrol() {
        follower.enabled = false;
    }

    public void TurnOff() {
        follower.enabled = false;
    }
}
