using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public int Wood = 0;
    public int Mineral = 0;
    public int Meat = 0;
    public int Gold = 0;

    public List<PlayerSaveData> Players = new List<PlayerSaveData>();
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
            //(Managers.UI.SceneUI as UI_GameScene)?.RefreshWoodText();
        }
    }

    public int Mineral
    {
        get { return _saveData.Mineral; }
        private set
        {
            _saveData.Mineral = value;
            //(Managers.UI.SceneUI as UI_GameScene)?.RefreshMineralText();
        }
    }

    public int Meat
    {
        get { return _saveData.Meat; }
        private set
        {
            _saveData.Meat = value;
            //(Managers.UI.SceneUI as UI_GameScene)?.RefreshMeatText();
        }
    }

    public int Gold
    {
        get { return _saveData.Gold; }
        private set
        {
            _saveData.Gold = value;
            //(Managers.UI.SceneUI as UI_GameScene)?.RefreshGoldText();
        }
    }

    public List<PlayerSaveData> AllPlayers { get { return _saveData.Players; } }
    public int TotalPlayerCount { get { return _saveData.Players.Count; } }
    public int UnownedPlayerCount { get { return _saveData.Players.Where(h => h.OwningState == PlayerOwningState.Unowned).Count(); } }
    public int OwnedPlayerCount { get { return _saveData.Players.Where(h => h.OwningState == PlayerOwningState.Owned).Count(); } }
    public int PickedPlayerCount { get { return _saveData.Players.Where(h => h.OwningState == PlayerOwningState.Picked).Count(); } }
    
    
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

        Debug.Log($"Save Game Loaded : {Path}");
        return true;
    }
    #endregion

    #region Action
    public event Action<Vector2> OnMoveDirChanged;
    public event Action<Define.JoystickStates> OnJoystickStateChanged;
    #endregion
}
