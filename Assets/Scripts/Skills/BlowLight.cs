using UnityEngine;
using System.Collections;
using UnityEditor;

public class BlowLight : ISkill {

    void Awake()
	{
        base.Awake(); 
	    Cooldown = 3;
        Name = "Blow Light";
	}

    void Update () {
        base.Update();

        if (IsActiveSkill())
        {
            if (Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.A) && IsReady())
            {
                StartCooldown();
                Debug.Log("Blow");
            }
        }
    }

    public override void OnClick()
    {
        SetActive(true);
    }
}
