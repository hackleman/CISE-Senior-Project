using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public void StartButtonPressed()
    {
        SceneLoader.LoadScene(SceneName.Level);
    }
}
