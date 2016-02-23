using UnityEngine;
using System.Linq;

public class SetTrap : ISkill
{
    TrapType SlctdTrap;
    public void Awake()
    {
        Name = "SetTrap";
        canDrop = false;

        SlctdTrap = TrapType.Poison;

        // Set key code:
        key = KeyCode.Mouse1;

        isStaticSkill = false;
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
        GetComponent<AvatarNetworkBehavior>().CmdSetTrapExplorer(transform.position, SlctdTrap);
        return null;
    }
}

