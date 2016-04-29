using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using UnityEngine.Networking;

public class ConeAttack : ISkill
{
    RpcNetworkAnimator animator;
    AvatarNetworkBehavior avatarNetwork;
    AvatarController avatarController;
    GameObject SpriteBoxObj;

    Vector3 HitBoxAA, HitBoxBB;

    //public LayerMask HitLayers = 1 << LayerMask.NameToLayer("Minion") | 1 << LayerMask.NameToLayer("Player");

    public LayerMask HitLayers;

    // Class Variable
    public int Damage = 2;
    public bool hasKnockBack = false;

    [Range(0.0f, 10.0f)]
    public float KnockBackMag = 5.0f;

    // Magic numbers to adjust the Hit colliders. Necessary because of Sacel in between childs and parents.
    float hitBoxXAdjst = 0.4f;
    float hitBoxYAdjst = 0.4f;

    public override string Name { get { return "Baton Bash"; } }

    public void Awake()
    {
        canDrop = false;

        avatarController = GetComponent<AvatarController>();
        avatarNetwork = GetComponent<AvatarNetworkBehavior>();
        animator = GetComponent<RpcNetworkAnimator>();

        SpriteBoxObj = transform.FindChild("AvatarRotation/SpriteBox").gameObject;
        SpriteBoxObj.SetActive(false);

        // Set key code:
        key = KeyCode.Mouse0;

        // ToolTip text
        ToolTip.Description =       "Swings batons switfly and deals damage.";

        ToolTip.FirstAttribute =    "Low";

        ToolTip.SecondLabel =       "Knockback:";
        ToolTip.SecondAttribute =   "Low";

        ToolTip.ThirdAttribute =    "High";

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
        if (avatarController.Disabled) return "You are incapacitated. Seek Help!";
        if (avatarController.isHidden) return "You are hiding.";

        // Turns to mouse position.
        TurnToMousePos();

        //show Graphic HitBox 
        SpriteBoxObj.SetActive(true);

        // Signal the animator
        animator.SetTrigger("ConeAttack");
        GetComponent<AvatarController>().isAttacking = true;

        return null;
    }

    void ConeAttackAnimationComplete()
    {
        if (!GetComponent<NetworkIdentity>().hasAuthority)
        {
            return;
        }

        SpriteBoxObj.SetActive(false);

        if (avatarController.Disabled || avatarController.isHidden) return;

        // Get targets from CloseBox
        var HitBoxObj = transform.Find("AvatarRotation/HitBox");
        var HitBoxCol = HitBoxObj.GetComponent<BoxCollider2D>();

        // Calculate Box size and position
        var HitBoxSize = new Vector2(HitBoxCol.size.x * hitBoxXAdjst, HitBoxCol.size.y * hitBoxYAdjst);
        var HitBoxOffset = new Vector2(HitBoxSize.x, HitBoxCol.offset.y);
        // Calculate Box Vertices
        HitBoxAA = HitBoxObj.TransformPoint(HitBoxOffset - HitBoxSize);
        HitBoxBB = HitBoxObj.TransformPoint(HitBoxOffset + HitBoxSize);

        // Check for hit.
        var TargetssHit = Physics2D.OverlapAreaAll(HitBoxAA, HitBoxBB, HitLayers);

        foreach (Collider2D trgt in TargetssHit)
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

            // Check for KnockBack enabled and if hitting a minion
            if (hasKnockBack && trgt.gameObject.layer == LayerMask.NameToLayer("Minion"))
            {
                avatarNetwork.CmdAssignDamageWithForce(trgt.gameObject, Damage, KnockBackMag);
            }
            else
            {   //if hitting player or knockBack disabled, assign damage without knockback
                avatarNetwork.CmdAssignDamage(trgt.gameObject, Damage);
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawLine(HitBoxAA, HitBoxBB);
    }
}
