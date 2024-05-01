using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using static Define;

public abstract class BaseScene : InitBase
{
    public Scenes SceneType { get; protected set; } = Scenes.Unknown;

    public override bool Init()
    {
        if (!base.Init()) return false;

        Object obj = GameObject.FindObjectOfType(typeof(EventSystem));
        if (obj == null)
        {
            GameObject go = new GameObject() { name = "@EventSystem" };
            go.AddComponent<EventSystem>();
            go.AddComponent<StandaloneInputModule>();
        }
        return true;
    }

    public abstract void Clear();
}
