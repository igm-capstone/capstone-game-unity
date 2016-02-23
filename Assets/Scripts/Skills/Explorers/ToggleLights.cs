using UnityEngine;
using System.Collections;
using System;

public class ToggleLights : ISkill
{
    public float BoxRadius = 0.8f;
    //public float BoxDist = 0.6f;

    public Transform LightBoxTrans;
    Vector3 lightBoxPos;

    LayerMask LightMask;

    public void Awake()
    {
        // ISkill SetUps
        Name = "ToggleLights";
        canDrop = false;

        key = KeyCode.E;

        if (LightBoxTrans == null)
        {
            LightBoxTrans = transform.FindChild("AvatarRotation").transform.FindChild("LightBox").transform;
        }

        LightMask = 1 << LayerMask.NameToLayer("LightSwitch");
    }

    void Update()
    {
        // Updates LightBox Position
        lightBoxPos = LightBoxTrans.position;

        // Check the Input.
        if (Input.GetKeyDown(key))
        {
            Use();
        }
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        Debug.Log("Using Skill " + Name);
        // Detects light inside ToggleBox
        var HitLight = Physics2D.OverlapCircle(lightBoxPos, BoxRadius, LightMask);

        if (HitLight == null)
            return "There are no light switches nearby... ";

        Debug.Log("Hit: " + HitLight);

        // Get light component
        var light = HitLight.gameObject.GetComponent<LightController>();
        if (!light) return Name + " skill needs to target a light switch.";

        // Change light status acordingly
        if (light != null && light.CurrentStatus == LightController.LghtStatus.On)
        {
            GetComponent<AvatarNetworkBehavior>().CmdChangeLightStatus(light.gameObject, LightController.LghtStatus.Dimmed);
            //light.ChangeStatusTo(LightController.LghtStatus.Dimmed);
        }
        else if (light != null && ((light.CurrentStatus == LightController.LghtStatus.Dimmed) || light.CurrentStatus == LightController.LghtStatus.Off))
        {
            GetComponent<AvatarNetworkBehavior>().CmdChangeLightStatus(light.gameObject, LightController.LghtStatus.On);
            //light.ChangeStatusTo(LightController.LghtStatus.On);
        }
        return null;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(lightBoxPos, BoxRadius);
    }
}
