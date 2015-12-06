using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;

public class SkillBar : MonoBehaviour
{
    public enum OnClickBehaviorType
    {
        SetActiveSkillOnClick,
        UseSkill
    }

    public int MaxSkills;
    public bool HideInactiveSkills = false;
    public OnClickBehaviorType OnClickBehavior;

    private List<ISkill> _skillList;
    private ISkill _activeSkill = null;
    private int _skillCapacity;

    void Awake()
    {
        SetSkillCapacity(MaxSkills);
        SetSkillList(transform.parent.GetComponentsInChildren<ISkill>().ToList<ISkill>());
    }

    public ISkill GetActiveSkill()
    {
        return _activeSkill;
    }

    public void SetActiveSkill(ISkill skill)
    {
        foreach (var s in _skillList)
        {
            if (HideInactiveSkills)
                s.SetVisibility(s == skill);
            else
                s.SetHighlight(s == skill);
        }
        _activeSkill = skill;
    }

    public void SetSkillList(List<ISkill> skillList)
    {
        if (skillList.Count() < _skillCapacity)
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
}
