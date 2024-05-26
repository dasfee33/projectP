using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaAirbone : AreaSkill
{
    public override void SetInfo(Creature owner, int skillTemplateID)
    {
        base.SetInfo(owner, skillTemplateID);

        _angleRange = 360;

        if (_indicator != null)
            _indicator.SetInfo(Owner, SkillData, Define.IndicatorTypes.Cone);

        _indicatorType = Define.IndicatorTypes.Cone;
    }

    public override void DoSkill()
    {
        base.DoSkill();
    }
}
