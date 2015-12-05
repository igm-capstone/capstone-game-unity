using UnityEngine;
using System.Collections;

public class DisableLight : ISkill
{
    private static GhostController ghost;
    public float Duration = 1;

    protected override void Usage(GameObject target)
    {
        if (ghost == null) ghost = GameObject.Find("Me").GetComponent<GhostController>();

        var light = target.GetComponent<LightController>();

        if (light != null && light.CurrentStatus == LightController.Status.On)
        {
            light.ToggleStatus(); //ghost.CmdLightHasBeenClicked(light.gameObject.name); //Toggle on server
            StartCoroutine(TurnBackOn(light));
        }
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
