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

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // This or post message to Game manager.
            Application.LoadLevel(Application.loadedLevel);
        }
    }

}
