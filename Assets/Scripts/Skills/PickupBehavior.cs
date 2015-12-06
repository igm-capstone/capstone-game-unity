using UnityEngine;
using System.Collections;

public class PickupBehavior : MonoBehaviour {

    public ISkill Skill;

	// Use this for initialization
	void Start ()
    {
	
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
        if (skillBar.IsFull())
        {
            skillBar.RemoveSkill(skillBar.GetSkill(0));
        }

        skillBar.AddSkill(Skill);
    }
}
