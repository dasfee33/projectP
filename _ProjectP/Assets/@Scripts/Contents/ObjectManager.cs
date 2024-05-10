using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class ObjectManager
{
    public List<Player> Players { get; } = new List<Player>();
    public List<Monster> Monsters { get; } = new List<Monster>();
    public List<Env> Envs { get; } = new List<Env>();
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
    public Transform EnvRoot { get { return GetRootTransform("@Envss"); } }
    #endregion

    public T Spawn<T>(Vector3 position, int templateID) where T : BaseObject
    {
        string prefabName = typeof(T).Name;

        GameObject go = Managers.Resource.Instantiate(prefabName);
        go.name = prefabName;
        go.transform.position = position;

        BaseObject obj = go.GetComponent<BaseObject>();

        if (obj.ObjectType == ObjectTypes.Creature)
        {
            // Data Check
            if (templateID != 0 && Managers.Data.CreatureDic.TryGetValue(templateID, out Data.CreatureData data) == false)
            {
                Debug.LogError($"ObjectManager Spawn Creature Failed! TryGetValue TemplateID : {templateID}");
                return null;
            }

            Creature creature = go.GetComponent<Creature>();
            switch (creature.CreatureType)
            {
                case CreatureTypes.Player:
                    obj.transform.parent = PlayerRoot;
                    Player player = creature as Player;
                    Players.Add(player);
                    break;
                case CreatureTypes.Monster:
                    obj.transform.parent = MonsterRoot;
                    Monster monster = creature as Monster;
                    Monsters.Add(monster);
                    break;
            }

            creature.SetInfo(templateID);
        }
        else if (obj.ObjectType == ObjectTypes.Projectile)
        {
            // TODO
        }
        else if (obj.ObjectType == ObjectTypes.Env)
        {
            // Data Check
            if (templateID != 0 && Managers.Data.EnvDic.TryGetValue(templateID, out Data.EnvData data) == false)
            {
                Debug.LogError($"ObjectManager Spawn Creature Failed! TryGetValue TemplateID : {templateID}");
                return null;
            }

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

        if (obj.ObjectType == ObjectTypes.Creature)
        {
            Creature creature = obj.GetComponent<Creature>();
            switch (creature.CreatureType)
            {
                case CreatureTypes.Player:
                    Player player = creature as Player;
                    Players.Remove(player);
                    break;
                case CreatureTypes.Monster:
                    Monster monster = creature as Monster;
                    Monsters.Remove(monster);
                    break;
            }
        }
        else if (obj.ObjectType == ObjectTypes.Projectile)
        {
            // TODO
        }
        else if (obj.ObjectType == ObjectTypes.Env)
        {
            Env env = obj as Env;
            Envs.Remove(env);
        }
        else if (obj.ObjectType == ObjectTypes.PlayerCamp)
        {
            Camp = null;
        }

        Managers.Resource.Destroy(obj.gameObject);
    }
}
