using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Networking;

public class PlantBehavior : NetworkBehaviour
{
    public int Damage;
    public float AttackRange = 3.5f;
    GameObject Target;
    RpcNetworkAnimator netAnim;
    private Collider2D[] collisions;

    // Use this for initialization
    void Start()
    {
        netAnim = GetComponent<RpcNetworkAnimator>();
        collisions = new Collider2D[4];
    }

    // Update is called once per frame
    void Update()
    {

        FindTarget();

        if (Target)
        {
            LookAtTarget();
        }
    }

    private void FindTarget()
    {
        if (Target)
        {
            return;
        }

        var playerMask = LayerMask.GetMask(new[] { "Player" });
        var collisionCount = Physics2D.OverlapCircleNonAlloc(transform.position, 3.5f, collisions, playerMask);
        for (int i = 0; i < collisionCount; i++)
        {
            var col = collisions[i];
            if (col.gameObject.tag == "Player")
            {
                Target = col.gameObject;
                netAnim.SetTrigger("Start_Bite");

                return;
            }
        }
    }

    private void LookAtTarget()
    {       
        Vector2 lookDir = Target.transform.position - transform.position;
        var container = transform.Find("Rotation");
        var newAngle = Mathf.LerpAngle(container.eulerAngles.z, Mathf.Atan2(lookDir.y, lookDir.x)* Mathf.Rad2Deg, Time.deltaTime);
        container.rotation = Quaternion.AngleAxis(newAngle, Vector3.forward);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, AttackRange);
    }

    // This method gets called by the Plant Animator via msg at the end of the Animation State.
    void BiteAnimationComplete()
    {
        if (!hasAuthority || !Target)
        {
            Target = null;
            return;
        }

        if ((Target.transform.position - transform.position).magnitude < AttackRange)
        {
            var avatarNB = Target.GetComponentInParent<AvatarNetworkBehavior>();
            avatarNB.CmdAssignDamage(avatarNB.gameObject, Damage);
        }

        Target = null;

    }

    [Command]
    public void CmdKill()
    {
        NetworkServer.Destroy(gameObject);
    }
}
