using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Sprint : ISkill
{
    public float Duration = 2;
    public float SpeedMultiplier = 2;

    private AvatarController avatarController;

    public void Awake()
    {
        Name = "Sprint";
        canDrop = false;
        avatarController = GetComponent<AvatarController>();

        // Set key code:
        key = KeyCode.Alpha1;
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
