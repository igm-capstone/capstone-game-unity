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

    void Start()
   { 
        _skillList = GetComponents<ISkill>();

        foreach (var skill in _skillList)
        {
            skill.enabled = true;
        }
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
                s.SkillBtnScript.SetVisibility(s == skill);
            
            if (OnClickBehavior == OnClickBehaviorType.SetActiveSkillOnClick)
                s.SkillBtnScript.SetHighlight(s == skill);
        }
        _activeSkill = skill;
    }
}
