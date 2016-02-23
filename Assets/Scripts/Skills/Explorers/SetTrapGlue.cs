using UnityEngine;
using System.Linq;

public class SetTrapGlue : ISkill
{
    TrapType SlctdTrap;
    public void Awake()
    {
        Name = "SetTrapGlue";
        canDrop = false;

        SlctdTrap = TrapType.Glue;

        // Set key code:
        key = KeyCode.T;
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

