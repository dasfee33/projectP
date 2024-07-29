using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using static Define;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public int Wood = 0;
    public int Mineral = 0;
    public int Meat = 0;
    public int Gold = 0;

    public List<PlayerSaveData> Players = new List<PlayerSaveData>();

    public int ItemDbIdGenerator = 1;
    public List<ItemSaveData> Items = new List<ItemSaveData>();

    public List<QuestSaveData> ProcessingQuests = new List<QuestSaveData>(); // 진행
    public List<QuestSaveData> CompletedQuests = new List<QuestSaveData>(); // 완료
    public List<QuestSaveData> RewardedQuests = new List<QuestSaveData>(); // 보상 받음
}

[Serializable]
public class PlayerSaveData
{
    public int DataId = 0;
    public int Level = 1;
    public int Exp = 0;
    public PlayerOwningState OwningState = PlayerOwningState.Unowned;
}

public enum PlayerOwningState
{
    Unowned,
    Owned,
    Picked,
}

[Serializable]
public class ItemSaveData
{
    public int InstanceId;
    public int DbId;
    public int TemplateId;
    public int Count;
    public int EquipSlot; // 장착 + 인벤 + 창고
    //public int OwnerId;
    public int EnchantCount;
}

[Serializable]
public class QuestSaveData
{
    public int TemplateId;
    public List<int> ProgressCount = new List<int>();
    public QuestStates State = QuestStates.None;
    public DateTime NextResetTime;

}

public class GameManager
{
    #region GameData
    GameSaveData _saveData = new GameSaveData();
    public GameSaveData SaveData { get { return _saveData; } set { _saveData = value; } }

    public int Wood
    {
        get { return _saveData.Wood; }
        private set
        {
            _saveData.Wood = value;
            BroadcastEvent(BroadcastEventTypes.ChangeWood, value);
        }
    }

    public int Mineral
    {
        get { return _saveData.Mineral; }
        private set
        {
            _saveData.Mineral = value;
            BroadcastEvent(BroadcastEventTypes.ChangeMineral, value);
        }
    }

    public int Meat
    {
        get { return _saveData.Meat; }
        private set
        {
            _saveData.Meat = value;
            BroadcastEvent(BroadcastEventTypes.ChangeMeat, value);
        }
    }

    public int Gold
    {
        get { return _saveData.Gold; }
        private set
        {
            _saveData.Gold = value;
            BroadcastEvent(BroadcastEventTypes.ChangeGold, value);
        }
    }

    public void BroadcastEvent(BroadcastEventTypes eventType, int value)
    {
        OnBroadcastEvent?.Invoke(eventType, value);
    }

    public List<PlayerSaveData> AllPlayers { get { return _saveData.Players; } }
    public int TotalPlayerCount { get { return _saveData.Players.Count; } }
    public int UnownedPlayerCount { get { return _saveData.Players.Where(h => h.OwningState == PlayerOwningState.Unowned).Count(); } }
    public int OwnedPlayerCount { get { return _saveData.Players.Where(h => h.OwningState == PlayerOwningState.Owned).Count(); } }
    public int PickedPlayerCount { get { return _saveData.Players.Where(h => h.OwningState == PlayerOwningState.Picked).Count(); } }
    
    public int GeneratorItemDbId()
    {
        int itemDbId = _saveData.ItemDbIdGenerator;
        _saveData.ItemDbIdGenerator++;
        return itemDbId;
    }
    #endregion

    #region Player
    private Vector2 moveDir;
    public Vector2 MoveDir
    {
        get { return moveDir; }
        set
        {
            moveDir = value;
            OnMoveDirChanged?.Invoke(value);
        }
    }

    private Define.JoystickStates joystickState;
    public Define.JoystickStates JoystickState
    {
        get { return joystickState; }
        set
        {
            joystickState = value;
            OnJoystickStateChanged?.Invoke(joystickState);
        }
    }
    #endregion

    #region Teleport
    public void TeleportHeroes(Vector3 position)
    {
        TeleportHeroes(Managers.Map.World2Cell(position));
    }

    public void TeleportHeroes(Vector3Int cellPos)
    {
        foreach (var player in Managers.Object.Players)
        {
            Vector3Int randCellPos = Managers.Game.GetNearbyPosition(player, cellPos);
            Managers.Map.MoveTo(player, randCellPos, forceMove: true);
        }

        Vector3 worldPos = Managers.Map.Cell2World(cellPos);
        Managers.Object.Camp.ForceMove(worldPos);
        Camera.main.transform.position = worldPos;
    }
    #endregion

    #region Helper
    public Vector3Int GetNearbyPosition(BaseObject hero, Vector3Int pivot, int range = 5)
    {
        int x = UnityEngine.Random.Range(-range, range);
        int y = UnityEngine.Random.Range(-range, range);

        for (int i = 0; i < 100; i++)
        {
            Vector3Int randCellPos = pivot + new Vector3Int(x, y, 0);
            if (Managers.Map.CanGo(hero, randCellPos))
                return randCellPos;
        }

        Debug.LogError($"GetNearbyPosition Failed");

        return Vector3Int.zero;
    }
    #endregion

    #region Save & Load	
    public string Path { get { return Application.persistentDataPath + "/SaveData.json"; } }

    public void InitGame()
    {
        if (File.Exists(Path))
            return;

        var players = Managers.Data.PlayerDic.Values.ToList();
        foreach (PlayerData player in players)
        {
            PlayerSaveData saveData = new PlayerSaveData() 
            {
                DataId = player.DataId,
            };

            SaveData.Players.Add(saveData);
        }

        // TEMP
        SaveData.Players[0].OwningState = PlayerOwningState.Picked;
        SaveData.Players[1].OwningState = PlayerOwningState.Owned;
    }

    public void SaveGame()
    {
        //Player

        //Item
        {
            SaveData.Items.Clear();
            foreach (var item in Managers.Inventory.AllItems)
                SaveData.Items.Add(item.SaveData);
        }
        //Quest
        {
            SaveData.ProcessingQuests.Clear();
            SaveData.CompletedQuests.Clear();
            SaveData.RewardedQuests.Clear();

            foreach (Quest item in Managers.Quest.ProcessingQuests)
                SaveData.ProcessingQuests.Add(item.SaveData);

            foreach (Quest item in Managers.Quest.CompletedQuests)
                SaveData.CompletedQuests.Add(item.SaveData);

            foreach (Quest item in Managers.Quest.RewardedQuests)
                SaveData.RewardedQuests.Add(item.SaveData);
        }

        string jsonStr = JsonUtility.ToJson(Managers.Game.SaveData);
        File.WriteAllText(Path, jsonStr);
        Debug.Log($"Save Game Completed : {Path}");
    }

    public bool LoadGame()
    {
        if (File.Exists(Path) == false)
            return false;

        string fileStr = File.ReadAllText(Path);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(fileStr);

        if (data != null)
            Managers.Game.SaveData = data;

        //Player

        //Item
        {
            Managers.Inventory.Clear();

            foreach (ItemSaveData itemSaveData in data.Items)
            {
                Managers.Inventory.AddItem(itemSaveData);
            }
        }

        //Quest
        {
            Managers.Quest.Clear();

            foreach (QuestSaveData questSaveData in data.ProcessingQuests)
            {
                Managers.Quest.AddQuest(questSaveData);
            }

            foreach (QuestSaveData questSaveData in data.CompletedQuests)
            {
                Managers.Quest.AddQuest(questSaveData);
            }

            foreach (QuestSaveData questSaveData in data.RewardedQuests)
            {
                Managers.Quest.AddQuest(questSaveData);
            }

            Managers.Quest.AddUnknownQuests();
        }

        Debug.Log($"Save Game Loaded : {Path}");
        return true;
    }
    #endregion

    #region Action
    public event Action<Vector2> OnMoveDirChanged;
    public event Action<Define.JoystickStates> OnJoystickStateChanged;

    public event Action<BroadcastEventTypes, int> OnBroadcastEvent;
    #endregion
}
