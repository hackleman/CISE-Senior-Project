using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuUI : MonoBehaviour
{
    public void StartButtonPressed()
    {
        SceneLoader.LoadScene(SceneName.Demo);
    }
}
