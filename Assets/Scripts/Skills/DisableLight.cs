using UnityEngine;
using System.Collections;

public class DisableLight : ISkill
{
    public float Duration = 2;

    protected override bool Usage(GameObject target, Vector3 clickWorldPos)
    {
        var light = target.GetComponent<LightController>();
        if (!light) return false;

        if (light != null && light.CurrentStatus == LightController.Status.On)
        {
            light.ToggleStatus(); //Off
            StartCoroutine(TurnBackOn(light));
        }
        return true;
    }

    IEnumerator TurnBackOn(LightController light)
    {
        yield return new WaitForSeconds(Duration);
        light.ToggleStatus(); //On
        yield return new WaitForSeconds(0.1f);
        light.ToggleStatus(); //Off
        yield return new WaitForSeconds(0.05f);
        light.ToggleStatus(); //On
        yield return new WaitForSeconds(0.1f);
        light.ToggleStatus(); //Off
        yield return new WaitForSeconds(0.1f);
        light.ToggleStatus(); //On
    }
}
