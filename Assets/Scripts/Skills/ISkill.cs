using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using System.Linq;

public struct ToolTipText
{

    public string Description;

    public string FirstLabel;
    public string SecondLabel;
    public string ThirdLabel;

    public string FirstAttribute;
    public string SecondAttribute;
    public string ThirdAttribute;
    
    // constructor for Default Values
    public ToolTipText (string _Dscrpt, string _FstAtt, string _SndAtt, string _ThdAtt)
    {
        Description = _Dscrpt;

        FirstLabel = "Attack:";
        SecondLabel = "Defense:";
        ThirdLabel = "Speed:";

        FirstAttribute = _FstAtt;
        SecondAttribute = _SndAtt;
        ThirdAttribute = _ThdAtt;
    }
}


public abstract class ISkill : MonoBehaviour
{
    public GameObject SkillButtonPrefab;
    public Sprite SkillSprite;
    public Sprite CooldownSprite;

    public string Name = "Skill";
    public float Cooldown = 2;
    //public float Duration = 0;

    [NonSerialized]
    public bool canDrop = false;

    public int cost = 0;
    public int UseCount = 0;

    public bool addToSkillBar = true;

    public ToolTipText ToolTip = new ToolTipText
                                ("Skill description here. \n Allows two lines.",
                                 "Attack: Regular",
                                 "Defense: Regular",
                                 "Speed: Regular");

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

    //ToolTip
    private GameObject toolTipObj;
    private Text toolTipDscrpt;

    private Text toolTipFstLbl;
    private Text toolTipSndLbl;
    private Text toolTipThdLbl;

    private Text toolTipFstAtt;
    private Text toolTipSndAtt;
    private Text toolTipThdAtt;

    private static bool toolTipChange = false;


    void OnEnable()
    {
        var mc = GameObject.Find("MainCanvas");
        toolTipObj = mc.transform.Find("SkillPanel/ToolTip").gameObject;
        
        // Gets Reference to the ToolTip texts.
        toolTipDscrpt = toolTipObj.transform.FindChild("Description").gameObject.GetComponent<Text>();

        toolTipFstLbl = toolTipObj.transform.FindChild("FirstLabel").gameObject.GetComponent<Text>();
        toolTipSndLbl = toolTipObj.transform.FindChild("SecondLabel").gameObject.GetComponent<Text>();
        toolTipThdLbl = toolTipObj.transform.FindChild("ThirdLabel").gameObject.GetComponent<Text>();

        toolTipFstAtt = toolTipObj.transform.FindChild("FirstAttribute").gameObject.GetComponent<Text>();
        toolTipSndAtt = toolTipObj.transform.FindChild("SecondAttribute").gameObject.GetComponent<Text>();
        toolTipThdAtt = toolTipObj.transform.FindChild("ThirdAttribute").gameObject.GetComponent<Text>();


        LastUse = float.MinValue;

        // CreateUI
        if (SkillButtonPrefab != null)
        {
            var skillBar = GameObject.Find("SkillBar");
            var button = Instantiate(SkillButtonPrefab);
            var showCallback = button.GetComponent<EventTrigger>().triggers.First(a => a.eventID == EventTriggerType.PointerEnter).callback;
            showCallback.AddListener(ShowToolTip);

            var hideCallback = button.GetComponent<EventTrigger>().triggers.First(a => a.eventID == EventTriggerType.PointerExit).callback;
            hideCallback.AddListener(HideToolTip);

            if (addToSkillBar)
            {
                button.transform.SetParent(skillBar.transform, false);
            }

            SkillBtnScript = button.GetComponent<SkillButton>();
            SkillBtnScript._skill = this;
            SkillBtnScript._skillBar = GetComponent<SkillBar>();
            SkillBtnScript.Init();
            //HideToolTip();
        }
    }

    void OnDisable()
    {
        if (SkillBtnScript != null)
        Destroy(SkillBtnScript.gameObject);
    }

    public bool Use() { return Use(null, Vector3.zero); }
    public bool Use(GameObject target, Vector3 clickWorldPos)
    {
        if (!IsReady())
        {
            HelpMessage.Instance.SetMessage(Name + " is still on cooldown!");
            return false;
        }

        string usageResult = Usage(target, clickWorldPos);
        if (usageResult == null)
        {
            LastUse = Time.time;
        }
        HelpMessage.Instance.SetMessage(usageResult);

        return usageResult == null;
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


    Coroutine coID;
    static Coroutine stopCoID;
    // Gets Called by Event Handler. Show/Hide Tooltip.
    void ShowToolTip(BaseEventData e)
    {
        if (toolTipChange)
        {
            StopCoroutine(stopCoID);
            toolTipChange = false;
        }
        UpdateToolTip();
        coID = StartCoroutine(MouseOverWait());
    }

    public void HideToolTip(BaseEventData e)
    {
        toolTipChange = true;
        stopCoID = StartCoroutine(NextToolTip());
    }

    public void UpdateToolTip()
    {
        toolTipDscrpt.text = ToolTip.Description;

        toolTipFstLbl.text = ToolTip.FirstLabel;
        toolTipSndLbl.text = ToolTip.SecondLabel;
        toolTipThdLbl.text = ToolTip.ThirdLabel;

        toolTipFstAtt.text = ToolTip.FirstAttribute;
        toolTipSndAtt.text = ToolTip.SecondAttribute;
        toolTipThdAtt.text = ToolTip.ThirdAttribute;
    }

    IEnumerator MouseOverWait()
    {
        if (toolTipChange)
            yield return null;

        yield return new WaitForSeconds(0.3f);
        toolTipObj.SetActive(true);
    }

    IEnumerator NextToolTip()
    {
        yield return new WaitForSeconds(1.5f);
        toolTipChange = false;
        toolTipObj.SetActive(false);
    }
}
