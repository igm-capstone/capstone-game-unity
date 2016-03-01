using UnityEngine;
using System.Collections;

public class DisableLight : ISkill
{
    SkillBar mySkillBar;

    public void Awake()
    {
        Name = "DisableLight";
        canDrop = false;
        cost = 0;

        key = KeyCode.Alpha1;

        mySkillBar = GetComponent<SkillBar>();
    }
    public void Update()
    {
        if (Input.GetKeyDown(key))
        {
            mySkillBar.SetActiveSkill(this);
        }
    }

    protected override string Usage(GameObject target, Vector3 clickWorldPos)
    {
        var light = target.GetComponent<LightController>();
        if (!light) return Name + " skill needs to target a light switch.";

        if (light != null && light.CurrentStatus == LightController.LghtStatus.On)
        {
            light.ChangeStatusTo(LightController.LghtStatus.Dimmed); //Disabled
            StartCoroutine(flickerToOff(light));
        }
        return null;
    }


    IEnumerator flickerToOff(LightController light)
    {
        yield return new WaitForSeconds(0.05f);
        light.ChangeStatusTo(LightController.LghtStatus.On); //On
        yield return new WaitForSeconds(0.1f);
        light.ChangeStatusTo(LightController.LghtStatus.Dimmed); //Disabled
        yield return new WaitForSeconds(0.05f);
        light.ChangeStatusTo(LightController.LghtStatus.On); //On
        yield return new WaitForSeconds(0.1f);
        light.ChangeStatusTo(LightController.LghtStatus.Dimmed); //Disabled
    }
}
