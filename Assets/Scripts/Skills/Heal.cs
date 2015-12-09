using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Heal : ISkill
{
    public int HealAmount = 1;
    public float AreaRadius = 2;

    RpcNetworkAnimator animator;

    AvatarNetworkBehavior avatarNetwork;
    AvatarController avatarController;

    public void Awake()
    {
        Name = "Heal";
        canDrop = true;
        animator = GetComponentInParent<RpcNetworkAnimator>();
        avatarNetwork = GetComponent<AvatarNetworkBehavior>();
        avatarController = GetComponent<AvatarController>();
        transform.Find("HealFX").localScale = new Vector3(AreaRadius, AreaRadius, 1);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            Use();
        }
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (avatarController.Disabled) return "You are incapacitated. Seek help!";
        
        animator.SetTrigger("Activated");

        var players = Physics2D.OverlapCircleAll(transform.position, AreaRadius, 1 << LayerMask.NameToLayer("Player"));

        foreach (var p in players)
        {
            GetComponent<AvatarNetworkBehavior>().CmdAssignDamage(p.gameObject, -HealAmount);
        }
        return null;
    }

}
