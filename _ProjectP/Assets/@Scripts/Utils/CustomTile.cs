using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CustomTile : Tile
{
    [Space]
    [Space]
    [Header("For Graphics")]
    public Define.ObjectTypes ObjectType;
    public Define.CreatureTypes CreatureType;
    public int DataTemplateID;
    public string Name;

}
