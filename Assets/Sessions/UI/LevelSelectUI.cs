using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectUI : MonoBehaviour
{
    // public LevelSelectButton buttonPrefab;
    // public FileCatalog catalog;
    // public Transform buttonContainer;
    // void Start()
    // {
    //     for (int i = 0; i < SessionManager.instance.catalog.length; i++)
    //     {
    //         LevelSelectButton newButton = Instantiate(buttonPrefab, buttonContainer);
    //         newButton.Initialize(this, i, catalog);
    //     }
    // }
   public void BackButtonPressed()
   {
       SceneLoader.LoadScene(SceneName.Menu);
   }

   public void TeeButtonPressed()
   {
       LevelButtonPressed(1);
   }

   public void TricubeButtonPressed()
   {
       LevelButtonPressed(0);
   }

   public void SuzanneButtonPressed()
   {
       LevelButtonPressed(2);
   }

   public void LevelButtonPressed(int levelIndex)
   {
       SessionManager.instance.CurrentFile = levelIndex;
       SceneLoader.LoadScene(SceneName.Demo);
   }
}
