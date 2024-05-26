using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class CCBase : EffectBase
{
	protected CreatureStates lastState;

	public override bool Init()
	{
		if (base.Init() == false)
			return false;

		EffectType = EffectTypes.CrowdControl;
		return true;
	}

	public override void ApplyEffect()
	{
		base.ApplyEffect();

		lastState = Owner.CreatureState;
		if (lastState == CreatureStates.OnDamaged)
			return;

		Owner.CreatureState = CreatureStates.OnDamaged;
	}

	public override bool ClearEffect(EffectClearTypes clearType)
	{
		if (base.ClearEffect(clearType) == true)
			Owner.CreatureState = lastState;

		return true;
	}

}