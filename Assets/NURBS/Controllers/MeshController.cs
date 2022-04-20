using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshController : MonoBehaviour
{
    public GameObject vertex;

    public void RenderControlMesh()
    {
        var mesh = Resources.Load<Mesh>(FileController.fileHandle);
        var offset = FileController.minBound[1];

        foreach (var v in mesh.vertices)
        {
            GameObject control = Instantiate(
                vertex,
                new Vector3(
                    (float)(v[0] * 0.5f),
                    (float)(v[1] - offset) * 0.5f,
                    (float)(v[2] * 0.5f)
                ),
                Quaternion.identity,
                transform
            ) as GameObject;
        }

    }
}
