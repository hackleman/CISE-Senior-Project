using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SessionManager : MonoBehaviour
{
    public static SessionManager instance;

    public FileCatalog catalog;
    int currentFile = 0;
    public int CurrentFile { set { currentFile = value;}}

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);

        } else if (instance != this)
        {
            Destroy(gameObject);
        }

        SceneManager.sceneLoaded += OnSceneLoad;
        LevelManager.OnLevelEnd += HandleLevelEnd;
    }

    void OnSceneLoad(Scene s, LoadSceneMode lsm)
    {
        if (instance != this)
        {
            return;
        }

        if (s.name == "Demo")
        {
            Debug.Log(catalog.GetFile(currentFile));
            LevelManager.StartLevel(catalog.GetFile(currentFile));
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
        LevelManager.OnLevelEnd -= HandleLevelEnd;
    }

    void HandleLevelEnd(bool completed)
    {
        if (instance != this)
        {
            return;
        }

        SceneLoader.LoadScene(SceneName.LevelEnd);
    }
}
