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

        CreatureType = CreatureTypes.Player;

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
        Skills.SetInfo(this, CreatureData.SkillIdList);
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
        // �̵� ���¸� ���� ����
        if (PlayerMoveState == PlayerMoveStates.ForceMove)
        {
            CreatureState = CreatureStates.Move;
            return;
        }

        // �ʹ� �־������� ���� ����

        // ���� ������
        Creature creature = FindClosestInRange(HERO_SEARCH_DISTANCE, Managers.Object.Monsters) as Creature;
        if (creature != null)
        {
            Target = creature;
            CreatureState = CreatureStates.Move;
            PlayerMoveState = PlayerMoveStates.TargetMonster;
            return;
        }

        // ä���� ���� ��
        Env env = FindClosestInRange(HERO_SEARCH_DISTANCE, Managers.Object.Envs) as Env;
        if (env != null)
        {
            Target = env;
            CreatureState = CreatureStates.Move;
            PlayerMoveState = PlayerMoveStates.CollectEnv;
            return;
        }

        // �����ϱ�
        if (NeedReturn)
        {
            CreatureState = CreatureStates.Move;
            PlayerMoveState = PlayerMoveStates.Return;
            return;
        }
    }

    protected override void UpdateMove() 
    {
        // ������ �ִٸ�, ���� �̵�
        if (PlayerMoveState == PlayerMoveStates.ForceMove)
        {
            FindPathResults result = FindPathAndMoveToCellPos(PlayerCampDest.position, HERO_DEFAULT_MOVE_DEPTH);
            return;
        }

        // �ֺ� ���� ��ġ
        if (PlayerMoveState == PlayerMoveStates.TargetMonster)
        {
            // ���� �׾����� ����.
            if (Target.IsValid() == false)
            {
                PlayerMoveState = PlayerMoveStates.None;
                CreatureState = CreatureStates.Move;
                return;
            }

            SkillBase skill = Skills.GetReadySkill();
            //ChaseOrAttackTarget(AttackDistance, HERO_SEARCH_DISTANCE);
            ChaseOrAttackTarget(HERO_SEARCH_DISTANCE, skill);
            return;
        }

        if (PlayerMoveState == PlayerMoveStates.CollectEnv)
        {
            // ���Ͱ� ������ ����.
            Creature creature = FindClosestInRange(HERO_SEARCH_DISTANCE, Managers.Object.Monsters) as Creature;
            if (creature != null)
            {
                Target = creature;
                PlayerMoveState = PlayerMoveStates.TargetMonster;
                CreatureState = CreatureStates.Move;
                return;
            }

            // Env �̹� ä�������� ����.
            if (Target.IsValid() == false)
            {
                PlayerMoveState = PlayerMoveStates.None;
                CreatureState = CreatureStates.Move;
                return;
            }

            SkillBase skill = Skills.GetReadySkill();
            ChaseOrAttackTarget(HERO_SEARCH_DISTANCE, skill);
            return;
        }

        // ���̱�
        if (PlayerMoveState == PlayerMoveStates.Return)
        {
            Vector3 destPos = PlayerCampDest.position;
            if (FindPathAndMoveToCellPos(destPos, HERO_DEFAULT_MOVE_DEPTH) == FindPathResults.Success)
                return;

            // ���� ���� �˻�.
            BaseObject obj = Managers.Map.GetObject(destPos);
            if (obj.IsValid())
            {
                // ���� �� �ڸ��� �����ϰ� �ִٸ�
                if (obj == this)
                {
                    PlayerMoveState = PlayerMoveStates.None;
                    NeedReturn = false;
                    return;
                }

                // �ٸ� ������ �����ִٸ�.
                Player player = obj as Player;
                if (player != null && player.CreatureState == CreatureStates.Idle)
                {
                    PlayerMoveState = PlayerMoveStates.None;
                    NeedReturn = false;
                    return;
                }
            }
        }
        //�����ٰ� ��������
        if(LerpCellPosCompleted)
            CreatureState = CreatureStates.Idle;
    }

    protected override void UpdateSkill() 
    {
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
