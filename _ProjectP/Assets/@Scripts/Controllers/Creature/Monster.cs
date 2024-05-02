using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class Monster : Creature
{
    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        CreatureType = CreatureTypes.Monster;
        CreatureState = CreatureStates.Idle;
        Speed = 3.0f;

        return true;
    }
}
