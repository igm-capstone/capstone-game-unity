using UnityEngine;
using UnityEngine.UI;

public abstract class ISkill : MonoBehaviour
{
    public string Name = "Skill";
    public float Cooldown = 2;

    private float _lastUse = float.MinValue;
    private Button _btn;
    private Image _fill;
    private Image _hl;
    private SkillBar _skillBar;

    void Start()
    {
        _btn = GetComponent<Button>();
        _fill = transform.Find("Fill").GetComponent<Image>();
        _hl = transform.Find("Highlight").GetComponent<Image>();
        _skillBar = GetComponentInParent<SkillBar>();

        if (_skillBar.OnClickBehavior == SkillBar.OnClickBehaviorType.SetActiveSkillOnClick)
            _btn.onClick.AddListener(SetActive);
        else
            _btn.onClick.AddListener(Use);
    }

    void Update()
    {
        _fill.fillAmount = 1 - ((Time.time - _lastUse) / Cooldown);
    }
    
    public void Use() { Use(null); }
    public void Use(GameObject target)
    {
        if (!IsReady()) return;
        Usage(target);
        _lastUse = Time.time;
    }

    protected abstract void Usage(GameObject target);

    public bool IsReady()
    {
        return (Time.time > _lastUse + Cooldown);
    }

    //Skillbar helpers
    public void SetActive()
    {
        _skillBar.SetActiveSkill(this);
    }

    public void SetHighlight(bool active)
    {
        _hl.enabled = active;
    }

    public void SetVisibility(bool active)
    {
        gameObject.SetActive(active);
    }

}
