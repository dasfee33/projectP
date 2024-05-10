using System;
using System.Collections.Generic;
using Spine;
using UnityEngine;
using static Define;

public class Player : Creature
{
    bool needReturn = true;
    public bool NeedReturn
    {
        get { return needReturn; }
        set
        {
            needReturn = value;

            if (value)
                ChangeColliderSize(ColliderSizes.Big);
            else
                TryResizeCollider();
        }
    }

    public override CreatureStates CreatureState
    {
        get { return creatureState; }
        set
        {
            if (creatureState != value)
            {
                base.CreatureState = value;

                if (value == CreatureStates.Move)
                    RigidBody.mass = CreatureData.Mass;
                else
                    RigidBody.mass = CreatureData.Mass * 0.1f;
            }
        }
    }

    PlayerMoveStates _playerMoveState = PlayerMoveStates.None;
    public PlayerMoveStates PlayerMoveState
    {
        get { return _playerMoveState; }
        private set
        {
            _playerMoveState = value;
            switch (value)
            {
                case PlayerMoveStates.CollectEnv:
                    NeedReturn = true;
                    break;
                case PlayerMoveStates.TargetMonster:
                    NeedReturn = true;
                    break;
                case PlayerMoveStates.ForceMove:
                    NeedReturn = true;
                    break;
            }
        }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        CreatureType = CreatureTypes.Player;

        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;

        StartCoroutine(CoUpdateAI());

        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        CreatureState = CreatureStates.Idle;
    }

    public Transform PlayerCampDest
    {
        get
        {
            PlayerCamp camp = Managers.Object.Camp;
            if (PlayerMoveState == PlayerMoveStates.Return)
                return camp.Pivot;

            return camp.Destination;
        }
    }


    #region AI
    public float SearchDistance { get; private set; } = 8.0f;
    public float AttackDistance
    {
        get
        {
            float targetRadius = (_target.IsValid() ? _target.ColliderRadius : 0);
            return ColliderRadius + targetRadius + 1.0f;
        }
    }
    public float StopDistance { get; private set; } = 1.0f;
    BaseObject _target;

    protected override void UpdateIdle() 
    {
        // 이동 상태면 강제 변경
        if (PlayerMoveState == PlayerMoveStates.ForceMove)
        {
            CreatureState = CreatureStates.Move;
            return;
        }

        // 너무 멀어졌으면 강제 변경

        // 몬스터 있을때
        Creature creature = FindClosestInRange(SearchDistance, Managers.Object.Monsters) as Creature;
        if (creature != null)
        {
            _target = creature;
            CreatureState = CreatureStates.Move;
            PlayerMoveState = PlayerMoveStates.TargetMonster;
            return;
        }

        // 채집물 있을 때
        Env env = FindClosestInRange(SearchDistance, Managers.Object.Envs) as Env;
        if (env != null)
        {
            _target = env;
            CreatureState = CreatureStates.Move;
            PlayerMoveState = PlayerMoveStates.CollectEnv;
            return;
        }

        // 복귀하기
        if (NeedReturn)
        {
            CreatureState = CreatureStates.Move;
            PlayerMoveState = PlayerMoveStates.Return;
            return;
        }
    }

    protected override void UpdateMove() 
    {
        // 누르고 있다면, 강제 이동
        if (PlayerMoveState == PlayerMoveStates.ForceMove)
        {
            Vector3 dir = PlayerCampDest.position - transform.position;
            SetRigidBodyVelocity(dir.normalized * MoveSpeed);
            return;
        }

        // 주변 몬스터 서치
        if (PlayerMoveState == PlayerMoveStates.TargetMonster)
        {
            // 몬스터 죽었으면 포기.
            if (_target.IsValid() == false)
            {
                PlayerMoveState = PlayerMoveStates.None;
                CreatureState = CreatureStates.Move;
                return;
            }

            ChaseOrAttackTarget(AttackDistance, SearchDistance);
            return;
        }

        if (PlayerMoveState == PlayerMoveStates.CollectEnv)
        {
            // 몬스터가 있으면 포기.
            Creature creature = FindClosestInRange(SearchDistance, Managers.Object.Monsters) as Creature;
            if (creature != null)
            {
                _target = creature;
                PlayerMoveState = PlayerMoveStates.TargetMonster;
                CreatureState = CreatureStates.Move;
                return;
            }

            // Env 이미 채집했으면 포기.
            if (_target.IsValid() == false)
            {
                PlayerMoveState = PlayerMoveStates.None;
                CreatureState = CreatureStates.Move;
                return;
            }

            ChaseOrAttackTarget(AttackDistance, SearchDistance);
            return;
        }

        // 모이기
        if (PlayerMoveState == PlayerMoveStates.Return)
        {
            Vector3 dir = PlayerCampDest.position - transform.position;
            float stopDistanceSqr = StopDistance * StopDistance;
            if (dir.sqrMagnitude <= StopDistance)
            {
                PlayerMoveState = PlayerMoveStates.None;
                CreatureState = CreatureStates.Idle;
                NeedReturn = false;
                return;
            }
            else
            {
                // 멀리 있을 수록 빨라짐
                float ratio = Mathf.Min(1, dir.magnitude); // TEMP
                float moveSpeed = MoveSpeed * (float)Math.Pow(ratio, 3);
                SetRigidBodyVelocity(dir.normalized * moveSpeed);
                return;
            }
        }

        //누르다가 떼었을때
        CreatureState = CreatureStates.Idle;
    }

    protected override void UpdateSkill() 
    {
        if (PlayerMoveState == PlayerMoveStates.ForceMove)
        {
            CreatureState = CreatureStates.Move;
            return;
        }

        if (_target.IsValid() == false)
        {
            CreatureState = CreatureStates.Move;
            return;
        }
    }
    protected override void UpdateDead() 
    { 

    }

    void ChaseOrAttackTarget(float attackRange, float chaseRange)
    {
        Vector3 dir = (_target.transform.position - transform.position);
        float distToTargetSqr = dir.sqrMagnitude;
        float attackDistanceSqr = attackRange * attackRange;

        if (distToTargetSqr <= attackDistanceSqr)
        {
            // 공격 범위 이내로 들어왔다면 공격.
            CreatureState = CreatureStates.Skill;
            return;
        }
        else
        {
            // 공격 범위 밖이라면 추적.
            SetRigidBodyVelocity(dir.normalized * MoveSpeed);

            // 너무 멀어지면 포기.
            float searchDistanceSqr = chaseRange * chaseRange;
            if (distToTargetSqr > searchDistanceSqr)
            {
                _target = null;
                PlayerMoveState = PlayerMoveStates.None;
                CreatureState = CreatureStates.Move;
            }
            return;
        }
    }

    BaseObject FindClosestInRange(float range, IEnumerable<BaseObject> objs)
    {
        BaseObject target = null;
        float bestDistanceSqr = float.MaxValue;
        float searchDistanceSqr = range * range;

        foreach (BaseObject obj in objs)
        {
            Vector3 dir = obj.transform.position - transform.position;
            float distToTargetSqr = dir.sqrMagnitude;

            // 서치 범위보다 멀리 있으면 스킵.
            if (distToTargetSqr > searchDistanceSqr)
                continue;

            // 이미 더 좋은 후보를 찾았으면 스킵.
            if (distToTargetSqr > bestDistanceSqr)
                continue;

            target = obj;
            bestDistanceSqr = distToTargetSqr;
        }

        return target;
    }
    #endregion
    private void TryResizeCollider()
    {
        // 일단 충돌체 아주 작게.
        ChangeColliderSize(ColliderSizes.Small);

        foreach (var hero in Managers.Object.Players)
        {
            if (hero.PlayerMoveState == PlayerMoveStates.Return)
                return;
        }

        // Return이 한 명도 없으면 콜라이더 조정.
        foreach (var hero in Managers.Object.Players)
        {
            // 단 채집이나 전투중이면 스킵.
            if (hero.CreatureState == CreatureStates.Idle)
                hero.ChangeColliderSize(ColliderSizes.Big);
        }
    }

    private void HandleOnJoystickStateChanged(JoystickStates joystickState)
    {
        switch (joystickState)
        {
            case JoystickStates.PointerDown:
                PlayerMoveState = PlayerMoveStates.ForceMove;
                break;
            case JoystickStates.Drag:
                PlayerMoveState = PlayerMoveStates.ForceMove;
                break;
            case JoystickStates.PointerUp:
                PlayerMoveState = PlayerMoveStates.None;
                break;
            default:
                break;
        }
    }

    

    public override void OnAnimEventHandler(TrackEntry trackEntry, Spine.Event e)
    {
        base.OnAnimEventHandler(trackEntry, e);

        // TODO
        CreatureState = CreatureStates.Move;

        // Skill
        if (_target.IsValid() == false)
            return;

        _target.OnDamaged(this);
    }
}
