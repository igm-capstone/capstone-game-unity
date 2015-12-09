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
        UseSkill,
        DoNothing
    }

    public int MaxSkills = 3;
    public bool HideInactiveSkills = false;
    public OnClickBehaviorType OnClickBehavior;

    private List<ISkill> _skillList;
    private ISkill _activeSkill = null;
    private int _skillCapacity;

    void Start()
   { 
        SetSkillCapacity(GetComponents<ISkill>().ToList().Count());
        SetSkillList(GetComponents<ISkill>().ToList());

        foreach (var skill in _skillList)
        {
            skill.enabled = !skill.canDrop;
        }
    }


    public ISkill GetActiveSkill()
    {
        return _activeSkill;
    }

    public void SetActiveSkill(ISkill skill)
    {
        HelpMessage.Instance.SetMessage(skill.Name + " skill active.");
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
}
