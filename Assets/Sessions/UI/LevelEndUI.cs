using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndUI : MonoBehaviour
{
    public void RestartButtonPressed()
    {
        SceneLoader.LoadScene(SceneName.Level);
    }

    public void MenuButtonPressed()
    {
        SceneLoader.LoadScene(SceneName.MenuMain);
    }
}
