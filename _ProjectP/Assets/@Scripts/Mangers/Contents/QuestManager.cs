using Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Define;

public class QuestManager
{
    public Dictionary<int, Quest> AllQuests = new Dictionary<int, Quest>();

    public List<Quest> WaitingQuests { get; } = new List<Quest>();
    public List<Quest> ProcessingQuests { get; } = new List<Quest>();
    public List<Quest> CompletedQuests { get; } = new List<Quest>();
    public List<Quest> RewardedQuests { get; } = new List<Quest>();

    public QuestManager()
    {
        Managers.Game.OnBroadcastEvent -= OnHandleBroadcastEvent;
        Managers.Game.OnBroadcastEvent += OnHandleBroadcastEvent;
    }

    public void AddUnknownQuests()
    {
        foreach (QuestData questData in Managers.Data.QuestDic.Values.ToList())
        {
            if (AllQuests.ContainsKey(questData.TemplateId))
                continue;

            QuestSaveData questSaveData = new QuestSaveData() {
                TemplateId = questData.TemplateId,
                State = Define.QuestStates.None,
                NextResetTime = DateTime.MaxValue,
            };

            for (int i = 0; i < questData.QuestTasks.Count; i++)
                questSaveData.ProgressCount.Add(0);

            AddQuest(questSaveData);
        }
    }

    public void CheckWaitingQuests()
    {
        // TODO
    }

    public void CheckProcessingQuests()
    {
        // TODO
    }

    public Quest AddQuest(QuestSaveData questInfo)
    {
        Quest quest = Quest.MakeQuest(questInfo);
        if (quest == null)
            return null;

        switch (quest.State)
        {
            case Define.QuestStates.None:
                WaitingQuests.Add(quest);
                break;
            case Define.QuestStates.Processing:
                ProcessingQuests.Add(quest);
                break;
            case Define.QuestStates.Completed:
                CompletedQuests.Add(quest);
                break;
            case Define.QuestStates.Rewarded:
                RewardedQuests.Add(quest);
                break;
        }

        AllQuests.Add(quest.TemplateId, quest);

        return quest;
    }

    public void Clear()
    {
        AllQuests.Clear();

        WaitingQuests.Clear();
        ProcessingQuests.Clear();
        CompletedQuests.Clear();
        RewardedQuests.Clear();
    }

    void OnHandleBroadcastEvent(BroadcastEventTypes eventType, int value)
    {
        foreach (Quest quest in ProcessingQuests)
        {
            quest.OnHandleBroadcastEvent(eventType, value);
        }
    }
}
