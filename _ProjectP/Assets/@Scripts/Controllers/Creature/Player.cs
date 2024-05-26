using System;
using System.Collections.Generic;
using Spine;
using UnityEngine;
using static Define;

public class Player : Creature
{
    public bool NeedReturn { get; set; }

    public override CreatureStates CreatureState
    {
        get { return creatureState; }
        set
        {
            if (creatureState != value)
            {
                base.CreatureState = value;

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

        ObjectType = ObjectTypes.Player;

        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;

        //Map
        Collider.isTrigger = true;
        RigidBody.simulated = false;

        StartCoroutine(CoUpdateAI());

        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        CreatureState = CreatureStates.Idle;

        // Skill
        Skills = gameObject.GetOrAddComponent<SkillComponent>();
        Skills.SetInfo(this, CreatureData);
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
    protected override void UpdateIdle() 
    {
        // 이동 상태면 강제 변경
        if (PlayerMoveState == PlayerMoveStates.ForceMove)
        {
            CreatureState = CreatureStates.Move;
            return;
        }

        // 몬스터 있을때
        Creature creature = FindClosestInRange(HERO_SEARCH_DISTANCE, Managers.Object.Monsters) as Creature;
        if (creature != null)
        {
            Target = creature;
            CreatureState = CreatureStates.Move;
            PlayerMoveState = PlayerMoveStates.TargetMonster;
            return;
        }

        // 채집물 있을 때
        Env env = FindClosestInRange(HERO_SEARCH_DISTANCE, Managers.Object.Envs) as Env;
        if (env != null)
        {
            Target = env;
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
        if(PlayerMoveState == PlayerMoveStates.ForcePath)
        {
            MoveByForcePath();
            return;
        }

        if (CheckPlayerCampDistanceAndForcePath()) return;

        // 누르고 있다면, 강제 이동
        if (PlayerMoveState == PlayerMoveStates.ForceMove)
        {
            FindPathResults result = FindPathAndMoveToCellPos(PlayerCampDest.position, HERO_DEFAULT_MOVE_DEPTH);
            return;
        }

        // 주변 몬스터 서치
        if (PlayerMoveState == PlayerMoveStates.TargetMonster)
        {
            // 몬스터 죽었으면 포기.
            if (Target.IsValid() == false)
            {
                PlayerMoveState = PlayerMoveStates.None;
                CreatureState = CreatureStates.Move;
                return;
            }

            //ChaseOrAttackTarget(AttackDistance, HERO_SEARCH_DISTANCE);
            ChaseOrAttackTarget(HERO_SEARCH_DISTANCE, AttackDistance);
            return;
        }

        if (PlayerMoveState == PlayerMoveStates.CollectEnv)
        {
            // 몬스터가 있으면 포기.
            Creature creature = FindClosestInRange(HERO_SEARCH_DISTANCE, Managers.Object.Monsters) as Creature;
            if (creature != null)
            {
                Target = creature;
                PlayerMoveState = PlayerMoveStates.TargetMonster;
                CreatureState = CreatureStates.Move;
                return;
            }

            // Env 이미 채집했으면 포기.
            if (Target.IsValid() == false)
            {
                PlayerMoveState = PlayerMoveStates.None;
                CreatureState = CreatureStates.Move;
                return;
            }

            ChaseOrAttackTarget(HERO_SEARCH_DISTANCE, AttackDistance);
            return;
        }

        // 모이기
        if (PlayerMoveState == PlayerMoveStates.Return)
        {
            Vector3 destPos = PlayerCampDest.position;
            if (FindPathAndMoveToCellPos(destPos, HERO_DEFAULT_MOVE_DEPTH) == FindPathResults.Success)
                return;

            // 실패 사유 검사.
            BaseObject obj = Managers.Map.GetObject(destPos);
            if (obj.IsValid())
            {
                // 내가 그 자리를 차지하고 있다면
                if (obj == this)
                {
                    PlayerMoveState = PlayerMoveStates.None;
                    NeedReturn = false;
                    return;
                }

                // 다른 영웅이 멈춰있다면.
                Player player = obj as Player;
                if (player != null && player.CreatureState == CreatureStates.Idle)
                {
                    PlayerMoveState = PlayerMoveStates.None;
                    NeedReturn = false;
                    return;
                }
            }
        }
        //누르다가 떼었을때
        if(LerpCellPosCompleted)
            CreatureState = CreatureStates.Idle;
    }

    Queue<Vector3Int> _forcePath = new Queue<Vector3Int>();

    bool CheckPlayerCampDistanceAndForcePath()
    {
        // 너무 멀어서 못 간다.
        Vector3 destPos = PlayerCampDest.position;
        Vector3Int destCellPos = Managers.Map.World2Cell(destPos);
        if ((CellPos - destCellPos).magnitude <= 10)
            return false;

        if (Managers.Map.CanGo(this, destCellPos, ignoreObjects: true) == false)
            return false;

        List<Vector3Int> path = Managers.Map.FindPath(this, CellPos, destCellPos, 100);
        if (path.Count < 2)
            return false;

        PlayerMoveState = PlayerMoveStates.ForcePath;

        _forcePath.Clear();
        foreach (var p in path)
        {
            _forcePath.Enqueue(p);
        }
        _forcePath.Dequeue();

        return true;
    }

    void MoveByForcePath()
    {
        if (_forcePath.Count == 0)
        {
            PlayerMoveState = PlayerMoveStates.None;
            return;
        }

        Vector3Int cellPos = _forcePath.Peek();

        if (MoveToCellPos(cellPos, 2))
        {
            _forcePath.Dequeue();
            return;
        }

        // 실패 사유가 영웅이라면.
        Player player = Managers.Map.GetObject(cellPos) as Player;
        if (player != null && player.CreatureState == CreatureStates.Idle)
        {
            PlayerMoveState = PlayerMoveStates.None;
            return;
        }
    }

    protected override void UpdateSkill() 
    {
        base.UpdateSkill();

        if (PlayerMoveState == PlayerMoveStates.ForceMove)
        {
            CreatureState = CreatureStates.Move;
            return;
        }

        if (Target.IsValid() == false)
        {
            CreatureState = CreatureStates.Move;
            return;
        }
    }
    protected override void UpdateDead() 
    {

    }

    #endregion
   
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

        
    }
}
