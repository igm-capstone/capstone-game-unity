using UnityEngine;
using System.Collections;

[RequireComponent(typeof(TargetFollower))]
public class ChaseTarget : MonoBehaviour
{
    private Transform target;

    private TargetFollower follower;

    void Awake()
    {
        follower = GetComponent<TargetFollower>();
    }

	void Update()
	{
        if (target == null)
        {
            target = GameObject.Find("Avatar(Clone)").transform;
        }

	    if ((target.position - transform.position).sqrMagnitude > 1)
	    {
            follower.MoveTowards(target);
	    }

	}
    
}
