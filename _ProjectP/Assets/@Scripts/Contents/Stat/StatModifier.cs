using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class StatModifier
{
    public readonly float Value;
    public readonly StatModTypes Type;
    public readonly int Order;
    public readonly object Source;

    public StatModifier(float value, StatModTypes type, int order, object source)
    {
        Value = value;
        Type = type;
        Order = order;
        Source = source;
    }

    public StatModifier(float value, StatModTypes type) : this(value, type, (int)type, null) { }

    public StatModifier(float value, StatModTypes type, int order) : this(value, type, order, null) { }

    public StatModifier(float value, StatModTypes type, object source) : this(value, type, (int)type, source) { }
}
