using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Player : Creature
{
    Vector2 moveDir = Vector2.zero;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        CreatureType = CreatureTypes.Player;
        CreatureState = CreatureStates.Idle;
        Speed = 5.0f;

        Managers.Game.OnMoveDirChanged -= HandleOnMoveDirChanged;
        Managers.Game.OnMoveDirChanged += HandleOnMoveDirChanged;
        Managers.Game.OnJoystickStateChanged -= HandleOnJoystickStateChanged;
        Managers.Game.OnJoystickStateChanged += HandleOnJoystickStateChanged;

        return true;
    }

    void Update()
    {
        transform.ObjectTranslate(moveDir * Time.deltaTime * Speed);
    }

    private void HandleOnMoveDirChanged(Vector2 dir)
    {
        moveDir = dir;
    }

    private void HandleOnJoystickStateChanged(JoystickStates joystickState)
    {
        switch (joystickState)
        {
            case JoystickStates.PointerDown:
                CreatureState = CreatureStates.Move;
                break;
            case JoystickStates.Drag:
                break;
            case JoystickStates.PointerUp:
                CreatureState = CreatureStates.Idle;
                break;
            default:
                break;
        }
    }
}
