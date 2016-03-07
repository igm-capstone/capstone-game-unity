using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;

public class GrenadeToss : ISkill
{
    RpcNetworkAnimator animator;
    AvatarNetworkBehavior avatarNetwork;
    AvatarController avatarController;

    // Class Variables
    public LayerMask HitLayers;

    public int Damage = 2;
    public bool hasKnockBack = false;

    // When changing this remeber to change the ExploDiameter variable on the ExplosionBehavior Script.
    public float ExploRadius = 3.5f;
    public float ThrowDistance = 7.0f;


    [SerializeField]
    [Range(0.0f, 10.0f)]
    float KnockBackMag = 5.0f;

    Vector3 ExploPos;

    public void Awake()
    {
        Name = "GrenadeToss";
        canDrop = false;

        animator = GetComponentInParent<RpcNetworkAnimator>();
        avatarNetwork = GetComponent<AvatarNetworkBehavior>();
        avatarController = GetComponent<AvatarController>();


        // Set key code:
        key = KeyCode.Mouse0;
    }

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            Use();
        }

        if (IsReady())
        {
            GetComponent<AvatarController>().isAttacking = false;
        }

    }// Update

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (avatarController.Disabled) return "You are incapacitated. Seek help!";
        if (avatarController.isHidden) return "You are hiding.";

        // Turns to mouse position.
        TurnToMousePos();
        
        // Trigger animation
        animator.SetTrigger("LongAttack");
        GetComponent<AvatarController>().isAttacking = true;

        // Calculate Explosion position
        ExploPos = GetPosForwardFromAvatar(ThrowDistance);

        // Instantiate explosion
        GetComponent<AvatarNetworkBehavior>().CmdSpawnExplosion(ExploPos, ExploRadius);

        // Get targets hit
        var TargetsHit = Physics2D.OverlapCircleAll(ExploPos, ExploRadius, HitLayers);

        foreach (var trgt in TargetsHit)
        {
            if (trgt.isTrigger)
            {
                continue;
            }

            // Check if knockBack is enabled and if hitting a minion.
            if (hasKnockBack && trgt.gameObject.layer == LayerMask.NameToLayer("Minion"))
            {
                avatarNetwork.CmdAssignDamageWithForce(trgt.gameObject, Damage, KnockBackMag);
            }
            else
            {
                avatarNetwork.CmdAssignDamage(trgt.gameObject, Damage);
            }
        } //foreach

        return null;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(ExploPos, ExploRadius);
    }
}