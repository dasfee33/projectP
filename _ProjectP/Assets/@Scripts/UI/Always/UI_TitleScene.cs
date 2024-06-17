using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Define;

public class UI_TitleScene : UI_Always
{
    enum GameObjects
    {
        StartImg,
    }

    enum Texts
    {
        DisplayText,
    }

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        BindObjects(typeof(GameObjects));
        BindTexts(typeof(Texts));

        GetObject((int)GameObjects.StartImg).BindEvent((evt) =>
        {
            Debug.Log("ChangeScene");
            Managers.Scene.LoadScene(Scenes.GameScene);
        });

        GetObject((int)GameObjects.StartImg).gameObject.SetActive(false);
        GetText((int)Texts.DisplayText).text = $"";

        StartLoadAssets();

        return true;
    }

    void StartLoadAssets()
    {
        Managers.Resource.LoadAllAsync<Object>("PreLoad", (key, count, totalCount) =>
        {
            Debug.Log($"{key} {count}/{totalCount}");

            if (count == totalCount)
            {
                Managers.Data.Init();

                //������ �ִ��� Ȯ��
                if(Managers.Game.LoadGame() == false)
                {
                    Managers.Game.InitGame();
                    Managers.Game.SaveGame();
                }

                GetObject((int)GameObjects.StartImg).gameObject.SetActive(true);
                GetText((int)Texts.DisplayText).text = "Touch To Start";


            }
        });
    }
}
