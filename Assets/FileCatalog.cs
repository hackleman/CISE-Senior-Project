using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Data File/Catalog")]
public class FileCatalog : ScriptableObject
{

    [SerializeField]
    string[] files;

    public int Length { get { return files.Length; } }

    public string GetFile(int index)
    {
        if (index >= files.Length)
        {
            return null;
        }

        return files[index];
    }

    public int GetIndex(string path)
    {
        return System.Array.IndexOf(files, path);
    }
}
