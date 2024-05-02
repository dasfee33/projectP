using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    public override bool Init()
    {
        if (!base.Init()) return false;

        SceneType = Define.Scenes.GameScene;

        GameObject map = Managers.Resource.Instantiate("BaseMap");
        map.transform.position = Vector3.zero;
        map.name = "@BaseMap";

        Managers.Resource.Instantiate("Player");

        return true;
    }

    public override void Clear()
    {

    }
}
