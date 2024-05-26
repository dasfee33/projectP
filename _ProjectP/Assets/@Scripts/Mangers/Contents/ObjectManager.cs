using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class ObjectManager
{
    public List<Player> Players { get; } = new List<Player>();
    public List<Monster> Monsters { get; } = new List<Monster>();
    public List<Projectile> Projectiles { get; } = new List<Projectile>();
    public List<Env> Envs { get; } = new List<Env>();
    public List<EffectBase> Effects { get; } = new List<EffectBase>();
    public PlayerCamp Camp { get; private set; }

    #region Roots
    public Transform GetRootTransform(string name)
    {
        GameObject root = GameObject.Find(name);
        if (root == null)
            root = new GameObject { name = name };

        return root.transform;
    }

    public Transform PlayerRoot { get { return GetRootTransform("@Players"); } }
    public Transform MonsterRoot { get { return GetRootTransform("@Monsters"); } }
    public Transform ProjectileRoot { get { return GetRootTransform("@Projectiles"); } }
    public Transform EnvRoot { get { return GetRootTransform("@Envss"); } }
    public Transform EffectRoot { get { return GetRootTransform("@Effects"); } }

    #endregion

    public void ShowDamageFont(Vector2 position, float damage, Transform parent, bool isCritical = false)
    {
        GameObject go = Managers.Resource.Instantiate("DamageFont", pooling: true);
        DamageFont damageText = go.GetComponent<DamageFont>();
        damageText.SetInfo(position, damage, parent, isCritical);
    }

    public GameObject SpawnGameObject(Vector3 position, string prefabName)
    {
        GameObject go = Managers.Resource.Instantiate(prefabName, pooling: true);
        go.transform.position = position;
        return go;
    }

    public T Spawn<T>(Vector3Int cellPos, int templateID) where T : BaseObject
    {
        Vector3 spawnPos = Managers.Map.Cell2World(cellPos);
        return Spawn<T>(spawnPos, templateID);
    }

    public T Spawn<T>(Vector3 position, int templateID) where T : BaseObject
    {
        string prefabName = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(prefabName);
        go.name = prefabName;
        go.transform.position = position;

        BaseObject obj = go.GetComponent<BaseObject>();

        if(obj.ObjectType == ObjectTypes.Player)
        {
            obj.transform.parent = PlayerRoot;
            Player player = go.GetComponent<Player>();
            Players.Add(player);
            player.SetInfo(templateID);
        }
        else if(obj.ObjectType == ObjectTypes.Monster)
        {
            obj.transform.parent = MonsterRoot;
            Monster monster = go.GetComponent<Monster>();
            Monsters.Add(monster);
            monster.SetInfo(templateID);
        }
        else if (obj.ObjectType == ObjectTypes.Projectile)
        {
            obj.transform.parent = ProjectileRoot;

            Projectile projectile = go.GetComponent<Projectile>();
            Projectiles.Add(projectile);

            projectile.SetInfo(templateID);
        }
        else if (obj.ObjectType == ObjectTypes.Env)
        {
            obj.transform.parent = EnvRoot;

            Env env = go.GetComponent<Env>();
            Envs.Add(env);

            env.SetInfo(templateID);

        }
        else if(obj.ObjectType == ObjectTypes.PlayerCamp)
        {
            Camp = go.GetComponent<PlayerCamp>();
        }

        return obj as T;
    }

    public void Despawn<T>(T obj) where T : BaseObject
    {
        ObjectTypes objectType = obj.ObjectType;

        if (obj.ObjectType == ObjectTypes.Player)
        {
            Player player = obj.GetComponent<Player>();
            Players.Remove(player);
        }
        else if (obj.ObjectType == ObjectTypes.Monster)
        {
            Monster monster = obj.GetComponent<Monster>();
            Monsters.Remove(monster);
        }
        else if (obj.ObjectType == ObjectTypes.Projectile)
        {
            Projectile projectile = obj as Projectile;
            Projectiles.Remove(projectile);
        }
        else if (obj.ObjectType == ObjectTypes.Env)
        {
            Env env = obj as Env;
            Envs.Remove(env);
        }
        else if (obj.ObjectType == ObjectTypes.Effect)
        {
            EffectBase effect = obj as EffectBase;
            Effects.Remove(effect);
        }
        else if (obj.ObjectType == ObjectTypes.PlayerCamp)
        {
            Camp = null;
        }

        Managers.Resource.Destroy(obj.gameObject);
    }

    #region Skill 판정
    public List<Creature> FindConeRangeTargets(Creature owner, Vector3 dir, float range, int angleRange, bool isAllies = false)
    {
        List<Creature> targets = new List<Creature>();
        List<Creature> ret = new List<Creature>();

        ObjectTypes targetType = Util.DetermineTargetType(owner.ObjectType, isAllies);

        if (targetType == ObjectTypes.Monster)
        {
            var objs = Managers.Map.GatherObjects<Monster>(owner.transform.position, range, range);
            targets.AddRange(objs);
        }
        else if (targetType == ObjectTypes.Player)
        {
            var objs = Managers.Map.GatherObjects<Player>(owner.transform.position, range, range);
            targets.AddRange(objs);
        }

        foreach (var target in targets)
        {
            // 1. 거리안에 있는지 확인
            var targetPos = target.transform.position;
            float distance = Vector3.Distance(targetPos, owner.transform.position);

            if (distance > range)
                continue;

            // 2. 각도 확인
            if (angleRange != 360)
            {
                BaseObject ownerTarget = (owner as Creature).Target;

                // 2. 부채꼴 모양 각도 계산
                float dot = Vector3.Dot((targetPos - owner.transform.position).normalized, dir.normalized);
                float degree = Mathf.Rad2Deg * Mathf.Acos(dot);

                if (degree > angleRange / 2f)
                    continue;
            }

            ret.Add(target);
        }

        return ret;
    }

    public List<Creature> FindCircleRangeTargets(Creature owner, Vector3 startPos, float range, bool isAllies = false)
    {
        HashSet<Creature> targets = new HashSet<Creature>();
        HashSet<Creature> ret = new HashSet<Creature>();

        ObjectTypes targetType = Util.DetermineTargetType(owner.ObjectType, isAllies);

        if (targetType == ObjectTypes.Monster)
        {
            var objs = Managers.Map.GatherObjects<Monster>(owner.transform.position, range, range);
            targets.AddRange(objs);
        }
        else if (targetType == ObjectTypes.Player)
        {
            var objs = Managers.Map.GatherObjects<Player>(owner.transform.position, range, range);
            targets.AddRange(objs);
        }

        foreach (var target in targets)
        {
            // 1. 거리안에 있는지 확인
            var targetPos = target.transform.position;
            float distSqr = (targetPos - startPos).sqrMagnitude;

            if (distSqr < range * range)
                ret.Add(target);
        }

        return ret.ToList();
    }

    #endregion
}
