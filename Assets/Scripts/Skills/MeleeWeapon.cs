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
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
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

        if (!minions.Any())
        {
            return null;
        }

        animator.SetTrigger("Attack");

        var minion = minions.Contains(lastTarget) ? lastTarget : minions.First();
        avatarNetwork.CmdAssignDamage(minion.gameObject, Damage);
        lastTarget = minion;
        agroTime = 2;

        return null;
    }
}
