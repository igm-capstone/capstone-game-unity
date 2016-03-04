using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;

public class PlantBehavior : NetworkBehaviour
{
    public int Damage;
    public float TriggerRadius = 3.5f;
    GameObject AtckTarget;
    RpcNetworkAnimator netAnim;
    private bool isAttacking;

    // Use this for initialization
    void Start()
    {
        netAnim = GetComponent<RpcNetworkAnimator>();
        foreach (var item in GetComponents<CircleCollider2D>())
        {
            if (item.isTrigger)
            {
                item.radius = TriggerRadius;
                break;
            }
        } 
    }

    // Update is called once per frame
    void Update()
    {
        if (AtckTarget)
        {
            LookAtTarget();
            MeeleeAttack();
        }
    }

    private void LookAtTarget()
    {       
        Vector2 lookDir = AtckTarget.transform.position - transform.position;
        transform.GetChild(0).rotation = Quaternion.AngleAxis(Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !AtckTarget)
        {
            AtckTarget = other.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject == AtckTarget)
        {
            AtckTarget = null;
        }
    }

    void MeeleeAttack()
    {
        if (isAttacking )
        {
            return;
        }

        isAttacking = true;
        netAnim.SetTrigger("Start_Bite");
    }

    // This method gets called by the Plant Animator via msg at the end of the Animation State.
    void BiteAnimationComplete()
    {
        if (!isAttacking)
        {
            return;
        }

        Debug.Log("Entrei em BiteAnimationComplete");
        // Hit (I guess)
        if (AtckTarget && hasAuthority && (AtckTarget.transform.position - transform.position).sqrMagnitude > TriggerRadius * TriggerRadius)
        {
            var avatarNB = AtckTarget.GetComponentInParent<AvatarNetworkBehavior>();
            avatarNB.CmdAssignDamage(avatarNB.gameObject, Damage);
        }

        isAttacking = false;
    }
}
