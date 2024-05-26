using System;
using Data;
using UnityEngine;
using static Define;

public class BuffBase : EffectBase
{
	public override bool Init()
    {
        if (base.Init() == false)
            return false;

        EffectType = EffectTypes.Buff;
        return true;
    }

    public override void SetInfo(int templateID, Creature owner, EffectSpawnTypes spawnType, SkillBase skill)
    {
        base.SetInfo(templateID, owner, spawnType, skill);

        if (EffectData.Amount < 0 || EffectData.PercentAdd < 0)
        {
            EffectType = EffectTypes.Debuff;
        }
    }
}