using UnityEngine;
using System.Collections;
using System.Linq;

[RequireComponent(typeof(TargetFollower))]
public class ChaseTarget : MonoBehaviour
{
    private Transform target;

    private TargetFollower follower;

    void Awake()
    {
        follower = GetComponent<TargetFollower>();
    }

    private float chaseTime;

	void Update()
	{
        if (target == null)
        {
            var pos = transform.position;
            var avatars = FindObjectsOfType<AvatarNetworkBehavior>();
            target = avatars.Select(a => a.transform).OrderBy(t => (t.position - pos).sqrMagnitude).FirstOrDefault();
        }

	    if ((target.position - transform.position).sqrMagnitude > 1)
	    {
            follower.MoveTowards(target);
	    }

	    chaseTime += Time.deltaTime;

	    if (chaseTime > 1)
	    {
	        target = null;
	        chaseTime = 0;
	    }
	}
    
}
