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
    GameObject AtckHitBox;
    
    // Class Variables
    public int Damage = 2;
    public bool hasKnockBack = false;

    [SerializeField]
    [Range(0.0f, 10.0f)]
    float KnockBackMag = 5.0f;

    public void Awake()
    {
        Name = "LongAttack";
        canDrop = false;

        animator = GetComponentInParent<RpcNetworkAnimator>();
        avatarNetwork = GetComponent<AvatarNetworkBehavior>();
        avatarController = GetComponent<AvatarController>();        

        AtckHitBox = transform.FindChild("AvatarRotation").FindChild("LongAttackHitBox").gameObject;
        AtckHitBox.SetActive(false);

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

        var hitbox = transform.Find("AvatarRotation/Hitbox");
        var hitboxCollider = hitbox.GetComponent<BoxCollider2D>();

        var hitboxSize = new Vector2(hitboxCollider.size.x * 1.15f, hitboxCollider.size.y * 0.5f);
        var hitboxOffset = new Vector2(hitboxSize.x, hitboxCollider.offset.y);

        var aa = hitbox.TransformPoint(hitboxOffset - hitboxSize);
        var bb = hitbox.TransformPoint(hitboxOffset + hitboxSize);
        AtckHitBox.SetActive(true);
        Debug.DrawLine(aa, bb, Color.yellow, 5);

        var minions = Physics2D.OverlapAreaAll(aa, bb, 1 << LayerMask.NameToLayer("Minion"));

        animator.SetTrigger("LongAttack");
        GetComponent<AvatarController>().isAttacking = true;

        lastTarget = minions.Contains(lastTarget) ? lastTarget : minions.FirstOrDefault();
        
        return null;
    }

    void LongAttackAnimationComplete()
    {
        //Debug.Log("Long Attack " + lastTarget);
        AtckHitBox.SetActive(false);
        if (lastTarget == null)
            return;

        if (hasKnockBack)
        {   // Assign damage with knockback.
            avatarNetwork.CmdAssignDamageWithForce(lastTarget.gameObject, Damage, KnockBackMag);
        }
        else
        {   // Damage without force
            avatarNetwork.CmdAssignDamage(lastTarget.gameObject, Damage);
        }

        lastTarget = null;
    }
}