using System.Collections;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public delegate void LevelStartHandler(string filename);
    public static event LevelStartHandler OnLevelStart;
    public delegate void LevelEndHandler(bool levelCompleted);
    public static event LevelEndHandler OnLevelEnd;

    private void Awake()
    {
        KeyboardController.onEscapeInput += EndLevel;
    }

    public static void StartLevel(string filename)
    {
        if (OnLevelStart != null)
        {
            OnLevelStart(filename);
        }
    }        
    
    void EndLevel()
    {
        KeyboardController.onEscapeInput -= EndLevel;

        if (OnLevelEnd != null)
        {
            OnLevelEnd(true);
        }
    }
}
