using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;
using Data;

public class EffectBase : BaseObject
{
    public Creature Owner;
    public SkillBase Skill;
    public EffectData EffectData;
    public EffectTypes EffectType;

    protected float Remains { get; set; } = 0;
    protected EffectSpawnTypes _spawnType;
    protected bool Loop { get; set; } = true;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        return true;
    }

    public virtual void SetInfo(int templateID, Creature owner, EffectSpawnTypes spawnType, SkillBase skill)
    {
        DataTemplateID = templateID;
        EffectData = Managers.Data.EffectDic[templateID];

        Skill = skill;

        Owner = owner;
        _spawnType = spawnType;

        if (string.IsNullOrEmpty(EffectData.SkeletonDataID) == false)
            SetSpineAnimation(EffectData.SkeletonDataID, SortingLayers.SKILL_EFFECT);

        EffectType = EffectData.EffectType;

        // AoE
        if (_spawnType == EffectSpawnTypes.External)
            Remains = float.MaxValue;
        else
            Remains = EffectData.TickTime * EffectData.TickCount;

        // Duration = EffectData.TickTime * EffectData.TickCount;
        // Period = EffectData.TickTime;
    }

    public virtual void ApplyEffect()
    {
        ShowEffect();
        StartCoroutine(CoStartTimer());
        
        // 이펙트 처리 
        // 도트 힐? 도트 뎀? 패시브 영구적? => 서브타입을 들고잇을 수있음
    }

    protected virtual void ShowEffect()
    {
        if (SkeletonAnim != null && SkeletonAnim.skeletonDataAsset != null)
            PlayAnimation(0, AnimName.IDLE, Loop);
    }

    protected void AddModifier(CreatureStat stat, object source, int order = 0)
    {
        if (EffectData.Amount != 0)
        {
            StatModifier add = new StatModifier(EffectData.Amount, StatModTypes.Add, order, source);
            stat.AddModifier(add);
        }

        if (EffectData.PercentAdd != 0)
        {
            StatModifier percentAdd = new StatModifier(EffectData.PercentAdd, StatModTypes.PercentAdd, order, source);
            stat.AddModifier(percentAdd);
        }

        if (EffectData.PercentMult != 0)
        {
            StatModifier percentMult = new StatModifier(EffectData.PercentMult, StatModTypes.PercentMult, order, source);
            stat.AddModifier(percentMult);
        }
    }

    protected void RemoveModifier(CreatureStat stat, object source)
    {
        stat.ClearModifiersFromSource(source);
    }

    public virtual bool ClearEffect(EffectClearTypes clearType)
    {
        Debug.Log($"ClearEffect - {gameObject.name} {EffectData.ClassName} -> {clearType}");

        switch (clearType)
        {
            case EffectClearTypes.TimeOut:
            case EffectClearTypes.TriggerOutAoE:
            case EffectClearTypes.EndOfAirborne:
                Managers.Object.Despawn(this);
                return true;

            case EffectClearTypes.ClearSkill:
                // AoE범위 안에 있는경우 해제 X
                if (_spawnType != EffectSpawnTypes.External)
                {
                    Managers.Object.Despawn(this);
                    return true;
                }
                break;
        }

        return false;
    }

    protected virtual void ProcessDot()
    {

    }

    protected virtual IEnumerator CoStartTimer()
    {
        float sumTime = 0f;

        ProcessDot();

        while (Remains > 0)
        {
            Remains -= Time.deltaTime;
            sumTime += Time.deltaTime;

            // 틱마다 ProcessDotTick 호출
            if (sumTime >= EffectData.TickTime)
            {
                ProcessDot();
                sumTime -= EffectData.TickTime;
            }

            yield return null;
        }

        Remains = 0;
        ClearEffect(EffectClearTypes.TimeOut);
    }
}
