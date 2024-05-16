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

        Managers.Map.LoadMap("BaseMap");

        PlayerCamp camp = Managers.Object.Spawn<PlayerCamp>(Vector3.zero, 0);
        camp.SetCellPos(new Vector3Int(0, 0, 0), true);

        for(int i = 0; i < 5; i++)
        {
            int playerTemplateID;
            if (i == 0 || i == 1) playerTemplateID = HERO_KNIGHT_ID + Random.Range(0, 4);
            else playerTemplateID = HERO_KNIGHT_ID;

            Vector3Int randCellPos = new Vector3Int(0 + Random.Range(-3, 3), 0 + Random.Range(-3, 3), 0);
            if (Managers.Map.CanGo(randCellPos) == false)
                continue;

            Player player = Managers.Object.Spawn<Player>(new Vector3Int(1, 0, 0), playerTemplateID);
            //player.SetCellPos(randCellPos, true);
            Managers.Map.MoveTo(player, randCellPos, true);
        }
        
        CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
        camera.Target = camp;

        Managers.UI.ShowBaseUI<UI_Joystick>();

        {
            Monster monster1 = Managers.Object.Spawn<Monster>(new Vector3(1, 1, 0), MONSTER_SLIME_ID);
            Monster monster2 = Managers.Object.Spawn<Monster>(new Vector3(1, 1, 0), MONSTER_GOBLIN_ARCHER_ID);
            Monster monster3 = Managers.Object.Spawn<Monster>(new Vector3(1, 1, 0), MONSTER_GOBLIN_ARCHER_ID);

            Managers.Map.MoveTo(monster1, new Vector3Int(0, 4, 0), true);
            Managers.Map.MoveTo(monster2, new Vector3Int(-11, 4, 0), true);
            Managers.Map.MoveTo(monster3, new Vector3Int(11, 4, 0), true);
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
