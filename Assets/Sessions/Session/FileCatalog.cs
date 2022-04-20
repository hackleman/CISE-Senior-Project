using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data File/File Catalog")]
public class FileCatalog : ScriptableObject
{
    [SerializeField]
    string[] files;

    public int length { get { return files.Length; } }

    public string GetFile (int index)
    {
        if (index >= files.Length || index < 0)
        {
            return null;
        }

        return files[index];
    }

    public int GetIndexOf (string file)
    {
        return System.Array.IndexOf(files, file);
    } 
     
}