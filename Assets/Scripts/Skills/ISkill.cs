using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class ISkill : MonoBehaviour
{
    public GameObject SkillButtonPrefab;
    public Sprite SkillSprite;

    public string Name = "Skill";
    public float Cooldown = 2;
    [NonSerialized]
    public bool canDrop = false;
    public int cost = 0;
    public int UseCount = 0;

    // Used for EquippedSkill.
    //public bool isStaticSkill = true;

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
    
    //Turns to mouse position when called.
    protected void TurnToMousePos()
    {
        // Mouse and Key controls - Rotation
        // Get mouse position in World Coordinates
        Vector3 mouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z);
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        // Calculate look position.
        float LookPosX = mouseWorldPos.x - transform.position.x;
        float LookPosY = mouseWorldPos.y - transform.position.y;
        
        // Apply rotation
        transform.GetChild(0).rotation = Quaternion.AngleAxis(Mathf.Atan2(LookPosY, LookPosX) * Mathf.Rad2Deg, Vector3.forward);
    }

    //Gets the Position a set distance forward from the avatar.
    protected Vector3 GetPosForwardFromAvatar(float _dist)
    {
        Vector3 retPosition = transform.position + (transform.GetChild(0).transform.right * _dist);

        return retPosition;
    }
}
