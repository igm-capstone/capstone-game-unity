using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Networking;

public class AttackTarget : NetworkBehaviour {

    private Transform target;

    public Action AttackCompleteCallback;
    public bool isAttacking;

    public void StartAttack()
    {
        if (target == null)
        {
            var pos = transform.position;
            var avatars = FindObjectsOfType<AvatarNetworkBehavior>();
            target = avatars.Select(a => a.transform).OrderBy(t => (t.position - pos).sqrMagnitude).FirstOrDefault();
        }

        isAttacking = true;

        if (CheckHit())
        {
            RpcAttack();
        }

    }

    public void CancelAttack()
    {
        target = null;
        if (isAttacking)
        {
            isAttacking = false;
            RpcCancel();
            if (AttackCompleteCallback != null) AttackCompleteCallback();
        }
    }

    void Update()
    {
        if (!target)
        {
            return;
        }


        if ((target.position - transform.position).magnitude > 3f)
        {
            CancelAttack();
        }
    }

    [ClientRpc]
    public void RpcAttack()
    {
        transform.Find("Model").GetComponent<Animator>().SetTrigger("Attack");
    }

    [ClientRpc]
    public void RpcCancel()
    {
        Debug.Log("Cancel Attack");
        transform.Find("Model").GetComponent<Animator>().SetTrigger("Iddle");
    }


    public void AttackAnimationComplete()
    {
        // was attack canceled?
        if (!isAttacking)
        {
            return;
        }

        // Damage
        Debug.Log("DAMAGE!");

        // exit
        target = null;
        if (AttackCompleteCallback != null) AttackCompleteCallback();
    }

    private bool CheckHit()
    {
        // check if target is in hit area
        return (target.position - transform.position).sqrMagnitude > 1;
    }
}
