using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class ISkill : MonoBehaviour
{
    public GameObject SkillButtonPrefab;
    public Sprite SkillSprite;
    public Sprite CooldownSprite;

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

    // This is used by the Spawn Ui Updater.
    [NonSerialized] public bool IsSpawnSkill = false;
    [NonSerialized] public float MinSpawnDist = 0.0f;
    [NonSerialized] public float MaxSpawnDist = 0.0f;

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
        // Get mouse position in World Coordinates
        Vector3 mouseWorldPos;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, LayerMask.GetMask(new[] { "Floor" })))
        {
            mouseWorldPos = hit.point;
        }
        else
        {
            mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        }


        // Calculate and apply look direction.
        Vector2 lookDir = mouseWorldPos - transform.position;
        transform.Find("AvatarRotation").rotation = Quaternion.AngleAxis(Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg, Vector3.forward);
    }

    //Gets the Position a set distance forward from the avatar.
    protected Vector3 GetPosForwardFromAvatar(float _dist)
    {
        Vector3 retPosition = transform.position + (transform.GetChild(0).transform.right * _dist);

        return retPosition;
    }
}
