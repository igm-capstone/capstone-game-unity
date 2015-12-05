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
    
    public void Use() { Use(null, Vector3.zero); }
    public void Use(GameObject target, Vector3 clickWorldPos)
    {
        if (!IsReady()) return;
        if (Usage(target, clickWorldPos))
            _lastUse = Time.time;
    }

    protected abstract bool Usage(GameObject target, Vector3 clickWorldPos);

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
