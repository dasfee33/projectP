using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;
using static Define;

public class Monster : Creature
{
    public Data.MonsterData MonsterData { get { return (Data.MonsterData)CreatureData; } }
    public override CreatureStates CreatureState 
    { 
        get { return base.CreatureState; }
        set
        {
            if(creatureState != value)
            {
                base.CreatureState = value;
                switch(value)
                {
                    case CreatureStates.Idle:
                        UpdateAITick = 0.5f;
                        break;
                    case CreatureStates.Move:
                        UpdateAITick = 0.0f;
                        break;
                    case CreatureStates.Skill:
                        UpdateAITick = 0.0f;
                        break;
                    case CreatureStates.Dead:
                        UpdateAITick = 1.0f;
                        break;
                }
            }
        }
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        ObjectType = ObjectTypes.Monster;

        StartCoroutine(CoUpdateAI());

        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        // State
        CreatureState = CreatureStates.Idle;
    }

    void Start()
    {
        initPos = transform.position;
    }

    #region AI
    private Vector3 destPos;
    private Vector3 initPos;

    protected override void UpdateIdle()
    {
        //Patrol
        {
            int patrolPercent = 10;
            int rand = Random.Range(0, 100);
            if (rand <= patrolPercent)
            {
                destPos = initPos + new Vector3(Random.Range(-2, 2), Random.Range(-2, 2));
                CreatureState = CreatureStates.Move;
                return;
            }
        }

        //Search Player
        Creature creature = FindClosestInRange(MONSTER_SEARCH_DISTANCE, Managers.Object.Players, func: IsValid) as Creature;
        if(creature != null)
        {
            Target = creature;
            CreatureState = CreatureStates.Move;
            return;
        }
    }

    protected override void UpdateMove()
    {
        if (Target.IsValid() == false)
        {
            Creature creature = FindClosestInRange(MONSTER_SEARCH_DISTANCE, Managers.Object.Players, func: IsValid) as Creature;
            if (creature != null)
            {
                Target = creature;
                CreatureState = CreatureStates.Move;
                return;
            }

            // Move
            FindPathAndMoveToCellPos(destPos, MONSTER_DEFAULT_MOVE_DEPTH);

            if (LerpCellPosCompleted)
            {
                CreatureState = CreatureStates.Idle;
                return;
            }
        }
        else
        {
            // Chase
            ChaseOrAttackTarget(MONSTER_SEARCH_DISTANCE, AttackDistance);

            // 너무 멀어지면 포기.
            if (Target.IsValid() == false)
            {
                Target = null;
                destPos = initPos;
                return;
            }
        }
    }

    protected override void UpdateSkill()
    {
        base.UpdateSkill();

        if(Target.IsValid() == false)
        {
            Target = null;
            destPos = initPos;
            CreatureState = CreatureStates.Move;
            return;
        }
    }

    protected override void UpdateDead()
    {
        
    }

    #endregion

    #region Battle
    public override void OnDamaged(BaseObject attacker, SkillBase skill)
    {
        base.OnDamaged(attacker, skill);
    }

    public override void OnDead(BaseObject attacker, SkillBase skill)
    {
        base.OnDead(attacker, skill);

        // TODO : Drop Item
        int dropItemId = MonsterData.DropItemId;

        RewardData rewardData = GetRandomReward();
        if(rewardData != null)
        {
            var itemHolder = Managers.Object.Spawn<ItemHolder>(transform.position, dropItemId);
            Vector2 ran = new Vector2(transform.position.x + Random.Range(-10, -15) * 0.1f, transform.position.y);
            Vector2 ran2 = new Vector2(transform.position.x + Random.Range(10, 15) * 0.1f, transform.position.y);
            Vector2 dropPos = Random.value < 0.5 ? ran : ran2;
            itemHolder.SetInfo(0, rewardData.ItemTemplateId, dropPos);
        }

        //BroadCast
        Managers.Game.BroadcastEvent(BroadcastEventTypes.KillMonster, MonsterData.DataId);

        Managers.Object.Despawn(this);
    }
    #endregion

    RewardData GetRandomReward()
    {
        if (MonsterData == null)
            return null;

        if (Managers.Data.DropTableDic.TryGetValue(MonsterData.DropItemId, out DropTableData dropTableData) == false)
            return null;

        if (dropTableData.Rewards.Count <= 0)
            return null;

        int sum = 0;

        int randValue = UnityEngine.Random.Range(0, 100);

        foreach(RewardData item in dropTableData.Rewards)
        {
            sum += item.Probability;

            if (randValue <= sum)
                return item;
        }

        //return dropTableData.Rewards.RandomElementByWeight(e => e.Probability);

        return null;
    }

    private int GetRewardExp()
    {
        if (MonsterData == null)
            return 0;

        if (Managers.Data.DropTableDic.TryGetValue(MonsterData.DropItemId, out DropTableData droptableData) == false)
            return 0;

        return droptableData.RewardExp;
    }
}
