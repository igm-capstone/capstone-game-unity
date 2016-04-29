using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

public class SkillBar : MonoBehaviour
{
    public enum OnClickBehaviorType
    {
        SetActiveSkillOnClick,
        UseSkill,
        DoNothing
    }

    public int MaxSkills = 3;
    public int totalEnergy = 200;
    public bool HideInactiveSkills = false;
    public OnClickBehaviorType OnClickBehavior;
    public float waitTimeRestoreEnergy = 2;
    public float[] restoreAmount = { 0.20f, 0.23f, 0.26f, 0.30f };
    public int restoreAmountLevel = 0;
    public GameObject energyUiPrefab;

    private List<ISkill> _skillList;
    private ISkill _activeSkill = null;

    private int _skillCapacity;
    private float availableEnergy;
    private Image energyUIFill;
    private Text energyUiText;



    void Start()
    { 
        SetSkillCapacity(GetComponents<ISkill>().ToList().Count());
        SetSkillList(GetComponents<ISkill>().ToList());

        foreach (var skill in _skillList)
        {

            skill.enabled = !skill.canDrop;
            /* for EquippedSkill
            if (skill.isStaticSkill)
            {
                skill.enabled = !skill.canDrop;
            }
            */
        }
        availableEnergy = totalEnergy;
        StartCoroutine(RestoreEnergy());

        if (energyUiPrefab != null)
        {
            var energyTransform = Instantiate(energyUiPrefab).transform;
            energyTransform.SetParent(GameObject.Find("SkillBar").transform, false);
            energyTransform.SetAsFirstSibling();
            energyUiText = energyTransform.Find("Text").GetComponent<Text>();
            energyUiText.text = EnergyLeft.ToString();
            energyUIFill = energyTransform.Find("Fill").GetComponent<Image>();
            energyUIFill.fillAmount = 1;
        }
    }
    public void UpgradeRestoreRate()
    {
        restoreAmountLevel = Mathf.Clamp(restoreAmountLevel + 1, 0, restoreAmount.Length - 1);
    }

    public ISkill GetActiveSkill()
    {
        return _activeSkill;
    }

    public void SetActiveSkill(ISkill skill)
    {
        //HelpMessage.Instance.SetMessage(skill.Name + " skill active.");
        foreach (var s in _skillList)
        {
            if (HideInactiveSkills)
                s.SkillBtnScript.SetVisibility(s == skill);
            
            s.IsActive = (s == skill);
        }
        _activeSkill = skill;
    }

    public void SetSkillEnabled(string name, bool enabled)
    {
        //  List<ISkill> skills = _skillList.Where(s => s.Name == name) as List<ISkill>;

        foreach (ISkill s in _skillList)
        {
            if (s.canDrop)
            {
                s.enabled = (s.Name == name);
            }
        }
    }

    public void SetSkillList(List<ISkill> skillList)
    {
        if (skillList.Count() <= _skillCapacity)
        {
            _skillList = skillList;
        }
    }

    public void ClearSkillList()
    {
        _skillList.Clear();
    }

    public void AddSkill(ISkill skill)
    {
        if (_skillList.Count < _skillCapacity)
        {
            _skillList.Add(skill);
        }
    }

    public ISkill GetSkill(int index)
    {
        return _skillList[index];
    }

    public void RemoveSkill(ISkill skill)
    {
        _skillList.Remove(skill);
    }

    public int GetSkillCount()
    {
        return _skillList.Count();
    }

    public int GetSkillCapacity()
    {
        return _skillCapacity;
    }

    public void SetSkillCapacity(int capacity)
    {
        _skillCapacity = capacity;
    }

    public bool IsFull()
    {
        return _skillList.Count() == _skillCapacity;
    }

    public int EnergyLeft
    {
        get { return Mathf.RoundToInt( availableEnergy); }
        set
        {
            availableEnergy = value;
            energyUiText.text = EnergyLeft.ToString();
        }
    }

    IEnumerator RestoreEnergy()
    {
        while (true)
        {
            yield return null;
            if (availableEnergy < totalEnergy)
            {
                availableEnergy = availableEnergy + restoreAmount[restoreAmountLevel] * (Time.deltaTime / waitTimeRestoreEnergy);
                availableEnergy = Mathf.Clamp(availableEnergy, 0, totalEnergy);
                energyUiText.text = EnergyLeft.ToString();
                energyUIFill.fillAmount = availableEnergy / totalEnergy;
            }
        }
    }
}
