using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;

public class LongAttack : ISkill {

    public int Damage = 2;

    RpcNetworkAnimator animator;
    AvatarNetworkBehavior avatarNetwork;
    AvatarController avatarController;
    Collider2D lastTarget;
    GameObject hb;

    public void Awake()
    {
        Name = "LongAttack";
        canDrop = true;

        animator = GetComponentInParent<RpcNetworkAnimator>();
        avatarNetwork = GetComponent<AvatarNetworkBehavior>();
        avatarController = GetComponent<AvatarController>();        

        hb = transform.FindChild("AvatarRotation").FindChild("LongAttackHitBox").gameObject;
        hb.SetActive(false);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            Use();
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
        hb.SetActive(true);
        Debug.DrawLine(aa, bb, Color.yellow, 5);

        var minions = Physics2D.OverlapAreaAll(aa, bb, 1 << LayerMask.NameToLayer("Minion"));
        animator.SetTrigger("LongAttack");        
        lastTarget = minions.Contains(lastTarget) ? lastTarget : minions.FirstOrDefault();
        
        return null;
    }

    void LongAttackAnimationComplete()
    {
        Debug.Log("Long Attack " + lastTarget);
        hb.SetActive(false);
        if (lastTarget == null)
            return;

        avatarNetwork.CmdAssignDamage(lastTarget.gameObject, Damage);
        lastTarget = null;
    }
}