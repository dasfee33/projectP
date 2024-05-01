using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Define
{
    public enum Scenes
    {
        Unknown,
        TitleScene,
        GameScene,
    }

    public enum UIEvents
    {
        Click,
        PointerDown,
        PointerUp,
        Drag,
    }

    public enum Sounds
    {
        Bgm,
        Effect,
        Max,
    }
}
