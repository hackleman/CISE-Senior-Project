using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionManager : MonoBehaviour
{
    public static SessionManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoad;
            LevelManager.OnLevelEnd += HandleLevelEnd;
            DontDestroyOnLoad(gameObject);

        } else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void OnSceneLoad(Scene s, LoadSceneMode lsm)
    {
        if (s.name == "Demo")
        {
            LevelManager.StartLevel("tricube");
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
        LevelManager.OnLevelEnd -= HandleLevelEnd;
    }

    void HandleLevelEnd(bool completed)
    {
        SceneLoader.LoadScene(SceneName.LevelEnd);
    }
}
