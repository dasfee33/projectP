using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

namespace Data
{
    #region CreatureData
    [Serializable]
    public class CreatureData
    {
        public int DataId;
        public string DescriptionTextID;
        public string Label;
        public float ColliderOffsetX;
        public float ColliderOffsetY;
        public float ColliderRadius;
        public float MaxHp;
        public float UpMaxHpBonus;
        public float Atk;
        public float AtkRange;
        public float AtkBonus;
        public float MoveSpeed;
        public float CriRate;
        public float Cridamage;
        public string IconImage;
        public string SkeletonDataID;
        public int DefaultSkill;
        public int EnvSkill;
        public int SkillA;
        public int SkillB;
    }
    #endregion

    #region MonsterData
    public class MonsterData : CreatureData
    {
        public int DropItemId;

        //TODO
        [NonSerialized]
        public DropTableData DropTable;
    }

    [Serializable]
    public class MonsterDataLoader : ILoader<int, MonsterData>
    {
        public List<MonsterData> monsters = new List<MonsterData>();

        public Dictionary<int, MonsterData> MakeDict()
        {
            Dictionary<int, MonsterData> dict = new Dictionary<int, MonsterData>();
            foreach (MonsterData monster in monsters)
                dict.Add(monster.DataId, monster);
            return dict;
        }
    }
    #endregion

    #region PlayerData
    public class PlayerData : CreatureData
    {
        
    }

    [Serializable]
    public class PlayerDataLoader : ILoader<int, PlayerData>
    {
        public List<PlayerData> players = new List<PlayerData>();

        public Dictionary<int, PlayerData> MakeDict()
        {
            Dictionary<int, PlayerData> dict = new Dictionary<int, PlayerData>();
            foreach (PlayerData player in players)
                dict.Add(player.DataId, player);
            return dict;
        }
    }
    #endregion

    #region PlayerInfoData
    [Serializable]
    public class PlayerInfoData
    {
        public int DataId;
        public string NameTextId;
        public string DescriptionTextId;
        public string Rarity;
        public float GachaSpawnWeight;
        public float GachaWeight;
        public int GachaExpCount;
        public string IconImage;
    }

    [Serializable]
    public class PlayerInfoDataLoader : ILoader<int, PlayerInfoData>
    {
        public List<PlayerInfoData> playerInfo = new List<PlayerInfoData>();
        public Dictionary<int, PlayerInfoData> MakeDict()
        {
            Dictionary<int, PlayerInfoData> dict = new Dictionary<int, PlayerInfoData>();
            foreach (PlayerInfoData info in playerInfo)
                dict.Add(info.DataId, info);
            return dict;
        }
    }
    #endregion

    #region SkillData
    [Serializable]
    public class SkillData
    {
        public int DataId;
        public string Name;
        public string ClassName;
        public string Description;
        public int ProjectileId;
        public string PrefabLabel;
        public string IconLabel;
        public string AnimName;
        public float CoolTime;
        public float DamageMultiplier;
        public float Duration;
        public float AnimImpactDuration;
        public string CastingSound;
        public float SkillRange;
        public float ScaleMultiplier;
        public int TargetCount;
        public List<int> EffectIds = new List<int>();
        public int NextLevelId;
        public int AoEId;
        public EffectSizes EffectSize;
    }

    [Serializable]
    public class SkillDataLoader : ILoader<int, SkillData>
    {
        public List<SkillData> skills = new List<SkillData>();

        public Dictionary<int, SkillData> MakeDict()
        {
            Dictionary<int, SkillData> dict = new Dictionary<int, SkillData>();
            foreach (SkillData skill in skills)
                dict.Add(skill.DataId, skill);
            return dict;
        }
    }
    #endregion

    #region ProjectileData
    [Serializable]
    public class ProjectileData
    {
        public int DataId;
        public string Name;
        public string ClassName;
        public string ComponentName;
        public string ProjectileSpriteName;
        public string PrefabLabel;
        public float Duration;
        public float HitSound;
        public float ProjRange;
        public float ProjSpeed;
    }

    [Serializable]
    public class ProjectileDataLoader : ILoader<int, ProjectileData>
    {
        public List<ProjectileData> projectiles = new List<ProjectileData>();

        public Dictionary<int, ProjectileData> MakeDict()
        {
            Dictionary<int, ProjectileData> dict = new Dictionary<int, ProjectileData>();
            foreach (ProjectileData projectile in projectiles)
                dict.Add(projectile.DataId, projectile);
            return dict;
        }
    }
    #endregion

    #region Env
    [Serializable]
    public class EnvData
    {
        public int DataId;
        public string DescriptionTextID;
        public string PrefabLabel;
        public float MaxHp;
        public int ResourceAmount;
        public float RegenTime;
        public List<String> SkeletonDataIDs = new List<String>();
        public int DropItemId;
    }

    [Serializable]
    public class EnvDataLoader : ILoader<int, EnvData>
    {
        public List<EnvData> envs = new List<EnvData>();
        public Dictionary<int, EnvData> MakeDict()
        {
            Dictionary<int, EnvData> dict = new Dictionary<int, EnvData>();
            foreach (EnvData env in envs)
                dict.Add(env.DataId, env);
            return dict;
        }
    }
    #endregion

    #region EffectData
    [Serializable]
    public class EffectData
    {
        public int DataId;
        public string Name;
        public string ClassName;
        public string DescriptionTextID;
        public string SkeletonDataID;
        public string IconLabel;
        public string SoundLabel;
        public float Amount;
        public float PercentAdd;
        public float PercentMult;
        public float TickTime;
        public float TickCount;
        public EffectTypes EffectType;
    }

    [Serializable]
    public class EffectDataLoader : ILoader<int, EffectData>
    {
        public List<EffectData> effects = new List<EffectData>();
        public Dictionary<int, EffectData> MakeDict()
        {
            Dictionary<int, EffectData> dict = new Dictionary<int, EffectData>();
            foreach (EffectData effect in effects)
                dict.Add(effect.DataId, effect);
            return dict;
        }
    }
    #endregion

    #region AoEData
    [Serializable]
    public class AoEData
    {
        public int DataId;
        public string Name;
        public string ClassName;
        public string SkeletonDataID;
        public string SoundLabel;
        public float Duration;
        public List<int> AllyEffects = new List<int>();
        public List<int> EnemyEffects = new List<int>();
        public string AnimName;
    }

    [Serializable]
    public class AoEDataLoader : ILoader<int, AoEData>
    {
        public List<AoEData> aoes = new List<AoEData>();
        public Dictionary<int, AoEData> MakeDict()
        {
            Dictionary<int, AoEData> dict = new Dictionary<int, AoEData>();
            foreach (AoEData aoe in aoes)
                dict.Add(aoe.DataId, aoe);
            return dict;
        }
    }
    #endregion

    #region NPC
    [Serializable]
    public class NpcData
    {
        public int DataId;
        public string Name;
        public string DescriptionTextID;
        public NpcTypes NpcType;
        public string PrefabLabel;
        public string SpriteName;
        public string SkeletonDataID;
    }

    [Serializable]
    public class NpcDataLoader : ILoader<int, NpcData>
    {
        public List<NpcData> creatures = new List<NpcData>();
        public Dictionary<int, NpcData> MakeDict()
        {
            Dictionary<int, NpcData> dict = new Dictionary<int, NpcData>();
            foreach (NpcData creature in creatures)
                dict.Add(creature.DataId, creature);
            return dict;
        }
    }
    #endregion

    #region TextData
    [Serializable]
    public class TextData
    {
        public string DataId;
        public string KOR;
    }

    [Serializable]
    public class TextDataLoader : ILoader<string, TextData>
    {
        public List<TextData> texts = new List<TextData>();
        public Dictionary<string, TextData> MakeDict()
        {
            Dictionary<string, TextData> dict = new Dictionary<string, TextData>();
            foreach (TextData text in texts)
                dict.Add(text.DataId, text);
            return dict;
        }
    }
    #endregion

    #region Item
    [Serializable]
    public class BaseData
    {
        public int DataId;
    }

    [Serializable]
    public class ItemData : BaseData
    {
        public string Name;
        public ItemGroupTypes ItemGroupType;
        public ItemTypes Type;
        public ItemSubTypes SubType;
        public ItemGrade Grade;
        public int MaxStack;
    }

    [Serializable]
    public class EquipmentData : ItemData
    {
        public int Damage;
        public int Defence;
        public int Speed;
    }

    [Serializable]
    public class ConsumableData : ItemData
    {
        public double Value;
        public int CoolTime;
    }

    [Serializable]
    public class ItemDataLoader<T> : ILoader<int, T> where T : BaseData
    {
        public List<T> items = new List<T>();

        public Dictionary<int, T> MakeDict()
        {
            Dictionary<int, T> dict = new Dictionary<int, T>();
            foreach (T item in items)
                dict.Add(item.DataId, item);

            return dict;
        }
    }
    #endregion

    #region DropTable

    public class RewardData
    {
        public int Probability; // 100분율
        public int ItemTemplateId;
        // public int Count;
    }

    [Serializable]
    public class DropTableData_Internal
    {
        public int DataId;
        public int RewardExp;
        public int Prob1;
        public int Item1;
        public int Prob2;
        public int Item2;
        public int Prob3;
        public int Item3;
        public int Prob4;
        public int Item4;
        public int Prob5;
        public int Item5;
    }

    [Serializable]
    public class DropTableData
    {
        public int DataId;
        public int RewardExp;
        public List<RewardData> Rewards = new List<RewardData>();
    }

    [Serializable]
    public class DropTableDataLoader : ILoader<int, DropTableData>
    {
        public List<DropTableData_Internal> dropTables = new List<DropTableData_Internal>();

        public Dictionary<int, DropTableData> MakeDict()
        {
            Dictionary<int, DropTableData> dict = new Dictionary<int, DropTableData>();

            foreach (DropTableData_Internal tempData in dropTables)
            {
                DropTableData data = new DropTableData() {
                    DataId = tempData.DataId,
                    RewardExp = tempData.RewardExp,
                };

                if (tempData.Item1 > 0)
                {
                    data.Rewards.Add(new RewardData() {
                        Probability = tempData.Prob1,
                        ItemTemplateId = tempData.Item1,
                    });
                }

                if (tempData.Item2 > 0)
                {
                    data.Rewards.Add(new RewardData() {
                        Probability = tempData.Prob2,
                        ItemTemplateId = tempData.Item2,
                    });
                }

                if (tempData.Item3 > 0)
                {
                    data.Rewards.Add(new RewardData() {
                        Probability = tempData.Prob3,
                        ItemTemplateId = tempData.Item3,
                    });
                }

                if (tempData.Item4 > 0)
                {
                    data.Rewards.Add(new RewardData() {
                        Probability = tempData.Prob4,
                        ItemTemplateId = tempData.Item4,
                    });
                }

                if (tempData.Item5 > 0)
                {
                    data.Rewards.Add(new RewardData() {
                        Probability = tempData.Prob5,
                        ItemTemplateId = tempData.Item5,
                    });
                }

                dict.Add(tempData.DataId, data);
            }

            return dict;
        }
    }


    #endregion

    #region QuestData

    [Serializable]
    public class QuestData
    {
        public int TemplateId;
        public string DescriptionTextId;
        public QuestPeriodTypes QuestPeriodType; //NPC?
        public int ResetPeriod; // 리셋 주기

        public QuestConditions Condition;
        public List<QuestTaskData> QuestTasks = new List<QuestTaskData>();
        public List<QuestRewardData> Rewards = new List<QuestRewardData>();
    }

    [Serializable]
    public class QuestTaskData
    {
        public QuestTaskTypes QuestTaskType;
        public string DescriptionTextId;

        public int TaskDataId;
        public int TaskCount;
    }

    [Serializable]
    public class QuestRewardData
    {
        public QuestRewardTypes RewardType;
        public int RewardDataId;
        public int RewardCount;
    }

    [Serializable]
    public class QuestDataLoader : ILoader<int, QuestData>
    {
        public List<QuestData> quests = new List<QuestData>();
        public Dictionary<int, QuestData> MakeDict()
        {
            Dictionary<int, QuestData> dict = new Dictionary<int, QuestData>();
            foreach (QuestData quest in quests)
                dict.Add(quest.TemplateId, quest);
            return dict;
        }
    }
    #endregion
}
