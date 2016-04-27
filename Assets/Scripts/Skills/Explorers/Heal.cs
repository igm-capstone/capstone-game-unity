using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Heal : ISkill
{
    public int HealAmount = 1;
    public float AreaRadius = 3;

    RpcNetworkAnimator animator;
    AvatarNetworkBehavior avatarNetwork;
    AvatarController avatarController;
    public Transform FX;

    public override string Name {  get { return "Heal"; } }

    public void Awake()
    {
        canDrop = false;

        animator = GetComponentInParent<RpcNetworkAnimator>();
        avatarNetwork = GetComponent<AvatarNetworkBehavior>();
        avatarController = GetComponent<AvatarController>();

        if (FX == null)
        {
           FX =  transform.FindChild("AvatarRotation").FindChild("AllAnimsInOne").FindChild("HealFX");
        }

        if (FX)
        {
            Transform oldParent = FX.parent;
            FX.parent = null;
            FX.localScale = new Vector3(AreaRadius, AreaRadius, 1);
            FX.parent = oldParent;
        }

        // Set key code:
        key = KeyCode.LeftShift;

        // ToolTip text
        ToolTip.Description =       "Recovers allies health around Explorer.";

        ToolTip.FirstLabel =        "Amount:";
        ToolTip.FirstAttribute =    HealAmount.ToString() + " HP";

        ToolTip.SecondLabel =       "Radius:";
        ToolTip.SecondAttribute =   AreaRadius.ToString() + "m";

        ToolTip.ThirdLabel =        "";
        ToolTip.ThirdAttribute =    "";

    }

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            Use();
        }
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (avatarController.Disabled) return "You are incapacitated. Seek help!";
        if (avatarController.isHidden) return "You are hiding.";

        animator.SetTrigger("Heal");

        var players = Physics2D.OverlapCircleAll(transform.position, AreaRadius, 1 << LayerMask.NameToLayer("Player"));

        foreach (var p in players)
        {
            // NEgative damage means healing.
            avatarNetwork.CmdAssignDamage(p.gameObject, -HealAmount);
        }
        return null;
    }

}
