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
        avatarController = GetComponent<AvatarController>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
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
