using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ASceneManager : MonoBehaviour
{
    public BaseScene CurrentScene { get { return GameObject.FindObjectOfType<BaseScene>(); } }

    public void LoadScene(Define.Scenes type)
    {
        //Managers.Clear();
        SceneManager.LoadScene(GetSceneName(type));
    }

    private string GetSceneName(Define.Scenes type)
    {
        string name = System.Enum.GetName(typeof(Define.Scenes), type);
        return name;
    }

    public void Clear()
    {
        //CurrentScene.Clear();
    }
}
