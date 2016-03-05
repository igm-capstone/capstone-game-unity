using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using UnityEngine.Audio;

public class MeleeWeapon : ISkill
{
    public int Damage = 1;
    public Rect hitbox;

    RpcNetworkAnimator animator;
    AvatarNetworkBehavior avatarNetwork;
    AvatarController avatarController;
    Collider2D lastTarget;
    float agroTime;

    public void Awake()
    {
        Name = "Melee";
        canDrop = false;

        avatarController = GetComponent<AvatarController>();
        avatarNetwork = GetComponent<AvatarNetworkBehavior>();
        animator = GetComponent<RpcNetworkAnimator>();

        key = KeyCode.M;
    }

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            Use();
        }

        if (agroTime > 0)
        {
            agroTime -= Time.deltaTime;

            if (agroTime <= 0)
            {
                agroTime = 0;
                lastTarget = null;
            }
        }
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (avatarController.Disabled) return "You are incapacitated. Seek help!";

        var hitbox = transform.Find("AvatarRotation/Hitbox");
        var hitboxCollider = hitbox.GetComponent<BoxCollider2D>();
        var hitboxOffset = hitboxCollider.offset;
        var hitboxSize = hitboxCollider.size * .5f;
        var aa = hitbox.TransformPoint(hitboxOffset - hitboxSize);
        var bb = hitbox.TransformPoint(hitboxOffset + hitboxSize);

        Debug.DrawLine(aa, bb, Color.magenta, 5);

        var minions = Physics2D.OverlapAreaAll(aa, bb, 1 << LayerMask.NameToLayer("Minion"));
        
        animator.SetTrigger("Attack");

        lastTarget = minions.Contains(lastTarget) ? lastTarget : minions.FirstOrDefault();

        if (lastTarget != null)
        {
            agroTime = 2;
        }

        return null;
    }

    void AttackAnimationComplete()
    {
        if (lastTarget == null)
        {
            return;
        }

        avatarNetwork.CmdAssignDamage(lastTarget.gameObject, Damage);
    }
}
