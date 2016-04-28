using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Sprint : ISkill
{
    public float Duration = 2;
    public float SpeedMultiplier = 2;
    public override string Name { get { return "Sprint"; } }

    private AvatarController avatarController;

    public void Awake()
    {
        canDrop = false;
        avatarController = GetComponent<AvatarController>();

        // Set key code:
        key = KeyCode.LeftShift;

        // ToolTip text
        ToolTip.Description =       "Doubles explorer Speed";

        ToolTip.FirstLabel =        "Duration:";
        ToolTip.FirstAttribute =    Duration.ToString() + " sec";

        ToolTip.SecondLabel =       "Cooldown:";
        ToolTip.SecondAttribute =   Cooldown.ToString() + " sec";

        ToolTip.ThirdLabel =        "";
        ToolTip.ThirdAttribute =    "";

    }

    public void Update()
    {
        if (Input.GetKeyDown(key))
        {
            Use();
        }
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (avatarController.Disabled) return "You are incapacitated. Seek help!";
        if (avatarController.isHidden) return "You are hiding";

        avatarController.MoveSpeed *= SpeedMultiplier;
        IsActive = true;
        StartCoroutine(EndEffect());

        return null;
    }

    IEnumerator EndEffect()
    {
        yield return new WaitForSeconds(Duration);
        IsActive = false;
        avatarController.MoveSpeed /= SpeedMultiplier;
    }
}
