using UnityEngine;
using System.Collections;

// This Script runs on the explorer's copy on the server. It is responsable for enabling or disabling the 

public class SpawnUIUpdater : MonoBehaviour
{
    SkillBar mySkillBar;
    ISkill curActiveSkill;

    GameObject SpawnUI;
    

    // Use this for initialization
    void Start ()
    {
        var Ghost = GameObject.Find("Me");
        if (Ghost.GetComponent<GhostController>() == null)
        {
            //This is not on the server, must be disabled
            this.enabled= false;
        }

        mySkillBar = Ghost.GetComponent<SkillBar>();
        SpawnUI = transform.GetChild(3).gameObject;
    }
	
	// Update is called once per frame
	void Update ()
    {
        curActiveSkill = mySkillBar.GetActiveSkill();

        var type = curActiveSkill.GetType();

        if  (type != null &&((type == typeof(SpawnMinion)) || 
                            (type == typeof(SpawnAOE)) ||
                            (type == typeof(SpawnPlant))))
        {
            SpawnUI.SetActive(true);
        }
        else
        {
            SpawnUI.SetActive(false);
        }
	}
}
