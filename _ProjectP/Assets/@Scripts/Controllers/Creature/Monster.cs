using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Monster : Creature
{
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

        CreatureType = CreatureTypes.Monster;

        StartCoroutine(CoUpdateAI());

        return true;
    }

    public override void SetInfo(int templateID)
    {
        base.SetInfo(templateID);

        // State
        CreatureState = CreatureStates.Idle;

        // Skill
        Skills = gameObject.GetOrAddComponent<SkillComponent>();
        Skills.SetInfo(this, CreatureData.SkillIdList);
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
        if (Target == null)
        {
            // Patrol or Return
            Vector3 dir = (destPos - transform.position);
            if (dir.sqrMagnitude <= 0.01f)
            {
                CreatureState = CreatureStates.Idle;
                return;
            }

            //SetRigidBodyVelocity(dir.normalized * MoveSpeed);
        }
        else
        {
            // Chase
            SkillBase skill = Skills.GetReadySkill();
            ChaseOrAttackTarget(MONSTER_SEARCH_DISTANCE, skill);

            //ChaseOrAttackTarget(MONSTER_SEARCH_DISTANCE, 5.0f);

            if(Target.IsValid() == false)
            {
                Target = null;
                destPos = initPos;
                return;
            }
        }
    }

    protected override void UpdateSkill()
    {
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

        Managers.Object.Despawn(this);
    }
    #endregion
}
