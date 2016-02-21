using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class ISkill : MonoBehaviour
{
    public GameObject SkillButtonPrefab;
    public Sprite SkillSprite;

    public string Name = "Skill";
    public float Cooldown = 2;
    public bool canDrop = false;
    public int cost = 0;

    [NonSerialized] public KeyCode key = KeyCode.None;
    [NonSerialized] public bool IsActive;

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
        if (!IsReady())
        {
            HelpMessage.Instance.SetMessage(Name + " is still on cooldown!");
            return;
        }

        string usageResult = Usage(target, clickWorldPos);
        if (usageResult == null)
        {
            LastUse = Time.time;
        }
        HelpMessage.Instance.SetMessage(usageResult);
    }

    protected abstract string Usage(GameObject target, Vector3 clickWorldPos);

    public bool IsReady()
    {
        return (Time.time > LastUse + Cooldown);
    }
}
