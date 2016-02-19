using UnityEngine;
using UnityEngine.Networking;

using System.Collections;

[Rig3DAsset("pickups", Rig3DExports.Position)]
public class PickupBehavior : NetworkBehaviour
{
    ISkill skill;

    [Export("skillName")]
    public string SkillName { get { return GetComponent<ISkill>().GetType().Name; } }

    // Use this for initialization
	void Awake ()
	{
        skill = GetComponent<ISkill>();
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

        skillBar.SetSkillEnabled(skill.Name, true);

        other.gameObject.GetComponent<AvatarNetworkBehavior>().CmdPickup(this.gameObject);
    }
}
