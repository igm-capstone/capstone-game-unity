using System;
using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.Networking;

public class ExplodeOnTarget : MinionBehaviour
{
    private bool isAttacking;
    public int AtckDamage;
    public float ExplosionRadius;

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

        Debug.Log("Trying to explode!");
        
        // Explode on players
        Vector2 Cur2DPosition = new Vector2(this.transform.position.x, this.transform.position.y);
        Collider2D[] NearbyAvatarsCol = Physics2D.OverlapCircleAll(Cur2DPosition, ExplosionRadius, LayerMask.GetMask("Player"));

        // Assign Damage
        foreach (var AvatarCol in NearbyAvatarsCol)
        {
            if (AvatarCol.gameObject.tag == "Player")
            {
                GetComponent<MinionController>().CmdAssignDamage(AvatarCol.gameObject, AtckDamage);
            }
        }

        // Kill self
        GetComponent<Health>().TakeDamage(int.MaxValue);
    }

    private bool CheckHit()
    {
        // check if target is in hit area
        return (Controller.ClosestAvatar.position - transform.position).sqrMagnitude > 1;
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, ExplosionRadius);
    }
}
