using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.IO;
using System.Text;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;

public class SurfaceController : MonoBehaviour {
    [SerializeField] protected SurfaceData data;
    [SerializeField] protected Material material;
    [SerializeField] protected Vector2Int subdivision;
    public SurfaceData Data { get => data; set { data = value; } }
    public Surface surface { get; protected set; }
    public List<Vector3> surfacePoints { get; set; }
    public Mesh mesh { get; private set; }
    protected MeshRenderer rendr;
    protected MeshFilter   filter;
    protected NativeArray<Vector3> vertices;
    public GameObject controlPoint;
    bool visibility = true;


    public void Awake()
    {
        KeyboardController.onVisibilityInput += ToggleVisibility;
    }

    public void DrawSurface() 
    {
        SetSurfaceData();
        surface = new Surface(data.cps, data.order, data.count.x, data.count.y);

        for (int y = 0; y < data.count.y; y++) 
        {
            for (int x = 0; x < data.count.x; x++) 
            {
                var i = x + y * data.count.x;
                surface.UpdateControlPoint(new Vector2Int(x, y), new ControlPoint(transform.position + data.cps[i].pos, data.cps[i].weight));
            }
        }

        CreateMesh();
        DrawControlPoints();

        mesh.triangles = mesh.triangles.Reverse().ToArray();
    }

    void OnDestroy() 
    {
        surface.Dispose();
        vertices.Dispose();
        KeyboardController.onVisibilityInput -= ToggleVisibility;
    }

    private void SetSurfaceData() 
    {
        var yOffset = FileController.minBound[1];

        List<ControlPoint> cpList = new List<ControlPoint>();

        Vector3 size = surfacePoints[surfacePoints.Count - 1];
        surfacePoints.RemoveAt(surfacePoints.Count - 1);

        foreach (Vector3 point in surfacePoints)
        {
            cpList.Add(new ControlPoint(new Vector3((float)(point[0] * 0.5f), (float)(point[1] - yOffset) * 0.5f, (float)(point[2] * 0.5f)), 1f));
        }

        data =  new SurfaceData(
            2, 
            new Vector2Int(2, 2), 
            new Vector2Int(Convert.ToInt32(size.x), Convert.ToInt32(size.y)),
            cpList
        );
    }

    private void DrawControlPoints() 
    {
        int index = 0;

        foreach (ControlPoint cp in data.cps) 
        {
            GameObject control = Instantiate(
                controlPoint,
                new Vector3(
                    cp.pos[0],
                    cp.pos[1],
                    cp.pos[2]
                    ),
                Quaternion.identity,
                transform
            ) as GameObject;

            control.GetComponent<ControlPointController>().SetIndex(index);

            index++;
        }
    }

    void CreateMesh() 
    {
        this.mesh = new Mesh();
        this.filter = gameObject.AddComponent<MeshFilter>();
        this.rendr = gameObject.AddComponent<MeshRenderer>();
        this.vertices = new NativeArray<Vector3>((subdivision.x + 1) * (subdivision.y + 1), Allocator.Persistent);

        var indices = new List<int>();

        var lx = subdivision.x + 1;
        var ly = subdivision.y + 1;
        var dx = 1f / subdivision.x;
        var dy = 1f / subdivision.y;

        for (int iy = 0; iy < ly; iy++)
        {
            for (int ix = 0; ix < lx; ix++) 
            {
                int i = ix + iy * lx;
                vertices[i] = surface.GetCurve(ix * dx, iy * dy);

                if(iy < subdivision.y && ix < subdivision.x) 
                {
                    indices.Add(i);
                    indices.Add(i + 1);
                    indices.Add(i + lx);
                    indices.Add(i + lx);
                    indices.Add(i + 1);
                    indices.Add(i + lx + 1);
                }
            }
        }

        Render(vertices, indices.ToArray());
    }

    [BurstCompile]
    struct UpdateMeshJob : IJobParallelFor 
    {
        [WriteOnly] public NativeArray<Vector3> vertices;
        [ReadOnly]  public NativeArray<ControlPoint> cps;
        public Vector2Int subdivision;
        public Vector2Int count;
        public Vector2 inverse;
        public int order;

        public void Execute(int id) 
        {
            var l = subdivision.x + 1;
            var x = (id % l) * inverse.x;
            var y = (id / l) * inverse.y;

            vertices[id] = SurfaceHelper.GetCurve(cps, x, y, order, count.x, count.y);
        }
    }

    public void UpdateMesh() 
    {
        var job = new UpdateMeshJob {
            vertices = vertices,
            cps      = surface.CPs,
            subdivision = subdivision,
            inverse   = new Vector2(1f / subdivision.x, 1f / subdivision.y),
            count    = data.count,
            order    = data.order
        };

        job.Schedule(vertices.Length, 0).Complete();

        Render(vertices);
    }

    void Render(NativeArray<Vector3> vtcs, int[] idcs = null)
    {
        this.mesh.SetVertices(vtcs);

        var indices = idcs ?? new int[0];

        if (indices.Length > 0) 
        {
            this.mesh.SetTriangles(idcs, 0);
        }

        this.mesh.RecalculateNormals();
        this.mesh.RecalculateTangents();
        this.mesh.RecalculateBounds();

        this.rendr.material = material;
        this.filter.mesh = mesh;
    }
    
    void ToggleVisibility()
    {
            if (!this.visibility)
            {
                this.visibility = true;

                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(true);
                }
            } else
            {
                this.visibility = false;

                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
    }
}
