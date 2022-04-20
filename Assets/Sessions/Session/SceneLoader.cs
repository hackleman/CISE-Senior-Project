using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneLoader
{
    public static void LoadScene(SceneName name)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene((int) name);
    }
}

public enum SceneName
{
    Menu,
    Demo,
    LevelEnd,
    LevelSelect
}