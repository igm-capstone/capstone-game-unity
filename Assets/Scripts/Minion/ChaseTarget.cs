using UnityEngine;
using System.Collections;
using System.Linq;

public class ChaseTarget : MinionBehaviour
{
	public override void UpdateBehaviour()
	{
       
	    var target = Controller.ClosestAvatar;
        if (target == null)
	    {
	        return;
	    }

	    var targetAvatar = target.GetComponent<AvatarController>();
	    if (targetAvatar != null && targetAvatar.Disabled)
	    {
	        target = null;
	        return;
	    }

	    if ((target.position - transform.position).magnitude > 1.5f)
	    {
            var success = Controller.Follower.MoveTowards(target.position, 15);
            if (!success)
            {
                Controller.DeactivateBehaviour();
            }
        }
	    else
	    {
	        // hit!
            Controller.ActivateBehaviour<AttackTarget>();
	        return;
	    }
	}
    


}
