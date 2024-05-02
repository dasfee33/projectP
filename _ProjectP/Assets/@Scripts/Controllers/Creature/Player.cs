using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Player : Creature
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        CreatureType = CreatureTypes.Player;
        CreatureState = CreatureStates.Idle;
        Speed = 5.0f;

        return true;
    }
}
