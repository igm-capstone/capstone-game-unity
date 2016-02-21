using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;

public class SkillBar : MonoBehaviour
{
    public enum OnClickBehaviorType
    {
        SetActiveSkillOnClick,
        UseSkill,
        DoNothing
    }

    public int MaxSkills = 3;
    public int totalEnergy = 100;
    public bool HideInactiveSkills = false;
    public OnClickBehaviorType OnClickBehavior;
    public int waitTimeRestoreEnergy = 2;
    public int restoreAmount = 5;
    public GameObject energyUiPrefab;

    private List<ISkill> _skillList;
    private ISkill _activeSkill = null;
    private int _skillCapacity;
    private int availableEnergy;
    private Text energyUiText;



    void Start()
    { 
        SetSkillCapacity(GetComponents<ISkill>().ToList().Count());
        SetSkillList(GetComponents<ISkill>().ToList());

        foreach (var skill in _skillList)
        {
            skill.enabled = !skill.canDrop;
        }
        availableEnergy = totalEnergy;
        StartCoroutine(RestoreEnergy(restoreAmount));

        if (energyUiPrefab != null)
        {
            var energyTransform = Instantiate(energyUiPrefab).transform;
            energyTransform.SetParent(GameObject.Find("SkillBar").transform, false);
            energyTransform.SetAsFirstSibling();
            energyUiText = energyTransform.Find("Text").GetComponent<Text>();
            energyUiText.text = EnergyLeft.ToString();
        }
    }

    void Update()
    {
        Debug.Log("availableEnergy" + availableEnergy);
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

    public int EnergyLeft
    {
        get { return availableEnergy; }
        set
        {
            availableEnergy = value;
            energyUiText.text = availableEnergy.ToString();

        }
    }

    IEnumerator RestoreEnergy(int restoreValuePerSecond)
    {
        while (true)
        {
            yield return new WaitForSeconds(waitTimeRestoreEnergy);
            if (availableEnergy < totalEnergy)
            {
                availableEnergy += restoreValuePerSecond;
                energyUiText.text = availableEnergy.ToString();
            }
        }
    }
}
