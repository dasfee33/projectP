using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Rendering;
using static Define;

public class Creature : BaseObject
{
    public BaseObject Target { get; protected set; }
    public SkillComponent Skills { get; protected set; }

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

        if (CreatureType == CreatureTypes.Player)
            CreatureData = Managers.Data.PlayerDic[templateID];
        else
            CreatureData = Managers.Data.MonsterDic[templateID];

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

        // Spine SkeletonAnimation�� SpriteRenderer �� ������� �ʰ� MeshRenderer�� �����.
        // �׷��⋚���� 2D Sort Axis�� �ȸ����� �Ǵµ� SortingGroup�� SpriteRenderer, MeshRenderer������ �����.
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

        // Map
        StartCoroutine(CoLerpToCellPos());
    }

    protected override void UpdateAnimation()
    {
        switch (CreatureState)
        {
            case CreatureStates.Idle:
                PlayAnimation(0, AnimName.IDLE, true);
                break;
            case CreatureStates.Skill:
                //PlayAnimation(0, AnimName.ATTACK_A, true);
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

    #region Battle
    public override void OnDamaged(BaseObject attacker, SkillBase skill)
    {
        base.OnDamaged(attacker, skill);

        if (attacker.IsValid() == false)
            return;

        Creature creature = attacker as Creature;
        if (creature == null)
            return;

        float finalDamage = creature.Atk; // TODO
        Hp = Mathf.Clamp(Hp - finalDamage, 0, MaxHp);

        if (Hp <= 0)
        {
            OnDead(attacker, skill);
            CreatureState = CreatureStates.Dead;
        }
    }

    public override void OnDead(BaseObject attacker, SkillBase skill)
    {
        base.OnDead(attacker, skill);


    }

    protected void ChaseOrAttackTarget(float chaseRange, SkillBase skill)
    {
        Vector3 dir = (Target.transform.position - transform.position);
        float distToTargetSqr = dir.sqrMagnitude;

        // TEMP
        float attackRange = HERO_DEFAULT_MELEE_ATTACK_RANGE;
        if (skill.SkillData.ProjectileId != 0)
            attackRange = HERO_DEFAULT_RANGED_ATTACK_RANGE;

        float finalAttackRange = attackRange + Target.ColliderRadius + ColliderRadius;
        float attackDistanceSqr = finalAttackRange * finalAttackRange;

        if (distToTargetSqr <= attackDistanceSqr)
        {
            // ���� ���� �̳��� ���Դٸ� ����.
            CreatureState = CreatureStates.Skill;
            skill.DoSkill();
            return;
        }
        else
        {
            // ���� ���� ���̶�� ����.
            FindPathAndMoveToCellPos(Target.transform.position, HERO_DEFAULT_MOVE_DEPTH);

            // �ʹ� �־����� ����.
            float searchDistanceSqr = chaseRange * chaseRange;
            if (distToTargetSqr > searchDistanceSqr)
            {
                Target = null;
                CreatureState = CreatureStates.Move;
            }
            return;
        }
    }

    //protected void ChaseOrAttackTarget(float attackRange, float chaseRange)
    //{
    //    Vector3 dir = (Target.transform.position - transform.position);
    //    float distToTargetSqr = dir.sqrMagnitude;
    //    float attackDistanceSqr = attackRange * attackRange;

    //    if (distToTargetSqr <= attackDistanceSqr)
    //    {
    //        // ���� ���� �̳��� ���Դٸ� ����.
    //        CreatureState = CreatureStates.Skill;
    //        return;
    //    }
    //    else
    //    {
    //        // ���� ���� ���̶�� ����.
    //        SetRigidBodyVelocity(dir.normalized * MoveSpeed);

    //        // �ʹ� �־����� ����.
    //        float searchDistanceSqr = chaseRange * chaseRange;
    //        if (distToTargetSqr > searchDistanceSqr)
    //        {
    //            Target = null;
    //            CreatureState = CreatureStates.Move;
    //        }
    //        return;
    //    }
    //}

    protected BaseObject FindClosestInRange(float range, IEnumerable<BaseObject> objs, Func<BaseObject, bool> func = null)
    {
        BaseObject target = null;
        float bestDistanceSqr = float.MaxValue;
        float searchDistanceSqr = range * range;

        foreach (BaseObject obj in objs)
        {
            Vector3 dir = obj.transform.position - transform.position;
            float distToTargetSqr = dir.sqrMagnitude;

            // ��ġ �������� �ָ� ������ ��ŵ.
            if (distToTargetSqr > searchDistanceSqr)
                continue;

            // �̹� �� ���� �ĺ��� ã������ ��ŵ.
            if (distToTargetSqr > bestDistanceSqr)
                continue;

            // �߰� ����
            if (func != null && func.Invoke(obj) == false)
                continue;

            target = obj;
            bestDistanceSqr = distToTargetSqr;
        }

        return target;
    }
    #endregion

    #region Add
    protected bool IsValid(BaseObject bo)
    {
        return bo.IsValid();
    }
    #endregion

    #region Map
    public FindPathResults FindPathAndMoveToCellPos(Vector3 destWorldPos, int maxDepth, bool forceMoveCloser = false)
    {
        Vector3Int destCellPos = Managers.Map.World2Cell(destWorldPos);
        return FindPathAndMoveToCellPos(destCellPos, maxDepth, forceMoveCloser);
    }

    public FindPathResults FindPathAndMoveToCellPos(Vector3Int destCellPos, int maxDepth, bool forceMoveCloser = false)
    {
        if (LerpCellPosCompleted == false)
            return FindPathResults.Fail_LerpCell;

        // A*
        //List<Vector3Int> path = Managers.Map.FindPath(CellPos, destCellPos, maxDepth);
        //if (path.Count < 2)
        //    return FindPathResults.Fail_NoPath;

        //if (forceMoveCloser)
        //{
        //    Vector3Int diff1 = CellPos - destCellPos;
        //    Vector3Int diff2 = path[1] - destCellPos;
        //    if (diff1.sqrMagnitude <= diff2.sqrMagnitude)
        //        return FindPathResults.Fail_NoPath;
        //}

        //Vector3Int dirCellPos = path[1] - CellPos;
        Vector3Int dirCellPos = destCellPos - CellPos;
        Vector3Int nextPos = CellPos + dirCellPos;

        if (Managers.Map.MoveTo(this, nextPos) == false)
            return FindPathResults.Fail_MoveTo;

        return FindPathResults.Success;
    }

    public bool MoveToCellPos(Vector3Int destCellPos, int maxDepth, bool forceMoveCloser = false)
    {
        if (LerpCellPosCompleted == false)
            return false;

        return Managers.Map.MoveTo(this, destCellPos);
    }

    protected IEnumerator CoLerpToCellPos()
    {
        while (true)
        {
            Player hero = this as Player;
            if (hero != null)
            {
                float div = 5;
                Vector3 campPos = Managers.Object.Camp.Destination.transform.position;
                Vector3Int campCellPos = Managers.Map.World2Cell(campPos);
                float ratio = Math.Max(1, (CellPos - campCellPos).magnitude / div);

                LerpToCellPos(CreatureData.MoveSpeed * ratio);
            }
            else
                LerpToCellPos(CreatureData.MoveSpeed);

            yield return null;
        }
    }
    #endregion

}
