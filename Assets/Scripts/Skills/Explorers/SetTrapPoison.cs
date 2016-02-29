using UnityEngine;
using System.Linq;

public class SetTrapPoison : ISkill
{
    TrapType SlctdTrap;

    public float spawnDistance;
    Vector3 spawnPosition;
    public void Awake()
    {
        Name = "SetTrapPoison";
        canDrop = false;

        SlctdTrap = TrapType.Poison;
        spawnDistance = 1.0f;

        // Set key code:
        key = KeyCode.Alpha1;
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
        // Face mouse direction
        TurnToMousePos();

        spawnPosition = GetPosForwardFromAvatar(spawnDistance);

        // Use skill
        GetComponent<AvatarNetworkBehavior>().CmdSetTrapExplorer(spawnPosition, SlctdTrap);
        return null;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(spawnPosition, 0.5f);
    }
}

