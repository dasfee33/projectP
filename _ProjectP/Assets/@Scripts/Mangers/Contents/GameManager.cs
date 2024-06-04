using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
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

    #region Action
    public event Action<Vector2> OnMoveDirChanged;
    public event Action<Define.JoystickStates> OnJoystickStateChanged;
    #endregion
}
