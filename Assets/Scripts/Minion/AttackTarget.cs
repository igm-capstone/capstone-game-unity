using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Networking;

public class AttackTarget : MinionBehaviour {


    public Action AttackCompleteCallback;

    public override void ActivateBehaviour()
    {
        if (CheckHit())
        {
            GetComponent<RpcNetworkAnimator>().SetTrigger("Attack");
        }
    }

    public override void DeactivateBehaviour()
    {
        GetComponent<RpcNetworkAnimator>().SetTrigger("Iddle");
        if (AttackCompleteCallback != null) AttackCompleteCallback();
    }

    public override void UpdateBehaviour()
    {
        var target = Controller.ClosestAvatar;

        var targetAvatar = target.GetComponent<AvatarController>();
        if (targetAvatar != null && targetAvatar.Disabled)
        {
            Controller.DeactivateBehaviour();
            return;
        }

        if ((target.position - transform.position).magnitude > 3f)
        {
            Controller.DeactivateBehaviour();
            return;
        }
    }

    public void AttackAnimationComplete()
    {
        //// was attack canceled?
        //if (!isAttacking)
        //{
        //    return;
        //}

        // Damage
        Debug.Log("DAMAGE!");
        GetComponent<MinionController>().CmdAssignDamage(Controller.ClosestAvatar.gameObject, 1);

        // exit
        if (AttackCompleteCallback != null) AttackCompleteCallback();
    }

    private bool CheckHit()
    {
        // check if target is in hit area
        return (Controller.ClosestAvatar.position - transform.position).sqrMagnitude > 1;
    }
}
