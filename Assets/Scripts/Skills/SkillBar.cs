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

    public bool HideInactiveSkills = false;
    public OnClickBehaviorType OnClickBehavior;

    private ISkill[] _skillList;
    private ISkill _activeSkill = null;

    void Awake()
    {
        _skillList = transform.parent.GetComponentsInChildren<ISkill>();
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
}
