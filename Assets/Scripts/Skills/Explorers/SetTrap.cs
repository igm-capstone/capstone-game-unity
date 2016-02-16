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

        key = KeyCode.Mouse1;
    }

    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            Debug.Log("Got key!");
            Use();
        }
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        Debug.Log("Trying to spawn Trap!");
        Debug.Log("Debug log" + TrapSpawnManager.Instance);
        Debug.Log("Debug " + transform);

        GetComponent<AvatarNetworkBehavior>().CmdSetTrapExplorer(transform.position, SlctdTrap);
        return null;
    }
}

