using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Networking;

public class AttackTarget : MinionBehaviour {
    

    private bool isAttacking;
    public int AtckDamage;

    public override void ActivateBehaviour()
    {
        isAttacking = true;
        if (CheckHit())
        {
            GetComponent<RpcNetworkAnimator>().SetTrigger("Attack");
        }
    }

    public override void DeactivateBehaviour()
    {
        isAttacking = false;
        GetComponent<RpcNetworkAnimator>().SetTrigger("Iddle");
    }

    public override void UpdateBehaviour()
    {
        var target = Controller.ClosestAvatar;
        if (!target) return;

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
        if (!isAttacking)
        {
            return;
        }

        // Damage
        var nearAvatar = Controller.ClosestAvatar;
        if (nearAvatar)
        {
            GetComponent<MinionController>().CmdAssignDamage(nearAvatar.gameObject, AtckDamage);
            Controller.DeactivateBehaviour();
        }
    }

    private bool CheckHit()
    {
        // check if target is in hit area
        return (Controller.ClosestAvatar.position - transform.position).sqrMagnitude > 1;
    }
}
