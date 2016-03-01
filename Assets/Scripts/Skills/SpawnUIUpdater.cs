using UnityEngine;
using System.Collections;

// This Script runs on the explorer's copy on the server. It is responsable for enabling or disabling the 

public class SpawnUIUpdater : MonoBehaviour
{
    SkillBar mySkillBar;
    ISkill curActiveSkill;

    GameObject SpawnUI;

    // Magic number to make the UI and the actual area coincide. 
    float ScaleAdjst = 2.0f;
    

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

        if  (curActiveSkill != null && curActiveSkill.IsSpawnSkill)
        { 
            SpawnUI.SetActive(true);

            Vector3 MinSpawnScale = new Vector3(curActiveSkill.MinSpawnDist * ScaleAdjst, curActiveSkill.MinSpawnDist * ScaleAdjst, 1.0f);

            Vector3 MaxSpawnScale = new Vector3(curActiveSkill.MaxSpawnDist * ScaleAdjst, curActiveSkill.MaxSpawnDist * ScaleAdjst, 1.0f);

            SpawnUI.transform.GetChild(0).GetComponent<RectTransform>().localScale = MinSpawnScale;

            SpawnUI.transform.GetChild(1).GetComponent<RectTransform>().localScale = MaxSpawnScale;
        }
        else
        {
            SpawnUI.SetActive(false);
        }
	}
}
