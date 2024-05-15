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

    #region Action
    public event Action<Vector2> OnMoveDirChanged;
    public event Action<Define.JoystickStates> OnJoystickStateChanged;
    #endregion
}
