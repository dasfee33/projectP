using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Data;
using static Define;

public class SkillComponent : InitBase
{
    public List<SkillBase> SkillList { get; } = new List<SkillBase>();
    public List<SkillBase> ActiveSkills { get; set; } = new List<SkillBase>();

    public SkillBase DefaultSkill { get; private set; }
    public SkillBase EnvSkill { get; private set; }
    public SkillBase ASkill { get; private set; }
    public SkillBase BSkill { get; private set; }

    public SkillBase CurrentSkill
    {
        get
        {
            if (ActiveSkills.Count == 0)
                return DefaultSkill;

            int randomIndex = UnityEngine.Random.Range(0, ActiveSkills.Count);
            return ActiveSkills[randomIndex];
        }
    }

    Creature _owner;

    public override bool Init()
    {
        if (base.Init() is false) return false;

        return true;
    }

    public void SetInfo(Creature owner, CreatureData creatureData)
    {
        _owner = owner;

        AddSkill(creatureData.DefaultSkill, SkillSlots.Default);
        AddSkill(creatureData.EnvSkill, SkillSlots.Env);
        AddSkill(creatureData.SkillA, SkillSlots.A);
        AddSkill(creatureData.SkillB, SkillSlots.B);
    }

    public void AddSkill(int skillTemplateID, Define.SkillSlots skillSlot)
    {
        if (skillTemplateID == 0)
            return;

        if(Managers.Data.SkillDic.TryGetValue(skillTemplateID, out var data) == false)
        {
            Debug.LogWarning($"Add Skill Failed {skillTemplateID}");
            return;
        }

        SkillBase skill = gameObject.AddComponent(Type.GetType(data.ClassName)) as SkillBase;
        if (skill == null)
            return;

        skill.SetInfo(_owner, skillTemplateID);

        SkillList.Add(skill);

        switch (skillSlot)
        {
            case Define.SkillSlots.Default:
                DefaultSkill = skill;
                break;
            case Define.SkillSlots.Env:
                EnvSkill = skill;
                break;
            case Define.SkillSlots.A:
                ASkill = skill;
                ActiveSkills.Add(skill);
                break;
            case Define.SkillSlots.B:
                BSkill = skill;
                ActiveSkills.Add(skill);
                break;
        }
    }

}
