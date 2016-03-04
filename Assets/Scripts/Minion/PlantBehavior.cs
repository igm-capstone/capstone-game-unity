using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;

public class PlantBehavior : NetworkBehaviour
{
    public int Damage;
    public float AttackRange = 3.5f;
    GameObject AtckTarget;
    RpcNetworkAnimator netAnim;
    private bool isAttacking;

    // Use this for initialization
    void Start()
    {
        netAnim = GetComponent<RpcNetworkAnimator>();
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
        var container = transform.Find("Rotation");
        var newAngle = Mathf.LerpAngle(container.eulerAngles.z, Mathf.Atan2(lookDir.y, lookDir.x)* Mathf.Rad2Deg, Time.deltaTime);
        container.rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!AtckTarget && other.gameObject.tag == "Player")
        {
            AtckTarget = other.gameObject;
            isAttacking = false;
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
        if (isAttacking)
        {
            return;
        }

        isAttacking = true;
        netAnim.SetTrigger("Start_Bite");
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }

    // This method gets called by the Plant Animator via msg at the end of the Animation State.
    void BiteAnimationComplete()
    {
        if (!isAttacking || !AtckTarget)
        {
            return;
        }

        if (AtckTarget && hasAuthority && (AtckTarget.transform.position - transform.position).magnitude < AttackRange)
        {
            var avatarNB = AtckTarget.GetComponentInParent<AvatarNetworkBehavior>();
            avatarNB.CmdAssignDamage(avatarNB.gameObject, Damage);
        }

        isAttacking = false;
    }

    [Command]
    public void CmdKill()
    {
        NetworkServer.Destroy(gameObject);
    }
}
