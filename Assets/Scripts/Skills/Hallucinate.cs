using UnityEngine;
using System.Collections;

public class Hallucinate : ISkill
{
    private static GhostController ghost;
    public float Duration = 10;

    protected override bool Usage(GameObject target, Vector3 clickWorldPos)
    {
        if (ghost == null) ghost = GameObject.Find("Me").GetComponent<GhostController>();

        return true;
    }
}
