using UnityEngine;
using System.Collections;

[RequireComponent(typeof(ISkill))]
public class PickupBehavior : MonoBehaviour {

    // Use this for initialization
	void Awake ()
	{
	    GetComponent<ISkill>().enabled = false;
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

        
        skillBar.AddSkill(GetComponent<ISkill>());

        Destroy(gameObject);
    }
}
