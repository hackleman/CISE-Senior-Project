using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System;
using UnityEngine;

public class FileController : MonoBehaviour {
    public GameObject mesh;
    public static string fileHandle;
    public static Vector3 minBound = Vector3.zero;
    public MeshController meshController;

    [DllImport("mylib")]
    public static extern int process_mesh(string file);

    private void Awake()
    {
        LevelManager.OnLevelStart += StartDemo;
    }

    public async void StartDemo(string file)
    {
        fileHandle = file;

        process_mesh("Assets/Resources/" + fileHandle + ".obj");

        string data = await FileToString(fileHandle);

        List<List<Vector3>> surfaceData = ParseFileData(data);

        foreach (List<Vector3> surface in surfaceData)
        {
            GameObject surfaceMesh = GameObject.Instantiate(mesh, new Vector3(0, 0, 0), Quaternion.identity, transform) as GameObject;

            var controller = surfaceMesh.GetComponent<SurfaceController>();
            controller.surfacePoints = surface;

            controller.DrawSurface();
        }

        meshController?.RenderControlMesh();
    }

    void OnDestroy() 
    {
        LevelManager.OnLevelStart -= StartDemo;
        File.Delete(fileHandle + ".bv");
        Destroy(gameObject);
    }

    private static List<List<Vector3>> ParseFileData(string bvData) 
    {

        char[] delim = new char[] { '\r', '\n' };
        string[] output = bvData.Split(delim, StringSplitOptions.RemoveEmptyEntries);

        List<Vector3> surfacePoints = new List<Vector3> { };
        List<List<Vector3>> surfaces = new List<List<Vector3>> { };

        for (int i = 0; i < output.Length; i++)
        {
            string[] values = output[i].Split(' ');
            if (values.Length == 2)
            {
                int sizeX = Convert.ToInt32(values[0]) + 1;
                int sizeY = Convert.ToInt32(values[1]) + 1;

                for (int j = 0; j < sizeX * sizeY; j++)
                {
                    string[] coords = output[i + j + 1].Split(' ');
                    if (coords.Length == 3)
                    {
                        float x = Convert.ToSingle(coords[0]);
                        float y = Convert.ToSingle(coords[1]);
                        float z = Convert.ToSingle(coords[2]);

                        if (x < minBound[0])
                        {
                            minBound[0] = x;
                        } 
                        if (y < minBound[1])
                        {
                            minBound[1] = y;
                        } 
                        if (z < minBound[2]) 
                        {
                            minBound[2] = z;
                        }

                        Vector3 coordVector = new Vector3(x, y, z);
                        surfacePoints.Add(coordVector);
                    }

                }

                surfacePoints.Add(new Vector3(sizeX, sizeY, 0));
                surfaces.Add(new List<Vector3>(surfacePoints));
                surfacePoints.Clear();
            }
        }

        return surfaces;
    }

    private static async Task<string> FileToString(string fileHandle)
    {
        string filePath = fileHandle + ".bv";

        using (FileStream sourceStream = new FileStream(filePath,
            FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 4096, useAsync: true))
        {
            StringBuilder sb = new StringBuilder();

            byte[] buffer = new byte[0x1000];
            int numRead;
            while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                string text = Encoding.UTF8.GetString(buffer, 0, numRead);
                sb.Append(text);
            }

            return sb.ToString();
        }
    }
}
