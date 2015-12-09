using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ISkill))]
public class PickupBehavior : MonoBehaviour
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

        Destroy(gameObject);
    }
}
