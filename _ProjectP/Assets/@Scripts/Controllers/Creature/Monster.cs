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
        Speed = 3.0f;

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
    public float SearchDistance { get; protected set; } = 8.0f;
    public float AttackDistance { get; protected set; } =4.0f;
    private Creature _target;
    private Vector3 destPos;
    private Vector3 initPos;

    protected override void UpdateIdle()
    {
        Debug.Log("Idle");

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
        {
            Creature target = null;
            float bestDistanceSqr = float.MaxValue;
            float searchDistanceSqr = SearchDistance * SearchDistance;

            foreach (Player player in Managers.Object.Players)
            {
                Vector3 dir = player.transform.position - this.transform.position;
                float distToTargetSqr = dir.sqrMagnitude;

                if (distToTargetSqr > searchDistanceSqr) continue;
                if (distToTargetSqr > bestDistanceSqr) continue;

                target = player;
                bestDistanceSqr = distToTargetSqr;
            }
            _target = target;

            if (_target != null)
                CreatureState = CreatureStates.Move;
        }
    }

    protected override void UpdateMove()
    {
        Debug.Log("Move");

        if (_target == null)
        {
            // Patrol or Return
            Vector3 dir = (destPos - transform.position);
            float moveDist = Mathf.Min(dir.magnitude, Time.deltaTime * Speed);
            transform.ObjectTranslate(dir.normalized * moveDist);

            if (dir.sqrMagnitude <= 0.01f)
            {
                CreatureState = CreatureStates.Idle;
            }
        }
        else
        {
            // Chase
            Vector3 dir = (_target.transform.position - transform.position);
            float distToTargetSqr = dir.sqrMagnitude;
            float attackDistanceSqr = AttackDistance * AttackDistance;

            if (distToTargetSqr < attackDistanceSqr)
            {
                // 공격 범위 이내로 들어왔으면 공격.
                CreatureState = CreatureStates.Skill;
                StartWait(2.0f);
            }
            else
            {
                // 공격 범위 밖이라면 추적.
                float moveDist = Mathf.Min(dir.magnitude, Time.deltaTime * Speed);
                transform.ObjectTranslate(dir.normalized * moveDist);

                // 너무 멀어지면 포기.
                float searchDistanceSqr = SearchDistance * SearchDistance;
                if (distToTargetSqr > searchDistanceSqr)
                {
                    destPos = initPos;
                    _target = null;
                    CreatureState = CreatureStates.Move;
                }
            }
        }
    }

    protected override void UpdateSkill()
    {
        Debug.Log("Skill");
        if (_coWait != null) return;

        CreatureState = CreatureStates.Move;
    }

    protected override void UpdateDead()
    {
        Debug.Log("Dead");
    }

    #endregion
}
