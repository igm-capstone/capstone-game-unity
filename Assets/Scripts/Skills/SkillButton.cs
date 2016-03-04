using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public ISkill _skill;
    public SkillBar _skillBar;
    
    private Button _btn;
    private Image _cdBackground;
    private Image _cd;
    private Image _icon;

    public void Init()
    {
        _btn = GetComponent<Button>();

        if (_skillBar.OnClickBehavior == SkillBar.OnClickBehaviorType.SetActiveSkillOnClick)
            _btn.onClick.AddListener(SetActiveClick);
        else if (_skillBar.OnClickBehavior == SkillBar.OnClickBehaviorType.UseSkill)
            _btn.onClick.AddListener(() => _skill.Use());
        else
            _btn.interactable = false;

        _cdBackground = transform.Find("ZLastLayer").GetComponent<Image>();
        //_cdBackground.enabled = false;
        _cd = transform.Find("Cooldown").GetComponent<Image>();
        _cd.sprite = _skill.CooldownSprite;
        _cd.type = Image.Type.Filled;
        _cd.fillAmount = 0;
        _icon = transform.Find("Image").GetComponent<Image>();
        _icon.sprite = _skill.SkillSprite;
        if (_skill.key != KeyCode.None)
        {
            // Test for mouse buttons
            if (_skill.key == KeyCode.Mouse0)
                transform.Find("Key").GetComponent<Text>().text = "LMBtn";

            else if (_skill.key == KeyCode.Mouse1)
                transform.Find("Key").GetComponent<Text>().text = "RMBtn";

            // Test for the Alphanumeric Keys strip on the keyboard. 48 => 0 and 57 => 9
            else if ((int)_skill.key >= 48 && (int)_skill.key <= 57)
                transform.Find("Key").GetComponent<Text>().text = ((int)_skill.key - 48).ToString();

            else
                transform.Find("Key").GetComponent<Text>().text = _skill.key.ToString();
        }

        if (_skill.cost != 0)
        {
            transform.Find("Cost").GetComponent<Text>().text = _skill.cost.ToString();
        }
        else
        {
            transform.Find("Cost").GetComponent<Text>().text = "";
        }

        if (_skill.UseCount != 0)
        {
            transform.Find("UseCount").GetComponent<Text>().text = _skill.UseCount.ToString();
        }
        else
        {
            transform.Find("UseCount").GetComponent<Text>().text = "";
        }

    }

    void Update()
    {
        _cd.fillAmount = 1 - ((Time.time - _skill.LastUse) / _skill.Cooldown);
        _cdBackground.fillAmount = 1 - ((Time.time - _skill.LastUse) / _skill.Cooldown);
        //if (!_skill.IsReady())
        //    _icon.enabled = false;
        //else
        //    _icon.enabled = true;

    }

    void SetActiveClick()
    {
        _skillBar.SetActiveSkill(_skill);
    }

    public void SetCooldown(bool active)
    {
        _cd.enabled = active;
    }

    public void SetVisibility(bool active)
    {
        gameObject.SetActive(active);
    }

    public void UpdateUseAmount()
    {
        if (_skill.UseCount != 0)
        {
            transform.Find("UseCount").GetComponent<Text>().text = _skill.UseCount.ToString();
        }
        else
        {
            transform.Find("UseCount").GetComponent<Text>().text = "";
        }
    }
}
