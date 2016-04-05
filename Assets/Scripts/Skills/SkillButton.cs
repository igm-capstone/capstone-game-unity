using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    public struct HSBColor
    {
        public float h;
        public float s;
        public float b;
        public float a;
    }


    public ISkill _skill;
    public SkillBar _skillBar;
    
    private Button _btn;
    private Image _cdBackground;
    private Image _skillBG;
    private Image _cd;
    private Image _icon;

    //ToolTip
    private GameObject toolTipObj;
    private Text toolTipDscrpt;
    private Text toolTipAtkAtt;
    private Text toolTipDefAtt;
    private Text toolTipSpdAtt;

    public void Init()
    {
        _btn = GetComponent<Button>();

        if (_skillBar.OnClickBehavior == SkillBar.OnClickBehaviorType.SetActiveSkillOnClick)
            _btn.onClick.AddListener(SetActiveClick);
        else if (_skillBar.OnClickBehavior == SkillBar.OnClickBehaviorType.UseSkill)
            _btn.onClick.AddListener(() => _skill.Use());
        else
            _btn.interactable = false;

        _cdBackground = transform.Find("CooldownBG").GetComponent<Image>();
        _cd = transform.Find("Cooldown").GetComponent<Image>();
        _cd.sprite = _skill.CooldownSprite;
        _cd.type = Image.Type.Filled;
        _cd.fillAmount = 0;
        _icon = transform.Find("Image").GetComponent<Image>();
        _icon.sprite = _skill.SkillSprite;
        _skillBG = transform.Find("SkillBG").GetComponent<Image>();
        if (_icon.sprite)
        {
            _skillBG.color = GetAverageColor(_icon.sprite, 1.2f, 0.8f);//  _skill.bgColor;
        }

        if (_skill.key != KeyCode.None)
        {
            // Test for mouse buttons
            if (_skill.key == KeyCode.Mouse0)
                transform.Find("Key").GetComponent<Text>().text = "LMB";

            else if (_skill.key == KeyCode.Mouse1)
                transform.Find("Key").GetComponent<Text>().text = "RMB";

            else if (_skill.key == KeyCode.LeftShift)
                transform.Find("Key").GetComponent<Text>().text = "LShift";

            else if (_skill.key == KeyCode.LeftControl)
                transform.Find("Key").GetComponent<Text>().text = "LCtrl";

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

        if (_skill.Name != null)
        {
            transform.Find("SkillName").GetComponent<Text>().text = _skill.Name;
        }
        else
        {
            transform.Find("SkillName").GetComponent<Text>().text = "";
        }

        // finds toolTip and Disables it if it is enabled.
        toolTipObj = transform.Find("ToolTip").gameObject;
        ShowToolTip(false);

        // Gets Reference to the ToolTip texts.
        toolTipDscrpt = toolTipObj.transform.FindChild("Description").gameObject.GetComponent<Text>();

        toolTipAtkAtt = toolTipObj.transform.FindChild("AttackAttribute").gameObject.GetComponent<Text>(); ;

        toolTipDefAtt = toolTipObj.transform.FindChild("DefenseAttribute").gameObject.GetComponent<Text>(); ;

        toolTipSpdAtt = toolTipObj.transform.FindChild("SpeedAtribute").gameObject.GetComponent<Text>();

        toolTipDscrpt.text = _skill.ToolTip.Description;
        toolTipAtkAtt.text = _skill.ToolTip.AtkAttrbt;
        toolTipDefAtt.text = _skill.ToolTip.DefAttrbt;
        toolTipSpdAtt.text = _skill.ToolTip.SpdAttrbt;

    }

    private Color GetAverageColor(Sprite sprite, float saturation, float brightness)
    {
        var rect = sprite.rect;
        Color[] pixels;

        try
        {
            pixels = sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
        }
        catch (Exception)
        {
            Debug.LogWarning("Skill sprite is not readable. Please check 'Read/Write Enabled' option on the asset import settings.");
            return Color.gray;
        }

        var opaque = pixels.Where(c => c.a > 0);
        Color color = opaque.Aggregate(Color.black, (acc, curr) => acc + curr) / (float)opaque.Count();
        color.a = 1;

        // get hsb values
        HSBColor hsb = FromColor(color);

        // apply brightness and saturation
        hsb.s *= saturation;
        hsb.b *= brightness;

        return ToColor(hsb);
    }

    public static HSBColor FromColor(Color color)
    {
        HSBColor ret = new HSBColor { a = color.a };

        float r = color.r;
        float g = color.g;
        float b = color.b;

        float max = Mathf.Max(r, Mathf.Max(g, b));

        if (max <= 0)
        {
            return ret;
        }

        float min = Mathf.Min(r, Mathf.Min(g, b));
        float dif = max - min;

        if (max > min)
        {
            if (g == max)
            {
                ret.h = (b - r) / dif * 60f + 120f;
            }
            else if (b == max)
            {
                ret.h = (r - g) / dif * 60f + 240f;
            }
            else if (b > g)
            {
                ret.h = (g - b) / dif * 60f + 360f;
            }
            else
            {
                ret.h = (g - b) / dif * 60f;
            }
            if (ret.h < 0)
            {
                ret.h = ret.h + 360f;
            }
        }
        else
        {
            ret.h = 0;
        }

        ret.h *= 1f / 360f;
        ret.s = (dif / max) * 1f;
        ret.b = max;

        return ret;
    }

    public static Color ToColor(HSBColor hsbColor)
    {
        float r = hsbColor.b;
        float g = hsbColor.b;
        float b = hsbColor.b;
        if (hsbColor.s != 0)
        {
            float max = hsbColor.b;
            float dif = hsbColor.b * hsbColor.s;
            float min = hsbColor.b - dif;

            float h = hsbColor.h * 360f;

            if (h < 60f)
            {
                r = max;
                g = h * dif / 60f + min;
                b = min;
            }
            else if (h < 120f)
            {
                r = -(h - 120f) * dif / 60f + min;
                g = max;
                b = min;
            }
            else if (h < 180f)
            {
                r = min;
                g = max;
                b = (h - 120f) * dif / 60f + min;
            }
            else if (h < 240f)
            {
                r = min;
                g = -(h - 240f) * dif / 60f + min;
                b = max;
            }
            else if (h < 300f)
            {
                r = (h - 240f) * dif / 60f + min;
                g = min;
                b = max;
            }
            else if (h <= 360f)
            {
                r = max;
                g = min;
                b = -(h - 360f) * dif / 60 + min;
            }
            else
            {
                r = 0;
                g = 0;
                b = 0;
            }
        }

        return new Color(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b), hsbColor.a);
    }

    void Update()
    {
        _cd.fillAmount = 1 - ((Time.time - _skill.LastUse) / _skill.Cooldown);
        _cdBackground.fillAmount = 1 - ((Time.time - _skill.LastUse) / _skill.Cooldown);

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

    // Gets Called by Event Handler. Show/Hide Tooltip.
    public void ShowToolTip(bool mustShow)
    {
        toolTipObj.SetActive(mustShow);
    }
}
