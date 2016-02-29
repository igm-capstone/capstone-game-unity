using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InteractSkill :  ISkill
{
    Vector3 IntrctBoxPos;
    public float IntrctBoxDist = 0.4f;
    public float IntrctBoxRad = 1;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    float ReviveHealPrcnt = 0.2f;

    LayerMask IntrctMask;

    AvatarNetworkBehavior avatarNetBhvr;

    public void Awake()
    {
        // ISkill SetUps
        Name = "InteractSkill";
        canDrop = false;
        key = KeyCode.E;

        avatarNetBhvr = GetComponent<AvatarNetworkBehavior>();

        // Assign interact mask, maybe not necessary
        IntrctMask =  1 << LayerMask.NameToLayer("LightSwitch") | 
                        1 << LayerMask.NameToLayer("Door") |
                        1 << LayerMask.NameToLayer("Player") |
                        1 << LayerMask.NameToLayer("HideSpot") |
                        1 << LayerMask.NameToLayer("Goal") ;

        // This is used to draw on the Unity Editor;
        IntrctBoxPos = GetPosForwardFromAvatar(IntrctBoxDist);
    }

    void Update()
    {
        // Check the Input.
        if (Input.GetKeyDown(key))
        {
            Use();
        }
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        IntrctBoxPos = GetPosForwardFromAvatar(IntrctBoxDist);
        // Detects light inside ToggleBox
        var HitTrgts = Physics2D.OverlapCircleAll(IntrctBoxPos, IntrctBoxRad, IntrctMask);

        foreach (var Trgt in HitTrgts)
        {
            if (Trgt.gameObject == this.gameObject)
            {
                continue;
            }

            if (Trgt.gameObject == null)
                return "Not me and still null obj...";

            Debug.Log("Hit: " + Trgt);
            // Get Interactable component

            var intrctScrpt = Trgt.gameObject.GetComponent<InteractableBehavior>();
            if (!intrctScrpt) return Trgt + " is not interactable.";
            InteractWithObj(Trgt.gameObject, intrctScrpt.Type);

        }
        return null;
    }

    void InteractWithObj(GameObject _targetObj, InteractableObject _targetType)
    {
        switch (_targetType)
        {
            case InteractableObject.Door:
                Debug.Log("Interacting with a door!");
                // This gets the Door Trigger, who is a child of the actual door.
                avatarNetBhvr.CmdDoor(_targetObj.transform.parent.gameObject);
                break;

            case InteractableObject.Player:
                Debug.Log("Interacting with a Player!");
                if (_targetObj.GetComponent<AvatarController>().Disabled)
                {
                    Debug.Log("Interacting with a Lamp!");
                    // If player is disabled revive. Amount is rounded down
                    int HealAmount = Mathf.FloorToInt(_targetObj.GetComponent<Health>().BaseHealth * ReviveHealPrcnt);
                    avatarNetBhvr.CmdAssignDamage(_targetObj, -HealAmount);
                }
                break;

            case InteractableObject.Light:
                Debug.Log("Interacting with a Lamp!");

                // Get light component
                var lightScr = _targetObj.GetComponent<LightController>();

                if (lightScr == null)
                {
                    Debug.Log("Light does no have LightController script");
                    break;
                }

                // Change light status acordingly
                if (lightScr.CurrentStatus == LightController.LghtStatus.On)
                {
                    avatarNetBhvr.CmdChangeLightStatus(lightScr.gameObject, LightController.LghtStatus.Dimmed);
                }
                else if ((lightScr.CurrentStatus == LightController.LghtStatus.Dimmed) || lightScr.CurrentStatus == LightController.LghtStatus.Off)
                {
                    avatarNetBhvr.CmdChangeLightStatus(lightScr.gameObject, LightController.LghtStatus.On);
                }
                break;

            case InteractableObject.Goal:
                Debug.Log("Interacting with a Goal!");
                break;

            default:
                break;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(IntrctBoxPos, IntrctBoxRad);
    }
}

