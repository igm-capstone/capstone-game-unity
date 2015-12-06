using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(ChaseTarget))]
[RequireComponent(typeof(AttackTarget))]
[RequireComponent(typeof(PatrolWaypoints))]
public class MinionController : NetworkBehaviour
{

    private ChaseTarget chase;
    private PatrolWaypoints patrol;
    private AttackTarget attack;

	// Use this for initialization
	void Awake () {
	    chase = GetComponent<ChaseTarget>();
	    patrol = GetComponent<PatrolWaypoints>();
	    attack = GetComponent<AttackTarget>();
	}

    void Start()
    {
        chase.enabled = false;
	    patrol.enabled = false;
        attack.enabled = true;
    }

    public void StartFollow() {
        if (attack.isAttacking)
        {
            return;
        }

        chase.enabled = true;
        patrol.enabled = false;
        attack.CancelAttack();
    }

    public void StartPatrol() {
        chase.enabled = false;
        patrol.enabled = true;
        attack.CancelAttack();
    }

    public void TurnOff() {
        chase.enabled = false;
        patrol.enabled = false;
        attack.CancelAttack();
    }

    public void Attack()
    {
        var currChase = chase.enabled;
        var currPatrol = patrol.enabled;

        chase.enabled = false;
        patrol.enabled = false;

        attack.AttackCompleteCallback = () =>
        {
            chase.enabled = currChase;
            patrol.enabled = currPatrol;
        };

        attack.StartAttack();
    }


    [Command]
    public void CmdKill()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    public void CmdAssignDamage(GameObject obj, int damage)
    {
        obj.GetComponent<Health>().TakeDamage(damage);
    }
}
