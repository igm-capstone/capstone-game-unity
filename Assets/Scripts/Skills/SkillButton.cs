using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public ISkill _skill;
    public SkillBar _skillBar;
    
    private Button _btn;
    private Image _fill;
    private Image _hl;
    private Image _icon;

    public void Init()
    {
        _btn = GetComponent<Button>();

        if (_skillBar.OnClickBehavior == SkillBar.OnClickBehaviorType.SetActiveSkillOnClick)
            _btn.onClick.AddListener(SetActiveClick);
        else if (_skillBar.OnClickBehavior == SkillBar.OnClickBehaviorType.UseSkill)
            _btn.onClick.AddListener(_skill.Use);
        else
            _btn.interactable = false;

        _fill = transform.Find("Fill").GetComponent<Image>();
        _hl = transform.Find("Highlight").GetComponent<Image>();
        _icon = transform.Find("Image").GetComponent<Image>();
        _icon.sprite = _skill.SkillSprite;
    }

    void Update()
    {
        _fill.fillAmount = 1 - ((Time.time - _skill.LastUse) / _skill.Cooldown);

        _hl.enabled = _skill.IsActive;
    }

    void SetActiveClick()
    {
        _skillBar.SetActiveSkill(_skill);
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
