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
            light.ToggleStatus(); //ghost.CmdLightHasBeenClicked(light.gameObject.name); //Toggle on server
            StartCoroutine(TurnBackOn(light));
        }
        return true;
    }

    IEnumerator TurnBackOn(LightController light)
    {
        yield return new WaitForSeconds(Duration);
        light.ToggleStatus(); //ghost.CmdLightHasBeenClicked(light.gameObject.name); //On
        yield return new WaitForSeconds(0.1f);
        light.ToggleStatus(); //ghost.CmdLightHasBeenClicked(light.gameObject.name); //Off
        yield return new WaitForSeconds(0.05f);
        light.ToggleStatus(); //ghost.CmdLightHasBeenClicked(light.gameObject.name); //On
        yield return new WaitForSeconds(0.1f);
        light.ToggleStatus(); //ghost.CmdLightHasBeenClicked(light.gameObject.name); //Off
        yield return new WaitForSeconds(0.1f);
        light.ToggleStatus(); //ghost.CmdLightHasBeenClicked(light.gameObject.name); //On
    }
}
