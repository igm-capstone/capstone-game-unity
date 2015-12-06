using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class ISkill : MonoBehaviour
{
    public GameObject SkillButtonPrefab;
    public Sprite SkillSprite;

    public string Name = "Skill";
    public float Cooldown = 2;

    public float LastUse { get; private set; }

    [NonSerialized]
    public SkillButton SkillBtnScript;
    
    void OnEnable()
    {
        LastUse = float.MinValue;

        // CreateUI
        if (SkillButtonPrefab != null)
        {
            var skillBar = GameObject.Find("SkillBar");
            var button = Instantiate(SkillButtonPrefab);
            button.transform.SetParent(skillBar.transform, false);

            SkillBtnScript = button.GetComponent<SkillButton>();
            SkillBtnScript._skill = this;
            SkillBtnScript._skillBar = GetComponent<SkillBar>();
            SkillBtnScript.Init();
        }
    }

    void OnDisable()
    {
        if (SkillBtnScript != null)
        Destroy(SkillBtnScript.gameObject);
    }

    public void Use() { Use(null, Vector3.zero); }
    public void Use(GameObject target, Vector3 clickWorldPos)
    {
        if (!IsReady()) return;
        if (Usage(target, clickWorldPos))
            LastUse = Time.time;
    }

    protected abstract bool Usage(GameObject target, Vector3 clickWorldPos);

    public bool IsReady()
    {
        return (Time.time > LastUse + Cooldown);
    }
}
