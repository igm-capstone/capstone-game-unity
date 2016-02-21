using UnityEngine;
using System.Collections;
using System;
using System.Linq;

public class ConeAttack : ISkill
{
    RpcNetworkAnimator animator;
    AvatarNetworkBehavior avatarNetwork;
    AvatarController avatarController;
    Transform avatarModelTransform;
    GameObject HitBoxObj;

    Vector3 CloseBoxAA, CloseBoxBB;
    Vector3 FarBoxAA, FarBoxBB;

    //public LayerMask HitLayers = 1 << LayerMask.NameToLayer("Minion") | 1 << LayerMask.NameToLayer("Player");

    public LayerMask HitLayers;

    // Class Variable
    public int Damage = 2;
    public float radius = 2;
    public bool hasKnockBack = false;

    [SerializeField]
    [Range(0.0f, 10.0f)]
    float KnockBackMag = 5.0f;

    // Magic numbers to adjust the Hit colliders. Necessary because of Sacel in between childs and parents.
    float closeBoxXAdjst = 0.3f;
    float closeBoxYAdjst = 0.5f;
    float farBoxXAdjst = 0.3f;
    float farBoxYAdjst = 0.5f;

    public void Awake()
    {
        Name = "ConeAttack";
        canDrop = false;

        avatarController = GetComponent<AvatarController>();
        avatarNetwork = GetComponent<AvatarNetworkBehavior>();
        animator = GetComponent<RpcNetworkAnimator>();

        avatarModelTransform = transform.FindChild("AvatarRotation").FindChild("AllAnimsInOne");
        HitBoxObj = transform.FindChild("AvatarRotation").FindChild("ConeAttackHitBox").gameObject;
        HitBoxObj.SetActive(false);

        key = KeyCode.O;
    }

    void Update()
    {
        if(Input.GetKeyDown(key))
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
        Debug.Log("Using Skill " + Name);

        if (avatarController.Disabled) return "You are incapacitated. Seek Help!";
        
        //show Graphic HitBox 
        HitBoxObj.SetActive(true);
        
        // Signal the animator
        animator.SetTrigger("ConeAttack");
        GetComponent<AvatarController>().isAttacking = true;

        // Get targets from CloseBox
        var CloseHitBox = transform.Find("AvatarRotation/CloseHitBox");
        var CloseBoxCol = CloseHitBox.GetComponent<BoxCollider2D>();
        // Calculate Box size and position
        var CloseBoxSize = new Vector2(CloseBoxCol.size.x * closeBoxXAdjst, CloseBoxCol.size.y * closeBoxYAdjst);
        var CloseBoxOffset = new Vector2(CloseBoxSize.x, CloseBoxCol.offset.y);
        // Calculate Box Vertices
        CloseBoxAA = CloseHitBox.TransformPoint(CloseBoxOffset - CloseBoxSize);
        CloseBoxBB = CloseHitBox.TransformPoint(CloseBoxOffset + CloseBoxSize);

        // Check for hit.
        var closeTargetsHit = Physics2D.OverlapAreaAll(CloseBoxAA, CloseBoxBB, HitLayers);

        // Get targets from FarBox
        var FarHitBox = transform.Find("AvatarRotation/FarHitBox");
        var FarBoxCol = FarHitBox.GetComponent<BoxCollider2D>();
        // Calculate Box size and position
        var FarBoxSize = new Vector2(FarBoxCol.size.x * farBoxXAdjst, FarBoxCol.size.y * farBoxYAdjst);
        var FarBoxOffset = new Vector2(FarBoxSize.x, FarBoxCol.offset.y);
        // Calculate Box Vertices
        FarBoxAA = FarHitBox.TransformPoint(FarBoxOffset - FarBoxSize);
        FarBoxBB = FarHitBox.TransformPoint(FarBoxOffset + FarBoxSize);

        // Check for hit.
        var farTargetsHit = Physics2D.OverlapAreaAll(FarBoxAA, FarBoxBB, HitLayers);

        var allTargetsHit = closeTargetsHit.Concat(farTargetsHit).Distinct();

        foreach (Collider2D trgt in allTargetsHit)
        {
            // Check if I am hitting myself
            if (trgt.gameObject == this.gameObject)
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
        return null;
    }

    void ConeAttackAnimationComplete()
    {
        HitBoxObj.SetActive(false);
    }
}
