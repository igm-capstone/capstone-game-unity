using UnityEngine;
using System.Linq;

public class SetTrapPoison : ISkill
{
    TrapType SlctdTrap;
    public void Awake()
    {
        Name = "SetTrapPoison";
        canDrop = false;

        SlctdTrap = TrapType.Poison;

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

        TurnToMousePos();

        GetComponent<AvatarNetworkBehavior>().CmdSetTrapExplorer(transform.position, SlctdTrap);
        return null;
    }
}

