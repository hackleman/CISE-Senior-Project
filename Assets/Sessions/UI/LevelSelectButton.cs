using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    LevelSelectUI ui;
    FileCatalog catalog;
    int fileIndex;

    public void Initialize(LevelSelectUI ui, int fileIndex, FileCatalog catalog)
    {
        this.ui = ui;
        this.fileIndex = fileIndex;
        this.catalog = catalog;
        GetComponentInChildren<Text>().text = catalog.GetFile(fileIndex);
    }

    public void OnPress()
    {
        ui.LevelButtonPressed(fileIndex);
    }
}
