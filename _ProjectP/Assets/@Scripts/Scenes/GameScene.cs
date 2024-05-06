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

        Player player = Managers.Object.Spawn<Player>(new Vector3(-10, -5), HERO_KNIGHT_ID);
        //player.CreatureState = CreatureStates.Move;

        CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
        camera.Target = player;

        Managers.UI.ShowBaseUI<UI_Joystick>();

        {
            Monster monster1 = Managers.Object.Spawn<Monster>(Vector3.up + Vector3.up, MONSTER_SLIME_ID);
            Monster monster2 = Managers.Object.Spawn<Monster>(Vector3.up + Vector3.right, MONSTER_BEAR_ID);
            Monster monster3 = Managers.Object.Spawn<Monster>(Vector3.up + Vector3.left, MONSTER_SPIDER_COMMON_ID);

            monster1.CreatureState = CreatureStates.Idle;
            monster2.CreatureState = CreatureStates.Idle;
            monster3.CreatureState = CreatureStates.Idle;
        }

        {
            Env env = Managers.Object.Spawn<Env>(new Vector3(0, 2, 0), ENV_TREE1_ID);
            env.EnvState = EnvStates.Idle;
        }
        //TODO

        return true;
    }

    public override void Clear()
    {

    }
}
