using UnityEngine;
using UnityEngine.Networking;

using System.Collections;

public class PickupBehavior : NetworkBehaviour
{
    ISkill skill;
    // Use this for initialization
	void Awake ()
	{
        skill = GetComponent<ISkill>() as ISkill;
       // skill.enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag != "Player")
        {
            return;
        }

        SkillBar skillBar = other.gameObject.GetComponent<SkillBar>();
        var g = GetComponent<ISkill>();
        skillBar.SetSkillEnabled(skill.Name, true);

        NetworkServer.Destroy(gameObject);
    }
}
