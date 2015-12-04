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

	    if (target == null)
	    {
	        return;
	    }

	    if ((target.position - transform.position).magnitude > 1.5f)
	    {


            follower.MoveTowards(target);
	    }
	    else
	    {
	        // hit!
            GetComponent<MinionController>().Attack();
	        return;
	    }

	    chaseTime += Time.deltaTime;

	    if (chaseTime > 1)
	    {
	        target = null;
	        chaseTime = 0;
	    }
	}
    


}
