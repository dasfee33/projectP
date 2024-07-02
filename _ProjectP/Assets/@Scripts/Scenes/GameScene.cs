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
        Managers.Map.StageTransition.SetInfo();

        var cellPos = Managers.Map.World2Cell(new Vector3(-100, -66));

        PlayerCamp camp = Managers.Object.Spawn<PlayerCamp>(Vector3.zero, 0);
        camp.SetCellPos(cellPos, true);

        for(int i = 0; i < 1; i++)
        {
            int playerTemplateID = HERO_LION_ID;
            //if (i == 0 || i == 1) playerTemplateID = HERO_KNIGHT_ID + Random.Range(0, 4);
            //else playerTemplateID = HERO_KNIGHT_ID;

            //Vector3Int randCellPos = new Vector3Int(0 + Random.Range(-3, 3), 0 + Random.Range(-3, 3), 0);
            //if (Managers.Map.CanGo(null, randCellPos) == false)
            //    continue;

            Player player = Managers.Object.Spawn<Player>(new Vector3Int(1, 0, 0), playerTemplateID);
            //player.SetCellPos(randCellPos, true);
            Managers.Map.MoveTo(player, cellPos, true);
        }
        
        CameraController camera = Camera.main.GetOrAddComponent<CameraController>();
        camera.Target = camp;

        Managers.UI.ShowBaseUI<UI_Joystick>();

        UI_GameScene sceneUI = Managers.UI.ShowAlwaysUI<UI_GameScene>();
        sceneUI.GetComponent<Canvas>().sortingOrder = 1;
        sceneUI.SetInfo();

        {
            //Monster monster1 = Managers.Object.Spawn<Monster>(new Vector3(10, 10, 0), MONSTER_BEAR_ID);
            //Monster monster2 = Managers.Object.Spawn<Monster>(new Vector3(1, 1, 0), MONSTER_GOBLIN_ARCHER_ID);
            //Monster monster3 = Managers.Object.Spawn<Monster>(new Vector3(1, 1, 0), MONSTER_GOBLIN_ARCHER_ID);

            //monster1.ExtraCells = 1;
            //monster2.ExtraCells = 1;
            //monster3.ExtraCells = 1;

            //Managers.Map.MoveTo(monster1, new Vector3Int(0, 4, 0), true);
            //Managers.Map.MoveTo(monster2, new Vector3Int(-11, 4, 0), true);
            //Managers.Map.MoveTo(monster3, new Vector3Int(11, 4, 0), true);
        }

        {
            Env env = Managers.Object.Spawn<Env>(new Vector3(0, 2, 0), ENV_TREE1_ID);
            env.EnvState = EnvStates.Idle;
        }
        //TODO

        Managers.UI.CacheAllPopups();

        return true;
    }

    public override void Clear()
    {

    }
}
