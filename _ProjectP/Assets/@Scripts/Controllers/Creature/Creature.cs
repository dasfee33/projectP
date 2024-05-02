using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Creature : BaseObject
{
    public float Speed { get; protected set; } = 1.0f;

    public CreatureTypes CreatureType { get; protected set; } = CreatureTypes.None;

    protected CreatureStates creatureState = CreatureStates.None;
    public virtual CreatureStates CreatureState
    {
        get { return creatureState; }
        set
        {
            if (creatureState != value)
            {
                creatureState = value;
                UpdateAnimation();
            }
        }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = ObjectTypes.Creature;
        CreatureState = CreatureStates.Idle;
        return true;
    }

    protected override void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case CreatureStates.Idle:
                PlayAnimation(0, AnimName.IDLE, true);
                break;
            case CreatureStates.Skill:
                PlayAnimation(0, AnimName.ATTACK_A, true);
                break;
            case CreatureStates.Move:
                PlayAnimation(0, AnimName.MOVE, true);
                break;
            case CreatureStates.Dead:
                PlayAnimation(0, AnimName.DEAD, true);
                RigidBody.simulated = false;
                break;
            default:
                break;
        }
    }
}
