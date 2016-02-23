using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;

public class LongAttack : ISkill
{
    RpcNetworkAnimator animator;
    AvatarNetworkBehavior avatarNetwork;
    AvatarController avatarController;
    Collider2D lastTarget;
    GameObject DisplayBox;

    public LayerMask HitLayers;

    // Class Variables
    public int Damage = 2;
    public bool hasKnockBack = false;

    [SerializeField]
    [Range(0.0f, 10.0f)]
    float KnockBackMag = 5.0f;

    Vector3 HitBoxAA, HitBoxBB;

    // Magic numbers to adjust the Hit colliders. Necessary because of Sacel in between childs and parents.
    float BoxXAdjst = 0.5f;
    float BoxYAdjst = 0.5f;

    public void Awake()
    {
        Name = "LongAttack";
        canDrop = false;

        animator = GetComponentInParent<RpcNetworkAnimator>();
        avatarNetwork = GetComponent<AvatarNetworkBehavior>();
        avatarController = GetComponent<AvatarController>();        

        DisplayBox = transform.FindChild("AvatarRotation").FindChild("LongAttackHitBox").gameObject;
        DisplayBox.SetActive(false);

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
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (avatarController.Disabled) return "You are incapacitated. Seek help!";

        // Turns to mouse position.
        TurnToMousePos();

        var hitBox = transform.Find("AvatarRotation/Hitbox");
        var hitBoxCol = hitBox.GetComponent<BoxCollider2D>();

        // Calculate Hitbox.
        var hitBoxSize = new Vector2(hitBoxCol.size.x * BoxXAdjst, hitBoxCol.size.y * BoxYAdjst);
        var hitBoxOffset = new Vector2(hitBoxSize.x, hitBoxCol.offset.y);

        HitBoxAA = hitBox.TransformPoint(hitBoxOffset - hitBoxSize);
        HitBoxBB = hitBox.TransformPoint(hitBoxOffset + hitBoxSize);

        DisplayBox.SetActive(true);

        // Trigger animation
        animator.SetTrigger("LongAttack");
        GetComponent<AvatarController>().isAttacking = true;

        // Get targets
        var TargetsHit = Physics2D.OverlapAreaAll(HitBoxAA, HitBoxBB, HitLayers);

        // Old implementation tied to animation:
        //lastTarget = TargetsHit.Contains(lastTarget) ? lastTarget : TargetsHit.FirstOrDefault();

        // New implementation cpoied from AOE attack.
        foreach (var trgt in TargetsHit)
        {
            // Check if I am hitting myself
            if (trgt.gameObject == this.gameObject)
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

    void LongAttackAnimationComplete()
    {
        DisplayBox.SetActive(false);

    /* Old implementation tied to animation
        if (lastTarget == null || lastTarget.gameObject == this.gameObject)
            return;

        // Check if knockBack is enabled and if hitting a minion.
        if (hasKnockBack && lastTarget.gameObject.layer == LayerMask.NameToLayer("Minion"))
        {   // Assign damage with knockback.
            avatarNetwork.CmdAssignDamageWithForce(lastTarget.gameObject, Damage, KnockBackMag);
        }
        else
        {   // Damage without force
            avatarNetwork.CmdAssignDamage(lastTarget.gameObject, Damage);
        }

        lastTarget = null;
    */
    }
}