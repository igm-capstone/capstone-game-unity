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
        Name = "Sword Swing";
        canDrop = false;

        animator = GetComponentInParent<RpcNetworkAnimator>();
        avatarNetwork = GetComponent<AvatarNetworkBehavior>();
        avatarController = GetComponent<AvatarController>();        

        DisplayBox = transform.FindChild("AvatarRotation/SpriteBox").gameObject;
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
        if (avatarController.isHidden) return "You are hiding";

        // Turns to mouse position.
        TurnToMousePos();

        DisplayBox.SetActive(true);

        // Trigger animation
        animator.SetTrigger("LongAttack");
        GetComponent<AvatarController>().isAttacking = true;

        return null;
    }

    void LongAttackAnimationComplete()
    {
        if (!GetComponent<NetworkIdentity>().hasAuthority)
        {
            return;
        }

        DisplayBox.SetActive(false);

        if (avatarController.Disabled || avatarController.isHidden) return;

        var hitBox = transform.Find("AvatarRotation/Hitbox");
        var hitBoxCol = hitBox.GetComponent<BoxCollider2D>();

        // Calculate Hitbox.
        var hitBoxSize = new Vector2(hitBoxCol.size.x * BoxXAdjst, hitBoxCol.size.y * BoxYAdjst);
        var hitBoxOffset = new Vector2(hitBoxSize.x, hitBoxCol.offset.y);

        HitBoxAA = hitBox.TransformPoint(hitBoxOffset - hitBoxSize);
        HitBoxBB = hitBox.TransformPoint(hitBoxOffset + hitBoxSize);

        // Get targets
        var TargetsHit = Physics2D.OverlapAreaAll(HitBoxAA, HitBoxBB, HitLayers);
        foreach (var trgt in TargetsHit)
        {
            // Check if I am hitting myself
            if (trgt.gameObject == this.gameObject)
            {
                continue;
            }

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
        }
    }
}