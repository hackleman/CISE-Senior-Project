using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class SurfaceData {
    public int order;
    public Vector2Int size;
    public Vector2Int count;
    public List<ControlPoint> cps;

    public SurfaceData(int o, Vector2Int s, Vector2Int c, List<ControlPoint> controlPoints) 
    { 
        order = o; 
        size = s; 
        count = c; 
        cps = controlPoints; 
    }
}
