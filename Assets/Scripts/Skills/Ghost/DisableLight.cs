using UnityEngine;
using System.Collections;

public class DisableLight : ISkill
{
    SkillBar mySkillBar;

    public override string Name { get { return "Put Out Lamp"; } }

    public void Awake()
    {
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

        if (light != null && light.CurrentStatus == LightController.LightStatus.On)
        {
            light.ChangeStatusTo(LightController.LightStatus.Dimmed); //Disabled
            StartCoroutine(flickerToOff(light));
        }
        return null;
    }


    IEnumerator flickerToOff(LightController light)
    {
        yield return new WaitForSeconds(0.05f);
        light.ChangeStatusTo(LightController.LightStatus.On); //On
        yield return new WaitForSeconds(0.1f);
        light.ChangeStatusTo(LightController.LightStatus.Dimmed); //Disabled
        yield return new WaitForSeconds(0.05f);
        light.ChangeStatusTo(LightController.LightStatus.On); //On
        yield return new WaitForSeconds(0.1f);
        light.ChangeStatusTo(LightController.LightStatus.Dimmed); //Disabled
    }
}
