using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Define;


public static class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();

        return component;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    public static T ParseEnum<T>(string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    public static Color HexToColor(string color)
    {
        if (color.Contains("#") == false)
            color = $"#{color}";

        ColorUtility.TryParseHtmlString(color, out Color parsedColor);

        return parsedColor;
    }

    public static ObjectTypes DetermineTargetType(ObjectTypes ownerType, bool findAllies)
    {
        if (ownerType == Define.ObjectTypes.Player)
        {
            return findAllies ? ObjectTypes.Player : ObjectTypes.Monster;
        }
        else if (ownerType == Define.ObjectTypes.Monster)
        {
            return findAllies ? ObjectTypes.Monster : ObjectTypes.Player;
        }

        return ObjectTypes.None;
    }

    public static float GetEffectRadius(EffectSizes size)
    {
        switch (size)
        {
            case EffectSizes.CircleSmall:
                return EFFECT_SMALL_RADIUS;
            case EffectSizes.CircleNormal:
                return EFFECT_NORMAL_RADIUS;
            case EffectSizes.CircleBig:
                return EFFECT_BIG_RADIUS;
            case EffectSizes.ConeSmall:
                return EFFECT_SMALL_RADIUS * 2f;
            case EffectSizes.ConeNormal:
                return EFFECT_NORMAL_RADIUS * 2f;
            case EffectSizes.ConeBig:
                return EFFECT_BIG_RADIUS * 2f;
            default:
                return EFFECT_SMALL_RADIUS;
        }
    }
}
