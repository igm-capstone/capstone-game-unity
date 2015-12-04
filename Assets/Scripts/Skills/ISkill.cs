using UnityEngine;
using System.Collections;
using UnityEditor.AnimatedValues;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public abstract class ISkill : MonoBehaviour
{
    public string Name;
    public float Cooldown;

    private float _lastUse = float.MinValue;
    private Button _btn;
    private Image _fill;
    private Image _hl;

    public void Awake()
    {
        _btn = GetComponent<Button>();
        _fill = transform.Find("Fill").GetComponent<Image>();
        _hl = transform.Find("Highlight").GetComponent<Image>();

        _btn.onClick.AddListener(OnClick);
    }

    public void Update()
    {
        _fill.fillAmount = 1 - ((Time.time - _lastUse) / Cooldown);
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            foreach (var skill in transform.parent.GetComponentsInChildren<ISkill>())
            {
                skill.SetActive(false);
            }
        }
        _hl.enabled = active;
    }

    public bool IsActiveSkill()
    {
        return _hl.enabled;
    }

    public bool IsReady()
    {
        return (Time.time > _lastUse + Cooldown);
    }

    public void StartCooldown()
    {
        _lastUse = Time.time;
    }

    public abstract void OnClick();

}
