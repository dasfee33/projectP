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

    public enum ObjectTypes
    {
        None,
        Creature,
        Projectile,
        Env,
    }

    public enum CreatureTypes
    {
        None,
        Player,
        Monster,
        Npc,
    }

    public enum CreatureStates
    {
        None,
        Idle,
        Move,
        Skill,
        Dead,
    }

    public enum JoystickStates
    {
        PointerDown,
        PointerUp,
        Drag,
    }

    public static class AnimName
    {
        public const string IDLE = "idle";
        public const string ATTACK_A = "attack_a";
        public const string ATTACK_B = "attack_b";
        public const string MOVE = "move";
        public const string DEAD = "dead";
    }
}
