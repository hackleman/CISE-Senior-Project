using UnityEngine;

[System.Serializable]
public struct ControlPoint {
    public Vector3 pos;
    public float weight;
    public ControlPoint(Vector3 p, float w) 
    {
         pos = p; 
         weight = w; 
    }
}
