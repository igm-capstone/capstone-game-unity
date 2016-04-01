using UnityEngine;
using System.Collections;
using System;

public class ToggleLights : ISkill
{
    public GameObject ToggleLightBox;
    public float BoxRadius = 1f;

    Vector3 TogglePos;
    float ToggleSize;

    LayerMask LightMask;
    AvatarController avatarController;
    public void Awake()
    {
        // ISkill SetUps
        Name = "ToggleLights";
        canDrop = false;

        key = KeyCode.E;

        // Get ToggleLightBox object.
        if (ToggleLightBox == null)
        {
            ToggleLightBox =  transform.FindChild("AvatarRotation").FindChild("LightHitBox").gameObject;
        }
        LightMask = 1 << LayerMask.NameToLayer("LightSwitch");

        avatarController = GetComponent<AvatarController>();
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
        if (avatarController.Disabled) return "You are incapacitated. Seek help!";
        if (avatarController.isHidden) return "You are hiding";

        // Detects light inside ToggleBox
        var HitLight = Physics2D.OverlapCircle(ToggleLightBox.transform.position, BoxRadius, LightMask);

        if (HitLight == null)
            return null;

        // Get light component
        var light = HitLight.gameObject.GetComponent<LightController>();
        if (!light) return Name + " skill needs to target a light switch.";

        // Change light status acordingly
        if (light != null && light.CurrentStatus == LightController.LightStatus.On)
        {
            GetComponent<AvatarNetworkBehavior>().CmdChangeLightStatus(light.gameObject, LightController.LightStatus.Dimmed);
            //light.ChangeStatusTo(LightController.LghtStatus.Dimmed);
        }
        else if (light != null && ((light.CurrentStatus == LightController.LightStatus.Dimmed) || light.CurrentStatus == LightController.LightStatus.Off))
        {
            GetComponent<AvatarNetworkBehavior>().CmdChangeLightStatus(light.gameObject, LightController.LightStatus.On);
            //light.ChangeStatusTo(LightController.LghtStatus.On);
        }
        return null;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(ToggleLightBox.transform.position, BoxRadius);
    }
}
