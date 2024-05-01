using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    public override bool Init()
    {
        if (!base.Init()) return false;

        SceneType = Define.Scenes.GameScene;

        return true;
    }

    public override void Clear()
    {

    }
}
