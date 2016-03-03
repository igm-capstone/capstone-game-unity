using UnityEngine;
using System.Collections;

public class PlantBehavior : MonoBehaviour
{
    public int Damage;
    bool hasTarget;
    GameObject AtckTarget;
    RpcNetworkAnimator netAnim;

    // Use this for initialization
    void Start()
    {
        hasTarget = false;
        netAnim = GetComponent<RpcNetworkAnimator>();

        netAnim.SetTrigger("GoIdle");
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTarget)
        {
            MeeleeAttack(AtckTarget);
        }
        else
        {
            netAnim.SetTrigger("GoIdle");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && hasTarget == false)
        {
            AtckTarget = other.gameObject;
            hasTarget = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && other.gameObject == AtckTarget)
        {
            hasTarget = false;
        }
    }

    void MeeleeAttack(GameObject Target)
    {
        // Turn to Target
        Vector2 lookDir = Target.transform.position - transform.position;
        transform.GetChild(0).rotation = Quaternion.AngleAxis(Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg, Vector3.forward);

        netAnim.SetTrigger("Start_Bite");

    }

    // This method gets called by the Plant Animator via msg at the end of the Animation State.
    void BiteAnimationComplete()
    {
        // Hit (I guess)
        if ((AtckTarget.transform.position - transform.position).sqrMagnitude > 1)
        {
            AtckTarget.GetComponent<AvatarNetworkBehavior>().CmdAssignDamage(AtckTarget, Damage);
        }
    }
}
