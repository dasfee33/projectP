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

    #region AI
    public float UpdateAITick { get; protected set; } = 0.0f;

    protected IEnumerator CoUpdateAI()
    {
        while (true)
        {
            switch (CreatureState)
            {
                case CreatureStates.Idle:
                    UpdateIdle();
                    break;
                case CreatureStates.Move:
                    UpdateMove();
                    break;
                case CreatureStates.Skill:
                    UpdateSkill();
                    break;
                case CreatureStates.Dead:
                    UpdateDead();
                    break;
            }

            if (UpdateAITick > 0)
                yield return new WaitForSeconds(UpdateAITick);
            else
                yield return null;
        }
    }
    protected virtual void UpdateIdle() { }
    protected virtual void UpdateMove() { }
    protected virtual void UpdateSkill() { }
    protected virtual void UpdateDead() { }
    #endregion

    #region Wait
    protected Coroutine _coWait;

    protected void StartWait(float seconds)
    {
        CancelWait();
        _coWait = StartCoroutine(CoWait(seconds));
    }

    IEnumerator CoWait(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _coWait = null;
    }

    protected void CancelWait()
    {
        if (_coWait != null)
            StopCoroutine(_coWait);
        _coWait = null;
    }
    #endregion
}
