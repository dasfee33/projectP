using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;

public class Creature : BaseObject
{
    public Data.CreatureData CreatureData { get; protected set; }
    public CreatureTypes CreatureType { get; protected set; } = CreatureTypes.None;

    #region Stats
    public float Hp { get; set; }
    public float MaxHp { get; set; }
    public float MaxHpBonusRate { get; set; }
    public float HealBonusRate { get; set; }
    public float HpRegen { get; set; }
    public float Atk { get; set; }
    public float AttackRate { get; set; }
    public float Def { get; set; }
    public float DefRate { get; set; }
    public float CriRate { get; set; }
    public float CriDamage { get; set; }
    public float DamageReduction { get; set; }
    public float MoveSpeedRate { get; set; }
    public float MoveSpeed { get; set; }
    #endregion

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

        return true;
    }

    public virtual void SetInfo(int templateID)
    {
        DataTemplateID = templateID;

        CreatureData = Managers.Data.CreatureDic[templateID];
        gameObject.name = $"{CreatureData.DataId}_{CreatureData.DescriptionTextID}";

        // Collider
        Collider.offset = new Vector2(CreatureData.ColliderOffsetX, CreatureData.ColliderOffstY);
        Collider.radius = CreatureData.ColliderRadius;

        // RigidBody
        RigidBody.mass = CreatureData.Mass;

        // Spine
        SkeletonAnim.skeletonDataAsset = Managers.Resource.Load<SkeletonDataAsset>(CreatureData.SkeletonDataID);
        SkeletonAnim.Initialize(true);

        // Register AnimEvent
        if (SkeletonAnim.AnimationState != null)
        {
            SkeletonAnim.AnimationState.Event -= OnAnimEventHandler;
            SkeletonAnim.AnimationState.Event += OnAnimEventHandler;
        }

        // Spine SkeletonAnimation은 SpriteRenderer 를 사용하지 않고 MeshRenderer을 사용함.
        // 그렇기떄문에 2D Sort Axis가 안먹히게 되는데 SortingGroup을 SpriteRenderer, MeshRenderer을같이 계산함.
        SortingGroup sg = Util.GetOrAddComponent<SortingGroup>(gameObject);
        sg.sortingOrder = SortingLayers.CREATURE;

        // Skills
        // CreatureData.SkillIdList;

        // Stat
        MaxHp = CreatureData.MaxHp;
        Hp = CreatureData.MaxHp;
        Atk = CreatureData.MaxHp;
        MaxHp = CreatureData.MaxHp;
        MoveSpeed = CreatureData.MoveSpeed;

        // State
        CreatureState = CreatureStates.Idle;

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

    public void ChangeColliderSize(ColliderSizes size = ColliderSizes.Normal)
    {
        switch (size)
        {
            case ColliderSizes.Small:
                Collider.radius = CreatureData.ColliderRadius * 0.8f;
                break;
            case ColliderSizes.Normal:
                Collider.radius = CreatureData.ColliderRadius;
                break;
            case ColliderSizes.Big:
                Collider.radius = CreatureData.ColliderRadius * 1.2f;
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

    #region Battle
    public override void OnDamaged(BaseObject attacker)
    {
        base.OnDamaged(attacker);

        if (attacker.IsValid() == false)
            return;

        Creature creature = attacker as Creature;
        if (creature == null)
            return;

        float finalDamage = creature.Atk; // TODO
        Hp = Mathf.Clamp(Hp - finalDamage, 0, MaxHp);

        if (Hp <= 0)
        {
            OnDead(attacker);
            CreatureState = CreatureStates.Dead;
        }
    }

    public override void OnDead(BaseObject attacker)
    {
        base.OnDead(attacker);


    }
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
