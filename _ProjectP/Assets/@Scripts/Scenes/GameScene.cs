using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class GameScene : BaseScene
{
    public override bool Init()
    {
        if (!base.Init()) return false;

        SceneType = Scenes.GameScene;

        GameObject map = Managers.Resource.Instantiate("BaseMap");
        map.transform.position = Vector3.zero;
        map.name = "@BaseMap";

        Player player = Managers.Object.Spawn<Player>(new Vector3(-10, -5));
        //player.CreatureState = CreatureStates.Move;

        CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
        camera.Target = player;

        Managers.UI.ShowBaseUI<UI_Joystick>();

        {
            Monster monster1 = Managers.Object.Spawn<Monster>(Vector3.up + Vector3.up);
            Monster monster2 = Managers.Object.Spawn<Monster>(Vector3.up + Vector3.right);
            Monster monster3 = Managers.Object.Spawn<Monster>(Vector3.up + Vector3.left);

            monster1.CreatureState = CreatureStates.Idle;
            monster2.CreatureState = CreatureStates.Idle;
            monster3.CreatureState = CreatureStates.Idle;
        
        }
        //TODO

        return true;
    }

    public override void Clear()
    {

    }
}
